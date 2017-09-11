using System;
namespace Jx.EntitySystem
{
	public class LogicParameter : LogicComponent
	{
		[Entity.FieldSerializeAttribute("parameterType")]
		internal Type parameterType;
		[Entity.FieldSerializeAttribute("parameterName")]
		internal string parameterName;

		public Type ParameterType
		{
			get
			{
				return this.parameterType;
			}
		}
		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}
		public override string ToString()
		{
			if (this.parameterType == null)
			{
				return "(not initialized)";
			}
			string text = this.parameterType.Name;
			if (!string.IsNullOrEmpty(this.parameterName))
			{
				text = text + " " + this.parameterName;
			}
			return text;
		}
	}
}
