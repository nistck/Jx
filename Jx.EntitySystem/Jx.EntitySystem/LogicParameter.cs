using System;
namespace Jx.EntitySystem
{
	public class LogicParameter : LogicComponent
	{
		[Entity.FieldSerializeAttribute("parameterType")]
		internal Type aBS;
		[Entity.FieldSerializeAttribute("parameterName")]
		internal string aBs;
		public Type ParameterType
		{
			get
			{
				return this.aBS;
			}
		}
		public string ParameterName
		{
			get
			{
				return this.aBs;
			}
		}
		public override string ToString()
		{
			if (this.aBS == null)
			{
				return "(not initialized)";
			}
			string text = this.aBS.Name;
			if (!string.IsNullOrEmpty(this.aBs))
			{
				text = text + " " + this.aBs;
			}
			return text;
		}
	}
}
