using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POEStashSorterModels
{

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }
        public RECT ToRectangle(POINT point)
        {
            return new RECT { Left = point.X, Top = point.Y, Right = Right - Left, Bottom = Bottom - Top };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }


    public static class ApplicationHelper
    {
        public static Process currentProcess;


        [DllImport("user32.dll")]
        private static extern
            bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern
            bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern
            bool IsIconic(IntPtr hWnd);

        public static bool OpenPathOfExile()
        {
            const int swRestore = 9;
            var arrProcesses = Process.GetProcessesByName("PathOfExile");
            if (arrProcesses.Length > 0)
            {
                currentProcess = arrProcesses[0];
                IntPtr hWnd = arrProcesses[0].MainWindowHandle;
                if (IsIconic(hWnd))
                    ShowWindowAsync(hWnd, swRestore);
                SetForegroundWindow(hWnd);
                return true;
            }
            else
            {
                arrProcesses = Process.GetProcessesByName("PathOfExileSteam");
                if (arrProcesses.Length > 0)
                {
                    currentProcess = arrProcesses[0];

                    IntPtr hWnd = arrProcesses[0].MainWindowHandle;
                    if (IsIconic(hWnd))
                        ShowWindowAsync(hWnd, swRestore);
                    SetForegroundWindow(hWnd);
                    return true;
                }
            }
            throw new Exception("Path Of Exile isn't running");
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hWnd, out RECT Rect);


        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);



        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ScreenToClient(IntPtr hWnd, out POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ClientToScreen(IntPtr hWnd, out POINT pt);



        public static RECT PathOfExileDimentions
        {
            get
            {
                //RECT clientRect = new RECT();
                //GetClientRect(currentProcess.MainWindowHandle, ref clientRect);

                //POINT point;
                //ScreenToClient(currentProcess.MainWindowHandle, out point);

                //RECT rect = new RECT();
                //rect.Left = point.X * -1 + clientRect.Left;
                //rect.Right = point.X * -1 + clientRect.Right;
                //rect.Top = point.Y * -1 + clientRect.Top;
                //rect.Bottom = point.Y * -1 + clientRect.Bottom;

                //return rect;
                RECT rect;
                POINT point;
                var handle = currentProcess.MainWindowHandle;
                GetClientRect(handle, out rect);
                ClientToScreen(handle, out point);
                return rect.ToRectangle(point);
            }
        }
        //Den buggede lidt :P MEGET^^ Men du trykkede os på den forkerte sorteings ting^^ <.< hvorfor er der to? Hvis den ene er forkert :P.. den
        //er bare ikke fikset helt endnu, den anden burde virke... tester lige igen..Lave lige en fail safe, såen så hvis det seker, breaker den.
    }
}
