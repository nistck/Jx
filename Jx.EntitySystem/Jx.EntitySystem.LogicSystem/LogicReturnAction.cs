using System;
using System.ComponentModel;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicReturnAction : LogicAction
	{
		[Entity.FieldSerializeAttribute("action")]
		private LogicAction abA;
		[Browsable(false)]
		public LogicAction Action
		{
			get
			{
				return this.abA;
			}
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			if (entity == this.abA)
			{
				this.abA = null;
			}
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			object result = null;
			if (this.abA != null)
			{
				result = this.abA.Execute(executeMethodInformation);
			}
			executeMethodInformation.NeedReturn = true;
			return result;
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicReturnAction logicReturnAction = (LogicReturnAction)source;
			if (logicReturnAction.Action != null)
			{
				this.abA = (LogicAction)logicReturnAction.Action.CloneWithCopyBrowsableProperties(false, this);
			}
		}
		public override string ToString()
		{
			if (base.ParentMethod.ReturnType == typeof(void))
			{
				return "return";
			}
			return string.Format("return {0}", (this.abA != null) ? this.abA.ToString() : "null");
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string text = "<!<!font bold return!>!>";
			if (base.ParentMethod.ReturnType == typeof(void))
			{
				return text;
			}
			if (this.abA != null)
			{
				return string.Format("return <!<!action v {0}!>!>", this.abA.ToString());
			}
			return text + " <!<!action i (null)!>!>";
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (!(linkName == "action"))
			{
				return base.OnLinkedTextClick(linkName);
			}
			LogicAction logicAction = this.abA;
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, base.ParentMethod.ReturnType, false, ref logicAction))
			{
				return false;
			}
			this.abA = logicAction;
			return true;
		}
	}
}
