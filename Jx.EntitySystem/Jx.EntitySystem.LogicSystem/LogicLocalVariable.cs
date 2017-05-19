using Jx.FileSystem;
using System;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicLocalVariable
	{
		private LogicParameter aAu;
		private LogicDeclareLocalVariableAction aAV;
		private Type aAv;
		private string aAW;
		private object aAw;
		public LogicDeclareLocalVariableAction DeclareAction
		{
			get
			{
				return this.aAV;
			}
		}
		public Type Type
		{
			get
			{
				return this.aAv;
			}
		}
		public string Name
		{
			get
			{
				return this.aAW;
			}
		}
		public object Value
		{
			get
			{
				return this.aAw;
			}
			set
			{
				this.aAw = value;
			}
		}
		internal LogicLocalVariable()
		{
		}
		internal LogicLocalVariable(LogicParameter logicParameter)
		{
			this.aAu = logicParameter;
			this.aAv = logicParameter.ParameterType;
			this.aAW = logicParameter.ParameterName;
		}
		internal LogicLocalVariable(LogicDeclareLocalVariableAction logicDeclareLocalVariableAction)
		{
			this.aAV = logicDeclareLocalVariableAction;
			this.aAv = logicDeclareLocalVariableAction.VariableType;
			this.aAW = logicDeclareLocalVariableAction.VariableName;
		}
		internal bool A(TextBlock textBlock)
		{
			if (textBlock.IsAttributeExist("methodParameter"))
			{
				this.aAu = (Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(textBlock.GetAttribute("methodParameter"))) as LogicParameter);
			}
			if (textBlock.IsAttributeExist("declareAction"))
			{
				this.aAV = (Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(textBlock.GetAttribute("declareAction"))) as LogicDeclareLocalVariableAction);
			}
			if (textBlock.IsAttributeExist("type"))
			{
				string attribute = textBlock.GetAttribute("type");
				Type left = null;
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					Assembly assembly = assemblies[i];
					left = assembly.GetType(attribute);
					if (left != null)
					{
						break;
					}
				}
				if (left == null)
				{
					Log.Warning("Entity System: Serialization error. The type is not found \"{0}\".", attribute);
					return false;
				}
				this.aAv = left;
			}
			if (textBlock.IsAttributeExist("name"))
			{
				this.aAW = textBlock.GetAttribute("name");
			}
			if (textBlock.IsAttributeExist("value"))
			{
				string attribute2 = textBlock.GetAttribute("value");
				EntityHelper.ConvertFromString(this.aAv, attribute2, null, out this.aAw);
			}
			return true;
		}
		internal void a(TextBlock textBlock)
		{
			if (this.aAu != null)
			{
				textBlock.SetAttribute("methodParameter", this.aAu.UIN.ToString());
			}
			if (this.aAV != null)
			{
				textBlock.SetAttribute("declareAction", this.aAV.UIN.ToString());
			}
			if (this.aAv != null)
			{
				textBlock.SetAttribute("type", this.aAv.FullName);
			}
			if (this.aAW != null)
			{
				textBlock.SetAttribute("name", this.aAW);
			}
			if (this.aAw != null)
			{
				string saveValueString = EntityHelper.ConvertToString(this.aAv, this.aAw, null);
				if (saveValueString != null)
				{
					textBlock.SetAttribute("value", saveValueString);
				}
			}
		}
	}
}
