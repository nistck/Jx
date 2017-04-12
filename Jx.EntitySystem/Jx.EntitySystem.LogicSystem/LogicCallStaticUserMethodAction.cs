using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallStaticUserMethodAction : LogicCallMethodAction
	{
		[Entity.FieldSerializeAttribute("className")]
		private string abo;
		private LogicClass abP;
		private LogicMethod abp;
		public string ClassName
		{
			get
			{
				return this.abo;
			}
		}
		private LogicClass A()
		{
			if (this.abP == null && !string.IsNullOrEmpty(this.abo))
			{
				this.abP = LogicSystemManager.Instance.MapClassManager.GetByName(this.abo);
			}
			return this.abP;
		}
		private LogicMethod a()
		{
			if (string.IsNullOrEmpty(base.MethodName))
			{
				return null;
			}
			if (this.abp == null && this.A() != null)
			{
				this.abp = this.A().GetMethodByName(base.MethodName);
			}
			return this.abp;
		}
		public void Init(LogicMethod method)
		{
			this.abP = null;
			this.abp = null;
			this.abo = method.ParentClass.ClassName;
			string[] array = new string[method.Parameters.Count];
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				array[i] = method.Parameters[i].ParameterType.Name;
			}
			base.InitMethodNameAndParameterTypes(method.MethodName, array);
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicCallStaticUserMethodAction logicCallStaticUserMethodAction = (LogicCallStaticUserMethodAction)source;
			this.abo = logicCallStaticUserMethodAction.abo;
		}
		public override string ToString()
		{
			string str = "";
			if (this.A() == null)
			{
				return string.Format("Error: Class not exists \"{0}\"", this.abo);
			}
			if (string.IsNullOrEmpty(base.MethodName))
			{
				return "LogicCallStaticUserMethodAction: ToString: MethodName = null";
			}
			if (this.a() == null)
			{
				return string.Format("Error: Method not exists \"{0}.{1}\"", this.abo, base.MethodName);
			}
			str = str + this.ClassName + ".";
			string methodFormatText = this.a().GetMethodFormatText();
			string[] args = base.ForToStringParameterActions();
			str += string.Format(methodFormatText, args);
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "";
			if (this.A() == null)
			{
				return string.Format("Error: Class not exists \"{0}\"", this.abo);
			}
			if (!string.IsNullOrEmpty(base.MethodName))
			{
				if (this.a() == null)
				{
					return string.Format("Error: Method not exists \"{0}.{1}\"", this.abo, base.MethodName);
				}
				str = str + this.ClassName + ".";
				string methodFormatText = this.a().GetMethodFormatText();
				string[] args = base.ForGetLinkedTextParameterActions();
				str += string.Format(methodFormatText, args);
			}
			else
			{
				Log.Fatal("LogicCallStaticUserMethodAction: ToString: MethodName = null");
			}
			return str + base.GetLinkedText(clickableLinks);
		}
		public override Type GetReturnType()
		{
			if (this.a() != null)
			{
				return this.a().ReturnType;
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
			Type parameterType = this.a().Parameters[num].ParameterType;
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
			if (string.IsNullOrEmpty(this.abo))
			{
				Log.Error("LogicCallStaticUserMethodAction: class not defined \"{0}\"");
				return null;
			}
			if (this.A() == null)
			{
				Log.Error("LogicCallStaticUserMethodAction: class not exists \"{0}\"", this.abo);
				return null;
			}
			if (string.IsNullOrEmpty(base.MethodName))
			{
				Log.Fatal("LogicCallStaticUserMethodAction: Execute: not implemented");
				return null;
			}
			if (this.a() == null)
			{
				Log.Error("LogicCallStaticUserMethodAction: class method not exists \"{0}.{1}\"", this.abo, base.MethodName);
				return null;
			}
			object[] array = new object[base.ParameterActions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (base.ParameterActions[i] != null)
				{
					array[i] = base.ParameterActions[i].Execute(executeMethodInformation);
				}
			}
			Type logicSystemScriptsAssemblyClassByClassName = EntitySystemWorld.Instance.GetLogicSystemScriptsAssemblyClassByClassName(this.ClassName);
			if (logicSystemScriptsAssemblyClassByClassName == null)
			{
				Log.Fatal("LogicCallStaticUserMethodAction: class not exists \"{0}\"", this.ClassName);
                return null;
			}
			MethodInfo method = logicSystemScriptsAssemblyClassByClassName.GetMethod(base.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = method.Invoke(null, array);
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
	}
}
