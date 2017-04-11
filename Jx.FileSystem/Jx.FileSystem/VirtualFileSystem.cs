using Jx.FileSystem.Archives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Jx;
using Jx.FileSystem.Internals;
using Jx.FileSystem.Internals.Natives;

namespace Jx.FileSystem
{
	public static class VirtualFileSystem
	{
		public class PreloadFileToMemoryItem
		{
			internal string path;
			internal volatile bool loaded;
			internal volatile string error = "";
			internal volatile byte[] data;
			public string Path
			{
				get
				{
					return this.path;
				}
			}
			public bool Loaded
			{
				get
				{
					return this.loaded;
				}
			}
			public string Error
			{
				get
				{
					return this.error;
				}
			}
			public byte[] Data
			{
				get
				{
					return this.data;
				}
			}
		}

		public sealed class DeploymentParametersClass
		{
			internal string defaultLanguage = "";
			public string DefaultLanguage
			{
				get
				{
					return this.defaultLanguage;
				}
			}
		}

		private static string resourceDirectoryPath;
		private static string executableDirectoryPath;
		internal static bool Initialized;
		private static Dictionary<string, string> fileRedirectionDefinitions;
		private static Dictionary<string, int> cachingExtensions = new Dictionary<string, int>();
		private static Dictionary<string, byte[]> bytesCache = new Dictionary<string, byte[]>();
		private static bool deployed;
		private static string userDirectoryPath = "";
		private static DeploymentParametersClass deploymentParameters;
		internal readonly static object syncVFS = new object();
		private static bool loggingFileOperatations;
		internal static Dictionary<string, PreloadFileToMemoryItem> preloadItems = new Dictionary<string, PreloadFileToMemoryItem>();

		public static string ResourceDirectoryPath
		{
			get
			{
				return resourceDirectoryPath;
			}
		}

		public static string ExecutableDirectoryPath
		{
			get
			{
				return executableDirectoryPath;
			}
		}
		public static bool Deployed
		{
			get
			{
				return deployed;
			}
		}
		public static DeploymentParametersClass DeploymentParameters
		{
			get
			{
				return deploymentParameters;
			}
		}
		public static string UserDirectoryPath
		{
			get
			{
				return userDirectoryPath;
			}
		}
		public static bool LoggingFileOperations
		{
			get
			{
				return loggingFileOperatations;
			}
			set
			{
				loggingFileOperatations = value;
			}
		}

        /// <summary>
        /// 替换 顺斜杠\\ -> 反斜杠/
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
		public static string NormalizePath(string path)
		{
			string text = path;
			if (text != null)
			{
				if (PlatformInfo.Platform == PlatformInfo.PlanformType.Windows)
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

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="logFileName">日志文件</param>
        /// <param name="correctCurrentDirectory">!</param>
        /// <param name="specialExecutableDirectoryPath">程序目录</param>
        /// <param name="specialResourceDirectoryPath">资源目录</param>
        /// <param name="specialUserDirectoryPath">用户目录</param>
        /// <param name="specialNativeLibrariesDirectoryPath">DLL目录</param>
        /// <returns></returns>
		public static bool Init(string logFileName, bool correctCurrentDirectory, string specialExecutableDirectoryPath, string specialResourceDirectoryPath, string specialUserDirectoryPath, string specialNativeLibrariesDirectoryPath)
		{
			logFileName = NormalizePath(logFileName);
            specialExecutableDirectoryPath = NormalizePath(specialExecutableDirectoryPath);
			specialResourceDirectoryPath = NormalizePath(specialResourceDirectoryPath);
			specialUserDirectoryPath = NormalizePath(specialUserDirectoryPath);
			specialNativeLibrariesDirectoryPath = NormalizePath(specialNativeLibrariesDirectoryPath);

			NativeLibraryManager.specialNativeLibrariesDirectoryPath = specialNativeLibrariesDirectoryPath;
			if (Initialized)
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
			executableDirectoryPath = specialExecutableDirectoryPath;
			if (string.IsNullOrEmpty(executableDirectoryPath))
			{
				executableDirectoryPath = PlatformNative.Get().GetExecutableDirectoryPath();
			}
			resourceDirectoryPath = specialResourceDirectoryPath;

			if (string.IsNullOrEmpty(resourceDirectoryPath))
			{
				resourceDirectoryPath = Path.Combine(executableDirectoryPath, "Data");
			}

			if (!string.IsNullOrEmpty(specialUserDirectoryPath))
			{
				userDirectoryPath = specialUserDirectoryPath;
			}
			bool flag = Type.GetType("Mono.Runtime", false) != null;
			if (flag)
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);
			}
			if (PlatformInfo.Platform == PlatformInfo.PlanformType.MacOSX)
			{
				NativeLibraryManager.PreLoadLibrary("MacAppNativeWrapper");
			}
			if (correctCurrentDirectory)
			{
				_CorrectCurrentDirectory();
			}

			EngineComponentManager.Init();
			if (!ArchiveManager.A())
			{
				Shutdown();
				return false;
			}
			Initialized = true;
			InitDeployment();
			string dumpRealFileName = null;
			if (!string.IsNullOrEmpty(logFileName))
			{
				dumpRealFileName = GetRealPathByVirtual(logFileName);
			}
			Log._Init(Thread.CurrentThread, dumpRealFileName);
			InitCachingExtensions();
			return true;
		}

