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
            OpeInfoTable.GetInstance().RegisterAllOpeToHotKey(Window.GetHWnd());

            ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;

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

                    OpeInfoTable.GetInstance().DoOpeScript(msg.wParam.ToInt32());
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
            OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(Window.GetHWnd());
            Window.MyShow();
        }

        public void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            OpeInfoTable.GetInstance().UnregisterAllOpeToHotKey(Window.GetHWnd());
            OpeScript.GetInstance().CloseLua();
            Application.Current.Shutdown();
        }
    }
}
