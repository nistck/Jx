using System;
using System.Collections.Generic;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicIfThenElseAction : LogicAction
	{
		[Entity.FieldSerializeAttribute("conditionAction")]
		private LogicAction aba;
		[Entity.FieldSerializeAttribute("trueActions")]
		private List<LogicAction> abB = new List<LogicAction>();
		[Entity.FieldSerializeAttribute("falseActions")]
		private List<LogicAction> abb = new List<LogicAction>();
		public LogicAction ConditionAction
		{
			get
			{
				return this.aba;
			}
		}
		public override void InsertAction(int index, LogicAction action)
		{
			if (index < this.abB.Count)
			{
				this.abB.Insert(index, action);
				return;
			}
			if (index == this.abB.Count)
			{
				this.abB.Add(action);
				return;
			}
			if (index < this.abB.Count + 1 + this.abb.Count)
			{
				this.abb.Insert(index - 1 - this.abB.Count, action);
				return;
			}
			this.abb.Add(action);
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			base.OnRemoveChild(entity);
			LogicAction logicAction = entity as LogicAction;
			if (logicAction != null)
			{
				if (this.aba == logicAction)
				{
					this.aba = null;
				}
				this.abB.Remove(logicAction);
				this.abb.Remove(logicAction);
			}
		}
		public override string ToString()
		{
			string text = "if ";
			if (this.aba != null)
			{
				text += this.aba.ToString();
			}
			else
			{
				text += "(null)";
			}
			return text;
		}
		public override string GetLinkedText(bool clickableLinks)
		{
			string text = "<!<!font bold if!>!> ";
			if (this.aba != null)
			{
				text = text + "<!<!conditionAction v " + this.aba.ToString() + "!>!>";
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
			LogicAction logicAction = this.aba;
			if (!LogicEditorFunctionality.Instance.ShowActionDialog(this, typeof(bool), false, ref logicAction))
			{
				return false;
			}
			this.aba = logicAction;
			return true;
		}
		public override List<object> GetChildBrowsableItems()
		{
			List<object> list = new List<object>();
			foreach (LogicAction current in this.abB)
			{
				list.Add(current);
			}
			list.Add("else");
			foreach (LogicAction current2 in this.abb)
			{
				list.Add(current2);
			}
			return list;
		}
		public override void GetAccessedLocalVariables(LogicAction downToChildAction, List<LogicLocalVariable> variables)
		{
			base.GetAccessedLocalVariables(downToChildAction, variables);
			int num;
			if (downToChildAction != null)
			{
				num = this.abB.IndexOf(downToChildAction);
			}
			else
			{
				num = this.abB.Count;
			}
			if (num != -1)
			{
				for (int i = 0; i < num; i++)
				{
					LogicDeclareLocalVariableAction logicDeclareLocalVariableAction = this.abB[i] as LogicDeclareLocalVariableAction;
					if (logicDeclareLocalVariableAction != null)
					{
						variables.Add(new LogicLocalVariable(logicDeclareLocalVariableAction));
					}
				}
				return;
			}
			if (downToChildAction != null)
			{
				num = this.abb.IndexOf(downToChildAction);
			}
			else
			{
				num = this.abb.Count;
			}
			if (num != -1)
			{
				for (int j = 0; j < num; j++)
				{
					LogicDeclareLocalVariableAction logicDeclareLocalVariableAction2 = this.abb[j] as LogicDeclareLocalVariableAction;
					if (logicDeclareLocalVariableAction2 != null)
					{
						variables.Add(new LogicLocalVariable(logicDeclareLocalVariableAction2));
					}
				}
			}
		}
		protected override void OnClone(Entity source)
		{
			base.OnClone(source);
			LogicIfThenElseAction logicIfThenElseAction = (LogicIfThenElseAction)source;
			if (logicIfThenElseAction.ConditionAction != null)
			{
				this.aba = (LogicAction)logicIfThenElseAction.ConditionAction.CloneWithCopyBrowsableProperties(false, this);
			}
			foreach (LogicAction current in logicIfThenElseAction.abB)
			{
				this.abB.Add((LogicAction)current.CloneWithCopyBrowsableProperties(false, this));
			}
			foreach (LogicAction current2 in logicIfThenElseAction.abb)
			{
				this.abb.Add((LogicAction)current2.CloneWithCopyBrowsableProperties(false, this));
			}
		}
		public override object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			if (this.aba == null)
			{
				Log.Error("LogicIfThenElseAction: ConditionAction = null");
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
					flag2 = (num < 10000);
					if (num >= 10000)
					{
						num -= 10000;
					}
				}
				else
				{
					flag2 = (bool)this.aba.Execute(executeMethodInformation);
				}
			}
			else
			{
				flag2 = (bool)this.aba.Execute(executeMethodInformation);
			}
			List<LogicAction> list = flag2 ? this.abB : this.abb;
			for (int i = num; i < list.Count; i++)
			{
				LogicAction logicAction = list[i];
				if (!flag)
				{
					executeMethodInformation.PushCallActionsLevelIndex(flag2 ? i : (i + 10000));
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
					executeMethodInformation.PushCallActionsLevelIndex((flag2 ? i : (i + 10000)) + 1);
					if (executeMethodInformation.LogicEntityObject != null)
					{
						executeMethodInformation.LogicEntityObject.A(LogicUtils.a(), LogicUtils.A());
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
			return null;
		}
	}
}
