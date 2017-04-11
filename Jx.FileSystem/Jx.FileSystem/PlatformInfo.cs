using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Jx.FileSystem
{
    internal static class PlatformInfo
    {
        public enum PlanformType
        {
            Windows,
            MacOSX,
            Android
        }
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct a
        {
            [DllImport("AndroidAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AndroidAppNativeWrapper_IsAndroid")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool IsAndroid();
        }

        private static PlanformType platformType;
        public static PlanformType Platform
        {
            get
            {
                return platformType;
            }
        }
        static PlatformInfo()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                platformType = PlanformType.MacOSX;
                try
                {
                    if (a.IsAndroid())
                    {
                        platformType = PlanformType.Android;
                    }
                    return;
                }
                catch
                {
                    return;
                }
            }
            platformType = PlanformType.Windows;
        }
    }
}
