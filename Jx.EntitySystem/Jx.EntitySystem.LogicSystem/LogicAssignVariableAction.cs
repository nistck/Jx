using System;
using System.Reflection;

namespace Jx.EntitySystem.LogicSystem
{
	public class LogicAssignVariableAction : LogicDotPathAction
	{
		[Entity.FieldSerializeAttribute("valueAction")]
		private LogicAction abj;
		public LogicAction ValueAction
		{
			get
			{
				return this.abj;
			}
		}
		public override string ToString()
		{
			string text = " = ";
			if (this.abj != null)
			{
				text += this.abj.ToString();
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
			if (this.abj != null)
			{
				str += this.abj.ToString();
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
			LogicAction logicAction = this.abj;
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, null, false, ref logicAction))
			{
				return false;
			}
			this.abj = logicAction;
			return true;
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			if (this.abj == entity)
			{
				this.abj = null;
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicAssignVariableAction logicAssignVariableAction = (LogicAssignVariableAction)source;
			if (logicAssignVariableAction.ValueAction != null)
			{
				this.abj = (LogicAction)logicAssignVariableAction.ValueAction.CloneWithCopyBrowsableProperties(false, this);
			}
		}
		public void Execute(LogicExecuteMethodInformation executeMethodInformation, LogicVariable variable)
		{
			object value;
			if (this.abj != null)
			{
				value = this.abj.Execute(executeMethodInformation);
			}
			else
			{
				value = null;
			}
			FieldInfo field = executeMethodInformation.LogicClassType.GetField(variable.VariableName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			field.SetValue(executeMethodInformation.LogicEntityObject, value);
		}
		public void Execute(LogicExecuteMethodInformation executeMethodInformation, LogicLocalVariable localVariable)
		{
			if (this.abj != null)
			{
				object value = this.abj.Execute(executeMethodInformation);
				localVariable.Value = value;
				return;
			}
			localVariable.Value = null;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			Log.Fatal("LogicAssignVariableAction: Execute: internal error");
			return null;
		}
	}
}
