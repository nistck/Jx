using Jx;
using Jx.FileSystem;
using System;
using System.Text;

namespace Jx.FileSystem.Internals.Natives
{
	internal abstract class PlatformNative
	{
		private static PlatformNative p;
		public abstract string GetExecutableDirectoryPath();
		public abstract IntPtr LoadLibrary(string path);
		public static PlatformNative Get()
		{
			if (p == null)
			{
				if (PlatformInfo.Platform == PlatformInfo.PlanformType.Android)
				{
					p = new AndroidPlatformNative();
				}
				else if (PlatformInfo.Platform == PlatformInfo.PlanformType.MacOSX)
				{
					p = new MacOSXPlatformNative();
				}
				else
				{
					p = new WindowsPlatformNative();
				}
			}
			return p;
		}
	}
     
}
