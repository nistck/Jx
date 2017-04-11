using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public static class LogicMethodPropertyInfoUtils
	{
		private static string A(string text)
		{
			if (text == "Addition")
			{
				return "+";
			}
			if (text == "Subtraction")
			{
				return "-";
			}
			if (text == "Multiply")
			{
				return "*";
			}
			if (text == "Division")
			{
				return "/";
			}
			if (text == "Equality")
			{
				return "==";
			}
			if (text == "Inequality")
			{
				return "!=";
			}
			if (text == "UnaryNegation")
			{
				return "-";
			}
			return text;
		}
		public static string GetMethodDisplayText(MethodInfo methodInfo, out bool displayAttributeUsed)
		{
			string text = "";
			LogicSystemMethodDisplayAttribute[] array = (LogicSystemMethodDisplayAttribute[])methodInfo.GetCustomAttributes(typeof(LogicSystemMethodDisplayAttribute), true);
			if (array.Length != 0)
			{
				text = array[0].DisplayText;
				displayAttributeUsed = true;
			}
			else
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (methodInfo.Name.Length > 3 && methodInfo.Name.Substring(0, 3) == "op_")
				{
					string text2 = LogicMethodPropertyInfoUtils.A(methodInfo.Name.Substring(3));
					if (parameters.Length == 1)
					{
						text = text2 + " " + parameters[0].ParameterType.Name;
					}
					else if (parameters.Length == 2)
					{
						text = string.Concat(new string[]
						{
							parameters[0].ParameterType.Name,
							" ",
							text2,
							" ",
							parameters[1].ParameterType.Name
						});
					}
					else
					{
						Log.Fatal("LogicMethodInfoUtils: GetMethodDisplayText: parameters.Length != 1 && parameters.Length != 2: not implemented");
					}
					displayAttributeUsed = false;
				}
				else
				{
					text += methodInfo.Name;
					if (parameters.Length != 0)
					{
						text += "( ";
						for (int i = 0; i < parameters.Length; i++)
						{
							if (i != 0)
							{
								text += ", ";
							}
							text = text + parameters[i].ParameterType.Name + " " + parameters[i].Name;
						}
						text += " )";
					}
					else
					{
						text += "()";
					}
					displayAttributeUsed = false;
				}
			}
			return text;
		}
		public static string GetMethodDisplayText(MethodInfo methodInfo)
		{
			bool flag;
			return LogicMethodPropertyInfoUtils.GetMethodDisplayText(methodInfo, out flag);
		}
		public static string GetMethodFormatText(MethodInfo methodInfo, out bool displayAttributeUsed)
		{
			string text = "";
			LogicSystemMethodDisplayAttribute[] array = (LogicSystemMethodDisplayAttribute[])methodInfo.GetCustomAttributes(typeof(LogicSystemMethodDisplayAttribute), true);
			if (array.Length != 0)
			{
				text = array[0].FormatText;
				displayAttributeUsed = true;
			}
			else
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (methodInfo.Name.Length > 3 && methodInfo.Name.Substring(0, 3) == "op_")
				{
					string str = LogicMethodPropertyInfoUtils.A(methodInfo.Name.Substring(3));
					if (parameters.Length == 1)
					{
						text = "( " + str + " {0} )";
					}
					else if (parameters.Length == 2)
					{
						text = "( {0} " + str + " {1} )";
					}
					else
					{
						Log.Fatal("LogicMethodInfoUtils: GetMethodFormatText: parameters.Length != 1 && parameters.Length != 2: not implemented");
					}
					displayAttributeUsed = true;
				}
				else
				{
					text += methodInfo.Name;
					if (parameters.Length != 0)
					{
						text += "( ";
						for (int i = 0; i < parameters.Length; i++)
						{
							if (i != 0)
							{
								text += ", ";
							}
							text = text + "{" + i.ToString() + "} ";
						}
						text += " )";
					}
					else
					{
						text += "()";
					}
					displayAttributeUsed = false;
				}
			}
			return text;
		}
		public static string GetMethodFormatText(MethodInfo methodInfo)
		{
			bool flag;
			return LogicMethodPropertyInfoUtils.GetMethodFormatText(methodInfo, out flag);
		}
		public static string GetPropertyDisplayText(PropertyInfo propertyInfo, out bool displayAttributeUsed)
		{
			string text = "";
			text += propertyInfo.Name;
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters.Length != 0)
			{
				text += "[ ";
				for (int i = 0; i < indexParameters.Length; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text = text + indexParameters[i].ParameterType.Name + " " + indexParameters[i].Name;
				}
				text += " ]";
			}
			displayAttributeUsed = false;
			return text;
		}
		public static string GetPropertyDisplayText(PropertyInfo propertyInfo)
		{
			bool flag;
			return LogicMethodPropertyInfoUtils.GetPropertyDisplayText(propertyInfo, out flag);
		}
		public static string GetPropertyFormatText(PropertyInfo propertyInfo, out bool displayAttributeUsed)
		{
			string text = "";
			text += propertyInfo.Name;
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters.Length != 0)
			{
				text += "[ ";
				for (int i = 0; i < indexParameters.Length; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text = text + "{" + i.ToString() + "} ";
				}
				text += " ]";
			}
			displayAttributeUsed = false;
			return text;
		}
		public static string GetPropertyFormatText(PropertyInfo propertyInfo)
		{
			bool flag;
			return LogicMethodPropertyInfoUtils.GetPropertyFormatText(propertyInfo, out flag);
		}
	}
}
