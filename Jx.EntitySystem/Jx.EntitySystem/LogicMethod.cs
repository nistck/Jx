using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Jx.Ext;


namespace Jx.EntitySystem
{
	public abstract class LogicMethod : LogicClassMember
	{
		[FieldSerializeAttribute("methodName")]
		private string methodName;
		[FieldSerializeAttribute("returnType")]
		private Type returnType = typeof(void);
		[FieldSerializeAttribute("parameters")]
		private List<LogicParameter> parameters = new List<LogicParameter>();
		[FieldSerializeAttribute("isEntityEventMethod")]
		internal bool isEntityEventMethod;
		private EventInfo eventInfo;
		public string MethodName
		{
			get
			{
				return this.methodName;
			}
			set
			{
				this.methodName = value;
			}
		}
		public List<LogicParameter> Parameters
		{
			get
			{
				return this.parameters;
			}
		}
		public Type ReturnType
		{
			get
			{
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}
		public bool IsEntityEventMethod
		{
			get
			{
				return this.isEntityEventMethod;
			}
		}
		public EventInfo EntityEventInfo
		{
			get
			{
				if (!this.isEntityEventMethod)
				{
					return null;
				}
				if (this.eventInfo == null)
				{
					Type aax = ((LogicEntityClass)base.ParentClass).EntityClassInfo.entityClassType;
					try
					{
						this.eventInfo = aax.GetEvent(this.MethodName);
					}
					catch (Exception ex)
					{
						Log.Error(ex.Message);
					}
				}
				return this.eventInfo;
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
			this.parameters.Add(logicParameter);
			return logicParameter;
		}
		public void DestroyAllParameters()
		{
			foreach (LogicParameter current in this.Parameters)
			{
				current.SetForDeletion(false);
			}
			this.parameters.Clear();
			Entities.Instance.DeleteEntitiesQueuedForDeletion();
		}
		public LogicParameter GetParameterByName(string name)
		{
			foreach (LogicParameter current in this.parameters)
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
				this.parameters.Remove(logicParameter);
			}
		}
		public string ToString(bool ignoreReturnType)
		{
			string text = "";
			if (!ignoreReturnType)
			{
				text = text + this.returnType.Name + " ";
			}
			text += this.MethodName;
			if (this.parameters.Count != 0)
			{
				text += "( ";
				for (int i = 0; i < this.parameters.Count; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text += this.parameters[i].ParameterType.Name;
					if (!string.IsNullOrEmpty(this.parameters[i].ParameterName))
					{
						text = text + " " + this.parameters[i].ParameterName;
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
				text += CJ.TypeToCSharpString(this.ReturnType);
			}
			else
			{
				text += "void";
			}
			text = text + " " + this.methodName;
			if (this.parameters.Count != 0)
			{
				text += "( ";
				for (int i = 0; i < this.parameters.Count; i++)
				{
					if (i != 0)
					{
						text += ", ";
					}
					text += CJ.TypeToCSharpString(this.parameters[i].ParameterType);
					if (!string.IsNullOrEmpty(this.parameters[i].ParameterName))
					{
						text = text + " " + this.parameters[i].ParameterName;
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
			if (this.parameters.Count != 0)
			{
				text += "( ";
				for (int i = 0; i < this.parameters.Count; i++)
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
