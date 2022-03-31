using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
//using LuaInterface;
using OpeLang;

namespace OpeWin
{
    class OpeScript
    {
        // Singleton Instance
        private static OpeScript Instance = new OpeScript();
        //private static Lua _Lua = new Lua();
        private static Interpreter _interpreter;

        private OpeScript()
        {
            try
            {
                _interpreter = new Interpreter("OpeWin.OpeLang_0.0.2.cgt");
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + Environment.NewLine + "OpeWin will exit...");
                Environment.Exit(-1);
            }

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

            //PrepareEnv(_Lua, ope);
            PrepareEnv(ope);

            try
            {
                //_Lua.DoString(ScriptHeader + input + ScriptFooter);
                _interpreter.Execute(input);
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

        //private string ScriptHeader = "local untrusted;" + Environment.NewLine
        //                    + "do" + Environment.NewLine
        //                    + " local _ENV = {"
        //                    + " Count = Count,"
        //                    + " Null = -1,"
        //                    + " Print = Print,"
        //                    + " Maximize = Maximize,"
        //                    + " Minimize = Minimize,"
        //                    + " Restore = Restore,"
        //                    + " MoveTo = MoveTo,"
        //                    + " VSMoveTo = VSMoveTo,"
        //                    + " MoveBy = MoveBy,"
        //                    + " ResizeTo = ResizeTo,"
        //                    + " VSResizeTo = VSResizeTo,"
        //                    + " ResizeBy = ResizeBy,"
        //                    + " ChangeMonitorFw = ChangeMonitorFw,"
        //                    + " ChangeMonitorBw = ChangeMonitorBw,"
        //                    + " ResetCount = ResetCount,"
        //                    + " Inspect = Inspect"
        //                    + "}" + Environment.NewLine
        //                    + " function untrusted()" + Environment.NewLine;

        //private string ScriptFooter = Environment.NewLine
        //                            + " end" + Environment.NewLine
        //                            + "end" + Environment.NewLine
        //                            + "untrusted()" + Environment.NewLine;

        private void PrepareEnv(/*Lua lua, */Ope ope)
        {
            // NOTICE: You must also change _ENV table in the ScriptHeader if you change following code. 

            //// variables(read only)
            //lua["Count"] = Ope.Count;

            //// functions
            //lua.RegisterFunction("Print", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Print"));
            //lua.RegisterFunction("Maximize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Maximize"));
            //lua.RegisterFunction("Minimize", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Minimize"));
            //lua.RegisterFunction("Restore", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Restore"));
            //lua.RegisterFunction("MoveTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveTo"));
            //lua.RegisterFunction("VSMoveTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("VSMoveTo"));
            //lua.RegisterFunction("MoveBy", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveBy"));
            //lua.RegisterFunction("ResizeTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeTo"));
            //lua.RegisterFunction("VSResizeTo", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("VSResizeTo"));
            //lua.RegisterFunction("ResizeBy", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeBy"));
            //lua.RegisterFunction("ChangeMonitorFw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorFw"));
            //lua.RegisterFunction("ChangeMonitorBw", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorBw"));
            //lua.RegisterFunction("ResetCount", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResetCount"));
            //lua.RegisterFunction("Inspect", Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Inspect"));

            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Print"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Maximize"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Minimize"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Restore"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveTo"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("VSMoveTo"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("MoveBy"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeTo"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("VSResizeTo"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResizeBy"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorFw"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ChangeMonitorBw"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("ResetCount"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("Inspect"));
            _interpreter.RegisterOpe(Ope.GetInstance(), Ope.GetInstance().GetType().GetMethod("GetCount"));
        }

        //public void CloseLua()
        //{
        //    _Lua.Close();
        //}
    }
}
