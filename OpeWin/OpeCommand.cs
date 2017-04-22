using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace OpeWin
{
    class OpeCommand
    {
        private TextBox TbxOputput = null;

        public int Count { get; set; }
        public int[] PrevOpeId = new int[5];

        public OpeCommand()
        {

        }

        public OpeCommand(TextBox output)
        {
            this.TbxOputput = output;
        }

        public void Print(String input)
        {
            if(TbxOputput != null)
            {
                TbxOputput.Text += (input + Environment.NewLine);
            }
        }

        public void Maximize()
        {
            Print("Maximize()");
        }

        public void Minimize()
        {
            Print("Minimize()");
        }

        public void MoveTo(double rate_x, double rate_y)
        {
            Print("MoveTo("+rate_x.ToString()+", "+rate_y.ToString()+")");
        }

        public void ResizeTo(double rate_width, double rate_height)
        {
            Print("ResizeTo("+rate_width.ToString()+", "+rate_height.ToString()+")");
        }

        public void ChangeMonitorFw()
        {
            Print("ChangeMonitorFw()");
        }

        public void ChangeMonitorBw()
        {
            Print("ChangeMonitorBw()");
        }
    }
}
