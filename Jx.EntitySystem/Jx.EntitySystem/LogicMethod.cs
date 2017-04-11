using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace Jx.EntitySystem
{
	public abstract class LogicMethod : LogicClassMember
	{
		[Entity.FieldSerializeAttribute("methodName")]
		private string aBx;
		[Entity.FieldSerializeAttribute("returnType")]
		private Type aBY = typeof(void);
		[Entity.FieldSerializeAttribute("parameters")]
		private List<LogicParameter> aBy = new List<LogicParameter>();
		[Entity.FieldSerializeAttribute("isEntityEventMethod")]
		internal bool aBZ;
		private EventInfo aBz;
		public string MethodName
		{
			get
			{
				return this.aBx;
			}
			set
			{
				this.aBx = value;
			}
		}
		public List<LogicParameter> Parameters
		{
			get
			{
				return this.aBy;
			}
		}
		public Type ReturnType
		{
			get
			{
				return this.aBY;
			}
			set
			{
				this.aBY = value;
			}
		}
		public bool IsEntityEventMethod
		{
			get
			{
				return this.aBZ;
			}
		}
		public EventInfo EntityEventInfo
		{
			get
			{
				if (!this.aBZ)
				{
					return null;
				}
				if (this.aBz == null)
				{
					Type aax = ((LogicEntityClass)base.ParentClass).EntityClassInfo.entityClassType;
					try
					{
						this.aBz = aax.GetEvent(this.MethodName);
					}
					catch (Exception ex)
					{
						Log.Error(ex.Message);
					}
				}
				return this.aBz;
			}
		}
		public LogicParameter CreateParameter(Type type, string name)
		{
			LogicParameter logicParameter = this.GetParameterByName(name);
			if (logicParameter != null)
			{
				Log.Fatal("LogicMethod: parameter with name \"{0}\" already created", name);
				return null;
			}
			logicParameter = (LogicParameter)Entities.Instance.Create(EntityTypes.Instance.GetByName("LogicParameter"), this);
			logicParameter.aBS = type;
			logicParameter.aBs = name;
			logicParameter.PostCreate();
			this.aBy.Add(logicParameter);
			return logicParameter;
		}
		public void DestroyAllParameters()
		{
			foreach (LogicParameter current in this.Parameters)
			{
				current.SetForDeletion(false);
			}
			this.aBy.Clear();
			Entities.Instance.DeleteEntitiesQueuedForDeletion();
		}
		public LogicParameter GetParameterByName(string name)
		{
			foreach (LogicParameter current in this.aBy)
			{
				if (current.ParameterName == name)
				{
					return current;
				}
			}
			return null;
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			LogicParameter logicParameter = entity as LogicParameter;
			if (logicParameter != null)
			{
				this.aBy.Remove(logicParameter);
			}
		}
		public string ToString(bool ignoreReturnType)
		{
			string text = "";
			if (!ignoreReturnType)
			{
				text = text + this.aBY.Name + " ";
			}
			text += this.MethodName;
			if (this.aBy.Count != 0)
			{
				text += "( ";
				for (int i = 0; i < this.aBy.Count; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text += this.aBy[i].ParameterType.Name;
					if (!string.IsNullOrEmpty(this.aBy[i].ParameterName))
					{
						text = text + " " + this.aBy[i].ParameterName;
					}
				}
				text += " )";
			}
			else
			{
				text += "()";
			}
			return text;
		}
		public override string ToString()
		{
			return this.ToString(false);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public List<string> GetCompileScriptsData(string namespaceName)
		{
			List<string> list = new List<string>();
			string text = "public ";
			if (!(base.Parent is LogicEntityClass))
			{
				text += "static ";
			}
			if (this.ReturnType != typeof(void))
			{
				text += Jx.Ext.CJ.TypeToCSharpString(this.ReturnType);
			}
			else
			{
				text += "void";
			}
			text = text + " " + this.aBx;
			if (this.aBy.Count != 0)
			{
				text += "( ";
				for (int i = 0; i < this.aBy.Count; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text += Jx.Ext.CJ.TypeToCSharpString(this.aBy[i].ParameterType);
					if (!string.IsNullOrEmpty(this.aBy[i].ParameterName))
					{
						text = text + " " + this.aBy[i].ParameterName;
					}
					else
					{
						text = text + " __emptyParam" + i.ToString();
					}
				}
				text += " )";
			}
			else
			{
				text += "()";
			}
			list.Add(text);
			list.Add("{");
			string[] compileScriptsBody = this.GetCompileScriptsBody(namespaceName);
			string[] array = compileScriptsBody;
			for (int j = 0; j < array.Length; j++)
			{
				string str = array[j];
				list.Add("\t" + str);
			}
			list.Add("}");
			return list;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected abstract string[] GetCompileScriptsBody(string namespaceName);
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetMethodFormatText()
		{
			string text = this.MethodName;
			if (this.aBy.Count != 0)
			{
				text += "( ";
				for (int i = 0; i < this.aBy.Count; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text = text + "{" + i.ToString() + "}";
				}
				text += " )";
			}
			else
			{
				text += "()";
			}
			return text;
		}
	}
}
