using Jx.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicDeclareLocalVariableAction : LogicAction
	{
		[Entity.FieldSerializeAttribute("variableTypeName")]
		private string abC;
		[Entity.FieldSerializeAttribute("variableName")]
		private string abc;
		[Entity.FieldSerializeAttribute("valueAction")]
		private LogicAction abD;
		private Type abd;
		public string VariableTypeName
		{
			get
			{
				return this.abC;
			}
		}
		public Type VariableType
		{
			get
			{
				if (this.abd == null)
				{
					LogicSystemClass byName = LogicSystemClasses.Instance.GetByName(this.abC);
					if (byName != null)
					{
						this.abd = byName.ClassType;
					}
				}
				return this.abd;
			}
		}
		[Browsable(false)]
		public string VariableName
		{
			get
			{
				return this.abc;
			}
			set
			{
				if (!StringUtils.IsCorrectIdentifierName(value))
				{
					throw new Exception("Incorrect variable name.");
				}
				this.abc = value;
			}
		}
		public LogicAction ValueAction
		{
			get
			{
				return this.abD;
			}
		}
		protected internal override void OnCreate()
		{
			base.OnCreate();
			if (string.IsNullOrEmpty(this.abC))
			{
				this.abC = typeof(string).Name;
			}
			if (string.IsNullOrEmpty(this.abc))
			{
				int num = 1;
				string b;
				while (true)
				{
					bool flag = false;
					b = "var" + num.ToString();
					List<LogicLocalVariable> accessedLocalVariables = base.ParentMethod.GetAccessedLocalVariables(null);
					foreach (LogicLocalVariable current in accessedLocalVariables)
					{
						if (current.Name == b)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					num++;
				}
				this.abc = b;
				return;
			}
		}
		public override string ToString()
		{
			string str = "declare ";
			str += this.VariableTypeName;
			str += " ";
			str += ((!string.IsNullOrEmpty(this.abc)) ? this.abc : "(null)");
			str += " = ";
			return str + ((this.abD != null) ? this.abD.ToString() : "null");
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string str = "<!<!font bold declare!>!> ";
			str = str + "<!<!variableType v " + this.VariableTypeName + "!>!>";
			str += " ";
			if (!string.IsNullOrEmpty(this.abc))
			{
				str = str + "<!<!variableName v " + this.abc + "!>!>";
			}
			else
			{
				str += "<!<!variableName i (null)!>!>";
			}
			str += " = ";
			return str + "<!<!valueAction v " + ((this.abD != null) ? this.abD.ToString() : "null") + "!>!>";
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (linkName == "variableType")
			{
				Type variableType = this.VariableType;
				if (!LogicEditorFunctionality.Instance.ShowTypeDialog(null, ref variableType))
				{
					return false;
				}
				if (variableType != this.VariableType)
				{
					if (this.abD != null)
					{
						this.abD.SetForDeletion(false);
						Entities.Instance.DeleteEntitiesQueuedForDeletion();
					}
					this.abC = variableType.Name;
					this.abd = null;
					if (SimpleTypesUtils.IsSimpleType(this.VariableType))
					{
						this.abD = (LogicAction)Entities.Instance.Create(EntityTypes.Instance.GetByName("LogicGetConstantValueAction"), this);
						((LogicGetConstantValueAction)this.abD).ValueType = this.VariableType;
						this.abD.PostCreate();
					}
				}
				return true;
			}
			else
			{
				if (!(linkName == "valueAction"))
				{
					return base.OnLinkedTextClick(linkName);
				}
				LogicAction logicAction = this.abD;
				if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, this.abd, false, ref logicAction))
				{
					return false;
				}
				this.abD = logicAction;
				return true;
			}
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			LogicLocalVariable logicLocalVariable = executeMethodInformation.DeclareLocalVariable(this);
			if (logicLocalVariable == null)
			{
				return null;
			}
			if (this.abD != null)
			{
				logicLocalVariable.Value = this.abD.Execute(executeMethodInformation);
			}
			return null;
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			if (this.abD == entity)
			{
				this.abD = null;
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicDeclareLocalVariableAction logicDeclareLocalVariableAction = (LogicDeclareLocalVariableAction)source;
			this.abC = logicDeclareLocalVariableAction.VariableTypeName;
			this.abc = logicDeclareLocalVariableAction.VariableName;
			if (logicDeclareLocalVariableAction.ValueAction != null)
			{
				this.abD = (LogicAction)logicDeclareLocalVariableAction.ValueAction.CloneWithCopyBrowsableProperties(false, this);
			}
		}
	}
}
