using System;
using System.Collections.Generic;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicSystemClass
	{
		private Type aAK;
		private Dictionary<string, MethodInfo> aAk = new Dictionary<string, MethodInfo>();
		private Dictionary<string, PropertyInfo> aAL = new Dictionary<string, PropertyInfo>();
		private LogicSystemClass aAl;
		private bool aAM;
		private PropertyInfo aAm;
		public string Name
		{
			get
			{
				return this.aAK.Name;
			}
		}
		public Dictionary<string, MethodInfo>.ValueCollection Methods
		{
			get
			{
				return this.aAk.Values;
			}
		}
		public Dictionary<string, PropertyInfo>.ValueCollection Properties
		{
			get
			{
				return this.aAL.Values;
			}
		}
		public Type ClassType
		{
			get
			{
				return this.aAK;
			}
		}
		public LogicSystemClass BaseClass
		{
			get
			{
				if (this.aAl == null && this.ClassType.BaseType != null)
				{
					this.aAl = LogicSystemClasses.Instance.GetByType(this.ClassType.BaseType);
				}
				return this.aAl;
			}
		}
		public bool CallStaticOverInstance
		{
			get
			{
				return this.aAM;
			}
		}
		public PropertyInfo CallStaticOverInstanceProperty
		{
			get
			{
				return this.aAm;
			}
		}
		public LogicSystemClass(Type classType)
		{
			this.aAK = classType;
			if (classType.GetCustomAttributes(typeof(LogicSystemCallStaticOverInstanceAttribute), false).Length != 0)
			{
				this.aAM = true;
				this.aAm = classType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
			}
			MethodInfo[] methods = classType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo[] array = methods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(true);
				bool flag = false;
				object[] array2 = customAttributes;
				for (int j = 0; j < array2.Length; j++)
				{
					object obj = array2[j];
					LogicSystemBrowsableAttribute logicSystemBrowsableAttribute = obj as LogicSystemBrowsableAttribute;
					if (logicSystemBrowsableAttribute != null)
					{
						flag = logicSystemBrowsableAttribute.Browsable;
						break;
					}
					LogicSystemMethodDisplayAttribute logicSystemMethodDisplayAttribute = obj as LogicSystemMethodDisplayAttribute;
					if (logicSystemMethodDisplayAttribute != null)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.DefineMethod(methodInfo);
				}
			}
			PropertyInfo[] properties = classType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] array3 = properties;
			for (int k = 0; k < array3.Length; k++)
			{
				PropertyInfo propertyInfo = array3[k];
				object[] customAttributes2 = propertyInfo.GetCustomAttributes(true);
				bool flag2 = false;
				object[] array4 = customAttributes2;
				for (int l = 0; l < array4.Length; l++)
				{
					object obj2 = array4[l];
					LogicSystemBrowsableAttribute logicSystemBrowsableAttribute2 = obj2 as LogicSystemBrowsableAttribute;
					if (logicSystemBrowsableAttribute2 != null)
					{
						flag2 = logicSystemBrowsableAttribute2.Browsable;
						break;
					}
					LogicSystemMethodDisplayAttribute logicSystemMethodDisplayAttribute2 = obj2 as LogicSystemMethodDisplayAttribute;
					if (logicSystemMethodDisplayAttribute2 != null)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					this.DefineProperty(propertyInfo);
				}
			}
		}

		internal void DefineMethod(MethodInfo methodInfo)
		{
			this.aAk.Add(DescribeMember(methodInfo), methodInfo);
		}

		internal void DefineProperty(PropertyInfo propertyInfo)
		{
			this.aAL.Add(DescribeMember(propertyInfo), propertyInfo);
		}
		public MethodInfo GetMethod(string name, string[] parameterTypeNames, bool inherit)
		{
			string text = name;
			if (parameterTypeNames != null)
			{
				for (int i = 0; i < parameterTypeNames.Length; i++)
				{
					string str = parameterTypeNames[i];
					text = text + " " + str;
				}
			}
			MethodInfo methodInfo;
			this.aAk.TryGetValue(text, out methodInfo);
			if (methodInfo != null)
			{
				return methodInfo;
			}
			if (this.BaseClass != null)
			{
				return this.BaseClass.GetMethod(name, parameterTypeNames, inherit);
			}
			return null;
		}
		public PropertyInfo GetProperty(string name, string[] parameterTypeNames, bool inherit)
		{
			string text = name;
			if (parameterTypeNames != null)
			{
				for (int i = 0; i < parameterTypeNames.Length; i++)
				{
					string str = parameterTypeNames[i];
					text = text + " " + str;
				}
			}
			PropertyInfo propertyInfo;
			this.aAL.TryGetValue(text, out propertyInfo);
			if (propertyInfo != null)
			{
				return propertyInfo;
			}
			if (this.BaseClass != null)
			{
				return this.BaseClass.GetProperty(name, parameterTypeNames, inherit);
			}
			return null;
		}

		private static string DescribeMember(MethodInfo methodInfo)
		{
			string text = methodInfo.Name;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				text = text + " " + parameterInfo.ParameterType.Name;
			}
			return text;
		}

		private static string DescribeMember(PropertyInfo propertyInfo)
		{
			string text = propertyInfo.Name;
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			for (int i = 0; i < indexParameters.Length; i++)
			{
				ParameterInfo parameterInfo = indexParameters[i];
				text = text + " " + parameterInfo.ParameterType.Name;
			}
			return text;
		}
		public string GetMethodDisplayText(MethodInfo methodInfo, bool ignoreClassName)
		{
			bool flag;
			string methodDisplayText = LogicMethodPropertyInfoUtils.GetMethodDisplayText(methodInfo, out flag);
			if (flag)
			{
				return methodDisplayText;
			}
			if (!ignoreClassName)
			{
				return this.GetDisplayName() + "." + methodDisplayText;
			}
			return methodDisplayText;
		}
		public string GetMethodDisplayText(MethodInfo methodInfo)
		{
			return this.GetMethodDisplayText(methodInfo, false);
		}
		public string GetMethodFormatText(MethodInfo methodInfo, bool ignoreClassName)
		{
			bool flag;
			string methodFormatText = LogicMethodPropertyInfoUtils.GetMethodFormatText(methodInfo, out flag);
			if (flag)
			{
				return methodFormatText;
			}
			if (!ignoreClassName)
			{
				return this.GetDisplayName() + "." + methodFormatText;
			}
			return methodFormatText;
		}
		public string GetMethodFormatText(MethodInfo methodInfo)
		{
			return this.GetMethodFormatText(methodInfo, false);
		}
		public string GetPropertyDisplayText(PropertyInfo propertyInfo, bool ignoreClassName)
		{
			bool flag;
			string propertyDisplayText = LogicMethodPropertyInfoUtils.GetPropertyDisplayText(propertyInfo, out flag);
			if (flag)
			{
				return propertyDisplayText;
			}
			if (!ignoreClassName)
			{
				return this.Name + "." + propertyDisplayText;
			}
			return propertyDisplayText;
		}
		public string GetPropertyDisplayText(PropertyInfo propertyInfo)
		{
			return this.GetPropertyDisplayText(propertyInfo, false);
		}
		public string GetPropertyFormatText(PropertyInfo propertyInfo, bool ignoreClassName)
		{
			bool flag;
			string propertyFormatText = LogicMethodPropertyInfoUtils.GetPropertyFormatText(propertyInfo, out flag);
			if (flag)
			{
				return propertyFormatText;
			}
			if (!ignoreClassName)
			{
				return this.Name + "." + propertyFormatText;
			}
			return propertyFormatText;
		}
		public string GetPropertyFormatText(PropertyInfo propertyInfo)
		{
			return this.GetPropertyFormatText(propertyInfo, false);
		}
		public string GetDisplayName()
		{
			string result = this.Name;
			LogicSystemClassDisplayAttribute[] array = (LogicSystemClassDisplayAttribute[])this.aAK.GetCustomAttributes(typeof(LogicSystemClassDisplayAttribute), false);
			if (array.Length != 0)
			{
				result = array[0].DisplayName;
			}
			return result;
		}
	}
}
