using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace OpeWin
{
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

            ope.UpdateCount(id); // must be executed before csll PrepareEnv()

            PrepareEnv(lua, ope);

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
                            + " ChangeMonitorBw = ChangeMonitorBw,"
                            + " ResetCount = ResetCount"
                            + "}" + Environment.NewLine
                            + " function untrusted()" + Environment.NewLine;

        private string ScriptFooter = Environment.NewLine
                                    + " end" + Environment.NewLine
                                    + "end" + Environment.NewLine
                                    + "untrusted()" + Environment.NewLine;

        private void PrepareEnv(NLua.Lua lua, Ope ope)
        {
            // NOTICE: You must also change _ENV table in the ScriptHeader if you change following code. 

            // variables(read only)
            lua["Count"] = Ope.Count;

            // functions
            lua.RegisterFunction("Print", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Print"));
            lua.RegisterFunction("Maximize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Maximize"));
            lua.RegisterFunction("Minimize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Minimize"));
            lua.RegisterFunction("MoveTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveTo"));
            lua.RegisterFunction("ResizeTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeTo"));
            lua.RegisterFunction("ChangeMonitorFw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorFw"));
            lua.RegisterFunction("ChangeMonitorBw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorBw"));
            lua.RegisterFunction("ResetCount", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResetCount"));
        }
    }
}
