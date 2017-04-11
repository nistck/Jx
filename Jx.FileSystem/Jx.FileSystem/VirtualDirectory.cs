using Jx.FileSystem.Archives;
using System;
using System.Collections.Generic;
using System.IO;
namespace Jx.FileSystem
{
	public static class VirtualDirectory
	{
		public static bool Exists(string path)
		{
			bool result;
			lock (VirtualFileSystem.syncVFS)
			{
				if (!VirtualFileSystem.Initialized)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = false;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualDirectory.Exists( \"{0}\" )", path);
					}
					string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
					if (Directory.Exists(realPathByVirtual))
					{
						result = true;
					}
					else if (VirtualDirectory.IsInArchive(path))
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}
		public static bool IsInArchive(string path)
		{
			bool result;
			lock (VirtualFileSystem.syncVFS)
			{
				if (!VirtualFileSystem.Initialized)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = false;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualDirectory.IsInArchive( \"{0}\" )", path);
					}
					string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
					if (Directory.Exists(realPathByVirtual))
					{
						result = false;
					}
					else
					{
						result = ArchiveManager.Instance.A(path);
					}
				}
			}
			return result;
		}
		public static string[] GetFiles(string path)
		{
			return VirtualDirectory.GetFiles(path, "*");
		}
		public static string[] GetFiles(string path, string searchPattern)
		{
			return VirtualDirectory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}
		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			string[] result;
			lock (VirtualFileSystem.syncVFS)
			{
				if (!VirtualFileSystem.Initialized)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = null;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualDirectory.GetFiles( \"{0}\", \"{1}\", \"{2}\" )", path, searchPattern, searchOption);
					}
					if (searchPattern.IndexOfAny(new char[]
					{
						'\\',
						'/',
						'?'
					}) != -1)
					{
						throw new ArgumentException("searchPattern: following characters: \\, /, ? is not supported.");
					}
					if (path.Contains(".."))
					{
						throw new ArgumentException("path: \"..\" is not supported.");
					}
					if (searchPattern.Contains(".."))
					{
						throw new ArgumentException("searchPattern: \"..\" is not supported.");
					}
					if (VirtualFileSystem.IsUserDirectoryPath(path))
					{
						string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
						string[] array;
						if (Directory.Exists(realPathByVirtual))
						{
							array = Directory.GetFiles(realPathByVirtual, searchPattern, searchOption);
						}
						else
						{
							array = new string[0];
						}
						int startIndex = VirtualFileSystem.UserDirectoryPath.Length + 1;
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "user:" + array[i].Substring(startIndex);
						}
						Array.Sort<string>(array);
						result = array;
					}
					else
					{
						string realPathByVirtual2 = VirtualFileSystem.GetRealPathByVirtual(path);
						string[] array2;
						if (Directory.Exists(realPathByVirtual2))
						{
							array2 = Directory.GetFiles(realPathByVirtual2, searchPattern, searchOption);
						}
						else
						{
							array2 = new string[0];
						}
						int startIndex2 = VirtualFileSystem.ResourceDirectoryPath.Length + 1;
						for (int j = 0; j < array2.Length; j++)
						{
							array2[j] = array2[j].Substring(startIndex2);
						}
						List<string> list = new List<string>(64);
						ArchiveManager.Instance.A(path, searchPattern, searchOption, list);
						if (list.Count != 0)
						{
							string[] array3 = array2;
							for (int k = 0; k < array3.Length; k++)
							{
								string item = array3[k];
								list.Add(item);
							}
							list.Sort();
							for (int l = list.Count - 1; l >= 1; l--)
							{
								if (string.Compare(list[l], list[l - 1], true) == 0)
								{
									list.RemoveAt(l);
								}
							}
							array2 = list.ToArray();
						}
						else
						{
							Array.Sort<string>(array2);
						}
						result = array2;
					}
				}
			}
			return result;
		}
		public static string[] GetDirectories(string path)
		{
			return VirtualDirectory.GetDirectories(path, "*");
		}
		public static string[] GetDirectories(string path, string searchPattern)
		{
			return VirtualDirectory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}
		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			string[] result;
			lock (VirtualFileSystem.syncVFS)
			{
				if (!VirtualFileSystem.Initialized)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = null;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualDirectory.GetDirectories( \"{0}\", \"{1}\", \"{2}\" )", path, searchPattern, searchOption);
					}
					if (searchPattern.IndexOfAny(new char[]
					{
						'\\',
						'/',
						'?'
					}) != -1)
					{
						throw new ArgumentException("searchPattern: following characters: \\, /, ? is not supported.");
					}
					if (path.Contains(".."))
					{
						throw new ArgumentException("path: \"..\" is not supported.");
					}
					if (searchPattern.Contains(".."))
					{
						throw new ArgumentException("searchPattern: \"..\" is not supported.");
					}
					if (VirtualFileSystem.IsUserDirectoryPath(path))
					{
						string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
						string[] array;
						if (Directory.Exists(realPathByVirtual))
						{
							array = Directory.GetDirectories(realPathByVirtual, searchPattern, searchOption);
						}
						else
						{
							array = new string[0];
						}
						int startIndex = VirtualFileSystem.UserDirectoryPath.Length + 1;
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "user:" + array[i].Substring(startIndex);
						}
						Array.Sort<string>(array);
						result = array;
					}
					else
					{
						string realPathByVirtual2 = VirtualFileSystem.GetRealPathByVirtual(path);
						string[] array2;
						if (Directory.Exists(realPathByVirtual2))
						{
							array2 = Directory.GetDirectories(realPathByVirtual2, searchPattern, searchOption);
						}
						else
						{
							array2 = new string[0];
						}
						int startIndex2 = VirtualFileSystem.ResourceDirectoryPath.Length + 1;
						for (int j = 0; j < array2.Length; j++)
						{
							array2[j] = array2[j].Substring(startIndex2);
						}
						List<string> list = new List<string>(64);
						ArchiveManager.Instance.a(path, searchPattern, searchOption, list);
						if (list.Count != 0)
						{
							string[] array3 = array2;
							for (int k = 0; k < array3.Length; k++)
							{
								string item = array3[k];
								list.Add(item);
							}
							list.Sort();
							for (int l = list.Count - 1; l >= 1; l--)
							{
								if (string.Compare(list[l], list[l - 1], true) == 0)
								{
									list.RemoveAt(l);
								}
							}
							array2 = list.ToArray();
						}
						else
						{
							Array.Sort<string>(array2);
						}
						result = array2;
					}
				}
			}
			return result;
		}
	}
}
