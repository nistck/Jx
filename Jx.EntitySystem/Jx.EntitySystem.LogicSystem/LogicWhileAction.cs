using System;
using System.Collections.Generic;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicWhileAction : LogicAction
	{
		[Entity.FieldSerializeAttribute("conditionAction")]
		private LogicAction abf;
		[Entity.FieldSerializeAttribute("actions")]
		private List<LogicAction> abG = new List<LogicAction>();
		public LogicAction ConditionAction
		{
			get
			{
				return this.abf;
			}
		}
		public override void InsertAction(int index, LogicAction action)
		{
			if (index < this.abG.Count)
			{
				this.abG.Insert(index, action);
				return;
			}
			this.abG.Add(action);
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			LogicAction logicAction = entity as LogicAction;
			if (logicAction != null)
			{
				if (this.abf == logicAction)
				{
					this.abf = null;
				}
				this.abG.Remove(logicAction);
			}
		}
		public override string ToString()
		{
			string text = "while ";
			if (this.abf != null)
			{
				text += this.abf.ToString();
			}
			else
			{
				text += "(null)";
			}
			return text;
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string text = "<!<!font bold while!>!> ";
			if (this.abf != null)
			{
				text = text + "<!<!conditionAction v " + this.abf.ToString() + "!>!>";
			}
			else
			{
				text += "<!<!conditionAction i (null)!>!>";
			}
			return text;
		}
		public override bool OnLinkedTextClick(string linkName)
		{
			if (!(linkName == "conditionAction"))
			{
				return base.OnLinkedTextClick(linkName);
			}
			LogicAction logicAction = this.abf;
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, typeof(bool), false, ref logicAction))
			{
				return false;
			}
			this.abf = logicAction;
			return true;
		}
		public override List<object> GetChildBrowsableItems()
		{
			List<object> list = new List<object>();
			foreach (LogicAction current in this.abG)
			{
				list.Add(current);
			}
			return list;
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicWhileAction logicWhileAction = (LogicWhileAction)source;
			if (logicWhileAction.ConditionAction != null)
			{
				this.abf = (LogicAction)logicWhileAction.ConditionAction.CloneWithCopyBrowsableProperties(false, this);
			}
			foreach (LogicAction current in logicWhileAction.abG)
			{
				this.abG.Add((LogicAction)current.CloneWithCopyBrowsableProperties(false, this));
			}
		}
		public override void GetAccessedLocalVariables(LogicAction downToChildAction, List<LogicLocalVariable> variables)
		{
			base.GetAccessedLocalVariables(downToChildAction, variables);
			int num;
			if (downToChildAction != null)
			{
				num = this.abG.IndexOf(downToChildAction);
			}
			else
			{
				num = this.abG.Count;
			}
			if (num != -1)
			{
				for (int i = 0; i < num; i++)
				{
					LogicDeclareLocalVariableAction logicDeclareLocalVariableAction = this.abG[i] as LogicDeclareLocalVariableAction;
					if (logicDeclareLocalVariableAction != null)
					{
						variables.Add(new LogicLocalVariable(logicDeclareLocalVariableAction));
					}
				}
			}
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			if (this.abf == null)
			{
				Log.Error("LogicWhileAction: ConditionAction = null");
				return null;
			}
			LogicEntityObject logicEntityObject = executeMethodInformation.LogicEntityObject;
			bool flag = false;
			int num = 0;
			bool flag2;
			if (logicEntityObject != null)
			{
				if (executeMethodInformation.CurrentClassActionsLevelIndex < executeMethodInformation.CallActionsLevelIndexes.Count)
				{
					flag = true;
					num = executeMethodInformation.CallActionsLevelIndexes[executeMethodInformation.CurrentClassActionsLevelIndex];
					flag2 = true;
				}
				else
				{
					flag2 = (bool)this.abf.Execute(executeMethodInformation);
				}
			}
			else
			{
				flag2 = (bool)this.abf.Execute(executeMethodInformation);
			}
			while (flag2)
			{
				for (int i = num; i < this.abG.Count; i++)
				{
					LogicAction logicAction = this.abG[i];
					if (!flag)
					{
						executeMethodInformation.PushCallActionsLevelIndex(i);
					}
					flag = false;
					executeMethodInformation.CurrentClassActionsLevelIndex++;
					object result = logicAction.Execute(executeMethodInformation);
					executeMethodInformation.CurrentClassActionsLevelIndex--;
					if (executeMethodInformation.NeedReturnForWait)
					{
						return null;
					}
					executeMethodInformation.PopCallActionsLevelIndex();
					if (LogicUtils.A() != 0f)
					{
						executeMethodInformation.PushCallActionsLevelIndex(i + 1);
						if (executeMethodInformation.LogicEntityObject != null)
						{
							executeMethodInformation.LogicEntityObject.CreateWaitingThreadItem(LogicUtils.a(), LogicUtils.A());
						}
						LogicUtils.A(0f);
						LogicUtils.A("");
						return null;
					}
					if (executeMethodInformation.NeedReturn)
					{
						return result;
					}
				}
				num = 0;
				flag2 = (bool)this.abf.Execute(executeMethodInformation);
			}
			return null;
		}
	}
}
