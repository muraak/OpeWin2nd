using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace OpeWin
{
    class WindowController
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        // size of a device name string
        private const int CCHDEVICENAME = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        struct DEVMODE
        {
            public const int CCHDEVICENAME = 32;
            public const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            [System.Runtime.InteropServices.FieldOffset(0)]
            public string dmDeviceName;
            [System.Runtime.InteropServices.FieldOffset(32)]
            public Int16 dmSpecVersion;
            [System.Runtime.InteropServices.FieldOffset(34)]
            public Int16 dmDriverVersion;
            [System.Runtime.InteropServices.FieldOffset(36)]
            public Int16 dmSize;
            [System.Runtime.InteropServices.FieldOffset(38)]
            public Int16 dmDriverExtra;
            [System.Runtime.InteropServices.FieldOffset(40)]
            public DM dmFields;

            [System.Runtime.InteropServices.FieldOffset(44)]
            Int16 dmOrientation;
            [System.Runtime.InteropServices.FieldOffset(46)]
            Int16 dmPaperSize;
            [System.Runtime.InteropServices.FieldOffset(48)]
            Int16 dmPaperLength;
            [System.Runtime.InteropServices.FieldOffset(50)]
            Int16 dmPaperWidth;
            [System.Runtime.InteropServices.FieldOffset(52)]
            Int16 dmScale;
            [System.Runtime.InteropServices.FieldOffset(54)]
            Int16 dmCopies;
            [System.Runtime.InteropServices.FieldOffset(56)]
            Int16 dmDefaultSource;
            [System.Runtime.InteropServices.FieldOffset(58)]
            Int16 dmPrintQuality;

            [System.Runtime.InteropServices.FieldOffset(44)]
            public POINTL dmPosition;
            [System.Runtime.InteropServices.FieldOffset(52)]
            public Int32 dmDisplayOrientation;
            [System.Runtime.InteropServices.FieldOffset(56)]
            public Int32 dmDisplayFixedOutput;

            [System.Runtime.InteropServices.FieldOffset(60)]
            public short dmColor; // See note below!
            [System.Runtime.InteropServices.FieldOffset(62)]
            public short dmDuplex; // See note below!
            [System.Runtime.InteropServices.FieldOffset(64)]
            public short dmYResolution;
            [System.Runtime.InteropServices.FieldOffset(66)]
            public short dmTTOption;
            [System.Runtime.InteropServices.FieldOffset(68)]
            public short dmCollate; // See note below!
            [System.Runtime.InteropServices.FieldOffset(70)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            [System.Runtime.InteropServices.FieldOffset(102)]
            public Int16 dmLogPixels;
            [System.Runtime.InteropServices.FieldOffset(104)]
            public Int32 dmBitsPerPel;
            [System.Runtime.InteropServices.FieldOffset(108)]
            public Int32 dmPelsWidth;
            [System.Runtime.InteropServices.FieldOffset(112)]
            public Int32 dmPelsHeight;
            [System.Runtime.InteropServices.FieldOffset(116)]
            public Int32 dmDisplayFlags;
            [System.Runtime.InteropServices.FieldOffset(116)]
            public Int32 dmNup;
            [System.Runtime.InteropServices.FieldOffset(120)]
            public Int32 dmDisplayFrequency;
        }

        struct POINTL
        {
            public Int32 x;
            public Int32 y;
        }

        /// <summary>
        /// Selects duplex or double-sided printing for printers capable of duplex printing. 
        /// </summary>
        internal enum DM : short
        {
            /// <summary>
            /// Unknown setting.
            /// </summary>
            DMDUP_UNKNOWN = 0,

            /// <summary>
            /// Normal (nonduplex) printing.
            /// </summary>
            DMDUP_SIMPLEX = 1,

            /// <summary>
            /// Long-edge binding, that is, the long edge of the page is vertical.
            /// </summary>
            DMDUP_VERTICAL = 2,

            /// <summary>
            /// Short-edge binding, that is, the long edge of the page is horizontal.
            /// </summary>
            DMDUP_HORIZONTAL = 3,
        }

        public struct RATE
        {
            public double x;
            public double y;
            public double width;
            public double height;
        }

        public static void MoveTo(double rate_x, double rate_y)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT dummy;
            int screen_num = GetCurtScreenRectAndReturnScreenNo(hWnd, out dummy);

            MoveTo(hWnd, screen_num, rate_x, rate_y);
        }

        private static void MoveTo(IntPtr hWnd, int screen_no, double rate_x, double rate_y)
        {
            RECT win_rect;
            GetWindowRect(hWnd, out win_rect);

            RECT screen_rect;
            GetScreenWorkAreaRect(screen_no, out screen_rect);

            RECT gap;
            CalcGap(hWnd, out gap);

            int x, y;
            int dummy = 0;

            if (rate_x >= 0.0 && rate_x <= 1.0)
            {
                x = screen_rect.left + RateToActualWidth(rate_x, screen_rect);
                x = x - gap.left;
            }
            else
            {
                x = win_rect.left;
            }

            if (rate_y >= 0.0 && rate_y <= 1.0)
            {
                y = screen_rect.top + RateToActualHeight(rate_y, screen_rect);
                y = y - gap.top;
            }
            else
            {
                y = win_rect.top;
            }

            SetWindowPos(
                hWnd,
                HWND_TOP,
                x, y,
                dummy, dummy,
                SWP_NOZORDER | SWP_NOSIZE);
        }

        public static void VSMoveTo(double rate_x, double rate_y)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT dummy;
            int screen_num = GetCurtScreenRectAndReturnScreenNo(hWnd, out dummy);

            VSMoveTo(hWnd, screen_num, rate_x, rate_y);
        }

        private static void VSMoveTo(IntPtr hWnd, int screen_no, double rate_x, double rate_y)
        {
            RECT win_rect;
            GetWindowRect(hWnd, out win_rect);

            RECT gap;
            CalcGap(hWnd, out gap);

            int x, y;

            if (rate_x >= 0.0 && rate_x <= 1.0)
            {
                x = GetSystemMetrics(SM_XVIRTUALSCREEN) + (int)(GetSystemMetrics(SM_CXVIRTUALSCREEN) * rate_x);
                x = x - gap.left;
            }
            else
            {
                x = win_rect.left;
            }

            if (rate_y >= 0.0 && rate_y <= 1.0)
            {
                y = GetSystemMetrics(SM_YVIRTUALSCREEN) + (int)(GetSystemMetrics(SM_CYVIRTUALSCREEN) * rate_y);
                y = y - gap.top;
            }
            else
            {
                y = win_rect.top;
            }

            int dummy = 0;

            SetWindowPos(
                hWnd, HWND_TOP,
                x,y,
                dummy, dummy,
                SWP_NOZORDER | SWP_NOSIZE);
        }

        public static void MoveBy(double rate_x, double rate_y)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT dummy;
            int screen_num = GetCurtScreenRectAndReturnScreenNo(hWnd, out dummy);

            MoveBy(hWnd, screen_num, rate_x, rate_y);
        }

        private static void MoveBy(IntPtr hWnd, int screen_no, double rate_x, double rate_y)
        {
            RECT win_rect;
            GetWindowRect(hWnd, out win_rect);

            RECT screen_rect;
            GetScreenWorkAreaRect(screen_no, out screen_rect);

            RECT gap;
            CalcGap(hWnd, out gap);

            int x, y;
            int dummy = 0;

            if (rate_x >= -1.0 && rate_x <= 1.0)
            {
                x = win_rect.left + RateToActualWidth(rate_x, screen_rect);
            }
            else
            {
                x = win_rect.left;
            }

            if (rate_y >= -1.0 && rate_y <= 1.0)
            {
                y = win_rect.top + RateToActualHeight(rate_y, screen_rect);
            }
            else
            {
                y = win_rect.top;
            }

            SetWindowPos(
                hWnd,
                HWND_TOP,
                x, y,
                dummy, dummy,
                SWP_NOZORDER | SWP_NOSIZE);
        }

        public static void ResizeTo(double rate_width, double rate_height)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT dummy;
            int screen_no = GetCurtScreenRectAndReturnScreenNo(hWnd, out dummy);

            ResizeTo(hWnd, screen_no, rate_width, rate_height);
        }

        public static void ResizeTo(IntPtr hWnd, int screen_no, double rate_width, double rate_height)
        {
            RECT win_rect;
            GetWindowRect(hWnd, out win_rect);

            RECT screen_rect;
            GetScreenWorkAreaRect(screen_no, out screen_rect);

            RECT gap;
            CalcGap(hWnd, out gap);

            int width, height;
            int dummy = 0;

            if (rate_width >= 0.0 && rate_width <= 1.0)
            {
                width = RateToActualWidth(rate_width, screen_rect);
                width = width + (gap.left + gap.right);
            }
            else
            {
                width = win_rect.right - win_rect.left;
            }

            if (rate_height >= 0.0 && rate_height <= 1.0)
            {
                height = RateToActualHeight(rate_height, screen_rect);
                height = height + (gap.top + gap.bottom);
            }
            else
            {
                height = win_rect.bottom - win_rect.top;
            }

            SetWindowPos(
                hWnd,
                HWND_TOP,
                dummy, dummy,
                width, height,
                SWP_NOZORDER | SWP_NOMOVE);
        }

        public static void VSResizeTo(double rate_width, double rate_height)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT dummy;
            int screen_no = GetCurtScreenRectAndReturnScreenNo(hWnd, out dummy);

            VSResizeTo(hWnd, screen_no, rate_width, rate_height);
        } 

        private static void VSResizeTo(IntPtr hWnd, int screen_no, double rate_width, double rate_height)
        {
            RECT win_rect;
            GetWindowRect(hWnd, out win_rect);

            RECT gap;
            CalcGap(hWnd, out gap);

            int width, height;

            if (rate_width >= 0.0 && rate_width <= 1.0)
            {
                width = (int)(GetSystemMetrics(SM_CXVIRTUALSCREEN) * rate_width);
                width = width + (gap.left + gap.right);
            }
            else
            {
                width = win_rect.right - win_rect.left;
            }

            if (rate_height >= 0.0 && rate_height <= 1.0)
            {
                height = (int)(GetSystemMetrics(SM_CYVIRTUALSCREEN) * rate_height);
                height = height + (gap.top + gap.bottom);
            }
            else
            {
                height = win_rect.bottom - win_rect.top;
            }

            int dummy = 0;

            SetWindowPos(
                hWnd, HWND_TOP,
                dummy, dummy,
                width, height,
                SWP_NOZORDER | SWP_NOMOVE);
        }

        public static void ResizeBy(double rate_width, double rate_height)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT dummy;
            int screen_no = GetCurtScreenRectAndReturnScreenNo(hWnd, out dummy);

            ResizeBy(hWnd, screen_no, rate_width, rate_height);
        }

        public static void ResizeBy(IntPtr hWnd, int screen_no, double rate_width, double rate_height)
        {
            RECT win_rect;
            GetWindowRect(hWnd, out win_rect);

            RECT screen_rect;
            GetScreenWorkAreaRect(screen_no, out screen_rect);

            RECT gap;
            CalcGap(hWnd, out gap);

            int width, height;
            int dummy = 0;

            if (rate_width >= -1.0 && rate_width <= 1.0)
            {
                width = (win_rect.right - win_rect.left) + RateToActualWidth(rate_width, screen_rect);
            }
            else
            {
                width = win_rect.right - win_rect.left;
            }

            if (rate_height >= -1.0 && rate_height <= 1.0)
            {
                height = (win_rect.bottom - win_rect.top) + RateToActualHeight(rate_height, screen_rect);
            }
            else
            {
                height = win_rect.bottom - win_rect.top;
            }

            SetWindowPos(
                hWnd,
                HWND_TOP,
                dummy, dummy,
                width, height,
                SWP_NOZORDER | SWP_NOMOVE);
        }

        public static void Maximize()
        {
            IntPtr hWnd_top = GetForegroundWindow();
            ShowWindow(hWnd_top, SW_MAXIMIZE);
        }

        public static void Minimize()
        {
            IntPtr hWnd_top = GetForegroundWindow();
            ShowWindow(hWnd_top, SW_MINIMIZE);
        }

        public static void Restore()
        {
            IntPtr hWnd_top = GetForegroundWindow();
            ShowWindow(hWnd_top, SW_RESTORE);
        }

        public enum Direction
        {
            FORWARD,
            BACKWORD
        }

        public static void ChangeMonitor(Direction direction)
        {
            IntPtr hWnd_top = GetForegroundWindow();

            RECT win_rect;

            bool isMaxmized = IsMaximized(hWnd_top);
            if (isMaxmized) Restore(); // without Restore(), such as MoveTo(), ResizeTo() etc.. don't work... 
            
            if (IsLaterWin10() == true)
            {
                DwmGetWindowAttribute(
                    hWnd_top, DWMWA_EXTENDED_FRAME_BOUNDS,
                    out win_rect, Marshal.SizeOf(typeof(RECT)));
            }
            else
            {
                GetWindowRect(hWnd_top, out win_rect);
            }

            RECT screen_rect;
            int curt_screen_no;

            curt_screen_no = GetCurtScreenRectAndReturnScreenNo(hWnd_top, out screen_rect);

            RATE win_rate;
            RectToRate(win_rect, screen_rect, out win_rate);

            int tgt_screen_no;
            if (direction == Direction.FORWARD)
            {
                tgt_screen_no = GetNextScreenNum(curt_screen_no);
            }
            else
            {
                tgt_screen_no = GetPrevScreenNum(curt_screen_no);
            }

            MoveTo(hWnd_top, tgt_screen_no, win_rate.x, win_rate.y);
            ResizeTo(hWnd_top, tgt_screen_no, win_rate.width, win_rate.height);

            if(isMaxmized)
            {
                Maximize();
            }
        }

        public static string Inspect()
        {
            string info = "";

            info += "【Virtual Screen Information】" + System.Environment.NewLine;
            info += String.Format("width:  {0}", GetSystemMetrics(SM_CXVIRTUALSCREEN)) + System.Environment.NewLine;
            info += String.Format("height: {0}", GetSystemMetrics(SM_CYVIRTUALSCREEN)) + System.Environment.NewLine;
            info += String.Format("Num of Screen: {0}", Screen.AllScreens.Length) + System.Environment.NewLine;

            IntPtr hWnd = GetForegroundWindow();
            RECT screen_rect;
            int screen_num = GetCurtScreenRectAndReturnScreenNo(hWnd, out screen_rect);
            GetWindowRect(hWnd, out screen_rect);
            RECT rect_correct;
            DwmGetWindowAttribute(
                hWnd,
                DWMWA_EXTENDED_FRAME_BOUNDS,
                out rect_correct,
                Marshal.SizeOf(typeof(RECT)));
            RECT gap;
            CalcGap(hWnd, out gap);

            info += "【Current Screen Information】" + System.Environment.NewLine;
            info += String.Format(" No: {0}", screen_num) + System.Environment.NewLine;
            info += String.Format(" RECT(c#_wa): (left: {0}, right: {1}, top: {2}, bottom: {3})",
                Screen.AllScreens[screen_num].WorkingArea.Left, Screen.AllScreens[screen_num].WorkingArea.Right,
                Screen.AllScreens[screen_num].WorkingArea.Top, Screen.AllScreens[screen_num].WorkingArea.Bottom) + System.Environment.NewLine;
            info += "【Current Window Information】" + System.Environment.NewLine;
            info += String.Format(" RECT(usr32): (left: {0}, right: {1}, top: {2}, bottom: {3})",
                screen_rect.left, screen_rect.right, screen_rect.top, screen_rect.bottom) + System.Environment.NewLine;
            info += String.Format(" RECT(dwm): (left: {0}, right: {1}, top: {2}, bottom: {3})",
                rect_correct.left, rect_correct.right, rect_correct.top, rect_correct.bottom) + System.Environment.NewLine;
            POINT point_start;
            POINT point_end;
            point_start.x = rect_correct.left;
            point_start.y = rect_correct.top;
            point_end.x = rect_correct.right;
            //point_end.y = rect_correct.bottom;
            //PhysicalToLogicalPointForPerMonitorDPI(hWnd, ref point_start);
            //PhysicalToLogicalPointForPerMonitorDPI(hWnd, ref point_end);
            //info += String.Format(" RECT(dwm_p2l): (left: {0}, right: {1}, top: {2}, bottom: {3})",
            //    point_start.x, point_end.x, point_start.y, point_end.y) + System.Environment.NewLine;
            //info += String.Format(" Gap: (left: {0}, right: {1}, top: {2}, bottom: {3})",
            //    gap.left, gap.right, gap.top, gap.bottom) + System.Environment.NewLine;

            return info;
        }

        private static int GetNextScreenNum(int curt_screen_no)
        {
            if ((curt_screen_no + 1) < Screen.AllScreens.Length - 1)
            {
                return (curt_screen_no + 1);
            }
            else
            {
                return Screen.AllScreens.Length - 1;
            }
        }

        private static int GetPrevScreenNum(int curt_screen_no)
        {
            if ((curt_screen_no - 1) >= 0)
            {
                return (curt_screen_no - 1);
            }
            else
            {
                return 0;
            }
        }

        private static void RectToRate(RECT win_rect, RECT screen_rect, out RATE win_rate)
        {
            win_rate = new RATE();

            win_rate.x = ((double)win_rect.left - screen_rect.left) 
                            / ((double)screen_rect.right - screen_rect.left);
            win_rate.y = ((double)win_rect.top - screen_rect.top)
                            / ((double)screen_rect.bottom - screen_rect.top);
            win_rate.width = ((double)win_rect.right - win_rect.left)
                            / ((double)screen_rect.right - screen_rect.left);
            win_rate.height = ((double)win_rect.bottom - win_rect.top)
                            / ((double)screen_rect.bottom - screen_rect.top);
        }

        private static void CommonProcForMoveAndResize(
            IntPtr hWnd,
            double rate_x, double rate_y,
            out int x, out int y,
            out RECT gap)
        {
            RECT curt_screen_rect;
            GetCurtScreenRectAndReturnScreenNo(hWnd, out curt_screen_rect);

            x = RateToActualWidth(rate_x, curt_screen_rect);
            y = RateToActualHeight(rate_y, curt_screen_rect);

            CalcGap(hWnd, out gap);
        }

        private static int RateToActualWidth(double rate, RECT screen_rect)
        {
            if (rate < -1.0) rate = 0.0;
            if (rate > 2.0) rate = 1.0;
            return (int)((double)(screen_rect.right - screen_rect.left) * rate);
        }

        private static int RateToActualHeight(double rate, RECT screen_rect)
        {
            if (rate < -1.0) rate = 0.0;
            if (rate > 2.0) rate = 1.0;
            return (int)((double)(screen_rect.bottom - screen_rect.top) * rate);
        }

        public static void CheckRateThrowsArgumentException(ref double rate_x, ref double rate_y) 
        {
            if (rate_x < 0.0 || rate_y < 0.0)
            {
                throw new ArgumentException();
            }

            rate_x = (rate_x <= 1.0) ? rate_x : 1.0;
            rate_y = (rate_y <= 1.0) ? rate_y : 1.0;
        }

        protected static int GetCurtScreenRectAndReturnScreenNo(IntPtr hWnd, out RECT rect)
        {
            RECT windowRect;
            GetWindowRect(hWnd, out windowRect);

            rect = new RECT();

            int overlapWidth = 0;
            int overlapHeight = 0;
            int overlapArea = 0;
            int maxOverlapArea = 0;

            int screen_num = 0;
            int idx = 0;

            foreach (Screen screen in Screen.AllScreens)
            {
                /* Calculate the overlap area between window and each monitors. */
                overlapWidth = Math.Min(screen.WorkingArea.Right, windowRect.right)
                                - Math.Max(screen.WorkingArea.X, windowRect.left);
                overlapHeight = Math.Min(screen.WorkingArea.Bottom, windowRect.bottom)
                                - Math.Max(screen.WorkingArea.Y, windowRect.top);

                if ((overlapWidth <= 0) || (overlapHeight <= 0))
                {
                    overlapArea = 0; // There is no overlap area.
                }
                else
                {
                    overlapArea = overlapWidth * overlapHeight;
                }

                /* Set the rect of monitor that has largest overlap area.   */
                /* We choose small number monitor if threr are several      */
                /* monitor having same overlap area.                        */
                if (overlapArea > maxOverlapArea)
                {
                    maxOverlapArea = overlapArea;

                    rect.left = screen.WorkingArea.X;
                    rect.top = screen.WorkingArea.Y;
                    rect.right = screen.WorkingArea.Right;
                    rect.bottom = screen.WorkingArea.Bottom;

                    screen_num = idx;
                }

                idx++;
            }
            
            /* If there are no overlap area, we set the primary monitor's rect. */
            if (maxOverlapArea == 0)
            {
                rect.left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.X;
                rect.top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Y;
                rect.right = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right;
                rect.bottom = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom;

                screen_num = 0;
            }

            return screen_num;
        }

        private static void CalcGap(IntPtr hWnd, out RECT gap)
        {
            if (IsLaterWin10() != true)
            {
                gap.left = 0;
                gap.top = 0;
                gap.right = 0;
                gap.bottom = 0;

                return;
            }

            RECT rect_correct = new RECT();
            DwmGetWindowAttribute(
                hWnd,
                DWMWA_EXTENDED_FRAME_BOUNDS,
                out rect_correct,
                Marshal.SizeOf(typeof(RECT)));

            RECT rect_gapped = new RECT();
            GetWindowRect(hWnd, out rect_gapped);

            Console.WriteLine(
                String.Format("DwmGetWindowAttribute:(left:{0} top:{1} right:{2} bottom:{3} )",
                rect_correct.left, rect_correct.top, rect_correct.right, rect_correct.bottom));
            Console.WriteLine(
                String.Format("GetWindowRect:(left:{0} top:{1} right:{2} bottom:{3} )",
                rect_gapped.left, rect_gapped.top, rect_gapped.right, rect_gapped.bottom));

            POINT start = new POINT();
            POINT end = new POINT();
            start.x = rect_correct.left;
            start.y = rect_correct.top;
            end.x = rect_correct.right;
            end.y = rect_correct.bottom;

            IntPtr startPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(start));
            IntPtr endPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(end));

            bool res1 = PhysicalToLogicalPointForPerMonitorDPI(hWnd, startPtr);
            bool res2 = PhysicalToLogicalPointForPerMonitorDPI(hWnd, endPtr);

            Console.WriteLine(
                String.Format("DwmGetWindowAttribute_P2L:(left:{0} top:{1} right:{2} bottom:{3} )",
                    start.x, start.y, end.x, end.y));

            //rect_correct.left = start.x;
            //rect_correct.top = start.y;
            //rect_correct.right = end.x;
            //rect_correct.bottom = end.y;

            int scale = 0;

            int res3 = GetScaleFactorForMonitor(hWnd, out scale);
            Console.WriteLine("scale: {0}", scale);
            double horzScale, vertScale;
            calcScaleFactorForMonitor(hWnd, out horzScale, out vertScale);

            gap.left = rect_correct.left - rect_gapped.left;
            gap.top = rect_correct.top - rect_gapped.top;
            gap.right = rect_gapped.right - rect_correct.right;
            gap.bottom = rect_gapped.bottom - rect_correct.bottom;
        }

        private static void calcScaleFactorForMonitor(IntPtr hWnd, out double horzScale, out double vertScale)
        {
            // Get the monitor that the window is currently displayed on
            // (where hWnd is a handle to the window of interest).
            IntPtr hMonitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);

            // Get the logical width and height of the monitor.
            MONITORINFOEX miex = new MONITORINFOEX();
            GetMonitorInfo(hMonitor, miex);
            int cxLogical = (miex.rcMonitor.right - miex.rcMonitor.left);
            int cyLogical = (miex.rcMonitor.bottom - miex.rcMonitor.top);

            Console.WriteLine("logical: (width:{0}, height:{1}", cxLogical, cyLogical);

            //// Get the physical width and height of the monitor.
            //DEVMODE dm = new DEVMODE();
            //dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            //dm.dmDriverExtra = 0;
            //EnumDisplaySettingsA(miex.szDevice.ToString(), ENUM_CURRENT_SETTINGS, ref dm);
            //int cxPhysical = dm.dmPelsWidth;
            //int cyPhysical = dm.dmPelsHeight;

            //// Calculate the scaling factor.
            //horzScale = ((double)cxPhysical / (double)cxLogical);
            //vertScale = ((double)cyPhysical / (double)cyLogical);

            horzScale = 0;
            vertScale = 0;
        }

        private static bool IsLaterWin10()
        {
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);

            if (regkey == null)
            {
                return false;
            }

            try
            {
                // The key "CurrentMajorVersionNumber" was added from win10.
                // So we can distinct later version by checking whether this key is exist or not.
                // If this key is't exsist, below method will throw the NullReferenceException.
                int major_version = (int)regkey.GetValue("CurrentMajorVersionNumber");


                regkey.Close();

                if (major_version >= 10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(NullReferenceException e)
            {
                regkey.Close();
                return false;
            }
        }

        private static void GetScreenWorkAreaRect(int screen_no, out RECT screen_rect)
        {
            screen_rect.left = Screen.AllScreens[screen_no].WorkingArea.Left;
            screen_rect.right = Screen.AllScreens[screen_no].WorkingArea.Right;
            screen_rect.top = Screen.AllScreens[screen_no].WorkingArea.Top;
            screen_rect.bottom = Screen.AllScreens[screen_no].WorkingArea.Bottom;
        }

        private static bool IsMaximized(IntPtr hWnd)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            GetWindowPlacement(hWnd, ref wp);

            return wp.showCmd == SW_MAXIMIZE;
        }

        /* See "https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx" *
         * about this function.                                                                  */
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd, int hWndInsertAfter,
            int x, int y, int cx, int cy, 
            int uFlags);
        // constants for param "hWndInsertAfter"
        public const int HWND_NOTOPMOST = -2;
        public const int HWND_TOPMOST = -1;
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        // constants for param "uFlags"
        // INFO: You can choose several options by using "|" operation.
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004; // Choose this normaly.
        public const int SWP_NOACTIVATE = 0x0010; // NOTICE: If you use this, you won't be able to make other windows on top from then acording to my experiment. 
        public const int SWP_SHOWWINDOW = 0x0040; 
        public const int SWP_HIDEWINDOW = 0x0080;

        /* See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633515(v=vs.85).aspx *
         * about this function.                                                                */
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        // constants for param "uCmd"
        public const int GW_HWNDNEXT = 2; // Get the next window's handle from hWnd on z-order.

        /* I believe you can use this without any information! */
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /* I believe you can use this without any information! */
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(
            IntPtr hwnd,
            int dwAttribute,
            out RECT pvAttribute,
            int cbAttribute);
        // constants for 2nd parametor
        public const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        /* I believe you can use this without any information! */
        [DllImport("user32.dll")]
        public static extern bool PhysicalToLogicalPointForPerMonitorDPI(
            IntPtr hWnd,
            IntPtr lpPoint);
        /* I believe you can use this without any information! */
        [DllImport("user32.dll")]
        public static extern bool LogicalToPhysicalPointForPerMonitorDPI(
            IntPtr hWnd,
            ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);
        // constants for 2nd parametor
        const int MONITOR_DEFAULTTONULL = 0;
        const int MONITOR_DEFAULTTOPRIMARY = 1;
        const int MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out]MONITORINFOEX lpmi);

        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettingsA(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);
        const int ENUM_CURRENT_SETTINGS = -1;
        const int ENUM_REGISTRY_SETTINGS = -2;


        [DllImport("Shcore.dll")]
        public static extern int GetScaleFactorForMonitor(
            IntPtr hMon,
            out int pScale);

        /* See https://msdn.microsoft.com/ja-jp/library/windows/desktop/ms633548(v=vs.85).aspx */
        /* about this function.                                                                */
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // constants for 2nd parametor
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(
            IntPtr hWnd,
            ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public RECT rcNormalPosition;
        }

        /* See：https://msdn.microsoft.com/ja-jp/library/windows/desktop/ms724385(v=vs.85).aspx */
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);
        // constants for first parametor
        public const int SM_XVIRTUALSCREEN = 76; //仮想画面の左端を取得する場合に指定
        public const int SM_YVIRTUALSCREEN = 77; //仮想画面の上端を取得する場合に指定
        public const int SM_CXVIRTUALSCREEN = 78; //仮想画面の幅を取得する場合に指定
        public const int SM_CYVIRTUALSCREEN = 79; //仮想画面の高さを取得する場合に指定
    }
}
