using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices; 

namespace Jx.FileSystem.Archives
{
	public class ArchiveManager
	{
		public struct FileInfo
		{
			private string ap;
			private Archive aQ;
			private string aq;
			private long aR;
			public string FileName
			{
				get
				{
					return this.ap;
				}
			}
			public Archive Archive
			{
				get
				{
					return this.aQ;
				}
			}
			public string InArchiveFileName
			{
				get
				{
					return this.aq;
				}
			}
			public long Length
			{
				get
				{
					return this.aR;
				}
			}
			public FileInfo(string fileName, Archive archive, string inArchiveFileName, long length)
			{
				this.ap = fileName;
				this.aQ = archive;
				this.aq = inArchiveFileName;
				this.aR = length;
			}
		}
		private class ClassA
		{
			internal string ar;
			internal bool aS;
			internal List<string> @as;
			internal Dictionary<string, ArchiveManager.ClassA> aT;
			public string Name
			{
				get
				{
					return this.ar;
				}
			}
			public bool RealFileSystemDirectory
			{
				get
				{
					return this.aS;
				}
			}
			public List<string> Files
			{
				get
				{
					return this.@as;
				}
			}
			public Dictionary<string, ArchiveManager.ClassA> Directories
			{
				get
				{
					return this.aT;
				}
			}
			public ClassA(string name, bool realFileSystemDirectory)
			{
				this.ar = name;
				this.aS = realFileSystemDirectory;
			}
		}
		private class ClassB
		{
			private ArchiveFactory at;
			private string aU = "";
			private float au = 0.5f;
			public ArchiveFactory Factory
			{
				get
				{
					return this.at;
				}
				set
				{
					this.at = value;
				}
			}
			public string SourceRealFileName
			{
				get
				{
					return this.aU;
				}
				set
				{
					this.aU = value;
				}
			}
			public float LoadingPriority
			{
				get
				{
					return this.au;
				}
				set
				{
					this.au = value;
				}
			}
		}
		private static ArchiveManager r;
		private List<ArchiveFactory> S = new List<ArchiveFactory>();
		private Dictionary<string, Archive> s = new Dictionary<string, Archive>();
		private Dictionary<string, ArchiveManager.FileInfo> T = new Dictionary<string, ArchiveManager.FileInfo>(256);
		private ArchiveManager.ClassA t;
		[CompilerGenerated]
		private static Comparison<ArchiveManager.ClassB> U;
		public static ArchiveManager Instance
		{
			get
			{
				return ArchiveManager.r;
			}
		}
		public ReadOnlyCollection<ArchiveFactory> Factories
		{
			get
			{
				return this.S.AsReadOnly();
			}
		}
		public IEnumerable<Archive> Archives
		{
			get
			{
				return this.s.Values;
			}
		}
		internal static bool A()
		{
			if (ArchiveManager.r != null)
			{
				Log.Fatal("ArchiveManager: Init: The instance is already initialized.");
			}
			ArchiveManager.r = new ArchiveManager();
			if (!ArchiveManager.r.B())
			{
				ArchiveManager.a();
				return false;
			}
			return true;
		}
		internal static void a()
		{
			if (ArchiveManager.r != null)
			{
				ArchiveManager.r.D();
				ArchiveManager.r = null;
			}
		}
		private bool B()
		{
			return this.b() && this.c();
		}
		private static void A<A>(List<A> list, Comparison<A> comparison)
		{
			int count = list.Count;
			for (int i = 0; i < count - 1; i++)
			{
				A a = list[i];
				A a2 = a;
				int num = i;
				for (int j = i + 1; j < count; j++)
				{
					A a3 = list[j];
					if (comparison(a2, a3) > 0)
					{
						a2 = a3;
						num = j;
					}
				}
				if (num != i)
				{
					list[i] = a2;
					list[num] = a;
				}
			}
		}
		private bool b()
		{
			EngineComponentManager.ComponentInfo[] componentsByType = EngineComponentManager.Instance.GetComponentsByType(EngineComponentManager.ComponentTypeFlags.Archive, true);
			List<Assembly> list = new List<Assembly>();
			EngineComponentManager.ComponentInfo[] array = componentsByType;
			for (int i = 0; i < array.Length; i++)
			{
				EngineComponentManager.ComponentInfo componentInfo = array[i];
				EngineComponentManager.ComponentInfo.PathInfo[] allEntryPointsForThisPlatform = componentInfo.GetAllEntryPointsForThisPlatform();
				for (int j = 0; j < allEntryPointsForThisPlatform.Length; j++)
				{
					EngineComponentManager.ComponentInfo.PathInfo pathInfo = allEntryPointsForThisPlatform[j];
					string fileName = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, pathInfo.Path);
					Assembly item = AssemblyUtils.LoadAssemblyByFileName(fileName);
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			List<Type> list2 = new List<Type>();
			foreach (Assembly current in list)
			{
				Type[] types = current.GetTypes();
				for (int k = 0; k < types.Length; k++)
				{
					Type type = types[k];
					if (typeof(ArchiveFactory).IsAssignableFrom(type) && !type.IsAbstract)
					{
						list2.Add(type);
					}
				}
			}
			foreach (Type current2 in list2)
			{
				ConstructorInfo constructor = current2.GetConstructor(new Type[0]);
				ArchiveFactory archiveFactory = (ArchiveFactory)constructor.Invoke(null);
				if (!archiveFactory.OnInit())
				{
					return false;
				}
				this.S.Add(archiveFactory);
			}
			return true;
		}
		private List<ArchiveManager.ClassB> C()
		{
			List<ArchiveManager.ClassB> list = new List<ArchiveManager.ClassB>();
			foreach (ArchiveFactory current in this.S)
			{
				string[] files = Directory.GetFiles(VirtualFileSystem.ResourceDirectoryPath, "*." + current.FileExtension, SearchOption.AllDirectories);
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string path = Path.ChangeExtension(text, ".archive");
					if (File.Exists(path))
					{
						TextBlock textBlock = TextBlockUtils.LoadFromRealFile(path);
						if (textBlock != null)
						{
							ArchiveManager.ClassB b = new ArchiveManager.ClassB();
							b.Factory = current;
							b.SourceRealFileName = text;
							if (textBlock.IsAttributeExist("loadingPriority"))
							{
								b.LoadingPriority = float.Parse(textBlock.GetAttribute("loadingPriority"));
							}
							list.Add(b);
						}
					}
				}
			}
			List<ArchiveManager.ClassB> arg_FA_0 = list;
			if (ArchiveManager.U == null)
			{
				ArchiveManager.U = new Comparison<ArchiveManager.ClassB>(ArchiveManager.A);
			}
			ArchiveManager.A<ArchiveManager.ClassB>(arg_FA_0, ArchiveManager.U);
			return list;
		}
		private bool c()
		{
			this.t = new ArchiveManager.ClassA(null, true);
			List<ArchiveManager.ClassB> list = this.C();
			foreach (ArchiveManager.ClassB current in list)
			{
				Archive archive = current.Factory.OnLoadArchive(current.SourceRealFileName);
				if (archive == null)
				{
					return false;
				}
				string key = VirtualFileSystem.NormalizePath(current.SourceRealFileName).ToLower();
				this.s.Add(key, archive);
				string virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(Path.GetDirectoryName(archive.FileName));
				string[] array;
				Archive.GetListFileInfo[] array2;
				archive.OnGetDirectoryAndFileList(out array, out array2);
				string[] array3 = array;
				for (int i = 0; i < array3.Length; i++)
				{
					string text = array3[i];
					string path = text.Trim(new char[]
					{
						'\\',
						'/'
					});
					string text2 = Path.Combine(virtualPathByReal, path);
					this.A(text2, true);
				}
				Archive.GetListFileInfo[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					Archive.GetListFileInfo getListFileInfo = array4[j];
					string text3 = VirtualFileSystem.NormalizePath(getListFileInfo.FileName);
					string directoryName = Path.GetDirectoryName(text3);
					string path2 = directoryName.Trim(new char[]
					{
						'\\',
						'/'
					});
					string text4 = Path.Combine(virtualPathByReal, path2);
					this.A(text4, true);
					string text5 = Path.Combine(virtualPathByReal, text3);
					ArchiveManager.ClassA a = this.A(Path.GetDirectoryName(text5), false);
					if (a.@as == null)
					{
						a.@as = new List<string>();
					}
					a.@as.Add(Path.GetFileName(text5));
					string key2 = text5.ToLower();
					ArchiveManager.FileInfo fileInfo;
					if (this.T.TryGetValue(key2, out fileInfo))
					{
						this.T.Remove(key2);
					}
					ArchiveManager.FileInfo value = new ArchiveManager.FileInfo(text5, archive, getListFileInfo.FileName, getListFileInfo.Length);
					this.T.Add(key2, value);
				}
			}
			return true;
		}
		private void D()
		{
			this.t = null;
			foreach (Archive current in this.s.Values)
			{
				current.Dispose();
			}
			this.s.Clear();
			foreach (ArchiveFactory current2 in this.S)
			{
				current2.Dispose();
			}
			this.S.Clear();
		}
		public VirtualFileStream FileOpen(string virtualPath)
		{
			VirtualFileStream result;
			lock (VirtualFileSystem.syncVFS)
			{
				ArchiveManager.FileInfo fileInfo;
				if (!GetFileInfo(virtualPath, out fileInfo))
				{
					result = null;
				}
				else
				{
					result = fileInfo.Archive.OnFileOpen(fileInfo.InArchiveFileName);
				}
			}
			return result;
		}

