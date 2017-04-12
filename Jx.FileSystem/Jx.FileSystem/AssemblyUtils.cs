using Jx;
using System;
using System.IO;
using System.Reflection;  

namespace Jx.FileSystem
{
	public static class AssemblyUtils
	{
        public static Assembly LoadAssemblyByRealFileName(string realFileName, bool returnNullIfFileIsNotExists)
        {
            string text = realFileName;
            if (!Path.IsPathRooted(realFileName))
            {
                text = Path.Combine(VirtualFileSystem.ExecutableDirectoryPath, text);
            }
            string extension = Path.GetExtension(text);
            if (string.IsNullOrEmpty(extension) || extension.ToLower() != ".dll")
            {
                text += ".dll";
            }
            if (returnNullIfFileIsNotExists && !File.Exists(text))
            {
                return null;
            }
            Assembly result;
            try
            {
                Assembly assembly = Assembly.LoadFrom(text);
                result = assembly;
            }
            catch (Exception ex)
            {
                try
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(text);
                    if (assemblyName == null)
                    {
                        Log.Fatal("The assembly is not found \"{0}\".", text);
                        return null;
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
                    byte[] rawAssembly = File.ReadAllBytes(text);
                    Assembly assembly3 = AppDomain.CurrentDomain.Load(rawAssembly);
                    result = assembly3;
                }
                catch
                {
                    Log.Fatal("Loading assembly failed \"{0}\". Error: {1}", text, ex.Message);
                    result = null;
                    return null;
                }
            }
            return result;
        }


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
                        return null;
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
                    return null;
				}
			}
			return result;
		}
	}
}
