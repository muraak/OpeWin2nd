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

        private static TextBox TbxOutput = null;

        public  static int Count { get; set; }
        public Queue<int> PrevOpeIds = new Queue<int>(5);

        private const int MAX_COUNT = 5;

        private Ope()
        {
            Initialize();
        }

        public static Ope GetInstance()
        {
            return Instance;
        }

        public void Initialize()
        {
            TbxOutput = null;
            Count = 0;
            PrevOpeIds.Clear();
        }

        public void Initialize(TextBox tbx_output)
        {
            TbxOutput = tbx_output;
            Count = 0;
            PrevOpeIds.Clear();
        }

        public void UpdateCount(int idx_of_sender)
        {
            if(PrevOpeIds.Count == 0)
            {
                Count = 0;

                return;
            }

            if(idx_of_sender == PrevOpeIds.Last<int>())
            {
                Count = (Count + 1) % MAX_COUNT;
            }
            else
            {
                Count = 0;
            }
        }

        public void EnqueuePrevId(int prev_id)
        {
            PrevOpeIds.Enqueue(prev_id);
        }

        public void Print(String input)
        {
            if(TbxOutput != null)
            {
                if (TbxOutput.Text != "")
                {
                    TbxOutput.Text += (Environment.NewLine + input);
                }
                else
                {
                    TbxOutput.Text += input;
                }

                TbxOutput.CaretIndex = TbxOutput.Text.Length;
                TbxOutput.ScrollToEnd();
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

            WindowController.MoveTo(rate_x, rate_y);
        }

        public void ResizeTo(double rate_width, double rate_height)
        {
            Print("ResizeTo("+rate_width.ToString()+", "+rate_height.ToString()+")");

            WindowController.ResizeTo(rate_width, rate_height);
        }

        public void ChangeMonitorFw()
        {
            Print("ChangeMonitorFw()");
        }

        public void ChangeMonitorBw()
        {
            Print("ChangeMonitorBw()");
        }

        public void ResetCount()
        {
            Count = -1;
        }
    }
}
