using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallNonStaticMethodAction : LogicCallMethodAction
	{
		private MethodInfo abt;
		private PropertyInfo abU;
		public MethodInfo SystemMethod
		{
			get
			{
				if (string.IsNullOrEmpty(base.MethodName))
				{
					return null;
				}
				if (this.abt == null)
				{
					LogicAllowCallDotPathAction logicAllowCallDotPathAction = base.Parent as LogicAllowCallDotPathAction;
					if (logicAllowCallDotPathAction != null)
					{
						Type returnType = logicAllowCallDotPathAction.GetReturnType();
						if (returnType != null)
						{
							LogicSystemClass byName = LogicSystemClasses.Instance.GetByName(returnType.Name);
							if (byName != null)
							{
								this.abt = byName.GetMethod(base.MethodName, base.ParameterTypeNames, true);
							}
						}
					}
				}
				return this.abt;
			}
		}
		public PropertyInfo SystemProperty
		{
			get
			{
				if (string.IsNullOrEmpty(base.PropertyName))
				{
					return null;
				}
				if (this.abU == null)
				{
					LogicAllowCallDotPathAction logicAllowCallDotPathAction = base.Parent as LogicAllowCallDotPathAction;
					if (logicAllowCallDotPathAction != null)
					{
						Type returnType = logicAllowCallDotPathAction.GetReturnType();
						if (returnType != null)
						{
							LogicSystemClass byName = LogicSystemClasses.Instance.GetByName(returnType.Name);
							if (byName != null)
							{
								this.abU = byName.GetProperty(base.PropertyName, base.ParameterTypeNames, true);
							}
						}
					}
				}
				return this.abU;
			}
		}
		public void Init(MethodInfo methodInfo)
		{
			this.abt = null;
			string[] array = null;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 0)
			{
				array = new string[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i] = parameters[i].ParameterType.Name;
				}
			}
			base.InitMethodNameAndParameterTypes(methodInfo.Name, array);
		}
		public void Init(PropertyInfo propertyInfo)
		{
			this.abt = null;
			string[] array = null;
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters.Length != 0)
			{
				array = new string[indexParameters.Length];
				for (int i = 0; i < indexParameters.Length; i++)
				{
					array[i] = indexParameters[i].ParameterType.Name;
				}
			}
			base.InitPropertyNameAndParameterTypes(propertyInfo.Name, array);
		}
		public override string ToString()
		{
			string str = " .";
			LogicAllowCallDotPathAction logicAllowCallDotPathAction = base.Parent as LogicAllowCallDotPathAction;
			if (logicAllowCallDotPathAction == null)
			{
				return "LogicCallNonStaticMethodAction: ToString: allowCallDotPathAction = null";
			}
			Type returnType = logicAllowCallDotPathAction.GetReturnType();
			if (returnType != null)
			{
				LogicSystemClass byName = LogicSystemClasses.Instance.GetByName(returnType.Name);
				if (byName == null)
				{
					return string.Format("Error: System class not defined \"{0}\"", returnType.Name);
				}
				if (!string.IsNullOrEmpty(base.MethodName))
				{
					if (this.SystemMethod == null)
					{
						return string.Format("Error: Method not exists \"{0}\"", base.MethodName);
					}
					string methodFormatText = byName.GetMethodFormatText(this.SystemMethod, true);
					string[] args = base.ForToStringParameterActions();
					str += string.Format(methodFormatText, args);
				}
				else
				{
					if (string.IsNullOrEmpty(base.PropertyName))
					{
						return "LogicCallNonStaticMethodAction: ToString: MethodName and PropertyName = null";
					}
					if (this.SystemProperty == null)
					{
						return string.Format("Error: Property not exists \"{0}\"", base.PropertyName);
					}
					string propertyFormatText = byName.GetPropertyFormatText(this.SystemProperty, true);
					string[] args2 = base.ForToStringParameterActions();
					str += string.Format(propertyFormatText, args2);
				}
			}
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = " .";
			LogicAllowCallDotPathAction logicAllowCallDotPathAction = base.Parent as LogicAllowCallDotPathAction;
			if (logicAllowCallDotPathAction == null)
			{
				Log.Fatal("LogicCallNonStaticMethodAction: ToString: allowCallDotPathAction = null");
			}
			Type returnType = logicAllowCallDotPathAction.GetReturnType();
			if (returnType != null)
			{
				LogicSystemClass byName = LogicSystemClasses.Instance.GetByName(returnType.Name);
				if (byName == null)
				{
					return string.Format("Error: System class not defined \"{0}\"", returnType.Name);
				}
				if (!string.IsNullOrEmpty(base.MethodName))
				{
					if (this.SystemMethod == null)
					{
						return string.Format("Error: Method not exists \"{0}\"", base.MethodName);
					}
					string methodFormatText = byName.GetMethodFormatText(this.SystemMethod, true);
					string[] args = base.ForGetLinkedTextParameterActions();
					str += string.Format(methodFormatText, args);
				}
				else if (!string.IsNullOrEmpty(base.PropertyName))
				{
					if (this.SystemProperty == null)
					{
						return string.Format("Error: Property not exists \"{0}\"", base.PropertyName);
					}
					string propertyFormatText = byName.GetPropertyFormatText(this.SystemProperty, true);
					string[] args2 = base.ForGetLinkedTextParameterActions();
					str += string.Format(propertyFormatText, args2);
				}
				else
				{
					Log.Fatal("LogicCallNonStaticMethodAction: ToString: MethodName and PropertyName = null");
				}
			}
			return str + base.GetLinkedText(clickableLinks);
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			int num;
			if (!int.TryParse(linkName, out num))
			{
				return base.OnLinkedTextClick(linkName);
			}
			Type parameterType;
			if (this.SystemMethod != null)
			{
				parameterType = this.SystemMethod.GetParameters()[num].ParameterType;
			}
			else
			{
				parameterType = this.SystemProperty.GetIndexParameters()[num].ParameterType;
			}
			LogicAction logicAction = base.ParameterActions[num];
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, parameterType, false, ref logicAction))
			{
				return false;
			}
			if (base.ParameterActions[num] != null && base.ParameterActions[num] != logicAction)
			{
				base.ParameterActions[num].SetForDeletion(false);
				Entities.Instance.DeleteEntitiesQueuedForDeletion();
			}
			base.ParameterActions[num] = logicAction;
			return true;
		}
		public override Type GetReturnType()
		{
			if (this.SystemMethod != null)
			{
				return this.SystemMethod.ReturnType;
			}
			if (this.SystemProperty != null)
			{
				return this.SystemProperty.PropertyType;
			}
			return null;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation, object thisObject)
		{
			object obj;
			if (!string.IsNullOrEmpty(base.MethodName))
			{
				if (this.SystemMethod == null)
				{
					Log.Error("LogicCallNonStaticMethodAction: Execute: method not exists \"{0}\"", base.MethodName);
					return null;
				}
				object[] array = new object[(base.ParameterActions != null) ? base.ParameterActions.Length : 0];
				for (int i = 0; i < array.Length; i++)
				{
					if (base.ParameterActions[i] != null)
					{
						array[i] = base.ParameterActions[i].Execute(executeMethodInformation);
					}
				}
				obj = this.SystemMethod.Invoke(thisObject, array);
			}
			else
			{
				if (string.IsNullOrEmpty(base.PropertyName))
				{
					Log.Fatal("LogicCallNonStaticMethodAction: Execute: not implemented");
					return null;
				}
				if (this.SystemProperty == null)
				{
					Log.Error("LogicCallNonStaticMethodAction: Execute: property not exists \"{0}\"", base.PropertyName);
					return null;
				}
				object[] array2 = null;
				if (base.ParameterActions != null)
				{
					array2 = new object[base.ParameterActions.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						if (base.ParameterActions[j] != null)
						{
							array2[j] = base.ParameterActions[j].Execute(executeMethodInformation);
						}
					}
				}
				if (base.DotPathAction != null && base.DotPathAction is LogicAssignPropertyAction)
				{
					((LogicAssignPropertyAction)base.DotPathAction).Execute(executeMethodInformation, thisObject, array2, this.SystemProperty);
					return null;
				}
				obj = this.SystemProperty.GetValue(thisObject, array2);
			}
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
	}
}