        public static void Shutdown()
		{
			ArchiveManager.a();
			EngineComponentManager.Unload();
			Initialized = false;
		}

		private static Assembly ResolveAssembly(object obj, ResolveEventArgs resolveEventArgs)
		{
			string name = resolveEventArgs.Name;
			string path = name.Substring(0, name.IndexOf(',')) + ".dll";
			string fileName = Path.Combine(ExecutableDirectoryPath, path);
			return AssemblyUtils.LoadAssemblyByFileName(fileName);
		}

		public static void _CorrectCurrentDirectory()
		{
			lock (VirtualFileSystem.syncVFS)
			{
				if (PlatformInfo.Platform == PlatformInfo.PlanformType.Windows)
				{
					Directory.SetCurrentDirectory(ExecutableDirectoryPath);
				}
			}
		}

		public static string GetVirtualPathByReal(string realPath)
		{
			if (realPath == null)
			{
				Log.Fatal("VirtualFileSystem: GetVirtualPathByReal: realPath == null.");
			}
			realPath = NormalizePath(realPath);
			string text;
			if (!Path.IsPathRooted(realPath))
			{
				text = Path.Combine(ExecutableDirectoryPath, realPath);
			}
			else
			{
				text = realPath;
			}
			if (text.Length <= ResourceDirectoryPath.Length)
			{
				return "";
			}
			if (!string.Equals(text.Substring(0, ResourceDirectoryPath.Length), ResourceDirectoryPath, StringComparison.OrdinalIgnoreCase))
			{
				return "";
			}
			return text.Substring(ResourceDirectoryPath.Length + 1, text.Length - ResourceDirectoryPath.Length - 1);
		}

		public static string GetRealPathByVirtual(string virtualPath)
		{
            if (PlatformInfo.Platform != PlatformInfo.PlanformType.Windows)
            {
                virtualPath = virtualPath.Replace('\\', '/');
            }
            if (virtualPath.Length >= 5 && virtualPath[4] == ':')
            {
                string a = virtualPath.Substring(0, 5);
                if (a == "user:")
                {
                    string p1 = Path.Combine(UserDirectoryPath, virtualPath.Substring(5));
                    return NormalizePath(p1);
                }
            }
            string p2 = Path.Combine(ResourceDirectoryPath, virtualPath);
            return NormalizePath(p2);
        }

		public static void AddFileRedirection(string originalFileName, string newFileName)
		{
			lock (syncVFS)
			{
				if (fileRedirectionDefinitions == null)
					fileRedirectionDefinitions = new Dictionary<string, string>();
				
				string text = NormalizePath(originalFileName).ToLower();
				string value = NormalizePath(newFileName);
				if (fileRedirectionDefinitions.ContainsKey(text))
				{
					Log.Fatal("VirtualFileSystem: AddFileRedirection: File redirection is already exists \"{0}\".", text);
				}
				fileRedirectionDefinitions.Add(text, value);
			}
		}

		public static void RemoveFileRedirection(string originalFileName)
		{
			lock (syncVFS)
			{
				string key = NormalizePath(originalFileName).ToLower();
				if (fileRedirectionDefinitions != null)
					fileRedirectionDefinitions.Remove(key);
			}
		}

