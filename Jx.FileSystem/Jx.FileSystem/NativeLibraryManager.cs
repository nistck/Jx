using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using Jx;
using Jx.FileSystem.Internals.Natives;

namespace Jx.FileSystem
{
	public static class NativeLibraryManager
	{
		internal static string specialNativeLibrariesDirectoryPath;
		private static object q = new object();
		private static Dictionary<string, int> R = new Dictionary<string, int>();
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "SetDllDirectory")]
		private static extern bool SetDllDirectory(string s);
		public static string GetNativeLibrariesDirectory()
		{
			if (!string.IsNullOrEmpty(NativeLibraryManager.specialNativeLibrariesDirectoryPath))
			{
				return NativeLibraryManager.specialNativeLibrariesDirectoryPath;
			}
			if (PlatformInfo.Platform == PlatformInfo.PlanformType.Windows)
			{
				if (IntPtr.Size == 8)
				{
					return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls\\Windows_x64");
				}
				return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls\\Windows_x86");
			}
			else if (PlatformInfo.Platform == PlatformInfo.PlanformType.MacOSX)
			{
				if (IntPtr.Size == 8)
				{
					return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls/MacOSX_x64");
				}
				return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls/MacOSX_x86");
			}
			else
			{
				if (PlatformInfo.Platform == PlatformInfo.PlanformType.Android)
				{
					return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls/Android_x86");
				}
				Log.Fatal("VirtualFileSystem: GetNativeLibraryDirectory: no code.");
				return null;
			}
		}
		public static void PreLoadLibrary(string baseName)
		{
			lock (NativeLibraryManager.q)
			{
				if (!NativeLibraryManager.R.ContainsKey(baseName))
				{
					NativeLibraryManager.R.Add(baseName, 0);
					if (PlatformInfo.Platform == PlatformInfo.PlanformType.Windows)
					{
						baseName += ".dll";
					}
					else if (PlatformInfo.Platform == PlatformInfo.PlanformType.MacOSX)
					{
						string path = Path.Combine(NativeLibraryManager.GetNativeLibrariesDirectory(), baseName + ".bundle");
						if (Directory.Exists(path))
						{
							baseName += ".bundle";
						}
						else
						{
							baseName += ".dylib";
						}
					}
					else if (PlatformInfo.Platform == PlatformInfo.PlanformType.Android)
					{
						string str = "lib";
						if (baseName.Length > 3 && baseName.Substring(0, 3) == "lib")
						{
							str = "";
						}
						baseName = str + baseName + ".so";
					}
					else
					{
						Log.Fatal("NativeLibraryManager: PreLoadLibrary: no code.");
					}
					string currentDirectory = Directory.GetCurrentDirectory();
					Directory.SetCurrentDirectory(NativeLibraryManager.GetNativeLibrariesDirectory());
					if (PlatformInfo.Platform == PlatformInfo.PlanformType.Windows)
					{
						try
						{
							NativeLibraryManager.SetDllDirectory(NativeLibraryManager.GetNativeLibrariesDirectory());
						}
						catch
						{
						}
					}
					string text = Path.Combine(NativeLibraryManager.GetNativeLibrariesDirectory(), baseName);
					if (PlatformNative.Get().LoadLibrary(text) == IntPtr.Zero)
					{
						Log.Fatal("Loading native library failed ({0}).", text);
					}
					Directory.SetCurrentDirectory(currentDirectory);
				}
			}
		}
	}
}
