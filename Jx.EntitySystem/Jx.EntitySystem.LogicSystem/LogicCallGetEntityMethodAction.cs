using System;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallGetEntityMethodAction : LogicCallMethodAction
	{
		[Entity.FieldSerializeAttribute("entity")]
		private Entity abT;
		public Entity Entity
		{
			get
			{
				return this.abT;
			}
			set
			{
				if (this.abT != null)
				{
					base.UnsubscribeToDeletionEvent(this.abT);
				}
				this.abT = value;
				if (this.abT != null)
				{
					base.SubscribeToDeletionEvent(this.abT);
				}
			}
		}
		protected override void OnDeleteSubscribedToDeletionEvent(Entity entity)
		{
			base.OnDeleteSubscribedToDeletionEvent(entity);
			if (entity == this.abT)
			{
				this.abT = null;
			}
		}
		public override string ToString()
		{
			string str = "GetEntity( ";
			if (this.abT != null)
			{
				string text = this.abT.Name;
				if (string.IsNullOrEmpty(text))
				{
					text = this.abT.ToString();
				}
				str += text;
			}
			else
			{
				str += "null";
			}
			str += " )";
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "GetEntity( ";
			if (this.abT != null)
			{
				string text = this.abT.Name;
				if (string.IsNullOrEmpty(text))
				{
					text = this.abT.ToString();
				}
				str = str + "<!<!entity v " + text + "!>!>";
			}
			else
			{
				str += "<!<!entity i null!>!>";
			}
			str += " )";
			return str + base.GetLinkedText(clickableLinks);
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (!(linkName == "entity"))
			{
				return base.OnLinkedTextClick(linkName);
			}
			Entity entity = this.abT;
			if (!MapEditorInterface.Instance.EntityUITypeEditorEditValue(null, null, ref entity))
			{
				return false;
			}
			this.Entity = entity;
			return true;
		}
		public override Type GetReturnType()
		{
			if (this.abT != null)
			{
				return this.abT.GetType();
			}
			return null;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			if (this.abT == null)
			{
				Log.Error("LogicCallGetEntityMethodAction: entity = null");
				return null;
			}
			object obj = this.abT;
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
	}
}
