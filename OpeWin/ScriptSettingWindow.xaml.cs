using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpeWin
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScriptSettingWindow : Window
    {
        NLua.Lua OpeLua;
        DataRow OpeInfo;

        string ScriptHeader = "local untrusted;" + Environment.NewLine
                            + "do" + Environment.NewLine
                            + " local _ENV = {Ope = Ope}" + Environment.NewLine
                            + " function untrusted()" + Environment.NewLine;

        string ScriptFooter = Environment.NewLine
                            + " end" + Environment.NewLine
                            + "end" + Environment.NewLine
                            + "untrusted()" + Environment.NewLine;

        public ScriptSettingWindow(DataRow ope_info)
        {
            InitializeComponent();

            this.OpeLua = new NLua.Lua();
            OpeLua["OpeCom"] = new OpeCommand();

            OpeInfo = ope_info;

            TbxScript.Text = OpeInfo["ScriptBody"].ToString();
        }

        private void BtnDo_Click(object sender, RoutedEventArgs e)
        {
            this.Do(TbxScript.Text);
        }

        private void Do(String input)
        {
            NLua.Lua lua = new NLua.Lua();

            try
            {
                lua["Ope"] = new OpeCommand(TbxOutput);
                Debug.Print(ScriptHeader + input + ScriptFooter);
                lua.DoString(ScriptHeader + input + ScriptFooter);
            }
            catch(Exception e)
            {

                 TbxOutput.Text += (e.Message + Environment.NewLine);
            }
            finally
            {
                lua.Close();
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            OpeInfo["ScriptBody"] = TbxScript.Text;

            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            TbxOutput.Clear();
        }
    }
}
