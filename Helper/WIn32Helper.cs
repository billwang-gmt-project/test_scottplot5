using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Helper
{
    public class WIn32Helper
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        public const uint MF_GRAYED = 0x00000001;
        public const uint MF_ENABLED = 0x00000000;
        public const uint SC_CLOSE = 0xF060;

        public static void EnableCloseButton(Window? window, bool enable)
        {
            if (window == null)
                return;
            window.Dispatcher.Invoke(new Action(() =>
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                if (enable)
                    EnableMenuItem(hMenu, SC_CLOSE, MF_ENABLED);
                else
                    EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
            }));
        }


        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string? cls, string win);
        [DllImport("user32")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        public static void ActivateWindow(string windowname)
        {
            var otherWindow = FindWindow(null, windowname);
            if (otherWindow != IntPtr.Zero)
            {
                SetForegroundWindow(otherWindow);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 SWP_NOACTIVATE = 0x0010;

    }
}
