using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

using Jx.FileSystem;

namespace Jx.Ext
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ExtendedFunctionalityDescriptorCustomTypeDescriptor :  CustomTypeDescriptor
    {
        public abstract object GetExtendedFunctionalityDescriptorObject();
        private static Assembly A(AssemblyName assemblyName)
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
        public virtual Type GetExtendedFunctionalityDescriptorType(object obj)
        {
            Type type = obj.GetType();
            while (type != null)
            {
                ExtendedFunctionalityDescriptorAttribute[] array = (ExtendedFunctionalityDescriptorAttribute[])type.GetCustomAttributes(typeof(ExtendedFunctionalityDescriptorAttribute), false);
                Trace.Assert(array.Length < 2, "attributes.Length < 2");
                if (array.Length == 1)
                {
                    ExtendedFunctionalityDescriptorAttribute extendedFunctionalityDescriptorAttribute = array[0];
                    if (extendedFunctionalityDescriptorAttribute.DescriptorType != null)
                    {
                        return extendedFunctionalityDescriptorAttribute.DescriptorType;
                    }
                    string descriptorTypeName = extendedFunctionalityDescriptorAttribute.DescriptorTypeName;
                    string[] array2 = descriptorTypeName.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (array2.Length != 2)
                    {
                        Log.Fatal("ExtendedPropertyDescriptor: Invalid type name \"{0}\".", descriptorTypeName);
                        return null;
                    }
                    string text = array2[0].Trim();
                    string text2 = array2[1].Trim() + ".dll";
                    Assembly assembly = AssemblyUtils.LoadAssemblyByRealFileName(text2, false);
                    Type type2 = assembly.GetType(text);
                    if (type2 == null)
                    {
                        Log.Fatal("ExtendedPropertyDescriptor: Type \"{0}\" not exist in assembly \"{1}\".", text, text2);
                        return null;
                    }
                    return type2;
                }
                else
                {
                    type = type.BaseType;
                }
            }
            return null;
        }
    }
}
