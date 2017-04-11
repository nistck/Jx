using System;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallAsIsMethodAction : LogicCallMethodAction
	{
		[Entity.FieldSerializeAttribute("asCast")]
		private bool abN;
		[Entity.FieldSerializeAttribute("castTypeName")]
		private string abn;
		private Type abO;
		public bool AsCast
		{
			get
			{
				return this.abN;
			}
		}
		private Type A()
		{
			if (this.abO == null && !string.IsNullOrEmpty(this.abn))
			{
				LogicSystemClass byName = LogicSystemClasses.Instance.GetByName(this.abn);
				if (byName != null)
				{
					this.abO = byName.ClassType;
				}
			}
			return this.abO;
		}
		public override Type GetReturnType()
		{
			if (this.abN)
			{
				return this.A();
			}
			return typeof(bool);
		}
		public void Init(bool asCast)
		{
			this.abO = null;
			this.abn = null;
			this.abN = asCast;
			Type type = this.a();
			if (type != null)
			{
				this.abn = type.Name;
			}
		}
		private Type a()
		{
			LogicCallMethodAction logicCallMethodAction = base.Parent as LogicCallMethodAction;
			if (logicCallMethodAction != null)
			{
				return logicCallMethodAction.GetReturnType();
			}
			LogicGetVariableAction logicGetVariableAction = base.Parent as LogicGetVariableAction;
			if (logicGetVariableAction != null)
			{
				return logicGetVariableAction.GetReturnType();
			}
			return null;
		}
		public override string ToString()
		{
			string str = " ";
			str += (this.abN ? "as" : "is");
			str += " ";
			if (this.A() != null)
			{
				str += this.A().Name;
			}
			else
			{
				str += "(null)";
			}
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = " ";
			str += (this.abN ? "as" : "is");
			str += " <!<!castType ";
			if (this.A() != null)
			{
				str = str + "v " + this.A().Name + "!>!>";
			}
			else
			{
				str += "i (null)!>!>";
			}
			return str + base.GetLinkedText(clickableLinks);
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (!(linkName == "castType"))
			{
				return base.OnLinkedTextClick(linkName);
			}
			Type type = this.a();
			if (type == null)
			{
				return false;
			}
			Type type2 = this.abO;
			if (!LogicEditorFunctionality.Instance.ShowTypeDialog(type, ref type2))
			{
				return false;
			}
			if (this.abn != type2.Name && base.DotPathAction != null)
			{
				base.DotPathAction.SetForDeletion(false);
				Entities.Instance.DeleteEntitiesQueuedForDeletion();
			}
			this.abO = null;
			this.abn = type2.Name;
			return true;
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicCallAsIsMethodAction logicCallAsIsMethodAction = (LogicCallAsIsMethodAction)source;
			this.abN = logicCallAsIsMethodAction.AsCast;
			this.abn = logicCallAsIsMethodAction.abn;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation, object thisObject)
		{
			if (this.A() == null)
			{
				Log.Error("LogicCallAsIsMethodAction: Execute: CastType = null");
				return null;
			}
			object obj;
			if (this.abN)
			{
				if (this.A().IsAssignableFrom(thisObject.GetType()))
				{
					obj = thisObject;
				}
				else
				{
					obj = null;
				}
			}
			else
			{
				obj = this.A().IsAssignableFrom(thisObject.GetType());
			}
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
	}
}
