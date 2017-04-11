using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicEventAction : LogicDotPathAction
	{
		[Entity.FieldSerializeAttribute("eventName")]
		private string abH;
		[Entity.FieldSerializeAttribute("methodName")]
		private string abh;
		private EventInfo abI;
		public string EventName
		{
			get
			{
				return this.abH;
			}
			set
			{
				this.abH = value;
			}
		}
		public string MethodName
		{
			get
			{
				return this.abh;
			}
			set
			{
				this.abh = value;
			}
		}
		public EventInfo EventInfo
		{
			get
			{
				if (this.abI == null)
				{
					try
					{
						Type returnType = ((LogicAllowCallDotPathAction)base.Parent).GetReturnType();
						this.abI = returnType.GetEvent(this.EventName);
					}
					catch
					{
					}
				}
				return this.abI;
			}
		}
		public override string ToString()
		{
			string text = " .";
			text += this.abH;
			text += " += ";
			if (!string.IsNullOrEmpty(this.abh))
			{
				text += this.abh;
			}
			else
			{
				text += "(null)";
			}
			return text;
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string text = " ." + this.abH + " ";
			text += "<!<!operation v +=!>!> ";
			if (!string.IsNullOrEmpty(this.abh))
			{
				text = text + "<!<!methodName v " + this.abh + "!>!>";
			}
			else
			{
				text += "<!<!methodName v (null)!>!>";
			}
			return text;
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			return base.OnLinkedTextClick(linkName);
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			Log.Fatal("LogicEventAction: Execute: internal error");
			return null;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation, object thisObject)
		{
			if (this.EventInfo == null)
			{
				Log.Error("LogicEventAction: Error: event \"{0}\" not exists", this.EventName);
				return null;
			}
			MethodInfo methodInfo = null;
			Type logicSystemScriptsAssemblyClassByClassName = EntitySystemWorld.Instance.GetLogicSystemScriptsAssemblyClassByClassName(base.ParentMethod.ParentClass.ClassName);
			try
			{
				methodInfo = logicSystemScriptsAssemblyClassByClassName.GetMethod(this.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			catch
			{
			}
			if (methodInfo == null)
			{
				Log.Error("LogicEventAction: Error: method \"{0}\" not exists ", this.MethodName);
				return null;
			}
			MethodInfo addMethod = this.EventInfo.GetAddMethod();
			Type parameterType = addMethod.GetParameters()[0].ParameterType;
			object logicEntityObject = executeMethodInformation.LogicEntityObject;
			Delegate @delegate = Delegate.CreateDelegate(parameterType, logicEntityObject, methodInfo);
			addMethod.Invoke(thisObject, new object[]
			{
				@delegate
			});
			return null;
		}
	}
}
