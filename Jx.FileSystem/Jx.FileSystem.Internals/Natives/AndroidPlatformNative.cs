using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Jx.FileSystem.Internals.Natives;

namespace Jx.FileSystem.Internals.Natives
{
	internal class AndroidPlatformNative : PlatformNative
	{
		[DllImport("AndroidAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "AndroidAppNativeWrapper_LoadLibrary")]
		public static extern IntPtr AndroidLoadLibrary(string name);
		public override string GetExecutableDirectoryPath()
		{
			string codeBase = Assembly.GetCallingAssembly().CodeBase;
			return Path.GetDirectoryName(codeBase.Replace("file://", ""));
		}
		public override IntPtr LoadLibrary(string path)
		{
			return AndroidPlatformNative.AndroidLoadLibrary(path);
		}
	}
}
