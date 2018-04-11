using Jx.FileSystem;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Jx.EntitySystem
{
	public class LogicSystemManager : LogicComponent
	{
		public const string referencesToCompileDLLDefault = "System.dll\r\nSystem.Drawing.dll\r\n\r\nEngineApp.dll\r\nUISystem.dll\r\nFileSystem.dll\r\nLog.dll\r\nMathEx.dll\r\nRenderer.dll\r\nPhysicsSystem.dll\r\nSoundSystem.dll\r\nEntitySystem.dll\r\nMapSystem.dll\r\nUtils.dll\r\nHeightmapTerrain.dll\r\nDecorativeObjectManager.dll\r\n\r\nProjectCommon.dll\r\nProjectEntities.dll";
		public const string usingNamespacesToCodeGenerationDefault = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Text;\r\n\r\nusing Engine;\r\nusing Engine.FileSystem;\r\nusing Engine.MathEx;\r\nusing Engine.Utils;\r\nusing Engine.Renderer;\r\nusing Engine.PhysicsSystem;\r\nusing Engine.SoundSystem;\r\nusing Engine.UISystem;\r\nusing Engine.EntitySystem;\r\nusing Engine.MapSystem;\r\n\r\nusing ProjectCommon;\r\nusing ProjectEntities;";
		private static LogicSystemManager aBQ;
		[Entity.FieldSerializeAttribute("referencesToCompileDLL")]
		private string aBq = "System.dll\r\nSystem.Drawing.dll\r\n\r\nEngineApp.dll\r\nUISystem.dll\r\nFileSystem.dll\r\nLog.dll\r\nMathEx.dll\r\nRenderer.dll\r\nPhysicsSystem.dll\r\nSoundSystem.dll\r\nEntitySystem.dll\r\nMapSystem.dll\r\nUtils.dll\r\nHeightmapTerrain.dll\r\nDecorativeObjectManager.dll\r\n\r\nProjectCommon.dll\r\nProjectEntities.dll";
		[Entity.FieldSerializeAttribute("usingNamespacesToCodeGeneration")]
		private string aBR = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Text;\r\n\r\nusing Engine;\r\nusing Engine.FileSystem;\r\nusing Engine.MathEx;\r\nusing Engine.Utils;\r\nusing Engine.Renderer;\r\nusing Engine.PhysicsSystem;\r\nusing Engine.SoundSystem;\r\nusing Engine.UISystem;\r\nusing Engine.EntitySystem;\r\nusing Engine.MapSystem;\r\n\r\nusing ProjectCommon;\r\nusing ProjectEntities;";
		[Entity.FieldSerializeAttribute("mapClassManager")]
		private LogicClassManager aBr;
		public static LogicSystemManager Instance
		{
			get
			{
				return LogicSystemManager.aBQ;
			}
		}
		public LogicClassManager MapClassManager
		{
			get
			{
				return this.aBr;
			}
		}
		[DefaultValue("System.dll\r\nSystem.Drawing.dll\r\n\r\nEngineApp.dll\r\nUISystem.dll\r\nFileSystem.dll\r\nLog.dll\r\nMathEx.dll\r\nRenderer.dll\r\nPhysicsSystem.dll\r\nSoundSystem.dll\r\nEntitySystem.dll\r\nMapSystem.dll\r\nUtils.dll\r\nHeightmapTerrain.dll\r\nDecorativeObjectManager.dll\r\n\r\nProjectCommon.dll\r\nProjectEntities.dll")]
		public string ReferencesToCompileDLL
		{
			get
			{
				return this.aBq;
			}
			set
			{
				this.aBq = value;
			}
		}
		[DefaultValue("using System;\r\nusing System.Collections.Generic;\r\nusing System.Text;\r\n\r\nusing Engine;\r\nusing Engine.FileSystem;\r\nusing Engine.MathEx;\r\nusing Engine.Utils;\r\nusing Engine.Renderer;\r\nusing Engine.PhysicsSystem;\r\nusing Engine.SoundSystem;\r\nusing Engine.UISystem;\r\nusing Engine.EntitySystem;\r\nusing Engine.MapSystem;\r\n\r\nusing ProjectCommon;\r\nusing ProjectEntities;")]
		public string UsingNamespacesToCodeGeneration
		{
			get
			{
				return this.aBR;
			}
			set
			{
				this.aBR = value;
			}
		}
		public LogicSystemManager()
		{
			if (LogicSystemManager.aBQ != null)
			{
				Log.Fatal("LogicSystemManager already created");
			}
			LogicSystemManager.aBQ = this;
		}
		protected override void OnPostCreate(bool loaded)
		{
			base.OnPostCreate(loaded);
			if (this.aBr == null)
			{
				this.aBr = (LogicClassManager)Entities.Instance.Create(EntityTypes.Instance.GetByName("LogicClassManager"), this);
				this.aBr.PostCreate();
			}
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			LogicSystemManager.aBQ = null;
		}
		public bool IsExistsScriptsForCompiling()
		{
			return this.aBr.IsExistsScriptsDataForCompiling();
		}
		private Assembly A(AssemblyName assemblyName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				if (assembly.FullName == assemblyName.FullName)
				{
					return assembly;
				}
			}
			return null;
		}
		public bool CompileScripts(string logicSystemDirectory, string dllFileName, out CompilerResults compilerResults, out List<LogicClass.CompileScriptsData> compileScriptsData)
		{
			compilerResults = null;
			compileScriptsData = null;
			string text = logicSystemDirectory + "\\_Temp";
			bool result;
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string directoryName = Path.GetDirectoryName(dllFileName);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				if (File.Exists(dllFileName))
				{
					File.Delete(dllFileName);
				}
				string[] files = Directory.GetFiles(text);
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string path = array[i];
					File.Delete(path);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				result = false;
				return result;
			}
			string text2 = "";
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dllFileName);
			string text3 = fileNameWithoutExtension;
			for (int j = 0; j < text3.Length; j++)
			{
				char c = text3[j];
				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '$' || c == '#')
				{
					text2 += c;
				}
				else
				{
					text2 += "_";
				}
			}
			text2 += "_LogicSystemScripts";
			List<LogicClass.CompileScriptsData> compileScriptsData2 = this.aBr.GetCompileScriptsData(text2);
			if (compileScriptsData2.Count == 0)
			{
				return true;
			}
			compileScriptsData = compileScriptsData2;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (LogicClass.CompileScriptsData current in compileScriptsData2)
			{
				string text4 = "";
				foreach (string current2 in current.Strings)
				{
					text4 = text4 + current2 + "\r\n";
				}
				list.Add(text4);
				string text5 = text + "\\" + current.ClassName + ".cs";
				try
				{
					using (StreamWriter streamWriter = new StreamWriter(text5))
					{
						streamWriter.Write(text4);
					}
				}
				catch (Exception ex2)
				{
					Log.Error(ex2.Message);
					result = false;
					return result;
				}
				list2.Add(text5);
			}
			string text6 = text + "\\_AssemblyInfo.cs";
			try
			{
				using (StreamWriter streamWriter2 = new StreamWriter(text6))
				{
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(dllFileName);
					Version version = executingAssembly.GetName().Version;
					string arg = string.Format("{0}.{1}.{2}.{3}", new object[]
					{
						version.Major,
						version.Minor,
						version.Build,
						version.Revision
					});
					streamWriter2.WriteLine("using System.Reflection;");
					streamWriter2.WriteLine("using System.Runtime.CompilerServices;");
					streamWriter2.WriteLine("using System.Runtime.InteropServices;");
					streamWriter2.WriteLine("[assembly: AssemblyTitle( \"{0}\" )]", fileNameWithoutExtension2);
					streamWriter2.WriteLine("[assembly: AssemblyDescription( \"\" )]");
					streamWriter2.WriteLine("[assembly: AssemblyConfiguration( \"\" )]");
					streamWriter2.WriteLine("[assembly: AssemblyCompany( \"NeoAxis Group Ltd.\" )]");
					streamWriter2.WriteLine("[assembly: AssemblyProduct( \"{0}\" )]", fileNameWithoutExtension2);
					streamWriter2.WriteLine("[assembly: AssemblyCopyright( \"Copyright (C) 2006-2014 NeoAxis Group Ltd.\" )]");
					streamWriter2.WriteLine("[assembly: AssemblyTrademark( \"\" )]");
					streamWriter2.WriteLine("[assembly: AssemblyCulture( \"\" )]");
					streamWriter2.WriteLine("[assembly: ComVisible( false )]");
					streamWriter2.WriteLine("[assembly: AssemblyVersion( \"{0}\" )]", arg);
					streamWriter2.WriteLine("[assembly: AssemblyFileVersion( \"{0}\" )]", arg);
				}
			}
			catch (Exception ex3)
			{
				Log.Error(ex3.Message);
				result = false;
				return result;
			}
			list2.Add(text6);
			List<string> list3 = new List<string>();
			string[] array2 = LogicSystemManager.Instance.ReferencesToCompileDLL.Split(new char[]
			{
				'\r',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int k = 0; k < array2.Length; k++)
			{
				string item = array2[k];
				if (!list3.Contains(item))
				{
					list3.Add(item);
				}
			}
			foreach (Assembly current3 in EntitySystemWorld.Instance.EntityClassAssemblies)
			{
				string item2 = current3.GetName().Name + ".dll";
				if (!list3.Contains(item2))
				{
					list3.Add(item2);
				}
			}
			bool flag = false;
			bool flag2 = false;
			foreach (string current4 in list3)
			{
				string text7 = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, current4);
				if (File.Exists(text7))
				{
					AssemblyName assemblyName = AssemblyName.GetAssemblyName(text7);
					Assembly assembly = this.A(assemblyName);
					if (assembly != null && assembly.ImageRuntimeVersion.Length > 3)
					{
						if (assembly.ImageRuntimeVersion.Substring(0, 4) == "v3.5")
						{
							flag = true;
						}
						if (assembly.ImageRuntimeVersion.Substring(0, 3) == "v4.")
						{
							flag2 = true;
						}
					}
				}
			}
			string value;
			if (flag2)
			{
				value = "v4.0";
			}
			else if (flag)
			{
				value = "v3.5";
			}
			else
			{
				value = "v2.0";
			}
			try
			{
				CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider(new Dictionary<string, string>
				{

					{
						"CompilerVersion",
						value
					}
				});
				CompilerParameters compilerParameters = new CompilerParameters();
				compilerParameters.GenerateExecutable = false;
				compilerParameters.GenerateInMemory = false;
				compilerParameters.OutputAssembly = dllFileName;
				compilerParameters.IncludeDebugInformation = true;
				compilerParameters.CompilerOptions = "/optimize /unsafe";
				foreach (string current5 in list3)
				{
					string path2 = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, current5);
					if (File.Exists(path2))
					{
						compilerParameters.ReferencedAssemblies.Add(current5);
					}
					else
					{
						path2 = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), current5);
						if (File.Exists(path2))
						{
							compilerParameters.ReferencedAssemblies.Add(current5);
						}
					}
				}
				compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(compilerParameters, list2.ToArray());
				bool flag3 = true;
				foreach (CompilerError compilerError in compilerResults.Errors)
				{
					if (!compilerError.IsWarning)
					{
						flag3 = false;
					}
				}
				result = flag3;
			}
			catch (Exception ex4)
			{
				Log.Error(ex4.Message);
				result = false;
			}
			return result;
		}
		public bool CompileScripts(string logicSystemDirectory, string dllFileName, out CompilerResults compilerResults)
		{
			List<LogicClass.CompileScriptsData> list;
			return this.CompileScripts(logicSystemDirectory, dllFileName, out compilerResults, out list);
		}
	}
}
