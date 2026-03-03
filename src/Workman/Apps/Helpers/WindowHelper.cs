using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Workman.Apps.Helpers
{
    public static class WindowHelper
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int GWLP_HWNDPARENT = -8;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const uint SWP_NOZORDER = 0x0004;
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 挂载钩子，防止窗口状态被系统改变（如 Win+D 触发的排序改变）
        /// </summary>
        public static void SetWindowHook(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
        }
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_WINDOWPOSCHANGING)
            {
                // 强制让窗口保持在最底层，不响应系统带来的层级改变
                WINDOWPOS wp = Marshal.PtrToStructure<WINDOWPOS>(lParam);
                wp.hwndInsertAfter = HWND_BOTTOM;
                wp.flags &= ~(uint)SWP_NOZORDER;
                Marshal.StructureToPtr(wp, lParam, false);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 设置窗口置底
        /// </summary>
        public static void SetWindowDownmost(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        /// <summary>
        /// 将窗口的所有者设为桌面 Progman
        /// 这能防止 Win+D 最小化，同时保留输入焦点
        /// </summary>
        /// <param name="window"></param>
        public static void SetWindowParentToProgman(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            IntPtr progman = FindWindow("Progman", null);
            SetWindowLongPtr(hwnd, GWLP_HWNDPARENT, progman);
        }

        /// <summary>
        /// 设置为 ToolWindow (不在 Alt+Tab 显示)
        /// </summary>
        public static void SetWindowToolStyle(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
        }
    }
}
