using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Jx.Win32Api
{
    public static class Shell32
    {
        public struct AT
        {
            public uint cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }
        private const int aQZ = 12;
        private const int aQz = 1;
        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "ShellExecuteEx")]
        private static extern bool A(ref Shell32.AT x);
        public static void ShellExecuteEx(string verb, string realFileName)
        {
            try
            {
                Shell32.AT a = default(Shell32.AT);
                a.cbSize = (uint)Marshal.SizeOf(typeof(Shell32.AT));
                a.fMask = 12u;
                a.lpVerb = verb;
                a.lpFile = realFileName;
                a.nShow = 1;
                Shell32.A(ref a);
            }
            catch (Exception)
            {
            }
        }
    }
}
