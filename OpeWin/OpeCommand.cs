using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace OpeWin
{
    class Ope
    {
        // Singleton Instance
        private static Ope Instance = new Ope();

        private static TextBox TbxOputput = null;

        public  static int Count { get; set; }
        public Queue<int> PrevOpeIds = new Queue<int>(5);

        private const int MAX_COUNT = 5;

        private Ope()
        {
            Initialize();
        }

        public static Ope GetInstance()
        {
            return Instance;
        }

        public void Initialize()
        {
            TbxOputput = null;
            Count = 0;
            PrevOpeIds.Clear();
        }

        public void Initialize(TextBox tbx_output)
        {
            TbxOputput = tbx_output;
            Count = 0;
            PrevOpeIds.Clear();
        }

        public void UpdateCount(int idx_of_sender)
        {
            if(PrevOpeIds.Count == 0)
            {
                Count = 0;

                return;
            }

            if(idx_of_sender == PrevOpeIds.Last<int>())
            {
                Count = (Count + 1) % MAX_COUNT;
            }
            else
            {
                Count = 0;
            }
        }

        public void EnqueuePrevId(int prev_id)
        {
            PrevOpeIds.Enqueue(prev_id);
        }

        public void Print(String input)
        {
            if(TbxOputput != null)
            {
                TbxOputput.Text += (input + Environment.NewLine);
            }
        }

        public void Maximize()
        {
            Print("Maximize()");
        }

        public void Minimize()
        {
            Print("Minimize()");
        }

        public void MoveTo(double rate_x, double rate_y)
        {
            Print("MoveTo("+rate_x.ToString()+", "+rate_y.ToString()+")");
        }

        public void ResizeTo(double rate_width, double rate_height)
        {
            Print("ResizeTo("+rate_width.ToString()+", "+rate_height.ToString()+")");
        }

        public void ChangeMonitorFw()
        {
            Print("ChangeMonitorFw()");
        }

        public void ChangeMonitorBw()
        {
            Print("ChangeMonitorBw()");
        }
    }

    class OpeScriptManager
    {
        // Singleton Instance
        private static OpeScriptManager Instance = new OpeScriptManager();

        private OpeScriptManager()
        {
            Initialize();
        }

        public static OpeScriptManager GetInstance()
        {
            return Instance;
        }

        public void Initialize()
        {
            Ope.GetInstance().Initialize();
        }

        public void Initialize(TextBox tbx_output)
        {
            Ope.GetInstance().Initialize(tbx_output);
        }

        public void DoScript(String input, int id)
        {
            NLua.Lua lua = new NLua.Lua();
            Ope ope = Ope.GetInstance();

            PrepareEnv(lua, ope);

            ope.UpdateCount(id);

            try
            {
                lua.DoString(ScriptHeader + input + ScriptFooter);
            }
            catch (Exception e)
            {
                ope.Print(e.Message);
            }
            finally
            {
                lua.Close();
            }

            ope.EnqueuePrevId(id);
        }

        private string ScriptHeader = "local untrusted;" + Environment.NewLine
                            + "do" + Environment.NewLine
                            + " local _ENV = {"
                            + " Count = Count,"
                            + " Print = Print,"
                            + " Maximize = Maximize,"
                            + " Minimize = Minimize,"
                            + " MoveTo = MoveTo,"
                            + " ResizeTo = ResizeTo,"
                            + " ChangeMonitorFw = ChangeMonitorFw,"
                            + " ChangeMonitorBw = ChangeMonitorBw"
                            + "}" + Environment.NewLine
                            + " function untrusted()" + Environment.NewLine;

        private string ScriptFooter = Environment.NewLine
                                    + " end" + Environment.NewLine
                                    + "end" + Environment.NewLine
                                    + "untrusted()" + Environment.NewLine;

        private void PrepareEnv(NLua.Lua lua, Ope ope)
        {
            // NOTICE: You must also change _ENV table in the ScriptHeader if you change following code. 

            // variables
            lua["Count"] = Ope.Count;

            // functions
            lua.RegisterFunction("Print", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Print"));
            lua.RegisterFunction("Maximize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Maximize"));
            lua.RegisterFunction("Minimize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Minimize"));
            lua.RegisterFunction("MoveTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveTo"));
            lua.RegisterFunction("ResizeTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeTo"));
            lua.RegisterFunction("ChangeMonitorFw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorFw"));
            lua.RegisterFunction("ChangeMonitorBw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorBw"));
        }
    }
}
