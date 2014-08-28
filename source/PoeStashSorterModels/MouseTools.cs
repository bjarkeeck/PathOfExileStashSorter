using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace POEStashSorterModels
{
    class MouseTools
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Vector2 GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Vector2(w32Mouse.X, w32Mouse.Y);
        }


        public static void MoveCursor(Vector2 p1, Vector2 p2, int step = 3)
        {
            Vector2 start = new Vector2((float)p1.X, (float)p1.Y);
            Vector2 end = new Vector2((float)p2.X, (float)p2.Y);
            Vector2 currentPos = start;

            float distance = Vector2.Distance(start, end);
            float angle = Vector2.Angle(start, end);

            for (float i = 0; i <= 200; i += step)
            {
                float factor = i / 200f;
                //factor = 0.000001f * (float)Math.Pow((100 - factor * 100) - 100, 4) / 100;

                float addDistance = distance * factor;


                currentPos = start + new Vector2((float)Math.Cos(angle) * addDistance, (float)Math.Sin(angle) * addDistance);

                SetCursorPos((int)currentPos.X, (int)currentPos.Y);
                Thread.Sleep(4);
            }
        }

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        private static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseClickEvent()
        {
            MouseTools.MouseEvent(MouseTools.MouseEventFlags.LeftDown);
            Thread.Sleep(70);
            MouseTools.MouseEvent(MouseTools.MouseEventFlags.LeftUp);

        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

        }

    }
}
