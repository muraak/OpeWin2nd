﻿using System;
using System.Collections.Generic;
using System.Data;
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

        public ScriptSettingWindow(DataRow ope_info)
        {
            InitializeComponent();

            this.OpeLua = new NLua.Lua();
            OpeLua["Ope"] = new OpeCommand();

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
                lua.DoString(input);
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
    }
}
