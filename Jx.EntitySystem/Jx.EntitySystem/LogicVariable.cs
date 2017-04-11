using System;
namespace Jx.EntitySystem
{
	public class LogicVariable : LogicClassMember
	{
		[Entity.FieldSerializeAttribute("variableType")]
		private Type aBt;
		[Entity.FieldSerializeAttribute("variableName")]
		private string aBU;
		[Entity.FieldSerializeAttribute("supportSerialization")]
		private bool aBu;
		public Type VariableType
		{
			get
			{
				return this.aBt;
			}
			set
			{
				this.aBt = value;
			}
		}
		public string VariableName
		{
			get
			{
				return this.aBU;
			}
			set
			{
				this.aBU = value;
			}
		}
		public bool SupportSerialization
		{
			get
			{
				return this.aBu;
			}
			set
			{
				this.aBu = value;
			}
		}
	}
}
