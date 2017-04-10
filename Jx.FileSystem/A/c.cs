using Jx;
using System;
using System.IO;
using System.Reflection;
namespace A
{
	internal static class c
	{
		public static Assembly LoadAssemblyByFileName(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (string.IsNullOrEmpty(extension) || extension.ToLower() != ".dll")
			{
				fileName += ".dll";
			}
			Assembly result;
			try
			{
				Assembly assembly = Assembly.LoadFrom(fileName);
				result = assembly;
			}
			catch (Exception ex)
			{
				try
				{
					AssemblyName assemblyName = AssemblyName.GetAssemblyName(fileName);
					if (assemblyName == null)
					{
						Log.Fatal("The assembly is not found \"{0}\".", fileName);
					}
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					for (int i = 0; i < assemblies.Length; i++)
					{
						Assembly assembly2 = assemblies[i];
						if (string.Compare(assembly2.FullName, assemblyName.FullName, true) == 0)
						{
							result = assembly2;
							return result;
						}
					}
					byte[] rawAssembly = File.ReadAllBytes(fileName);
					Assembly assembly3 = AppDomain.CurrentDomain.Load(rawAssembly);
					result = assembly3;
				}
				catch
				{
					Log.Fatal("Loading assembly failed \"{0}\". Error: {1}", fileName, ex.Message);
					result = null;
				}
			}
			return result;
		}
	}
}
