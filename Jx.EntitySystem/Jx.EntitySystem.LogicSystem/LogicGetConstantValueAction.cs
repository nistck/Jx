using Jx.FileSystem;
using Jx.Ext;
using System;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicGetConstantValueAction : LogicAction
	{
		[Entity.FieldSerializeAttribute("valueType")]
		private Type aBw;
		private object aBX;
		public Type ValueType
		{
			get
			{
				return this.aBw;
			}
			set
			{
				if (this.aBw == value)
				{
					return;
				}
				this.aBw = value;
				if (this.aBw == typeof(string))
				{
					this.aBX = "";
					return;
				}
				this.aBX = SimpleTypesUtils.GetSimpleTypeDefaultValue(this.aBw);
			}
		}
		public object Value
		{
			get
			{
				return this.aBX;
			}
			set
			{
				this.aBX = value;
			}
		}
		protected override bool OnLoad(TextBlock block)
		{
			if (!base.OnLoad(block))
			{
				return false;
			}
			if (block.IsAttributeExist("value"))
			{
				this.aBX = SimpleTypesUtils.GetSimpleTypeValue(this.aBw, block.GetAttribute("value"));
			}
			return true;
		}
		protected override void OnSave(TextBlock block)
		{
			base.OnSave(block);
			if (this.aBX != null)
			{
				block.SetAttribute("value", this.aBX.ToString());
			}
		}
		public override string ToString()
		{
			if (this.aBX == null)
			{
				return "(null)";
			}
			if (this.aBw == typeof(string))
			{
				return "\"" + StringUtils.EncodeDelimiterFormatString(this.aBX.ToString()).Replace('#', '_') + "\"";
			}
			return this.aBX.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "";
			str += "<!<!value v ";
			if (this.aBX != null)
			{
				if (this.aBw == typeof(string))
				{
					str = str + "\"" + StringUtils.EncodeDelimiterFormatString(this.aBX.ToString()).Replace('#', '_') + "\"";
				}
				else
				{
					str += this.aBX.ToString();
				}
			}
			else
			{
				str += "null";
			}
			return str + " !>!>";
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			return this.aBX;
		}
	}
}
