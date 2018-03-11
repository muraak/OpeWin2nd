using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using LuaInterface;

namespace OpeWin
{
    class OpeScript
    {
        // Singleton Instance
        private static OpeScript Instance = new OpeScript();
        private static Lua _Lua = new Lua();

        private OpeScript()
        {
            Initialize();
        }

        public static OpeScript GetInstance()
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
            Ope ope = Ope.GetInstance();

            ope.UpdateCount(id); // must be executed before csll PrepareEnv()

            PrepareEnv(_Lua, ope);

            try
            {
                _Lua.DoString(ScriptHeader + input + ScriptFooter);
            }
            catch (Exception e)
            {
                ope.Print(e.Message);
            }
            finally
            {
                
            }

            ope.EnqueuePrevId(id);
        }

        private string ScriptHeader = "local untrusted;" + Environment.NewLine
                            + "do" + Environment.NewLine
                            + " local _ENV = {"
                            + " Count = Count,"
                            + " Null = -1,"
                            + " Print = Print,"
                            + " Maximize = Maximize,"
                            + " Minimize = Minimize,"
                            + " Restore = Restore,"
                            + " MoveTo = MoveTo,"
                            + " MoveBy = MoveBy,"
                            + " ResizeTo = ResizeTo,"
                            + " ResizeBy = ResizeBy,"
                            + " ChangeMonitorFw = ChangeMonitorFw,"
                            + " ChangeMonitorBw = ChangeMonitorBw,"
                            + " ResetCount = ResetCount"
                            + "}" + Environment.NewLine
                            + " function untrusted()" + Environment.NewLine;

        private string ScriptFooter = Environment.NewLine
                                    + " end" + Environment.NewLine
                                    + "end" + Environment.NewLine
                                    + "untrusted()" + Environment.NewLine;

        private void PrepareEnv(Lua lua, Ope ope)
        {
            // NOTICE: You must also change _ENV table in the ScriptHeader if you change following code. 

            // variables(read only)
            lua["Count"] = Ope.Count;

            // functions
            lua.RegisterFunction("Print", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Print"));
            lua.RegisterFunction("Maximize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Maximize"));
            lua.RegisterFunction("Minimize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Minimize"));
            lua.RegisterFunction("Restore", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Restore"));
            lua.RegisterFunction("MoveTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveTo"));
            lua.RegisterFunction("MoveBy", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveBy"));
            lua.RegisterFunction("ResizeTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeTo"));
            lua.RegisterFunction("ResizeBy", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeBy"));
            lua.RegisterFunction("ChangeMonitorFw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorFw"));
            lua.RegisterFunction("ChangeMonitorBw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorBw"));
            lua.RegisterFunction("ResetCount", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResetCount"));
        }

        public void CloseLua()
        {
            _Lua.Close();
        }
    }
}