		public bool GetFileInfo(string virtualPath, out ArchiveManager.FileInfo fileInfo)
		{
			bool result;
			lock (VirtualFileSystem.syncVFS)
			{
				string key = VirtualFileSystem.NormalizePath(virtualPath).ToLower();
				if (!this.T.TryGetValue(key, out fileInfo))
				{
					fileInfo = default(ArchiveManager.FileInfo);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public Archive GetArchive(string realPath)
		{
			Archive result;
			lock (VirtualFileSystem.syncVFS)
			{
				string key = VirtualFileSystem.NormalizePath(realPath).ToLower();
				Archive archive;
				if (!this.s.TryGetValue(key, out archive))
				{
					result = null;
				}
				else
				{
					result = archive;
				}
			}
			return result;
		}
		private ArchiveManager.ClassA A(string text, bool flag)
		{
			string[] array = text.Split(new char[]
			{
				'\\',
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			string text2 = "";
			ArchiveManager.ClassA a = this.t;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text3 = array2[i];
				text2 = Path.Combine(text2, text3);
				ArchiveManager.ClassA a2 = null;
				string key = text3.ToLower();
				if (a.Directories != null)
				{
					a.Directories.TryGetValue(key, out a2);
				}
				if (a2 == null)
				{
					if (!flag)
					{
						return null;
					}
					string path = Path.Combine(VirtualFileSystem.ResourceDirectoryPath, text2);
					bool realFileSystemDirectory = Directory.Exists(path);
					a2 = new ArchiveManager.ClassA(text3, realFileSystemDirectory);
					if (a.aT == null)
					{
						a.aT = new Dictionary<string, ArchiveManager.ClassA>();
					}
					a.aT.Add(key, a2);
				}
				a = a2;
			}
			return a;
		}
		internal bool A(string text)
		{
			ArchiveManager.ClassA a = this.A(text, false);
			return a != null && !a.RealFileSystemDirectory;
		}
		private bool A(string text, string text2)
		{
			if (text2 == "*" || text2 == "*.*")
			{
				return true;
			}
			string text3 = text.ToLower();
			string text4 = text2.ToLower();
			int num = 0;
			int num2 = 0;
			int num3 = text4.Length;
			while (num != text3.Length && num2 != text4.Length)
			{
				if (text4[num2] == '*')
				{
					num3 = num2;
					num2++;
					if (num2 == text4.Length)
					{
						num = text3.Length;
					}
					else
					{
						while (num != text3.Length)
						{
							if (text3[num] == text4[num2])
							{
								break;
							}
							num++;
						}
					}
				}
				else if (text4[num2] != text3[num])
				{
					if (num3 == text4.Length)
					{
						return false;
					}
					num2 = num3;
					num3 = text4.Length;
				}
				else
				{
					num2++;
					num++;
				}
			}
			return num2 == text4.Length && num == text3.Length;
		}
		private void A(string path, ArchiveManager.ClassA a, string text, SearchOption searchOption, List<string> list)
		{
			if (searchOption == SearchOption.AllDirectories && a.Directories != null)
			{
				foreach (ArchiveManager.ClassA current in a.Directories.Values)
				{
					string text2 = Path.Combine(path, current.Name);
					this.A(text2, current, text, searchOption, list);
				}
			}
			if (a.Files != null)
			{
				for (int i = 0; i < a.Files.Count; i++)
				{
					string text3 = a.Files[i];
					if (this.A(text3, text))
					{
						string item = Path.Combine(path, text3);
						list.Add(item);
					}
				}
			}
		}
		private void a(string path, ArchiveManager.ClassA a, string text, SearchOption searchOption, List<string> list)
		{
			if (a.Directories != null)
			{
				if (searchOption == SearchOption.AllDirectories)
				{
					foreach (ArchiveManager.ClassA current in a.Directories.Values)
					{
						string text2 = Path.Combine(path, current.Name);
						this.a(text2, current, text, searchOption, list);
					}
				}
				foreach (ArchiveManager.ClassA current2 in a.Directories.Values)
				{
					if (!current2.RealFileSystemDirectory)
					{
						string name = current2.Name;
						if (this.A(name, text))
						{
							string item = Path.Combine(path, name);
							list.Add(item);
						}
					}
				}
			}
		}
		internal void A(string text, string text2, SearchOption searchOption, List<string> list)
		{
			ArchiveManager.ClassA a = this.A(text, false);
			if (a != null)
			{
				this.A(text, a, text2, searchOption, list);
			}
		}
		internal void a(string text, string text2, SearchOption searchOption, List<string> list)
		{
			ArchiveManager.ClassA a = this.A(text, false);
			if (a != null)
			{
				this.a(text, a, text2, searchOption, list);
			}
		}
		[CompilerGenerated]
		private static int A(ArchiveManager.ClassB b, ArchiveManager.ClassB b2)
		{
			if (b.LoadingPriority > b2.LoadingPriority)
			{
				return -1;
			}
			if (b.LoadingPriority < b2.LoadingPriority)
			{
				return 1;
			}
			return 0;
		}
	}
}
