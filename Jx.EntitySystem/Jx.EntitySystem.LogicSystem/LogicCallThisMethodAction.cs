using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallThisMethodAction : LogicCallMethodAction
	{
		[Entity.FieldSerializeAttribute("logicMethod")]
		private LogicMethod abQ;
		[Entity.FieldSerializeAttribute("entityOwnerProperty")]
		private bool abq;
		public LogicMethod LogicMethod
		{
			get
			{
				return this.abQ;
			}
		}
		public bool EntityOwnerProperty
		{
			get
			{
				return this.abq;
			}
		}
		public void Init(LogicMethod method)
		{
			string[] array = new string[method.Parameters.Count];
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				array[i] = method.Parameters[i].ParameterType.Name;
			}
			base.InitMethodNameAndParameterTypes(null, array);
			this.abQ = method;
			base.SubscribeToDeletionEvent(this.abQ);
		}
		public void InitAsEntityOwnerProperty()
		{
			base.Reset();
			this.abq = true;
		}
		public override string ToString()
		{
			string str = "";
			if (!this.abq)
			{
				if (this.LogicMethod == null)
				{
					return string.Format("Error: Method not exists \"{0}\"", "null");
				}
				string methodFormatText = this.abQ.GetMethodFormatText();
				string[] args = base.ForToStringParameterActions();
				str += string.Format(methodFormatText, args);
			}
			else
			{
				str += "Owner";
			}
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "";
			if (!this.abq)
			{
				if (this.LogicMethod == null)
				{
					return string.Format("Error: Method not exists \"{0}\"", "null");
				}
				string methodFormatText = this.abQ.GetMethodFormatText();
				string[] args = base.ForGetLinkedTextParameterActions();
				str += string.Format(methodFormatText, args);
			}
			else
			{
				str += "Owner";
			}
			return str + base.GetLinkedText(clickableLinks);
		}
		public override Type GetReturnType()
		{
			if (this.LogicMethod != null)
			{
				return this.LogicMethod.ReturnType;
			}
			if (this.abq)
			{
				LogicEntityClass logicEntityClass = (LogicEntityClass)base.ParentMethod.ParentClass;
				if (logicEntityClass.EntityClassInfo != null)
				{
					return logicEntityClass.EntityClassInfo.EntityClassType;
				}
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
			Type parameterType = this.abQ.Parameters[num].ParameterType;
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
		protected override void OnDeleteSubscribedToDeletionEvent(Entity entity)
		{
			base.OnDeleteSubscribedToDeletionEvent(entity);
			if (this.abQ == entity)
			{
				this.abQ = null;
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicCallThisMethodAction logicCallThisMethodAction = (LogicCallThisMethodAction)source;
			if (logicCallThisMethodAction.LogicMethod != null)
			{
				this.abQ = logicCallThisMethodAction.LogicMethod;
				base.SubscribeToDeletionEvent(this.abQ);
			}
			this.abq = logicCallThisMethodAction.EntityOwnerProperty;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			object obj;
			if (!this.abq)
			{
				if (this.LogicMethod == null)
				{
					Log.Error("LogicCallThisMethodAction: class method not exists \"{0}\"", "null");
					return null;
				}
				MethodInfo method = executeMethodInformation.LogicClassType.GetMethod(this.abQ.MethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
				{
					Log.Fatal("LogicCallThisMethodAction: MethodInfo = null");
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
				obj = method.Invoke(executeMethodInformation.LogicEntityObject, array);
			}
			else
			{
				PropertyInfo property = executeMethodInformation.LogicClassType.GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property == null)
				{
					Log.Fatal("LogicCallThisMethodAction: propertyInfo = null");
					return null;
				}
				obj = property.GetValue(executeMethodInformation.LogicEntityObject, null);
			}
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation, object thisObject)
		{
			Log.Fatal("LogicCallThisMethodAction: Execute: internal error");
			return null;
		}
	}
}
