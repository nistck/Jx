using System;
using System.Collections.Generic;
using Jx.Ext;

namespace Jx.EntitySystem.LogicSystem
{
	public class LogicDesignerMethod : LogicMethod
	{
		[Entity.FieldSerializeAttribute("actions")]
		private List<LogicAction> actions = new List<LogicAction>();
		public List<LogicAction> Actions
		{
			get
			{
				return this.actions;
			}
		}
		public object Execute(Type staticClassType, object[] parameterValues)
		{
			if (staticClassType == null)
			{
				Log.Fatal("LogicDesignedMethod: staticClassType = null");
                return null;
			}

			LogicExecuteMethodInformation logicExecuteMethodInformation = new LogicExecuteMethodInformation(this, staticClassType);
			if (!this.A(logicExecuteMethodInformation, parameterValues))
			{
				return null;
			}
			return this.A(logicExecuteMethodInformation);
		}
		public object Execute(LogicEntityObject logicEntityObject, object[] parameterValues)
		{
			if (logicEntityObject == null)
			{
				Log.Fatal("LogicDesignedMethod: logicEntityObject = null");
                return null;
			}

			LogicExecuteMethodInformation logicExecuteMethodInformation;
			if (logicEntityObject.GetCurrentExecutingMethodInformations() != null && logicEntityObject.GetCurrentExecutingMethodLevel() + 1 < logicEntityObject.GetCurrentExecutingMethodInformations().Count)
			{
				logicExecuteMethodInformation = logicEntityObject.GetCurrentExecutingMethodInformations()[logicEntityObject.GetCurrentExecutingMethodLevel() + 1];
			}
			else
			{
				logicExecuteMethodInformation = new LogicExecuteMethodInformation(this, logicEntityObject);
				if (!this.A(logicExecuteMethodInformation, parameterValues))
				{
					return null;
				}
			}
			object result;
			if (logicEntityObject.GetCurrentExecutingMethodInformations() == null)
			{
				logicEntityObject.SetCurrentExecutingMethodInformations(new List<LogicExecuteMethodInformation>());
				result = this.A(logicExecuteMethodInformation);
                logicEntityObject.SetCurrentExecutingMethodInformations((List<LogicExecuteMethodInformation>)null);
			}
			else
			{
				result = this.A(logicExecuteMethodInformation);
			}
			return result;
		}

		private bool A(LogicExecuteMethodInformation logicExecuteMethodInformation, object[] array)
		{
			for (int i = 0; i < base.Parameters.Count; i++)
			{
				LogicParameter logicParameter = base.Parameters[i];
				object obj = array[i];
				if (!logicParameter.parameterType.IsAssignableFrom((obj != null) ? obj.GetType() : null))
				{
					Log.Error("Method: invalid parameter value type \"{0}\"", logicParameter.ParameterName);
					return false;
				}
				if (!string.IsNullOrEmpty(logicParameter.ParameterName))
				{
					LogicLocalVariable logicLocalVariable = logicExecuteMethodInformation.DeclareLocalVariable(logicParameter);
					logicLocalVariable.Value = obj;
				}
			}
			return true;
		}

