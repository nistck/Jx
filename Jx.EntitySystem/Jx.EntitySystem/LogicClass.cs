using System;
using System.Collections.Generic;
using System.ComponentModel;
using Jx.Ext;

namespace Jx.EntitySystem
{
	public class LogicClass : LogicComponent
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class CompileScriptsData
		{
			private string className;
			private List<string> strings = new List<string>();
			private List<int> lineNumbers = new List<int>();
			public string ClassName
			{
				get
				{
					return this.className;
				}
				set
				{
					this.className = value;
				}
			}
			public List<string> Strings
			{
				get
				{
					return this.strings;
				}
				set
				{
					this.strings = value;
				}
			}
			public List<int> LineNumbers
			{
				get
				{
					return this.lineNumbers;
				}
				set
				{
					this.lineNumbers = value;
				}
			}
		}
		[Entity.FieldSerializeAttribute("className")]
		internal string className;
		[Entity.FieldSerializeAttribute("customScriptCodeCreated")]
		private bool customScriptCodeCreated;
		[Entity.FieldSerializeAttribute("customScriptCode")]
		private string customScriptCode;

		private Dictionary<string, LogicMethod> methods;
		private Dictionary<string, LogicVariable> variables;

		public LogicClassManager ClassManager
		{
			get
			{
				return (LogicClassManager)base.Parent;
			}
		}

		public string ClassName
		{
			get
			{
				return this.className;
			}
			set
			{
				this.className = value;
			}
		}
		public Dictionary<string, LogicMethod>.ValueCollection Methods
		{
			get
			{
				this.initMethods();
				return this.methods.Values;
			}
		}
		public Dictionary<string, LogicVariable>.ValueCollection Variables
		{
			get
			{
				this.initVariables();
				return this.variables.Values;
			}
		}

		public bool CustomScriptCodeCreated
		{
			get
			{
				return this.customScriptCodeCreated;
			}
			set
			{
				this.customScriptCodeCreated = value;
			}
		}

		public string CustomScriptCode
		{
			get
			{
				return this.customScriptCode;
			}
			set
			{
				this.customScriptCode = value;
			}
		}

		private void initMethods()
		{
			if (this.methods == null)
			{
				this.methods = new Dictionary<string, LogicMethod>();
				foreach (Entity current in base.Children)
				{
					LogicMethod logicMethod = current as LogicMethod;
					if (logicMethod != null)
					{
						this.methods.Add(logicMethod.MethodName, logicMethod);
					}
				}
			}
		}
		private void initVariables()
		{
			if (this.variables == null)
			{
				this.variables = new Dictionary<string, LogicVariable>();
				foreach (Entity current in base.Children)
				{
					LogicVariable logicVariable = current as LogicVariable;
					if (logicVariable != null)
					{
						this.variables.Add(logicVariable.VariableName, logicVariable);
					}
				}
			}
		}

		public LogicMethod GetMethodByName(string methodName)
		{
			this.initMethods();
			LogicMethod result;
			this.methods.TryGetValue(methodName, out result);
			return result;
		}

		public LogicVariable GetVariableByName(string variableName)
		{
			this.initVariables();
			LogicVariable result;
			this.variables.TryGetValue(variableName, out result);
			return result;
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			if (this.methods != null)
			{
				LogicMethod logicMethod = entity as LogicMethod;
				if (logicMethod != null)
				{
					this.methods.Remove(logicMethod.MethodName);
				}
			}
			if (this.variables != null)
			{
				LogicVariable logicVariable = entity as LogicVariable;
				if (logicVariable != null)
				{
					this.variables.Remove(logicVariable.VariableName);
				}
			}
			base.OnRemoveChild(entity);
		}
		public LogicMethod CreateMethod(LogicMethodType methodType, string methodName)
		{
			this.initMethods();
			LogicMethod logicMethod = this.GetMethodByName(methodName);
			if (logicMethod != null)
			{
				Log.Fatal("LogicClass: already create method \"{0}\"", methodName);
				return null;
			}
			logicMethod = (LogicMethod)Entities.Instance.Create(methodType, this);
			logicMethod.MethodName = methodName;
			logicMethod.PostCreate();
			this.methods.Add(methodName, logicMethod);
			return logicMethod;
		}
		public LogicVariable CreateVariable(Type variableType, string variableName)
		{
			this.initVariables();
			LogicVariable logicVariable = this.GetVariableByName(variableName);
			if (logicVariable != null)
			{
				Log.Fatal("LogicClass: already create variable \"{0}\"", variableName);
				return null;
			}
			logicVariable = (LogicVariable)Entities.Instance.Create("LogicVariable", this);
			logicVariable.VariableType = variableType;
			logicVariable.VariableName = variableName;
			logicVariable.PostCreate();
			this.variables.Add(variableName, logicVariable);
			return logicVariable;
		}
		public override string ToString()
		{
			return this.className;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LogicClass.CompileScriptsData GetCompileScriptsData(string namespaceName)
		{
			LogicClass.CompileScriptsData compileScriptsData = new LogicClass.CompileScriptsData();
			compileScriptsData.ClassName = this.className;
			string[] array = LogicSystemManager.Instance.UsingNamespacesToCodeGeneration.Split(new char[]
			{
				'\r',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string item = array[i];
				compileScriptsData.Strings.Add(item);
			}
			compileScriptsData.Strings.Add("");
			compileScriptsData.Strings.Add(string.Format("namespace {0}", namespaceName));
			compileScriptsData.Strings.Add("{");
			string item2;
			if (this is LogicEntityClass)
			{
				item2 = "\tpublic class " + this.className + " : Engine.EntitySystem.LogicSystem.LogicEntityObject";
			}
			else
			{
				item2 = "\tpublic static class " + this.className;
			}
			compileScriptsData.Strings.Add(item2);
			compileScriptsData.Strings.Add("\t{");
			this.GetCompileScriptsClassBody(namespaceName, compileScriptsData);
			compileScriptsData.Strings.Add("\t}");
			compileScriptsData.Strings.Add("}");
			return compileScriptsData;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void GetCompileScriptsClassBody(string namespaceName, LogicClass.CompileScriptsData data)
		{
			if (this.CustomScriptCodeCreated && !string.IsNullOrEmpty(this.CustomScriptCode))
			{
				data.LineNumbers.Add(data.Strings.Count);
				string[] array = this.CustomScriptCode.Split(new char[]
				{
					'\n'
				}, StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("\r", "");
				}
				string[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					string str = array2[j];
					data.Strings.Add("\t\t" + str);
				}
				data.Strings.Add("");
			}
			foreach (LogicVariable current in this.Variables)
			{
				if (current.SupportSerialization)
				{
					data.Strings.Add("\t\t[Engine.EntitySystem.Entity.FieldSerialize]");
				}
				string text = "\t\tpublic ";
				if (!(this is LogicEntityClass))
				{
					text += "static ";
				}
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					CJ.TypeToCSharpString(current.VariableType),
					" ",
					current.VariableName,
					";"
				});
				data.Strings.Add(text);
			}
			data.Strings.Add("\t\t");
			foreach (LogicMethod current2 in this.Methods)
			{
				data.LineNumbers.Add(data.Strings.Count + 1);
				List<string> compileScriptsData = current2.GetCompileScriptsData(namespaceName);
				foreach (string current3 in compileScriptsData)
				{
					data.Strings.Add("\t\t" + current3);
				}
				data.Strings.Add("");
			}
		}

		internal void Add(LogicMethod logicMethod)
		{
			this.initMethods();
			this.methods.Add(logicMethod.MethodName, logicMethod);
		}
	}
}
