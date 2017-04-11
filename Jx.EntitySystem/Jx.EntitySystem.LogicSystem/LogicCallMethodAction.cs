using System;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicCallMethodAction : LogicAllowCallDotPathAction
	{
		[Entity.FieldSerializeAttribute("methodName")]
		private string abk;
		[Entity.FieldSerializeAttribute("propertyName")]
		private string abL;
		[Entity.FieldSerializeAttribute("parameterTypeNames")]
		private string[] abl;
		[Entity.FieldSerializeAttribute("parameterActions")]
		private LogicAction[] abM;
		public string MethodName
		{
			get
			{
				return this.abk;
			}
		}
		public string[] ParameterTypeNames
		{
			get
			{
				return this.abl;
			}
		}
		public string PropertyName
		{
			get
			{
				return this.abL;
			}
		}
		public LogicAction[] ParameterActions
		{
			get
			{
				return this.abM;
			}
		}
		protected internal void Reset()
		{
			this.abk = null;
			this.abl = null;
			this.abL = null;
			if (this.abM != null)
			{
				for (int i = 0; i < this.abM.Length; i++)
				{
					if (this.abM[i] != null)
					{
						this.abM[i].SetForDeletion(false);
						Entities.Instance.DeleteEntitiesQueuedForDeletion();
					}
				}
			}
		}
		protected void InitMethodNameAndParameterTypes(string methodName, string[] parameterTypeNames)
		{
			this.Reset();
			this.abk = methodName;
			this.abl = parameterTypeNames;
			if (parameterTypeNames != null)
			{
				this.abM = new LogicAction[parameterTypeNames.Length];
			}
		}
		protected void InitPropertyNameAndParameterTypes(string propertyName, string[] parameterTypeNames)
		{
			this.Reset();
			this.abL = propertyName;
			this.abl = parameterTypeNames;
			if (parameterTypeNames != null)
			{
				this.abM = new LogicAction[parameterTypeNames.Length];
			}
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			LogicAction logicAction = entity as LogicAction;
			if (logicAction != null && this.abM != null)
			{
				for (int i = 0; i < this.abM.Length; i++)
				{
					if (this.abM[i] == logicAction)
					{
						this.abM[i] = null;
						return;
					}
				}
			}
		}
		public void SetParameterAction(int index, LogicAction action)
		{
			if (this.abM[index] != null)
			{
				this.abM[index].SetForDeletion(false);
				Entities.Instance.DeleteEntitiesQueuedForDeletion();
			}
			this.abM[index] = action;
			if (this.abM[index] != null && this.abM[index].Parent != this)
			{
				Log.Fatal("LogicCallMethodAction: set parameter action: invalid action");
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicCallMethodAction logicCallMethodAction = (LogicCallMethodAction)source;
			this.abk = logicCallMethodAction.abk;
			this.abL = logicCallMethodAction.abL;
			if (logicCallMethodAction.abl != null)
			{
				this.abl = new string[logicCallMethodAction.abl.Length];
				for (int i = 0; i < logicCallMethodAction.abl.Length; i++)
				{
					this.abl[i] = logicCallMethodAction.abl[i];
				}
				this.abM = new LogicAction[logicCallMethodAction.abM.Length];
				for (int j = 0; j < logicCallMethodAction.abM.Length; j++)
				{
					if (logicCallMethodAction.abM[j] != null)
					{
						this.abM[j] = (LogicAction)logicCallMethodAction.abM[j].CloneWithCopyBrowsableProperties(false, this);
					}
				}
			}
		}
		protected internal string[] ForToStringParameterActions()
		{
			if (this.ParameterActions == null)
			{
				return new string[0];
			}
			string[] array = new string[this.ParameterActions.Length];
			for (int i = 0; i < this.ParameterActions.Length; i++)
			{
				LogicAction logicAction = this.ParameterActions[i];
				if (logicAction != null)
				{
					string[] array2;
					IntPtr intPtr;
					(array2 = array)[(int)(intPtr = (IntPtr)i)] = array2[(int)intPtr] + logicAction.ToString();
				}
				else
				{
					array[i] = "null";
				}
			}
			return array;
		}
		protected internal string[] ForGetLinkedTextParameterActions()
		{
			if (this.ParameterActions == null)
			{
				return new string[0];
			}
			string[] array = new string[this.ParameterActions.Length];
			for (int i = 0; i < this.ParameterActions.Length; i++)
			{
				LogicAction logicAction = this.ParameterActions[i];
				if (logicAction != null)
				{
					array[i] = string.Format("<!<!{0} v {1}!>!>", i.ToString(), logicAction.ToString());
				}
				else
				{
					array[i] = string.Format("<!<!{0} v null!>!>", i.ToString());
				}
			}
			return array;
		}
	}
}
