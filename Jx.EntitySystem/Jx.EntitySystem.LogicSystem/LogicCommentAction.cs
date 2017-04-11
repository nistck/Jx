using Jx.Ext;
using System;

namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCommentAction : LogicAction
	{
		[Entity.FieldSerializeAttribute("text")]
		private string abE = "";
		public string Text
		{
			get
			{
				return this.abE;
			}
			set
			{
				this.abE = value.Trim();
			}
		}
		public override string ToString()
		{
			string text = "Comment: ";
			if (string.IsNullOrEmpty(this.abE))
			{
				text += "(Empty)";
			}
			else
			{
				text = text + "\"" + StringUtils.EncodeDelimiterFormatString(this.abE).Replace('#', '_') + "\"";
			}
			return text;
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "Comment: ";
			str += "<!<!text v ";
			if (string.IsNullOrEmpty(this.abE))
			{
				str += "(Empty)";
			}
			else
			{
				str = str + "\"" + StringUtils.EncodeDelimiterFormatString(this.abE).Replace('#', '_') + "\"";
			}
			return str + " !>!>";
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			return null;
		}
	}
}
