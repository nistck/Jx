using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Jx
{
    public static class Shell32Api
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
        private static extern bool A(ref Shell32Api.AT x);
        public static void ShellExecuteEx(string verb, string realFileName)
        {
            try
            {
                Shell32Api.AT a = default(Shell32Api.AT);
                a.cbSize = (uint)Marshal.SizeOf(typeof(Shell32Api.AT));
                a.fMask = 12u;
                a.lpVerb = verb;
                a.lpFile = realFileName;
                a.nShow = 1;
                Shell32Api.A(ref a);
            }
            catch (Exception)
            {
            }
        }
    }
}
