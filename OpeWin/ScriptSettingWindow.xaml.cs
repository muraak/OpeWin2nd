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

            OpeInfo = ope_info;

            TbxScript.Text = OpeInfo["ScriptBody"].ToString();

            Ope ope = Ope.GetInstance();
            ope.Initialize(TbxOutput);
        }

        private void BtnDo_Click(object sender, RoutedEventArgs e)
        {
            this.DoScript(TbxScript.Text);

            TbxOutput.ScrollToEnd();
        }

        private void DoScript(String input)
        {
            NLua.Lua lua = new NLua.Lua();

            Ope ope = Ope.GetInstance();

            ope.UpdateCount(int.Parse(OpeInfo["ID"].ToString()));

            lua["Ope"] = ope;

            try
            {
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

            ope.EnqueuePrevId(int.Parse(OpeInfo["ID"].ToString()));
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