		internal object A(LogicExecuteMethodInformation logicExecuteMethodInformation)
		{
			LogicEntityObject logicEntityObject = logicExecuteMethodInformation.LogicEntityObject;
			bool flag = false;
			int num = 0;
			if (logicEntityObject != null)
			{
				LogicEntityObject expr_0F = logicEntityObject;
				expr_0F.SetCurrentExecutingMethodLevel(expr_0F.GetCurrentExecutingMethodLevel() + 1);
				if (logicEntityObject.GetCurrentExecutingMethodLevel() >= logicEntityObject.GetCurrentExecutingMethodInformations().Count)
				{
					logicEntityObject.GetCurrentExecutingMethodInformations().Add(logicExecuteMethodInformation);
				}
				else
				{
					if (logicEntityObject.GetCurrentExecutingMethodInformations()[logicEntityObject.GetCurrentExecutingMethodLevel()] != logicExecuteMethodInformation)
					{
						Log.Fatal("LogicDesignerMethod: Internal error: Execute: logicEntityObject.CurrentExecutingMethodInformations[logicEntityObject.CurrentExecutingMethodLevel] != executeMethodInformation");
					}
					flag = true;
					num = logicExecuteMethodInformation.CallActionsLevelIndexes[logicExecuteMethodInformation.CurrentClassActionsLevelIndex];
				}
			}
			object result = null;
			for (int i = num; i < this.actions.Count; i++)
			{
				LogicAction logicAction = this.actions[i];
				if (!flag)
				{
					logicExecuteMethodInformation.PushCallActionsLevelIndex(i);
				}
				flag = false;
				logicExecuteMethodInformation.CurrentClassActionsLevelIndex++;
				object obj = logicAction.Execute(logicExecuteMethodInformation);
				logicExecuteMethodInformation.CurrentClassActionsLevelIndex--;
				if (logicExecuteMethodInformation.NeedReturnForWait)
				{
					if (logicEntityObject != null)
					{
						LogicEntityObject expr_C7 = logicEntityObject;
						expr_C7.SetCurrentExecutingMethodLevel(expr_C7.GetCurrentExecutingMethodLevel() - 1);
					}
					return null;
				}
				logicExecuteMethodInformation.PopCallActionsLevelIndex();
				if (LogicUtils.A() != 0f)
				{
					logicExecuteMethodInformation.PushCallActionsLevelIndex(i + 1);
					if (logicExecuteMethodInformation.LogicEntityObject != null)
					{
						logicExecuteMethodInformation.LogicEntityObject.CreateWaitingThreadItem(LogicUtils.a(), LogicUtils.A());
					}
					LogicUtils.A(0f);
					LogicUtils.A("");
					if (logicEntityObject != null)
					{
						LogicEntityObject expr_128 = logicEntityObject;
						expr_128.SetCurrentExecutingMethodLevel(expr_128.GetCurrentExecutingMethodLevel() - 1);
					}
					return null;
				}
				if (logicExecuteMethodInformation.NeedReturn)
				{
					result = obj;
					break;
				}
			}
			if (logicExecuteMethodInformation.LogicEntityObject != null)
			{
				logicExecuteMethodInformation.LogicEntityObject.GetCurrentExecutingMethodInformations().RemoveAt(logicExecuteMethodInformation.LogicEntityObject.GetCurrentExecutingMethodInformations().Count - 1);
			}
			if (logicEntityObject != null)
			{
				LogicEntityObject expr_18A = logicEntityObject;
				expr_18A.SetCurrentExecutingMethodLevel(expr_18A.GetCurrentExecutingMethodLevel() - 1);
			}
			return result;
		}
		public void InsertAction(int index, LogicAction action)
		{
			if (index < this.actions.Count)
			{
				this.actions.Insert(index, action);
				return;
			}
			this.actions.Add(action);
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			LogicAction logicAction = entity as LogicAction;
			if (logicAction != null)
			{
				this.actions.Remove(logicAction);
			}
		}
		private void A(LogicAction logicAction, List<LogicLocalVariable> list)
		{
			LogicDeclareLocalVariableAction logicDeclareLocalVariableAction = logicAction as LogicDeclareLocalVariableAction;
			if (logicDeclareLocalVariableAction != null)
			{
				list.Add(new LogicLocalVariable(logicDeclareLocalVariableAction));
			}
			List<object> childBrowsableItems = logicAction.GetChildBrowsableItems();
			if (childBrowsableItems != null)
			{
				foreach (object current in childBrowsableItems)
				{
					LogicAction logicAction2 = current as LogicAction;
					if (logicAction2 != null)
					{
						this.A(logicAction2, list);
					}
				}
			}
		}
		public List<LogicLocalVariable> GetAccessedLocalVariables(LogicAction fromAction)
		{
			List<LogicLocalVariable> list = new List<LogicLocalVariable>();
			foreach (LogicParameter current in base.Parameters)
			{
				list.Add(new LogicLocalVariable(current));
			}
			foreach (LogicAction current2 in this.Actions)
			{
				this.A(current2, list);
			}
			return list;
		}
		protected override string[] GetCompileScriptsBody(string namespaceName)
		{
			List<string> list = new List<string>();
			list.Add(string.Format("Engine.EntitySystem.LogicClass __class = Engine.EntitySystem.LogicSystemManager.Instance.MapClassManager.GetByName( \"{0}\" );", base.ParentClass.ClassName));
			list.Add(string.Format("Engine.EntitySystem.LogicSystem.LogicDesignerMethod __method = (Engine.EntitySystem.LogicSystem.LogicDesignerMethod)__class.GetMethodByName( \"{0}\" );", base.MethodName));
			string text = "";
			if (base.ReturnType != typeof(void))
			{
				text += string.Format("return ({0})", CJ.TypeToCSharpString(base.ReturnType));
			}
			text += "__method.Execute( ";
			if (base.ParentClass is LogicEntityClass)
			{
				text += "this";
			}
			else
			{
				text += string.Format("EntitySystemWorld.Instance.GetLogicSystemScriptsAssemblyClassByClassName( \"{0}\" )", base.ParentClass.ClassName);
			}
			text += string.Format(", new object[ {0} ]", base.Parameters.Count.ToString());
			text += "{ ";
			for (int i = 0; i < base.Parameters.Count; i++)
			{
				if (i != 0)
				{
					text += ", ";
				}
				LogicParameter logicParameter = base.Parameters[i];
				if (!string.IsNullOrEmpty(logicParameter.ParameterName))
				{
					text += logicParameter.ParameterName;
				}
				else
				{
					text = text + "__emptyParam" + i.ToString();
				}
			}
			text += " } );";
			list.Add(text);
			return list.ToArray();
		}
	}
}
