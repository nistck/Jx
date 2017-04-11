using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallStaticSystemMethodAction : LogicCallMethodAction
	{
		[Entity.FieldSerializeAttribute("className")]
		private string abR;
		private LogicSystemClass abr;
		private MethodInfo abS;
		private PropertyInfo abs;
		public MethodInfo SystemMethod
		{
			get
			{
				if (string.IsNullOrEmpty(base.MethodName))
				{
					return null;
				}
				if (this.abS == null && this.A() != null)
				{
					this.abS = this.A().GetMethod(base.MethodName, base.ParameterTypeNames, true);
				}
				return this.abS;
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
				if (this.abs == null && this.A() != null)
				{
					this.abs = this.A().GetProperty(base.PropertyName, base.ParameterTypeNames, true);
				}
				return this.abs;
			}
		}
		public string ClassName
		{
			get
			{
				return this.abR;
			}
		}
		private LogicSystemClass A()
		{
			if (this.abr == null && !string.IsNullOrEmpty(this.abR))
			{
				this.abr = LogicSystemClasses.Instance.GetByName(this.abR);
			}
			return this.abr;
		}
		public void Init(Type classType, MethodInfo methodInfo)
		{
			this.abr = null;
			this.abS = null;
			this.abR = classType.Name;
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
		public void Init(Type classType, PropertyInfo propertyInfo)
		{
			this.abr = null;
			this.abS = null;
			this.abR = classType.Name;
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
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicCallStaticSystemMethodAction logicCallStaticSystemMethodAction = (LogicCallStaticSystemMethodAction)source;
			this.abR = logicCallStaticSystemMethodAction.abR;
		}
		public override string ToString()
		{
			string str = "";
			if (this.A() == null)
			{
				return string.Format("Error: Class not exists \"{0}\"", this.abR);
			}
			if (!string.IsNullOrEmpty(base.MethodName))
			{
				if (this.SystemMethod == null)
				{
					return string.Format("Error: Method not exists \"{0}.{1}\"", this.abR, base.MethodName);
				}
				string methodFormatText = this.A().GetMethodFormatText(this.SystemMethod);
				string[] array = base.ForToStringParameterActions();
				if (array.Length == 0)
				{
					return "(not initialized)";
				}
				str += string.Format(methodFormatText, array);
			}
			else
			{
				if (string.IsNullOrEmpty(base.PropertyName))
				{
					return "LogicCallStaticSystemMethodAction: ToString: MethodName and PropertyName = null";
				}
				if (this.SystemProperty == null)
				{
					return string.Format("Error: Property not exists \"{0}.{1}\"", this.abR, base.PropertyName);
				}
				string propertyFormatText = this.A().GetPropertyFormatText(this.SystemProperty);
				string[] args = base.ForToStringParameterActions();
				str += string.Format(propertyFormatText, args);
			}
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "";
			if (this.A() == null)
			{
				return string.Format("Error: Class not exists \"{0}\"", this.abR);
			}
			if (!string.IsNullOrEmpty(base.MethodName))
			{
				if (this.SystemMethod == null)
				{
					return string.Format("Error: Method not exists \"{0}.{1}\"", this.abR, base.MethodName);
				}
				string methodFormatText = this.A().GetMethodFormatText(this.SystemMethod);
				string[] args = base.ForGetLinkedTextParameterActions();
				str += string.Format(methodFormatText, args);
			}
			else if (!string.IsNullOrEmpty(base.PropertyName))
			{
				if (this.SystemProperty == null)
				{
					return string.Format("Error: Property not exists \"{0}.{1}\"", this.abR, base.PropertyName);
				}
				string propertyFormatText = this.A().GetPropertyFormatText(this.SystemProperty);
				string[] args2 = base.ForGetLinkedTextParameterActions();
				str += string.Format(propertyFormatText, args2);
			}
			else
			{
				Log.Fatal("LogicCallStaticSystemMethodAction: ToString: MethodName and PropertyName = null");
			}
			return str + base.GetLinkedText(clickableLinks);
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
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			if (string.IsNullOrEmpty(this.abR))
			{
				Log.Error("LogicCallStaticSystemMethodAction: class not defined \"{0}\"");
				return null;
			}
			if (this.A() == null)
			{
				Log.Error("LogicCallStaticSystemMethodAction: class not exists \"{0}\"", this.abR);
				return null;
			}
			object obj;
			if (!string.IsNullOrEmpty(base.MethodName))
			{
				if (this.SystemMethod == null)
				{
					Log.Error("LogicCallStaticSystemMethodAction: class method not exists \"{0}.{1}\"", this.abR, base.MethodName);
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
				if (!this.abr.CallStaticOverInstance)
				{
					obj = this.SystemMethod.Invoke(null, array);
				}
				else
				{
					object value = this.abr.CallStaticOverInstanceProperty.GetValue(null, null);
					obj = this.SystemMethod.Invoke(value, array);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(base.PropertyName))
				{
					Log.Fatal("LogicCallStaticSystemMethodAction: Execute: not implemented");
					return null;
				}
				if (this.SystemProperty == null)
				{
					Log.Error("LogicCallStaticSystemMethodAction: Execute: property not exists \"{0}\"", base.PropertyName);
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
				if (!this.abr.CallStaticOverInstance)
				{
					obj = this.SystemProperty.GetValue(null, array2);
				}
				else
				{
					object value2 = this.abr.CallStaticOverInstanceProperty.GetValue(null, null);
					if (base.DotPathAction != null && base.DotPathAction is LogicAssignPropertyAction)
					{
						((LogicAssignPropertyAction)base.DotPathAction).Execute(executeMethodInformation, value2, array2, this.SystemProperty);
						return null;
					}
					obj = this.SystemProperty.GetValue(value2, array2);
				}
			}
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
	}
}
