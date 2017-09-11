using Jx.FileSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicExecuteMethodInformation
	{
		private LogicMethod aAO;
		private Type aAo;
		private Entity aAP;
		private LogicEntityObject aAp;
		private bool aAQ;
		private bool aAq;
		private Dictionary<string, LogicLocalVariable> aAR = new Dictionary<string, LogicLocalVariable>();
		private int aAr;
		private List<int> aAS = new List<int>();
		public Type LogicClassType
		{
			get
			{
				return this.aAo;
			}
		}
		public LogicEntityObject LogicEntityObject
		{
			get
			{
				return this.aAp;
			}
		}
		public bool NeedReturn
		{
			get
			{
				return this.aAQ;
			}
			set
			{
				this.aAQ = value;
			}
		}
		public bool NeedReturnForWait
		{
			get
			{
				return this.aAq;
			}
			set
			{
				this.aAq = value;
			}
		}
		public LogicMethod Method
		{
			get
			{
				return this.aAO;
			}
		}
		public int CurrentClassActionsLevelIndex
		{
			get
			{
				return this.aAr;
			}
			set
			{
				this.aAr = value;
			}
		}
		public List<int> CallActionsLevelIndexes
		{
			get
			{
				return this.aAS;
			}
		}
		internal LogicExecuteMethodInformation()
		{
		}
		public LogicExecuteMethodInformation(LogicMethod method, LogicEntityObject logicEntityObject)
		{
			this.aAO = method;
			this.aAp = logicEntityObject;
			this.aAo = logicEntityObject.GetType();
		}
		public LogicExecuteMethodInformation(LogicMethod method, Type staticClassType)
		{
			this.aAO = method;
			this.aAo = staticClassType;
		}
		public LogicLocalVariable GetLocalVariable(string name)
		{
			LogicLocalVariable result;
			if (!this.aAR.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}
		public LogicLocalVariable DeclareLocalVariable(LogicParameter methodParameter)
		{
			LogicLocalVariable logicLocalVariable = this.GetLocalVariable(methodParameter.ParameterName);
			if (logicLocalVariable != null)
			{
				Log.Error("Declare Local Variable: name already defined \"{0}\" by local variable.", methodParameter.ParameterName);
				return null;
			}
			logicLocalVariable = new LogicLocalVariable(methodParameter);
			this.aAR.Add(methodParameter.parameterName, logicLocalVariable);
			return logicLocalVariable;
		}
		public LogicLocalVariable DeclareLocalVariable(LogicDeclareLocalVariableAction declareVariableAction)
		{
			LogicLocalVariable logicLocalVariable = this.GetLocalVariable(declareVariableAction.VariableName);
			if (logicLocalVariable == null)
			{
				logicLocalVariable = new LogicLocalVariable(declareVariableAction);
				this.aAR.Add(declareVariableAction.VariableName, logicLocalVariable);
				return logicLocalVariable;
			}
			if (logicLocalVariable.DeclareAction == declareVariableAction)
			{
				return logicLocalVariable;
			}
			Log.Error("Declare Local Variable: name already defined \"{0}\" by local variable", declareVariableAction.VariableName);
			return null;
		}
		public void PushCallActionsLevelIndex(int index)
		{
			this.aAS.Add(index);
		}
		public int PopCallActionsLevelIndex()
		{
			int result = this.aAS[this.aAS.Count - 1];
			this.aAS.RemoveAt(this.aAS.Count - 1);
			return result;
		}
		internal bool A(TextBlock textBlock)
		{
			if (textBlock.IsAttributeExist("method"))
			{
				this.aAO = (Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(textBlock.GetAttribute("method"))) as LogicMethod);
			}
			if (textBlock.IsAttributeExist("logicClassType"))
			{
				string attribute = textBlock.GetAttribute("logicClassType");
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
					Log.Warning("Entity System: Serialization error. The logic class type is not found \"{0}\".", attribute);
					return false;
				}
				this.aAo = left;
			}
			if (textBlock.IsAttributeExist("logicEntityObjectOwnerEntity"))
			{
				this.aAP = Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(textBlock.GetAttribute("logicEntityObjectOwnerEntity")));
			}
			if (textBlock.IsAttributeExist("needReturn"))
			{
				this.aAQ = bool.Parse(textBlock.GetAttribute("needReturn"));
			}
			if (textBlock.IsAttributeExist("needReturnForWait"))
			{
				this.aAq = bool.Parse(textBlock.GetAttribute("needReturnForWait"));
			}
			TextBlock textBlock2 = textBlock.FindChild("localVariablesBlock");
			if (textBlock2 != null)
			{
				foreach (TextBlock current in textBlock2.Children)
				{
					LogicLocalVariable logicLocalVariable = new LogicLocalVariable();
					if (!logicLocalVariable.A(current))
					{
						return false;
					}
					this.aAR.Add(logicLocalVariable.Name, logicLocalVariable);
				}
			}
			if (textBlock.IsAttributeExist("currentClassActionsLevelIndex"))
			{
				this.aAr = int.Parse(textBlock.GetAttribute("currentClassActionsLevelIndex"));
			}
			if (textBlock.IsAttributeExist("callActionsLevelIndexes"))
			{
				string[] array = textBlock.GetAttribute("callActionsLevelIndexes").Split(new char[]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					string s = array2[j];
					this.aAS.Add(int.Parse(s));
				}
			}
			return true;
		}
		internal void A()
		{
			if (this.aAP != null)
			{
				this.aAp = this.aAP.LogicObject;
				this.aAP = null;
			}
		}
		internal void a(TextBlock textBlock)
		{
			if (this.aAO != null)
			{
				textBlock.SetAttribute("method", this.aAO.UIN.ToString());
			}
			if (this.aAo != null)
			{
				textBlock.SetAttribute("logicClassType", this.aAo.FullName);
			}
			if (this.aAp != null)
			{
				textBlock.SetAttribute("logicEntityObjectOwnerEntity", this.aAp.OwnerEntity().UIN.ToString());
			}
			textBlock.SetAttribute("needReturn", this.aAQ.ToString());
			textBlock.SetAttribute("needReturnForWait", this.aAq.ToString());
			if (this.aAR.Count != 0)
			{
				TextBlock textBlock2 = textBlock.AddChild("localVariablesBlock");
				foreach (LogicLocalVariable current in this.aAR.Values)
				{
					TextBlock textBlock3 = textBlock2.AddChild("item");
					current.a(textBlock3);
				}
			}
			textBlock.SetAttribute("currentClassActionsLevelIndex", this.aAr.ToString());
			if (this.aAS.Count != 0)
			{
				string text = "";
				foreach (int current2 in this.aAS)
				{
					if (text != "")
					{
						text += " ";
					}
					text += current2.ToString();
				}
				textBlock.SetAttribute("callActionsLevelIndexes", text);
			}
		}
	}
}