		internal static string RedirectFile(string p, bool flag)
		{
			string result;
			lock (syncVFS)
			{
				if (fileRedirectionDefinitions == null)
				{
					result = p;
				}
				else
				{
					string text2 = p;
					if (!flag)
					{
						text2 = NormalizePath(text2);
					}
					text2 = text2.ToLower();

					string text3;
					if (!fileRedirectionDefinitions.TryGetValue(text2, out text3))
					{
						result = p;
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
			return RedirectFile(originalFileName, false);
		}

		private static void InitDeployment()
		{
			string userDirectory = null;
			string deploymentCfgPath = "Base/Constants/Deployment.config";
			deployed = false;
			if (VirtualFile.Exists(deploymentCfgPath))
			{
				deployed = true;
				deploymentParameters = new DeploymentParametersClass();
				try
				{
					using (VirtualFileStream virtualFileStream = VirtualFile.Open(deploymentCfgPath))
					{
						using (StreamReader streamReader = new StreamReader(virtualFileStream))
						{
							while (true)
							{
								string line = streamReader.ReadLine();
								if (line == null)
									break;

                                line = line.Trim();
								if (!(line == "") && (line.Length < 2 || !(line.Substring(0, 2) == "//")))
								{
									int eqPosition = line.IndexOf('=');
									if (eqPosition != -1)
									{
										string name = line.Substring(0, eqPosition).Trim();
										string value = line.Substring(eqPosition + 1).Trim();
										if (value != "")
										{
											if (name == "userDirectory")
											{
												userDirectory = value;
											}
											if (name == "defaultLanguage")
											{
												deploymentParameters.defaultLanguage = value;
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
					Log.Fatal("VirtualFileSystem: Loading file failed {0} ({1}).", deploymentCfgPath, ex.Message);
					return;
				}
			}

			if (string.IsNullOrEmpty(userDirectoryPath))
			{
				if (!string.IsNullOrEmpty(userDirectory))
				{
					string path = null;
					if (PlatformInfo.Platform == PlatformInfo.PlanformType.Windows)
					{
						path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					}
					else if (PlatformInfo.Platform == PlatformInfo.PlanformType.MacOSX)
					{
						path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Application Support");
					}
					else if (PlatformInfo.Platform == PlatformInfo.PlanformType.Android)
					{
						userDirectoryPath = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "UserSettings");
					}
					else
					{
						Log.Fatal("VirtualFileSystem: InitDeploymentInfoAndUserDirectory: Unknown platform.");
					}
					userDirectoryPath = Path.Combine(path, userDirectory);
					return;
				}
				userDirectoryPath = Path.Combine(ExecutableDirectoryPath, "UserSettings");
			}
		}

		private static void InitCachingExtensions()
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
							cachingExtensions.Add(value, 0);
						}
					}
				}
			}
		}

		internal static bool IsCachingExtension(string path)
		{
			if (IsUserDirectoryPath(path))
			{
				return false;
			}
			string extension = Path.GetExtension(path);
			return cachingExtensions.ContainsKey(extension);
		}

		internal static byte[] LoadCachedBytes(string key)
		{
			byte[] result;
			if (!bytesCache.TryGetValue(key, out result))
				return null;
			
			return result;
		}

		internal static void CacheBytes(string key, byte[] value)
		{
			bytesCache.Add(key, value);
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

		private static void PreloadFileToMemory(object obj)
		{
			PreloadFileToMemoryItem preloadFileToMemoryItem = (PreloadFileToMemoryItem)obj;
			try
			{
				using (VirtualFileStream virtualFileStream = VirtualFile.Open(preloadFileToMemoryItem.Path))
				{
					byte[] bytesBuf = new byte[virtualFileStream.Length];
					if (virtualFileStream.Read(bytesBuf, 0, bytesBuf.Length) != bytesBuf.Length)
					{
						throw new Exception("Unable to load all data.");
					}
					preloadFileToMemoryItem.data = bytesBuf;
					preloadFileToMemoryItem.loaded = true;
				}
			}
			catch (Exception ex)
			{
				preloadFileToMemoryItem.error = ex.Message;
			}
		}

		public static PreloadFileToMemoryItem PreloadFileToMemoryFromBackgroundThread(string path)
		{
			PreloadFileToMemoryItem result;
			lock (syncVFS)
			{
				string key = path.ToLower();
				PreloadFileToMemoryItem preloadFileToMemoryItem;
				if (preloadItems.TryGetValue(key, out preloadFileToMemoryItem))
				{
					result = preloadFileToMemoryItem;
				}
				else
				{
					preloadFileToMemoryItem = new PreloadFileToMemoryItem();
					preloadFileToMemoryItem.path = path;
					preloadItems.Add(key, preloadFileToMemoryItem);
					Task task = new Task(new Action<object>(PreloadFileToMemory), preloadFileToMemoryItem);
					task.Start();
					result = preloadFileToMemoryItem;
				}
			}
			return result;
		}
		public static void UnloadPreloadedFileToMemory(string path)
		{
			lock (VirtualFileSystem.syncVFS)
			{
				string key = path.ToLower();
				VirtualFileSystem.preloadItems.Remove(key);
			}
		}
		public static void UnloadPreloadedFileToMemory(VirtualFileSystem.PreloadFileToMemoryItem item)
		{
			lock (VirtualFileSystem.syncVFS)
			{
				VirtualFileSystem.UnloadPreloadedFileToMemory(item.Path);
			}
		}
	}
}
