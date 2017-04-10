using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace A
{
	internal class H : E
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "GetModuleFileName")]
		private static extern int A(IntPtr p1, StringBuilder p2, int p3);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "LoadLibrary")]
		private static extern IntPtr A(string p);
		public override string GetExecutableDirectoryPath()
		{
			string directoryName;
			try
			{
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
				directoryName = Path.GetDirectoryName(fileName);
			}
			catch
			{
				Module m = Assembly.GetExecutingAssembly().GetModules()[0];
				IntPtr intPtr = Marshal.GetHINSTANCE(m);
				if (intPtr == new IntPtr(-1))
				{
					intPtr = IntPtr.Zero;
				}
				StringBuilder stringBuilder = new StringBuilder(260);
				H.A(intPtr, stringBuilder, stringBuilder.Capacity);
				directoryName = Path.GetDirectoryName(Path.GetFullPath(stringBuilder.ToString()));
			}
			return directoryName;
		}
		public override IntPtr LoadLibrary(string path)
		{
			return H.A(path);
		}
	}
	internal class h : E
	{
		[DllImport("MacAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "MacAppNativeWrapper_LoadLibrary")]
		public static extern IntPtr MacLoadLibrary(string name);
		public override string GetExecutableDirectoryPath()
		{
			string codeBase = Assembly.GetCallingAssembly().CodeBase;
			return Path.GetDirectoryName(codeBase.Replace("file://", ""));
		}
		public override IntPtr LoadLibrary(string path)
		{
			return h.MacLoadLibrary(path);
		}
	}
}
