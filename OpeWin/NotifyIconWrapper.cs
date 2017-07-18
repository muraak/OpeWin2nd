using System;
using System.ComponentModel;
using System.Windows;

namespace OpeWin
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            // Add click event to context menu items.
            this.toolStripMenuItem_Setting.Click += this.toolStripMenuItem_Setting_Click;
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void toolStripMenuItem_Setting_Click(object sender, EventArgs e)
        {
            var setting_window = new MainSettingWindow();
            setting_window.Show();
        }

        public void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            // exit this app
            Application.Current.Shutdown();
        }
    }
}
