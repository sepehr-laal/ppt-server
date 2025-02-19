﻿namespace Helios
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value + _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
    }

    public static class User32
    {
        const string DllName = "user32.dll";

        public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public enum PrintWindowFlags : uint
        {
            PW_ALL = 0x00000000,
            PW_CLIENTONLY = 0x00000001
        }

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, PrintWindowFlags nFlags);

        [DllImport(DllName)]
        public static extern int EnumWindows(EnumWindowsCallback cb, int lPar);

        [DllImport(DllName)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        public enum WindowMessages : uint
        {
            WM_COMMAND = 0x0111,
            WM_IME_NOTIFY = 0x0282
        }

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, WindowMessages uMsg, IntPtr wParam, IntPtr lParam);

        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx"/>
        [Flags]
        enum SetWindowPosFlags : uint
        {
            SWP_NOMOVE = 0x0002,
            SWP_NOSIZE = 0x0001,
            SWP_SHOWWINDOW = 0x0040
        }

        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx"/>
        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx"/>
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633539(v=vs.85).aspx"/>
        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(System.IntPtr hWnd);

        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633505(v=vs.85).aspx"/>
        [DllImport(DllName)]
        static extern System.IntPtr GetForegroundWindow();

        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633553(v=vs.85).aspx"/>
        [DllImport(DllName, SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public static void ActivateWindow(IntPtr window)
        {
            if (GetForegroundWindow() != window)
            {
                SwitchToThisWindow(window, true);
                SetForegroundWindow(window);

                // NOTE: the following is taken from Phoenix' code
                // after calling it, the window will never leave
                // foreground. Call it only if the above are not working.
                /*User32.SetWindowPos(
                    RenderWindowHwnd,
                    User32.HWND_TOPMOST,
                    0, 0, 0, 0,
                    User32.SetWindowPosFlags.SWP_NOSIZE |
                    User32.SetWindowPosFlags.SWP_NOMOVE |
                    User32.SetWindowPosFlags.SWP_SHOWWINDOW);*/
            }
        }

        [DllImport(DllName)]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        public static void InvalidateWindow(IntPtr window)
        {
            RedrawWindow(window, IntPtr.Zero, IntPtr.Zero, 0x0400/*RDW_FRAME*/ | 0x0100/*RDW_UPDATENOW*/ | 0x0001/*RDW_INVALIDATE*/);
        }
    }
}
