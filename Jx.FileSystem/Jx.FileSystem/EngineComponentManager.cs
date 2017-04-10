using A;
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
				internal string aX = "";
				internal EngineComponentManager.PlatformFlags ax = EngineComponentManager.PlatformFlags.Windows | EngineComponentManager.PlatformFlags.MacOSX;
				internal bool aY;
				public string Path
				{
					get
					{
						return this.aX;
					}
				}
				public EngineComponentManager.PlatformFlags Platforms
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
						return this.aY;
					}
				}
				internal PathInfo()
				{
				}
				public override string ToString()
				{
					if (string.IsNullOrEmpty(this.aX))
					{
						return "(is not initialized)";
					}
					return this.aX;
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
				switch (D.Platform)
				{
				case D.A.Windows:
					return (this.aF & EngineComponentManager.PlatformFlags.Windows) != (EngineComponentManager.PlatformFlags)0;
				case D.A.MacOSX:
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
				switch (D.Platform)
				{
				case D.A.Windows:
				{
					List<EngineComponentManager.ComponentInfo.PathInfo> list = new List<EngineComponentManager.ComponentInfo.PathInfo>();
					foreach (EngineComponentManager.ComponentInfo.PathInfo current in this.af)
					{
						if (current.aY && (current.ax & EngineComponentManager.PlatformFlags.Windows) != (EngineComponentManager.PlatformFlags)0)
						{
							bool flag = false;
							if (IntPtr.Size == 8)
							{
								if (current.aX.ToLower().Contains("nativedlls") && current.aX.ToLower().Contains("windows_x86"))
								{
									flag = true;
								}
							}
							else if (current.aX.ToLower().Contains("nativedlls") && current.aX.ToLower().Contains("windows_x64"))
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
				case D.A.MacOSX:
				{
					List<EngineComponentManager.ComponentInfo.PathInfo> list2 = new List<EngineComponentManager.ComponentInfo.PathInfo>();
					foreach (EngineComponentManager.ComponentInfo.PathInfo current2 in this.af)
					{
						if (current2.aY && (current2.ax & EngineComponentManager.PlatformFlags.MacOSX) != (EngineComponentManager.PlatformFlags)0)
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
				switch (D.Platform)
				{
				case D.A.Windows:
					return (this.ae & EngineComponentManager.PlatformFlags.Windows) != (EngineComponentManager.PlatformFlags)0;
				case D.A.MacOSX:
					return (this.ae & EngineComponentManager.PlatformFlags.MacOSX) != (EngineComponentManager.PlatformFlags)0;
				default:
					return false;
				}
			}
		}
		private static EngineComponentManager I;
		private Dictionary<string, EngineComponentManager.ComponentInfo> i = new Dictionary<string, EngineComponentManager.ComponentInfo>();
		public static EngineComponentManager Instance
		{
			get
			{
				return EngineComponentManager.I;
			}
		}
		public ICollection<EngineComponentManager.ComponentInfo> Components
		{
			get
			{
				return this.i.Values;
			}
		}
		internal static void A()
		{
			if (EngineComponentManager.I != null)
			{
				Log.Fatal("EngineComponentManager: Init: The instance of the manager is already initialized.");
			}
			EngineComponentManager.I = new EngineComponentManager();
			EngineComponentManager.I.b();
		}
		internal static void a()
		{
			if (EngineComponentManager.I != null)
			{
				EngineComponentManager.I.C();
				EngineComponentManager.I = null;
			}
		}
		private void A(string text)
		{
			string arg;
			TextBlock textBlock = TextBlockUtils.LoadFromRealFile(text, out arg);
			if (textBlock != null)
			{
				try
				{
					EngineComponentManager.ComponentInfo componentInfo = new EngineComponentManager.ComponentInfo();
					componentInfo.aD = Path.GetFileNameWithoutExtension(text);
					if (this.i.ContainsKey(componentInfo.aD))
					{
						Log.Fatal("EngineComponentManager: ParseComponentFile: The component with a name \"{0}\" is already registered.", componentInfo.aD);
					}
					this.i.Add(componentInfo.aD, componentInfo);
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
							pathInfo.aX = current.GetAttribute("path");
							if (current.IsAttributeExist("platforms"))
							{
								pathInfo.ax = (EngineComponentManager.PlatformFlags)Enum.Parse(typeof(EngineComponentManager.PlatformFlags), current.GetAttribute("platforms"));
							}
							if (current.IsAttributeExist("entryPoint"))
							{
								pathInfo.aY = bool.Parse(current.GetAttribute("entryPoint"));
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
					Log.Warning("EngineComponentManager: Parsing file \"{0}\" failed ({1}).", text, ex.Message);
					return;
				}
			}
			Log.Warning("EngineComponentManager: Parsing file \"{0}\" failed ({1}).", text, arg);
		}
		private void B()
		{
			string path = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, "Components");
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*.component", SearchOption.AllDirectories);
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					this.A(text);
				}
			}
		}
		private void b()
		{
			this.B();
		}
		private void C()
		{
		}
		public EngineComponentManager.ComponentInfo GetComponentByName(string name)
		{
			EngineComponentManager.ComponentInfo result;
			if (this.i.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}
		public EngineComponentManager.ComponentInfo[] GetComponentsByType(EngineComponentManager.ComponentTypeFlags typeFlags, bool onlySupportedOnThisPlatform)
		{
			List<EngineComponentManager.ComponentInfo> list = new List<EngineComponentManager.ComponentInfo>();
			foreach (EngineComponentManager.ComponentInfo current in this.Components)
			{
				if ((current.aE & typeFlags) != (EngineComponentManager.ComponentTypeFlags)0 && (!onlySupportedOnThisPlatform || current.IsSupportedOnThisPlatform()))
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}
	}
}
