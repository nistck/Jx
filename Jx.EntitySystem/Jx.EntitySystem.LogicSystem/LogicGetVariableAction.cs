using System;
using System.Collections.Generic;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicGetVariableAction : LogicAllowCallDotPathAction
	{
		[Entity.FieldSerializeAttribute("variableName")]
		private string abm;
		public string VariableName
		{
			get
			{
				return this.abm;
			}
			set
			{
				this.abm = value;
			}
		}
		public override string ToString()
		{
			string str = "";
			if (!string.IsNullOrEmpty(this.abm))
			{
				str += this.abm;
			}
			else
			{
				str += "(null)";
			}
			return str + base.ToString();
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "";
			if (!string.IsNullOrEmpty(this.abm))
			{
				str += this.abm;
			}
			else
			{
				str += "(null)";
			}
			return str + base.GetLinkedText(clickableLinks);
		}
		public override Type GetReturnType()
		{
			if (!string.IsNullOrEmpty(this.VariableName))
			{
				LogicVariable variableByName = base.ParentMethod.ParentClass.GetVariableByName(this.VariableName);
				if (variableByName != null)
				{
					return variableByName.VariableType;
				}
			}
			List<LogicLocalVariable> accessedLocalVariables = base.ParentMethod.GetAccessedLocalVariables(this);
			foreach (LogicLocalVariable current in accessedLocalVariables)
			{
				if (current.Name == this.VariableName)
				{
					return current.Type;
				}
			}
			return null;
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			if (string.IsNullOrEmpty(this.abm))
			{
				Log.Error("Get Variable: variableName not defined");
				return null;
			}
			object obj = null;
			LogicVariable variableByName = base.ParentMethod.ParentClass.GetVariableByName(this.abm);
			if (variableByName != null)
			{
				if (base.DotPathAction != null && base.DotPathAction is LogicAssignVariableAction)
				{
					((LogicAssignVariableAction)base.DotPathAction).Execute(executeMethodInformation, variableByName);
					return null;
				}
				FieldInfo field = executeMethodInformation.LogicClassType.GetField(variableByName.VariableName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				obj = field.GetValue(executeMethodInformation.LogicEntityObject);
			}
			LogicLocalVariable logicLocalVariable = null;
			if (variableByName == null)
			{
				logicLocalVariable = executeMethodInformation.GetLocalVariable(this.abm);
				if (logicLocalVariable != null)
				{
					if (base.DotPathAction != null && base.DotPathAction is LogicAssignVariableAction)
					{
						((LogicAssignVariableAction)base.DotPathAction).Execute(executeMethodInformation, logicLocalVariable);
						return null;
					}
					obj = logicLocalVariable.Value;
				}
			}
			if (variableByName == null && logicLocalVariable == null)
			{
				Log.Error("Get Variable: variable not exists \"{0}\"", this.abm);
				return null;
			}
			if (base.DotPathAction != null)
			{
				obj = base.DotPathAction.Execute(executeMethodInformation, obj);
			}
			return obj;
		}
	}
}
