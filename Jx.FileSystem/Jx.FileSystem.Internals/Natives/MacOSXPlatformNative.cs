using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Jx.FileSystem.Internals.Natives;

namespace Jx.FileSystem.Internals.Natives
{ 
	internal class MacOSXPlatformNative : PlatformNative
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
			return MacOSXPlatformNative.MacLoadLibrary(path);
		}
	}
}
