using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace Wallpaper_Engine
{
    class Wallpaper
    {
        private IntPtr hWndC;

        public Wallpaper(IntPtr hWndC)
        {
            this.hWndC = hWndC;
        }

        private delegate bool EnumWindowCallBack(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowCallBack lpEnumFunc, IntPtr lparam);

        [DllImport("user32.Dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        
        public void SetBottom()
        {
            EnumWindows(EnumerateWindow, IntPtr.Zero);
        }

        private bool EnumerateWindow(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder sbClassName = new StringBuilder(256);
            GetClassName(hWnd, sbClassName, 256);
            string className = sbClassName.ToString();
            if (className.Equals("WorkerW") || className.Equals("Progman"))
            {
                IntPtr hDt = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);

                if (hDt != null && hDt != IntPtr.Zero)
                {
                    IntPtr hWndP = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                    SetParent(hWndC, hWndP);
                }
            }
            return true;
        }
    }
}
