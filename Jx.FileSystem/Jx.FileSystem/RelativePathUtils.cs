using System;
using System.IO;
namespace Jx.FileSystem
{
	public static class RelativePathUtils
	{
		public static string ConvertToFullPath(string ownerDirectoryName, string path)
		{
			if (!string.IsNullOrEmpty(path) && path.Length > 1 && path[0] == '.' && (path[1] == '\\' || path[1] == '/'))
			{
				if (ownerDirectoryName == null)
				{
					ownerDirectoryName = "";
				}
				return Path.Combine(ownerDirectoryName, path.Substring(2));
			}
			return path;
		}

		public static string ConvertToRelativePath(string ownerDirectoryName, string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				string text = VirtualFileSystem.NormalizePath(path);
				if (string.IsNullOrEmpty(ownerDirectoryName))
				{
					return Path.Combine(".", text);
				}
				string text2 = VirtualFileSystem.NormalizePath(ownerDirectoryName);
				if (string.Compare(text, 0, text2, 0, text2.Length, true) == 0)
				{
					return Path.Combine(".", text.Substring(text2.Length + 1));
				}
			}
			return path;
		}

	}
}
