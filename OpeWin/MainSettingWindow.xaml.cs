﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace OpeWin
{
    /// <summary>
    /// MainSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainSettingWindow : Window
    {
        private KeyboardHook.LowLevelKeyboardProc _LLKeyboardProc;
        private IntPtr hHandle;

        public MainSettingWindow()
        {
            InitializeComponent();
            

            dgOpeList.DataContext = OpeInfoTable.GetInstance();

            _LLKeyboardProc = new KeyboardHook.LowLevelKeyboardProc(LLKeyboardProc);
            hHandle = KeyboardHook.SetHook(_LLKeyboardProc);
        }

        private void RegisterHotkeys()
        {
            OpeInfoTable.GetInstance().RegisterAllOpeToHotKey(GetHWnd());
        }

        private void UnregisterHotkeys()
        {
            OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(GetHWnd());
        }

        public IntPtr GetHWnd()
        {
            return new WindowInteropHelper(this).Handle;
        }


        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:

                    OpeInfoTable.GetInstance().DoOpeScript(wParam.ToInt32());

                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }


        private void dgOpeList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Style style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            dgOpeList.Columns[0].CellStyle = style;
            dgOpeList.Columns[3].Visibility = Visibility.Hidden;
            dgOpeList.Columns[4].Visibility = Visibility.Hidden;
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

            ScriptSettingWindow script_setting_window 
                = new ScriptSettingWindow(OpeInfoTable.GetInstance().GetRowById(idx));
            script_setting_window.Owner = this;
            script_setting_window.ShowDialog();
        }

        private void dgOpeList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataRowView selected_item = (DataRowView)dgOpeList.SelectedItem;

            ModifierKeys modifierKeys = Keyboard.Modifiers;
            if(isBeingPressedWinKey)
            {
                modifierKeys |= ModifierKeys.Windows;
            }

            HotKey hot_key = new HotKey();
            if (hot_key.CanSet(e, modifierKeys))
            {
                hot_key.Set(e, modifierKeys);
            }
            else
            {
                return;
            }

            selected_item.Row["HotKey"] = hot_key.MyToString();
            selected_item.Row["HotKeyObject"] = hot_key;
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpeInfoTable.GetInstance().Save();

            OpeScript.GetInstance().Initialize();
            
            MyHide();
            e.Cancel = true;
        }

        public void MyHide()
        {
            WindowState = System.Windows.WindowState.Normal;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.ToolWindow;
            WindowState = System.Windows.WindowState.Minimized;
        }

        public void MyShow()
        {
            WindowState = System.Windows.WindowState.Normal;
            ShowInTaskbar = true;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        public void HideFromAltTabMenu()
        {
            Show();
            MyHide();
        }

        private void btnStartDebug_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString() == "Start Debug")
            {
                ((Button)sender).Content = "Stop Debug";
                MakeDisenableDgOpeList();
                RowDefinition row_def = new RowDefinition();
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                grdMain.RowDefinitions[2].Height = (GridLength)gridLengthConverter.ConvertFrom("1*");
                grdDebugSet.Visibility = Visibility.Visible;

                OpeScript.GetInstance().Initialize(TbxOutput);

                OpeInfoTable.GetInstance().RegisterAllOpeToHotKey(GetHWnd());
            }
            else
            {
                ((Button)sender).Content = "Start Debug";
                grdDebugSet.Visibility = Visibility.Hidden;
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                grdMain.RowDefinitions[2].Height = (GridLength)gridLengthConverter.ConvertFrom("0");
                MakeEnableDgOpeList();

                OpeScript.GetInstance().Initialize();

                OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(GetHWnd());
            }
        }

        private void MakeDisenableDgOpeList()
        {
           foreach(DataGridRow row in GetDataGridRows(dgOpeList))
            {
                row.IsEnabled = false;
            }
        }

        private void MakeEnableDgOpeList()
        {
            foreach (DataGridRow row in GetDataGridRows(dgOpeList))
            {
                row.IsEnabled = true;
            }
        }

        private IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            TbxOutput.Clear();
        }

        private bool isBeingPressedWinKey = false;
        private IntPtr LLKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0)
            {
                if (((int)wParam & KeyboardHook.WM_KEYDOWN) == KeyboardHook.WM_KEYDOWN)
                {
                    KeyboardHook.KBDLLHOOKSTRUCT kbd
                        = (KeyboardHook.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(
                            lParam, typeof(KeyboardHook.KBDLLHOOKSTRUCT));

                    
                    if (kbd.vkCode == 0x5B || kbd.vkCode == 0x5C)
                    {
                        if (((int)kbd.flags & 0x80) != 0)
                        {
                            /* Right or left winkey was released. */
                            isBeingPressedWinKey = false;
                            return (IntPtr)0;
                        }
                        else
                        {
                            /* Right or left winkey was pressed. */
                            isBeingPressedWinKey = true;
                            return (IntPtr)0;
                        }
                    }
                }

            }

            return KeyboardHook.CallNextHookEx(hHandle, nCode, wParam, lParam);
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
}
