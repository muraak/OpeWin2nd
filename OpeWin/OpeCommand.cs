using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace OpeWin
{
    class Ope
    {
        // Singleton Instance
        private static Ope Instance = new Ope();

        private static TextBox TbxOputput = null;

        public  static int Count { get; set; }
        public int[] PrevOpeId = new int[5];

        private Ope()
        {
            Initialize();
        }

        public void Initialize()
        {
            TbxOputput = null;
            Count = 0;

            for (int i = 0; i < PrevOpeId.Length; i++)
            {
                PrevOpeId[i] = -1;
            }
        }

        public void Initialize(TextBox tbx_output)
        {
            TbxOputput = tbx_output;
            Count = 0;

            for (int i = 0; i < PrevOpeId.Length; i++)
            {
                PrevOpeId[i] = -1;
            }
        }

        public static Ope GetInstance()
        {
            return Instance;
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
