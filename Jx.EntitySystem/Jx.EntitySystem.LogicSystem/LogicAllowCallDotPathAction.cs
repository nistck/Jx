using System;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicAllowCallDotPathAction : LogicDotPathAction
	{
		[Entity.FieldSerializeAttribute("dotPathAction")]
		private LogicDotPathAction abg;
		public LogicDotPathAction DotPathAction
		{
			get
			{
				return this.abg;
			}
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			if (this.abg == entity)
			{
				this.abg = null;
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicAllowCallDotPathAction logicAllowCallDotPathAction = (LogicAllowCallDotPathAction)source;
			if (logicAllowCallDotPathAction.abg != null)
			{
				this.abg = (LogicDotPathAction)logicAllowCallDotPathAction.abg.CloneWithCopyBrowsableProperties(false, this);
			}
		}
		public virtual Type GetReturnType()
		{
			return null;
		}
		public override string ToString()
		{
			string text = "";
			Type returnType = this.GetReturnType();
			if (returnType != null && returnType != typeof(void) && this.DotPathAction != null)
			{
				text += this.DotPathAction.ToString();
			}
			return text;
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string text = "";
			Type returnType = this.GetReturnType();
			if (returnType != null && returnType != typeof(void))
			{
				if (this.DotPathAction != null)
				{
					text = text + "<!<!dotPathAction v " + this.DotPathAction.ToString() + "!>!>";
				}
				else if (clickableLinks)
				{
					text += " <!<!dotPathAction v [...]!>!>";
				}
			}
			return text;
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (!(linkName == "dotPathAction"))
			{
				return base.OnLinkedTextClick(linkName);
			}
			LogicAction logicAction = this.abg;
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, null, true, ref logicAction))
			{
				return false;
			}
			if (logicAction != this.abg)
			{
				this.abg = (LogicDotPathAction)logicAction;
			}
			return true;
		}
	}
}
