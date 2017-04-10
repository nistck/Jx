using A;
using Jx.FileSystem.Archives;
using System;
using System.IO;
namespace Jx.FileSystem
{
	public static class VirtualFile
	{
		public static bool Exists(string path)
		{
			bool result;
			lock (VirtualFileSystem.O)
			{
				if (!VirtualFileSystem.k)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = false;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualFile.Exists( \"{0}\" )", path);
					}
					path = VirtualFileSystem.NormalizePath(path);
					path = VirtualFileSystem.A(path, true);
					string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
					if (File.Exists(realPathByVirtual))
					{
						result = true;
					}
					else if (VirtualFile.IsInArchive(path))
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
		public static VirtualFileStream Open(string path)
		{
			VirtualFileStream result;
			lock (VirtualFileSystem.O)
			{
				if (!VirtualFileSystem.k)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = null;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualFile.Open( \"{0}\" )", path);
					}
					path = VirtualFileSystem.NormalizePath(path);
					path = VirtualFileSystem.A(path, true);
					bool flag2 = VirtualFileSystem.A(path);
					if (flag2)
					{
						byte[] array = VirtualFileSystem.a(path);
						if (array != null)
						{
							result = new MemoryVirtualFileStream(array);
							return result;
						}
					}
					if (VirtualFileSystem.P.Count != 0)
					{
						string key = path.ToLower();
						VirtualFileSystem.PreloadFileToMemoryItem preloadFileToMemoryItem;
						if (VirtualFileSystem.P.TryGetValue(key, out preloadFileToMemoryItem) && preloadFileToMemoryItem.aj)
						{
							result = new MemoryVirtualFileStream(preloadFileToMemoryItem.ak);
							return result;
						}
					}
					VirtualFileStream virtualFileStream = null;
					string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
					try
					{
						if (D.Platform == D.A.Windows)
						{
							virtualFileStream = new g(realPathByVirtual);
						}
						else if (D.Platform == D.A.MacOSX)
						{
							virtualFileStream = new f(realPathByVirtual);
						}
						else
						{
							virtualFileStream = new G(realPathByVirtual);
						}
					}
					catch (FileNotFoundException)
					{
					}
					catch (Exception ex)
					{
						throw ex;
					}
					if (virtualFileStream == null)
					{
						virtualFileStream = ArchiveManager.Instance.FileOpen(path);
						if (virtualFileStream == null)
						{
							throw new FileNotFoundException("File not found.", path);
						}
					}
					if (flag2)
					{
						byte[] array2 = new byte[virtualFileStream.Length];
						if ((long)virtualFileStream.Read(array2, 0, (int)virtualFileStream.Length) == virtualFileStream.Length)
						{
							VirtualFileSystem.A(path, array2);
						}
						virtualFileStream.Position = 0L;
					}
					result = virtualFileStream;
				}
			}
			return result;
		}
		public static long GetLength(string path)
		{
			long result;
			lock (VirtualFileSystem.O)
			{
				if (!VirtualFileSystem.k)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = 0L;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualFile.GetLength( \"{0}\" )", path);
					}
					path = VirtualFileSystem.NormalizePath(path);
					path = VirtualFileSystem.A(path, true);
					string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
					try
					{
						FileInfo fileInfo = new FileInfo(realPathByVirtual);
						result = fileInfo.Length;
						return result;
					}
					catch (FileNotFoundException)
					{
					}
					ArchiveManager.FileInfo fileInfo2;
					if (!ArchiveManager.Instance.GetFileInfo(path, out fileInfo2))
					{
						throw new FileNotFoundException("File not found.", path);
					}
					result = fileInfo2.Length;
				}
			}
			return result;
		}
		public static bool IsInArchive(string path)
		{
			bool result;
			lock (VirtualFileSystem.O)
			{
				if (!VirtualFileSystem.k)
				{
					Log.Fatal("VirtualFileSystem: File system is not initialized.");
					result = false;
				}
				else
				{
					if (VirtualFileSystem.LoggingFileOperations)
					{
						Log.Info("Logging File Operations: VirtualFile.IsInArchive( \"{0}\" )", path);
					}
					path = VirtualFileSystem.NormalizePath(path);
					path = VirtualFileSystem.A(path, true);
					string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
					if (File.Exists(realPathByVirtual))
					{
						result = false;
					}
					else
					{
						ArchiveManager.FileInfo fileInfo;
						result = ArchiveManager.Instance.GetFileInfo(path, out fileInfo);
					}
				}
			}
			return result;
		}
		public static bool IsArchive(string path)
		{
			bool result;
			lock (VirtualFileSystem.O)
			{
				if (VirtualFileSystem.LoggingFileOperations)
				{
					Log.Info("Logging File Operations: VirtualFile.IsArchive( \"{0}\" )", path);
				}
				path = VirtualFileSystem.NormalizePath(path);
				path = VirtualFileSystem.A(path, true);
				string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(path);
				result = (ArchiveManager.Instance.GetArchive(realPathByVirtual) != null);
			}
			return result;
		}
		public static byte[] ReadAllBytes(string path)
		{
			byte[] result;
			using (VirtualFileStream virtualFileStream = VirtualFile.Open(path))
			{
				byte[] array = new byte[virtualFileStream.Length];
				if (virtualFileStream.Read(array, 0, array.Length) != array.Length)
				{
					throw new EndOfStreamException();
				}
				result = array;
			}
			return result;
		}
	}
}
