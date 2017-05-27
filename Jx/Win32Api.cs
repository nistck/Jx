using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Jx
{
    public static class Win32Api
    {
        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref SYSTEM_INFO cpuinfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }


        public static SYSTEM_INFO GetSystemInfo()
        {
            SYSTEM_INFO info = new SYSTEM_INFO();
            GetSystemInfo(ref info);
            return info;
        }

        public static uint GetNumberOfProcessors()
        {
            SYSTEM_INFO info = GetSystemInfo();
            return info.dwNumberOfProcessors;
        }
    }
}
