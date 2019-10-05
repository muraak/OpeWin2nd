using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace OpeWin
{
    public partial class NotifyIconWrapper : Component
    {
        private MainSettingWindow Window;

        public NotifyIconWrapper()
        {
            InitializeComponent();

            Window = new MainSettingWindow();
            Window.HideFromAltTabMenu();
 
            OpeScript.GetInstance().Initialize();

            ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;

            //Add events to task tray icon.
            notifyIcon.DoubleClick += new EventHandler(TrayIcon_DoubleClicked);
            // Add click event to context menu items.
            this.toolStripMenuItem_Setting.Click += this.toolStripMenuItem_Setting_Click;
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;

        }

        private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg.message)
            {
                case WM_HOTKEY:
                    int id = msg.wParam.ToInt32();
                    int combo_id;
                    if (ComboKey.FindComboToFire(id, out combo_id))
                    {
                        OpeInfoTable.GetInstance().DoOpeScript(combo_id);
                    }
                    else
                    {
                        OpeInfoTable.GetInstance().DoOpeScript(id);
                    }
                    handled = true;
                    break;
            }
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void toolStripMenuItem_Setting_Click(object sender, EventArgs e)
        {
            OpenSetting();
        }

        public void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(Window.GetHWnd());
            //OpeScript.GetInstance().CloseLua();
            Application.Current.Shutdown();
        }

        private void OpenSetting()
        {
            OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(Window.GetHWnd());
            Window.MyShow();
        }

        private void TrayIcon_DoubleClicked(Object obj, EventArgs e)
        {
            OpenSetting();
        }
    
    }
}
