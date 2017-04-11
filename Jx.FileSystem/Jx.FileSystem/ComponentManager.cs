using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Jx.FileSystem
{
	public sealed class EngineComponentManager
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

			internal string aD = "";
			internal string ad = "";
			internal EngineComponentManager.ComponentTypeFlags aE;
			internal EngineComponentManager.PlatformFlags ae = EngineComponentManager.PlatformFlags.Windows | EngineComponentManager.PlatformFlags.MacOSX;
			internal EngineComponentManager.PlatformFlags aF;
			internal List<EngineComponentManager.ComponentInfo.PathInfo> af = new List<EngineComponentManager.ComponentInfo.PathInfo>();
			internal ReadOnlyCollection<EngineComponentManager.ComponentInfo.PathInfo> aG;
			internal string ag = "";
			public string Name
			{
				get
				{
					return this.aD;
				}
			}
			public string FullName
			{
				get
				{
					return this.ad;
				}
			}
			public EngineComponentManager.ComponentTypeFlags ComponentTypes
			{
				get
				{
					return this.aE;
				}
			}
			public EngineComponentManager.PlatformFlags Platforms
			{
				get
				{
					return this.ae;
				}
			}
			public EngineComponentManager.PlatformFlags EnableByDefaultPlatforms
			{
				get
				{
					return this.aF;
				}
			}
			public ReadOnlyCollection<EngineComponentManager.ComponentInfo.PathInfo> Paths
			{
				get
				{
					return this.aG;
				}
			}
			public string MacOSXApplicationName
			{
				get
				{
					return this.ag;
				}
				set
				{
					this.ag = value;
				}
			}
			internal ComponentInfo()
			{
				this.aG = new ReadOnlyCollection<EngineComponentManager.ComponentInfo.PathInfo>(this.af);
			}
			public bool IsEnabledByDefaultForThisPlatform()
			{
				switch (PlatformInfo.Platform)
				{
				case PlatformInfo.PlanformType.Windows:
					return (this.aF & EngineComponentManager.PlatformFlags.Windows) != (EngineComponentManager.PlatformFlags)0;
				case PlatformInfo.PlanformType.MacOSX:
					return (this.aF & EngineComponentManager.PlatformFlags.MacOSX) != (EngineComponentManager.PlatformFlags)0;
				default:
					return false;
				}
			}
			public override string ToString()
			{
				if (!string.IsNullOrEmpty(this.ad))
				{
					return this.ad;
				}
				return this.aD;
			}
			public EngineComponentManager.ComponentInfo.PathInfo[] GetAllEntryPointsForThisPlatform()
			{
				switch (PlatformInfo.Platform)
				{
				case PlatformInfo.PlanformType.Windows:
				{
					List<EngineComponentManager.ComponentInfo.PathInfo> list = new List<EngineComponentManager.ComponentInfo.PathInfo>();
					foreach (EngineComponentManager.ComponentInfo.PathInfo current in this.af)
					{
						if (current.entryPoint && (current.ax & EngineComponentManager.PlatformFlags.Windows) != (EngineComponentManager.PlatformFlags)0)
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
					List<EngineComponentManager.ComponentInfo.PathInfo> list2 = new List<EngineComponentManager.ComponentInfo.PathInfo>();
					foreach (EngineComponentManager.ComponentInfo.PathInfo current2 in this.af)
					{
						if (current2.entryPoint && (current2.ax & EngineComponentManager.PlatformFlags.MacOSX) != (EngineComponentManager.PlatformFlags)0)
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
			public EngineComponentManager.ComponentInfo.PathInfo GetFirstEntryPointForThisPlatform()
			{
				EngineComponentManager.ComponentInfo.PathInfo[] allEntryPointsForThisPlatform = this.GetAllEntryPointsForThisPlatform();
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
					return (this.ae & EngineComponentManager.PlatformFlags.Windows) != (EngineComponentManager.PlatformFlags)0;
				case PlatformInfo.PlanformType.MacOSX:
					return (this.ae & EngineComponentManager.PlatformFlags.MacOSX) != (EngineComponentManager.PlatformFlags)0;
				default:
					return false;
				}
			}
		}
		private static EngineComponentManager instance;
		private Dictionary<string, ComponentInfo> componentsDic = new Dictionary<string, ComponentInfo>();
		public static EngineComponentManager Instance
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
			instance = new EngineComponentManager();
			instance._Startup();
		}

		internal static void Unload()
		{
			if (instance != null)
			{
				instance.C();
				instance = null;
			}
		}

		private void CreateComponent(string p)
		{
            string arg;
			TextBlock textBlock = TextBlockUtils.LoadFromRealFile(p, out arg);
			if (textBlock != null)
			{
				try
				{
					ComponentInfo componentInfo = new ComponentInfo();
					componentInfo.aD = Path.GetFileNameWithoutExtension(p);
					if (this.componentsDic.ContainsKey(componentInfo.aD))
					{
						Log.Fatal("EngineComponentManager: ParseComponentFile: The component with a name \"{0}\" is already registered.", componentInfo.aD);
					}

					this.componentsDic.Add(componentInfo.aD, componentInfo);
					componentInfo.ad = textBlock.GetAttribute("fullName", "[FULL NAME IS NOT SPECIFIED]");
					if (textBlock.IsAttributeExist("componentTypes"))
					{
						componentInfo.aE = (EngineComponentManager.ComponentTypeFlags)Enum.Parse(typeof(EngineComponentManager.ComponentTypeFlags), textBlock.GetAttribute("componentTypes"));
					}
					if (textBlock.IsAttributeExist("platforms"))
					{
						componentInfo.ae = (EngineComponentManager.PlatformFlags)Enum.Parse(typeof(EngineComponentManager.PlatformFlags), textBlock.GetAttribute("platforms"));
					}
					if (textBlock.IsAttributeExist("enableByDefaultPlatforms"))
					{
						string attribute = textBlock.GetAttribute("enableByDefaultPlatforms");
						if (!string.IsNullOrEmpty(attribute))
						{
							componentInfo.aF = (EngineComponentManager.PlatformFlags)Enum.Parse(typeof(EngineComponentManager.PlatformFlags), attribute);
						}
						else
						{
							componentInfo.aF = (EngineComponentManager.PlatformFlags)0;
						}
					}
					foreach (TextBlock current in textBlock.Children)
					{
						if (current.Name == "path")
						{
							EngineComponentManager.ComponentInfo.PathInfo pathInfo = new EngineComponentManager.ComponentInfo.PathInfo();
							pathInfo.path = current.GetAttribute("path");
							if (current.IsAttributeExist("platforms"))
							{
								pathInfo.ax = (EngineComponentManager.PlatformFlags)Enum.Parse(typeof(EngineComponentManager.PlatformFlags), current.GetAttribute("platforms"));
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
						componentInfo.ag = textBlock.GetAttribute("macOSXApplicationName");
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
					CreateComponent(file);
				}
			}
		}

		private void _Startup()
		{
			this.InitComponents();
		}
		private void C()
		{
		}

		public ComponentInfo GetComponentByName(string name)
		{
			EngineComponentManager.ComponentInfo result;
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
				if ((current.aE & typeFlags) != (ComponentTypeFlags)0 && (!onlySupportedOnThisPlatform || current.IsSupportedOnThisPlatform()))
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}
	}
}
