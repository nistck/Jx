using A;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using Jx;

namespace Jx.FileSystem
{
	public static class NativeLibraryManager
	{
		internal static string Q;
		private static object q = new object();
		private static Dictionary<string, int> R = new Dictionary<string, int>();
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "SetDllDirectory")]
		private static extern bool A(string s);
		public static string GetNativeLibrariesDirectory()
		{
			if (!string.IsNullOrEmpty(NativeLibraryManager.Q))
			{
				return NativeLibraryManager.Q;
			}
			if (D.Platform == D.A.Windows)
			{
				if (IntPtr.Size == 8)
				{
					return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls\\Windows_x64");
				}
				return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls\\Windows_x86");
			}
			else if (D.Platform == D.A.MacOSX)
			{
				if (IntPtr.Size == 8)
				{
					return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls/MacOSX_x64");
				}
				return Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "NativeDlls/MacOSX_x86");
			}
			else
			{
				if (D.Platform == D.A.Android)
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
					if (D.Platform == D.A.Windows)
					{
						baseName += ".dll";
					}
					else if (D.Platform == D.A.MacOSX)
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
					else if (D.Platform == D.A.Android)
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
					if (D.Platform == D.A.Windows)
					{
						try
						{
							NativeLibraryManager.A(NativeLibraryManager.GetNativeLibrariesDirectory());
						}
						catch
						{
						}
					}
					string text = Path.Combine(NativeLibraryManager.GetNativeLibrariesDirectory(), baseName);
					if (E.Get().LoadLibrary(text) == IntPtr.Zero)
					{
						Log.Fatal("Loading native library failed ({0}).", text);
					}
					Directory.SetCurrentDirectory(currentDirectory);
				}
			}
		}
	}
}
