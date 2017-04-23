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
using System.Windows.Shapes;

namespace OpeWin
{
    /// <summary>
    /// MainSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainSettingWindow : Window
    {
        DataTable OpeInfoTable;

        public MainSettingWindow()
        {
            InitializeComponent();

            OpeInfoTable = new DataTable();

            OpeInfoTableManager.GenerateDefaultData(OpeInfoTable, 20);

            dgOpeList.DataContext = OpeInfoTable;
        }

        private void dgOpeList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Style style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            dgOpeList.Columns[0].CellStyle = style;
            dgOpeList.Columns[3].Visibility = Visibility.Hidden;
        }

        private void dgOpeList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn();
            column.Header = "Script";
            column.CellTemplate = this.Resources["dtmpEditScript"] as DataTemplate;
            dgOpeList.Columns.Add(column);
        }

        private void btnEditScript_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow curt_row = VisualTreeUtil.FindParent<DataGridRow>(sender as DependencyObject);

            int idx = int.Parse((dgOpeList.Columns[0].GetCellContent(curt_row) as TextBlock).Text);

            ScriptSettingWindow script_setting_window = new ScriptSettingWindow(OpeInfoTable.Rows[idx]);
            script_setting_window.Owner = this;
            script_setting_window.ShowDialog();
        }
    }

    class VisualTreeUtil
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }

    class OpeInfoTableManager
    {
        public static void GenerateDefaultData(DataTable table, int num_of_ope)
        {
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("HotKey");
            table.Columns.Add("ScriptBody");

            for (int idx = 0; idx < num_of_ope; idx++)
            {
                table.Rows.Add(table.NewRow());
                table.Rows[idx]["ID"] = idx;
                table.Rows[idx]["Name"] = "New Ope";
                table.Rows[idx]["HotKey"] = "None";
                table.Rows[idx]["ScriptBody"] = @"Ope:Print(""Hello world!"")";
            }
        }
    }
}
