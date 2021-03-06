﻿using System;
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

        public int GetCount()
        {
            return Count;
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

        public void Inspect()
        {
            if(isSimulationMode())
            {
                Print(WindowController.Inspect());
            }
        }

        private bool isSimulationMode()
        {
            return TbxOutput != null;
        }

        public void Maximize()
        {
            if (isSimulationMode())
            {
                Print("Maximize()");
            }
            else
            {
                WindowController.Maximize();
            }
        }

        public void Minimize()
        {
            if (isSimulationMode())
            {
                Print("Minimize()");
            }
            else
            {
                WindowController.Minimize();
            }
        }

        public void Restore()
        {
            if (isSimulationMode())
            {
                Print("Restore()");
            }
            else
            {
                WindowController.Restore();
            }
        }

        public void MoveTo(double rate_x, double rate_y)
        {
            if (isSimulationMode())
            {
                Print("MoveTo(" + rate_x.ToString() + ", " + rate_y.ToString() + ")");
            }
            else
            {
                WindowController.Restore();
                WindowController.MoveTo(rate_x, rate_y);
            }
        }

        public void VSMoveTo(double rate_x, double rate_y)
        {
            if (isSimulationMode())
            {
                Print("VSMoveTo(" + rate_x.ToString() + ", " + rate_y.ToString() + ")");
            }
            else
            {
                WindowController.Restore();
                WindowController.VSMoveTo(rate_x, rate_y);
            }
        }

        public void MoveBy(double rate_x, double rate_y)
        {
            if (isSimulationMode())
            {
                Print("MoveBy(" + rate_x.ToString() + ", " + rate_y.ToString() + ")");
            }
            else
            {
                WindowController.Restore();
                WindowController.MoveBy(rate_x, rate_y);
            }
        }

        public void ResizeTo(double rate_width, double rate_height)
        {
            if (isSimulationMode())
            {
                Print("ResizeTo(" + rate_width.ToString() + ", " + rate_height.ToString() + ")");
            }
            else
            {
                WindowController.Restore();
                WindowController.ResizeTo(rate_width, rate_height);
            }
        }

        public void VSResizeTo(double rate_width, double rate_height)
        {
            if (isSimulationMode())
            {
                Print("VSResizeTo(" + rate_width.ToString() + ", " + rate_height.ToString() + ")");
            }
            else
            {
                WindowController.Restore();
                WindowController.VSResizeTo(rate_width, rate_height);
            }
        }

        public void ResizeBy(double rate_width, double rate_height)
        {
            if (isSimulationMode())
            {
                Print("ResizeBy(" + rate_width.ToString() + ", " + rate_height.ToString() + ")");
            }
            else
            {
                WindowController.Restore();
                WindowController.ResizeBy(rate_width, rate_height);
            }
        }

        public void ChangeMonitorFw()
        {
            if (isSimulationMode())
            {
                Print("ChangeMonitorFw()");
            }
            else
            {
                WindowController.ChangeMonitor(WindowController.Direction.FORWARD);
            }
        }

        public void ChangeMonitorBw()
        {
            if (isSimulationMode())
            {
                Print("ChangeMonitorBw()");
            }
            else
            {
                WindowController.ChangeMonitor(WindowController.Direction.BACKWORD);
            }
        }

        public void ResetCount()
        {
            Count = -1;
        }
    }
}
