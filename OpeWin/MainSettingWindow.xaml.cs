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
using System.Windows.Controls.Primitives;
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
    /// Interaction Logics of MainSettingWindow.xaml
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

        private void dgOpeList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Style style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            dgOpeList.Columns[0].CellStyle = style;
            dgOpeList.Columns[3].Visibility = Visibility.Hidden;
            dgOpeList.Columns[4].Visibility = Visibility.Hidden;
            dgOpeList.Columns[5].Visibility = Visibility.Hidden;

            dgOpeList.Columns[0].IsReadOnly = true;
            dgOpeList.Columns[1].IsReadOnly = false;
            dgOpeList.Columns[2].IsReadOnly = false;
            dgOpeList.Columns[3].IsReadOnly = true;
            dgOpeList.Columns[4].IsReadOnly = true;
            
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
            if (dgOpeList.CurrentColumn.Header.ToString() != "HotKey")
                return;

            ModifierKeys modifierKeys = Keyboard.Modifiers;

            DataRowView selected_item = (DataRowView)dgOpeList.SelectedItem;

            if(isBeingPressedWinKey)
            {
                modifierKeys |= ModifierKeys.Windows;
            }

            if (HotKey.CanSet(e, modifierKeys))
            {
                OpeInfoTable.SetHotkey(e, modifierKeys, selected_item.Row);
                e.Handled = true;
            }
            else
            {
                if(HotKey.IsClearKey(e))
                {
                    OpeInfoTable.ClearHotKey(selected_item.Row);
                }

                if(modifierKeys == ModifierKeys.None && e.Key == Key.F2)
                {
                    e.Handled = false;
                    return;
                }

                e.Handled = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWithCancel();
            e.Cancel = true;
        }

        public void MyHide()
        {
            if(btnSimulation.IsChecked == true)
            {
                StopSimulation();
            }

            RegisterHotkeys();
            Hide();
        }

        public void MyShow()
        {
            UnregisterHotkeys();
            Show();
        }

        public void HideFromAltTabMenu()
        {
            MyHide();
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpeInfoTable.GetInstance().AddRowAsDefault();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

            if (dgOpeList.SelectedItem is DataRowView)
            {
                ((DataRowView)dgOpeList.SelectedItem).Row.Delete();
            }
        }

        private void btnSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)sender).IsChecked == true)
            {
                StartSimulation();
            }
            else
            {
                StopSimulation();
            }
        }

        private void StartSimulation()
        {
            MakeDisenableDgOpeList();
            RowDefinition row_def = new RowDefinition();
            GridLengthConverter gridLengthConverter = new GridLengthConverter();
            RDefSim.Height = (GridLength)gridLengthConverter.ConvertFrom("1*");
            grdDebugSet.Visibility = Visibility.Visible;

            OpeScript.GetInstance().Initialize(TbxOutput);

            OpeInfoTable.GetInstance().RegisterAllOpeToHotKey(GetHWnd());

            btnSimulation.IsChecked = true;
        }

        private void StopSimulation()
        {
            grdDebugSet.Visibility = Visibility.Hidden;
            GridLengthConverter gridLengthConverter = new GridLengthConverter();
            RDefSim.Height = (GridLength)gridLengthConverter.ConvertFrom("0");
            MakeEnableDgOpeList();

            OpeScript.GetInstance().Initialize();

            OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(GetHWnd());

            btnSimulation.IsChecked = false;

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWithCancel();
        }

        private void CloseWithOK()
        {
            UnregisterHotkeys();

            OpeInfoTable.GetInstance().Save();

            OpeScript.GetInstance().Initialize();

            MyHide();
        }

        private void CloseWithCancel()
        {
            UnregisterHotkeys();

            OpeInfoTable.GetInstance().RollBackBeforeEditing();

            // Update reference.
            dgOpeList.DataContext = OpeInfoTable.GetInstance();
            dgOpeList.Items.Refresh();

            OpeScript.GetInstance().Initialize();

            MyHide();
        }

        private void dgOpeList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column.Header.ToString() != "HotKey")
            {
                return;
            }

            if(e.EditingElement is TextBox)
            {
                if(((TextBox)e.EditingElement).Text.StartsWith("combo") == false)
                {
                    ((TextBox)e.EditingElement).Text = String.Format("combo({{}}, {0})", ComboKey.GetNextHighestPriority());
                    ((TextBox)e.EditingElement).CaretIndex = "combo({".Length;
                }
            }
        }

        private void dgOpeList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() != "HotKey")
            {
                return;
            }

            if(e.EditAction == DataGridEditAction.Cancel)
            {
                return;
            }

            if (e.EditingElement is TextBox)
            {
                if (OpeInfoTable.SetComboKey(((TextBox)e.EditingElement).Text, ((DataRowView)e.Row.DataContext).Row))
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }

            }
        }

        private void dgOpeList_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
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
