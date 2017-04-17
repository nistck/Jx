using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Jx.FileSystem
{
	public sealed class ComponentManager
	{
		[Flags]
		public enum ComponentTypeFlags
		{
			RenderingSystem = 1,
			PhysicsSystem = 2,
			SoundSystem = 4,
			HighLevelMaterialClasses = 8,
			CompositorInstanceClasses = 16,
			GUIControlClasses = 32,
			Archive = 64,
			EntityClasses = 128,
			EngineInitializationClass = 256,
			ResourceEditorAddon = 512,
			ImportingModel = 1024,
			MapEditorAddon = 2048,
			StaticLightingCalculation = 4096,
			Localization = 8192,
			Application = 16384,
			Other = 32768
		}
		[Flags]
		public enum PlatformFlags
		{
			Windows = 1,
			MacOSX = 2
		}
		public class ComponentInfo
		{
			public class PathInfo
			{
				internal string path = "";
				internal PlatformFlags ax = PlatformFlags.Windows | PlatformFlags.MacOSX;
				internal bool entryPoint;

				public string Path
				{
					get
					{
						return this.path;
					}
				}
				public PlatformFlags Platforms
				{
					get
					{
						return this.ax;
					}
				}
				public bool EntryPoint
				{
					get
					{
						return this.entryPoint;
					}
				}

				internal PathInfo()
				{
				}

				public override string ToString()
				{
					if (string.IsNullOrEmpty(this.path))
					{
						return "(is not initialized)";
					}
					return this.path;
				}
			}

			internal string name = "";
			internal string fullName = "";
			internal ComponentTypeFlags componentTypeFlag;
			internal PlatformFlags platformFlag = PlatformFlags.Windows | PlatformFlags.MacOSX;
			internal PlatformFlags aF;
			internal List<PathInfo> af = new List<PathInfo>();
			internal ReadOnlyCollection<PathInfo> paths;
			internal string applicationName_MacOSX = "";

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string FullName
			{
				get
				{
					return this.fullName;
				}
			}
			public ComponentTypeFlags ComponentTypes
			{
				get
				{
					return this.componentTypeFlag;
				}
			}
			public PlatformFlags Platforms
			{
				get
				{
					return this.platformFlag;
				}
			}
			public PlatformFlags EnableByDefaultPlatforms
			{
				get
				{
					return this.aF;
				}
			}
			public ReadOnlyCollection<PathInfo> Paths
			{
				get
				{
					return this.paths;
				}
			}

			public string MacOSXApplicationName
			{
				get
				{
					return this.applicationName_MacOSX;
				}
				set
				{
					this.applicationName_MacOSX = value;
				}
			}

			internal ComponentInfo()
			{
				this.paths = new ReadOnlyCollection<ComponentManager.ComponentInfo.PathInfo>(this.af);
			}

			public bool IsEnabledByDefaultForThisPlatform()
			{
				switch (PlatformInfo.Platform)
				{
				case PlatformInfo.PlanformType.Windows:
					return (this.aF & ComponentManager.PlatformFlags.Windows) != (ComponentManager.PlatformFlags)0;
				case PlatformInfo.PlanformType.MacOSX:
					return (this.aF & ComponentManager.PlatformFlags.MacOSX) != (ComponentManager.PlatformFlags)0;
				default:
					return false;
				}
			}

			public override string ToString()
			{
				if (!string.IsNullOrEmpty(this.fullName))
				{
					return this.fullName;
				}
				return this.name;
			}

			public PathInfo[] GetAllEntryPointsForThisPlatform()
			{
				switch (PlatformInfo.Platform)
				{
				case PlatformInfo.PlanformType.Windows:
				{
					List<PathInfo> list = new List<PathInfo>();
					foreach (PathInfo current in this.af)
					{
						if (current.entryPoint && (current.ax & PlatformFlags.Windows) != (ComponentManager.PlatformFlags)0)
						{
							bool flag = false;
							if (IntPtr.Size == 8)
							{
								if (current.path.ToLower().Contains("nativedlls") && current.path.ToLower().Contains("windows_x86"))
								{
									flag = true;
								}
							}
							else if (current.path.ToLower().Contains("nativedlls") && current.path.ToLower().Contains("windows_x64"))
							{
								flag = true;
							}
							if (!flag)
							{
								list.Add(current);
							}
						}
					}
					return list.ToArray();
				}
				case PlatformInfo.PlanformType.MacOSX:
				{
					List<PathInfo> list2 = new List<PathInfo>();
					foreach (PathInfo current2 in this.af)
					{
						if (current2.entryPoint && (current2.ax & PlatformFlags.MacOSX) != (PlatformFlags)0)
						{
							list2.Add(current2);
						}
					}
					return list2.ToArray();
				}
				default:
					return null;
				}
			}
			public PathInfo GetFirstEntryPointForThisPlatform()
			{
				PathInfo[] allEntryPointsForThisPlatform = this.GetAllEntryPointsForThisPlatform();
				if (allEntryPointsForThisPlatform.Length != 0)
				{
					return allEntryPointsForThisPlatform[0];
				}
				return null;
			}
			public bool IsSupportedOnThisPlatform()
			{
				switch (PlatformInfo.Platform)
				{
				case PlatformInfo.PlanformType.Windows:
					return (this.platformFlag & PlatformFlags.Windows) != (PlatformFlags)0;
				case PlatformInfo.PlanformType.MacOSX:
					return (this.platformFlag & PlatformFlags.MacOSX) != (PlatformFlags)0;
				default:
					return false;
				}
			}
		}
		private static ComponentManager instance;
		private Dictionary<string, ComponentInfo> componentsDic = new Dictionary<string, ComponentInfo>();
		public static ComponentManager Instance
		{
			get
			{
				return instance;
			}
		}
		public ICollection<ComponentInfo> Components
		{
			get
			{
				return componentsDic.Values;
			}
		}

		internal static void Init()
		{
			if (instance != null)
			{
				Log.Fatal("EngineComponentManager: Init: The instance of the manager is already initialized.");
			}
			instance = new ComponentManager();
			instance._Startup();
		}

		internal static void Unload()
		{
			if (instance != null)
			{
				instance.unload();
				instance = null;
			}
		}

		private void LoadComponent(string p)
		{
            string arg;
			TextBlock textBlock = TextBlockUtils.LoadFromRealFile(p, out arg);
			if (textBlock != null)
			{
				try
				{
					ComponentInfo componentInfo = new ComponentInfo();
					componentInfo.name = Path.GetFileNameWithoutExtension(p);
					if (this.componentsDic.ContainsKey(componentInfo.name))
					{
						Log.Fatal("EngineComponentManager: ParseComponentFile: The component with a name \"{0}\" is already registered.", componentInfo.name);
                        return;
					}

					this.componentsDic.Add(componentInfo.name, componentInfo);
					componentInfo.fullName = textBlock.GetAttribute("fullName", "[FULL NAME IS NOT SPECIFIED]");
					if (textBlock.IsAttributeExist("componentTypes"))
					{
						componentInfo.componentTypeFlag = (ComponentTypeFlags)Enum.Parse(typeof(ComponentTypeFlags), textBlock.GetAttribute("componentTypes"));
					}
					if (textBlock.IsAttributeExist("platforms"))
					{
						componentInfo.platformFlag = (PlatformFlags)Enum.Parse(typeof(PlatformFlags), textBlock.GetAttribute("platforms"));
					}
					if (textBlock.IsAttributeExist("enableByDefaultPlatforms"))
					{
						string attribute = textBlock.GetAttribute("enableByDefaultPlatforms");
						if (!string.IsNullOrEmpty(attribute))
						{
							componentInfo.aF = (PlatformFlags)Enum.Parse(typeof(PlatformFlags), attribute);
						}
						else
						{
							componentInfo.aF = (ComponentManager.PlatformFlags)0;
						}
					}
					foreach (TextBlock current in textBlock.Children)
					{
						if (current.Name == "path")
						{
							ComponentInfo.PathInfo pathInfo = new ComponentInfo.PathInfo();
							pathInfo.path = current.GetAttribute("path");
							if (current.IsAttributeExist("platforms"))
							{
								pathInfo.ax = (PlatformFlags)Enum.Parse(typeof(PlatformFlags), current.GetAttribute("platforms"));
							}
							if (current.IsAttributeExist("entryPoint"))
							{
								pathInfo.entryPoint = bool.Parse(current.GetAttribute("entryPoint"));
							}
							componentInfo.af.Add(pathInfo);
						}
					}
					if (textBlock.IsAttributeExist("macOSXApplicationName"))
					{
						componentInfo.applicationName_MacOSX = textBlock.GetAttribute("macOSXApplicationName");
					}
					return;
				}
				catch (Exception ex)
				{
					Log.Warning("EngineComponentManager: Parsing file \"{0}\" failed ({1}).", p, ex.Message);
					return;
				}
			}
			Log.Warning("EngineComponentManager: Parsing file \"{0}\" failed ({1}).", p, arg);
		}

		private void InitComponents()
		{
			string path = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "Components");
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*.component", SearchOption.AllDirectories); 
				for (int i = 0; i < files.Length; i++)
				{
					string file = files[i];
                    Log.Info(">> ÕÒµ½×é¼þ: {0}", file);
					LoadComponent(file);
				}
			}
		}

		private void _Startup()
		{
			this.InitComponents();
		}

		private void unload()
		{
		}

		public ComponentInfo GetComponentByName(string name)
		{
			ComponentInfo result;
			if (this.componentsDic.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		public ComponentInfo[] GetComponentsByType(ComponentTypeFlags typeFlags, bool onlySupportedOnThisPlatform)
		{
			List<ComponentInfo> list = new List<ComponentInfo>();
			foreach (ComponentInfo current in this.Components)
			{
				if ((current.componentTypeFlag & typeFlags) != (ComponentTypeFlags)0 && (!onlySupportedOnThisPlatform || current.IsSupportedOnThisPlatform()))
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}
	}
}
