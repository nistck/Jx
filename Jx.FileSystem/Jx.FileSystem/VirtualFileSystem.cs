using A;
using Jx.FileSystem.Archives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Jx;

namespace Jx.FileSystem
{
	public static class VirtualFileSystem
	{
		public class PreloadFileToMemoryItem
		{
			internal string aJ;
			internal volatile bool aj;
			internal volatile string aK = "";
			internal volatile byte[] ak;
			public string Path
			{
				get
				{
					return this.aJ;
				}
			}
			public bool Loaded
			{
				get
				{
					return this.aj;
				}
			}
			public string Error
			{
				get
				{
					return this.aK;
				}
			}
			public byte[] Data
			{
				get
				{
					return this.ak;
				}
			}
		}
		public sealed class DeploymentParametersClass
		{
			internal string aL = "";
			public string DefaultLanguage
			{
				get
				{
					return this.aL;
				}
			}
		}
		private static string j;
		private static string K;
		internal static bool k;
		private static Dictionary<string, string> L;
		private static Dictionary<string, int> l = new Dictionary<string, int>();
		private static Dictionary<string, byte[]> M = new Dictionary<string, byte[]>();
		private static bool m;
		private static string N = "";
		private static VirtualFileSystem.DeploymentParametersClass n;
		internal static object O = new object();
		private static bool o;
		internal static Dictionary<string, VirtualFileSystem.PreloadFileToMemoryItem> P = new Dictionary<string, VirtualFileSystem.PreloadFileToMemoryItem>();
		public static string ResourceDirectoryPath
		{
			get
			{
				return VirtualFileSystem.j;
			}
		}
		public static string ExecutableDirectoryPath
		{
			get
			{
				return VirtualFileSystem.K;
			}
		}
		public static bool Deployed
		{
			get
			{
				return VirtualFileSystem.m;
			}
		}
		public static VirtualFileSystem.DeploymentParametersClass DeploymentParameters
		{
			get
			{
				return VirtualFileSystem.n;
			}
		}
		public static string UserDirectoryPath
		{
			get
			{
				return VirtualFileSystem.N;
			}
		}
		public static bool LoggingFileOperations
		{
			get
			{
				return VirtualFileSystem.o;
			}
			set
			{
				VirtualFileSystem.o = value;
			}
		}
		public static string NormalizePath(string path)
		{
			string text = path;
			if (text != null)
			{
				if (D.Platform == D.A.Windows)
				{
					text = text.Replace('/', '\\');
				}
				else
				{
					text = text.Replace('\\', '/');
				}
			}
			return text;
		}
		public static bool Init(string logFileName, bool correctCurrentDirectory, string specialExecutableDirectoryPath, string specialResourceDirectoryPath, string specialUserDirectoryPath, string specialNativeLibrariesDirectoryPath)
		{
			logFileName = VirtualFileSystem.NormalizePath(logFileName);
			specialExecutableDirectoryPath = VirtualFileSystem.NormalizePath(specialExecutableDirectoryPath);
			specialResourceDirectoryPath = VirtualFileSystem.NormalizePath(specialResourceDirectoryPath);
			specialUserDirectoryPath = VirtualFileSystem.NormalizePath(specialUserDirectoryPath);
			specialNativeLibrariesDirectoryPath = VirtualFileSystem.NormalizePath(specialNativeLibrariesDirectoryPath);
			NativeLibraryManager.Q = specialNativeLibrariesDirectoryPath;
			if (VirtualFileSystem.k)
			{
				Log.Fatal("VirtualFileSystem: Init: File system already initialized.");
				return false;
			}
			if (!string.IsNullOrEmpty(specialExecutableDirectoryPath) && !Path.IsPathRooted(specialExecutableDirectoryPath))
			{
				Log.Fatal("VirtualFileSystem: Init: Special executable directory path must be rooted.");
				return false;
			}
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			VirtualFileSystem.K = specialExecutableDirectoryPath;
			if (string.IsNullOrEmpty(VirtualFileSystem.K))
			{
				VirtualFileSystem.K = E.Get().GetExecutableDirectoryPath();
			}
			VirtualFileSystem.j = specialResourceDirectoryPath;
			if (string.IsNullOrEmpty(VirtualFileSystem.j))
			{
				VirtualFileSystem.j = Path.Combine(VirtualFileSystem.K, "Data");
			}
			if (!string.IsNullOrEmpty(specialUserDirectoryPath))
			{
				VirtualFileSystem.N = specialUserDirectoryPath;
			}
			bool flag = Type.GetType("Mono.Runtime", false) != null;
			if (flag)
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(VirtualFileSystem.A);
			}
			if (D.Platform == D.A.MacOSX)
			{
				NativeLibraryManager.PreLoadLibrary("MacAppNativeWrapper");
			}
			if (correctCurrentDirectory)
			{
				VirtualFileSystem._CorrectCurrentDirectory();
			}
			EngineComponentManager.A();
			if (!ArchiveManager.A())
			{
				VirtualFileSystem.Shutdown();
				return false;
			}
			VirtualFileSystem.k = true;
			VirtualFileSystem.A();
			string dumpRealFileName = null;
			if (!string.IsNullOrEmpty(logFileName))
			{
				dumpRealFileName = VirtualFileSystem.GetRealPathByVirtual(logFileName);
			}
			Log._Init(Thread.CurrentThread, dumpRealFileName);
			VirtualFileSystem.a();
			return true;
		}
		public static void Shutdown()
		{
			ArchiveManager.a();
			EngineComponentManager.a();
			VirtualFileSystem.k = false;
		}
		private static Assembly A(object obj, ResolveEventArgs resolveEventArgs)
		{
			string name = resolveEventArgs.Name;
			string path = name.Substring(0, name.IndexOf(',')) + ".dll";
			string fileName = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, path);
			return c.LoadAssemblyByFileName(fileName);
		}
		public static void _CorrectCurrentDirectory()
		{
			lock (VirtualFileSystem.O)
			{
				if (D.Platform == D.A.Windows)
				{
					Directory.SetCurrentDirectory(VirtualFileSystem.ExecutableDirectoryPath);
				}
			}
		}
		public static string GetVirtualPathByReal(string realPath)
		{
			if (realPath == null)
			{
				Log.Fatal("VirtualFileSystem: GetVirtualPathByReal: realPath == null.");
			}
			realPath = VirtualFileSystem.NormalizePath(realPath);
			string text;
			if (!Path.IsPathRooted(realPath))
			{
				text = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, realPath);
			}
			else
			{
				text = realPath;
			}
			if (text.Length <= VirtualFileSystem.ResourceDirectoryPath.Length)
			{
				return "";
			}
			if (!string.Equals(text.Substring(0, VirtualFileSystem.ResourceDirectoryPath.Length), VirtualFileSystem.ResourceDirectoryPath, StringComparison.OrdinalIgnoreCase))
			{
				return "";
			}
			return text.Substring(VirtualFileSystem.ResourceDirectoryPath.Length + 1, text.Length - VirtualFileSystem.ResourceDirectoryPath.Length - 1);
		}
		public static string GetRealPathByVirtual(string virtualPath)
		{
			if (D.Platform != D.A.Windows)
			{
				virtualPath = virtualPath.Replace('\\', '/');
			}
			if (virtualPath.Length >= 5 && virtualPath[4] == ':')
			{
				string a = virtualPath.Substring(0, 5);
				if (a == "user:")
				{
					return Path.Combine(VirtualFileSystem.UserDirectoryPath, virtualPath.Substring(5));
				}
			}
			return Path.Combine(VirtualFileSystem.ResourceDirectoryPath, virtualPath);
		}
		public static void AddFileRedirection(string originalFileName, string newFileName)
		{
			lock (VirtualFileSystem.O)
			{
				if (VirtualFileSystem.L == null)
				{
					VirtualFileSystem.L = new Dictionary<string, string>();
				}
				string text = VirtualFileSystem.NormalizePath(originalFileName).ToLower();
				string value = VirtualFileSystem.NormalizePath(newFileName);
				if (VirtualFileSystem.L.ContainsKey(text))
				{
					Log.Fatal("VirtualFileSystem: AddFileRedirection: File redirection is already exists \"{0}\".", text);
				}
				VirtualFileSystem.L.Add(text, value);
			}
		}
		public static void RemoveFileRedirection(string originalFileName)
		{
			lock (VirtualFileSystem.O)
			{
				string key = VirtualFileSystem.NormalizePath(originalFileName).ToLower();
				if (VirtualFileSystem.L != null)
				{
					VirtualFileSystem.L.Remove(key);
				}
			}
		}
		internal static string A(string text, bool flag)
		{
			string result;
			lock (VirtualFileSystem.O)
			{
				if (VirtualFileSystem.L == null)
				{
					result = text;
				}
				else
				{
					string text2 = text;
					if (!flag)
					{
						text2 = VirtualFileSystem.NormalizePath(text2);
					}
					text2 = text2.ToLower();
					string text3;
					if (!VirtualFileSystem.L.TryGetValue(text2, out text3))
					{
						result = text;
					}
					else
					{
						result = text3;
					}
				}
			}
			return result;
		}
		public static string GetRedirectedFileName(string originalFileName)
		{
			return VirtualFileSystem.A(originalFileName, false);
		}
		private static void A()
		{
			string text = null;
			string text2 = "Base/Constants/Deployment.config";
			VirtualFileSystem.m = false;
			if (VirtualFile.Exists(text2))
			{
				VirtualFileSystem.m = true;
				VirtualFileSystem.n = new VirtualFileSystem.DeploymentParametersClass();
				try
				{
					using (VirtualFileStream virtualFileStream = VirtualFile.Open(text2))
					{
						using (StreamReader streamReader = new StreamReader(virtualFileStream))
						{
							while (true)
							{
								string text3 = streamReader.ReadLine();
								if (text3 == null)
								{
									break;
								}
								text3 = text3.Trim();
								if (!(text3 == "") && (text3.Length < 2 || !(text3.Substring(0, 2) == "//")))
								{
									int num = text3.IndexOf('=');
									if (num != -1)
									{
										string a = text3.Substring(0, num).Trim();
										string text4 = text3.Substring(num + 1).Trim();
										if (text4 != "")
										{
											if (a == "userDirectory")
											{
												text = text4;
											}
											if (a == "defaultLanguage")
											{
												VirtualFileSystem.n.aL = text4;
											}
										}
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.Fatal("VirtualFileSystem: Loading file failed {0} ({1}).", text2, ex.Message);
					return;
				}
			}
			if (string.IsNullOrEmpty(VirtualFileSystem.N))
			{
				if (!string.IsNullOrEmpty(text))
				{
					string path = null;
					if (D.Platform == D.A.Windows)
					{
						path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					}
					else if (D.Platform == D.A.MacOSX)
					{
						path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Application Support");
					}
					else if (D.Platform == D.A.Android)
					{
						VirtualFileSystem.N = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "UserSettings");
					}
					else
					{
						Log.Fatal("VirtualFileSystem: InitDeploymentInfoAndUserDirectory: Unknown platform.");
					}
					VirtualFileSystem.N = Path.Combine(path, text);
					return;
				}
				VirtualFileSystem.N = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "UserSettings");
			}
		}
		private static void a()
		{
			string path = "Base/Constants/FileSystem.config";
			if (VirtualFile.Exists(path))
			{
				TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(path);
				if (textBlock != null)
				{
					TextBlock textBlock2 = textBlock.FindChild("cachingExtensions");
					if (textBlock2 != null)
					{
						foreach (TextBlock.Attribute current in textBlock2.Attributes)
						{
							string value = current.Value;
							VirtualFileSystem.l.Add(value, 0);
						}
					}
				}
			}
		}
		internal static bool A(string path)
		{
			if (VirtualFileSystem.IsUserDirectoryPath(path))
			{
				return false;
			}
			string extension = Path.GetExtension(path);
			return VirtualFileSystem.l.ContainsKey(extension);
		}
		internal static byte[] a(string key)
		{
			byte[] result;
			if (!VirtualFileSystem.M.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}
		internal static void A(string key, byte[] value)
		{
			VirtualFileSystem.M.Add(key, value);
		}
		public static bool IsUserDirectoryPath(string path)
		{
			if (path.Length >= 5 && path[4] == ':')
			{
				string a = path.Substring(0, 5);
				if (a == "user:")
				{
					return true;
				}
			}
			return false;
		}
		private static void A(object obj)
		{
			VirtualFileSystem.PreloadFileToMemoryItem preloadFileToMemoryItem = (VirtualFileSystem.PreloadFileToMemoryItem)obj;
			try
			{
				using (VirtualFileStream virtualFileStream = VirtualFile.Open(preloadFileToMemoryItem.Path))
				{
					byte[] array = new byte[virtualFileStream.Length];
					if (virtualFileStream.Read(array, 0, array.Length) != array.Length)
					{
						throw new Exception("Unable to load all data.");
					}
					preloadFileToMemoryItem.ak = array;
					preloadFileToMemoryItem.aj = true;
				}
			}
			catch (Exception ex)
			{
				preloadFileToMemoryItem.aK = ex.Message;
			}
		}
		public static VirtualFileSystem.PreloadFileToMemoryItem PreloadFileToMemoryFromBackgroundThread(string path)
		{
			VirtualFileSystem.PreloadFileToMemoryItem result;
			lock (VirtualFileSystem.O)
			{
				string key = path.ToLower();
				VirtualFileSystem.PreloadFileToMemoryItem preloadFileToMemoryItem;
				if (VirtualFileSystem.P.TryGetValue(key, out preloadFileToMemoryItem))
				{
					result = preloadFileToMemoryItem;
				}
				else
				{
					preloadFileToMemoryItem = new VirtualFileSystem.PreloadFileToMemoryItem();
					preloadFileToMemoryItem.aJ = path;
					VirtualFileSystem.P.Add(key, preloadFileToMemoryItem);
					Task task = new Task(new Action<object>(VirtualFileSystem.A), preloadFileToMemoryItem);
					task.Start();
					result = preloadFileToMemoryItem;
				}
			}
			return result;
		}
		public static void UnloadPreloadedFileToMemory(string path)
		{
			lock (VirtualFileSystem.O)
			{
				string key = path.ToLower();
				VirtualFileSystem.P.Remove(key);
			}
		}
		public static void UnloadPreloadedFileToMemory(VirtualFileSystem.PreloadFileToMemoryItem item)
		{
			lock (VirtualFileSystem.O)
			{
				VirtualFileSystem.UnloadPreloadedFileToMemory(item.Path);
			}
		}
	}
}
