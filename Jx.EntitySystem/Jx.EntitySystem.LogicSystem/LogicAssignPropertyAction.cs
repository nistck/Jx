using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicAssignPropertyAction : LogicDotPathAction
	{
		[Entity.FieldSerializeAttribute("valueAction")]
		private LogicAction abJ;
		public LogicAction ValueAction
		{
			get
			{
				return this.abJ;
			}
		}
		public override string ToString()
		{
			string text = " = ";
			if (this.abJ != null)
			{
				text += this.abJ.ToString();
			}
			else
			{
				text += "null";
			}
			return text;
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = " = <!<!valueAction v ";
			if (this.abJ != null)
			{
				str += this.abJ.ToString();
			}
			else
			{
				str += "null";
			}
			return str + "!>!>";
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (!(linkName == "valueAction"))
			{
				return base.OnLinkedTextClick(linkName);
			}
			LogicAction logicAction = this.abJ;
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, null, false, ref logicAction))
			{
				return false;
			}
			this.abJ = logicAction;
			return true;
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			if (this.abJ == entity)
			{
				this.abJ = null;
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicAssignPropertyAction logicAssignPropertyAction = (LogicAssignPropertyAction)source;
			if (logicAssignPropertyAction.ValueAction != null)
			{
				this.abJ = (LogicAction)logicAssignPropertyAction.ValueAction.CloneWithCopyBrowsableProperties(false, this);
			}
		}
		public void Execute(LogicExecuteMethodInformation executeMethodInformation, object thisObject, object[] parameters, PropertyInfo propertyInfo)
		{
			if (this.abJ != null)
			{
				object value = this.abJ.Execute(executeMethodInformation);
				propertyInfo.SetValue(thisObject, value, parameters);
				return;
			}
			propertyInfo.SetValue(thisObject, null, parameters);
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			Log.Fatal("LogicAssignPropertyAction: Execute: internal error");
			return null;
		}
	}
}
