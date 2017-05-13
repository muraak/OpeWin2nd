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

        public ScriptSettingWindow(DataRow ope_info)
        {
            InitializeComponent();

            OpeInfo = ope_info;

            TbxScript.Text = OpeInfo["ScriptBody"].ToString();

            OpeScriptManager.GetInstance().Initialize(TbxOutput);
        }

        private void BtnDo_Click(object sender, RoutedEventArgs e)
        {
            OpeScriptManager.GetInstance().DoScript(
                TbxScript.Text, 
                int.Parse(OpeInfo["ID"].ToString()));
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
