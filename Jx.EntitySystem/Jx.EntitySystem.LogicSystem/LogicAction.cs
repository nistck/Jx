using System;
using System.Collections.Generic;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicAction : LogicComponent
	{
		private LogicDesignerMethod aBN;
		public LogicDesignerMethod ParentMethod
		{
			get
			{
				if (this.aBN == null)
				{
					Entity parent = base.Parent;
					while (base.Parent != null)
					{
						this.aBN = (parent as LogicDesignerMethod);
						if (this.aBN != null)
						{
							break;
						}
						parent = parent.Parent;
					}
				}
				return this.aBN;
			}
		}
		public virtual object Execute(LogicExecuteMethodInformation executeMethodInformation)
		{
			Log.Fatal("LogicAction: Execute");
			return null;
		}
		public virtual string GetLinkedText(bool clickableLinks)
		{
			return "(not implemented)";
		}
		public virtual bool OnLinkedTextClick(string linkName)
		{
			Log.Warning("OnLinkedTextClick not implemented (link name \"{0}\")", linkName);
			return false;
		}
		public virtual List<object> GetChildBrowsableItems()
		{
			return null;
		}
		public virtual void InsertAction(int index, LogicAction action)
		{
			Log.Fatal("LogicAction: InsertAction: internal error");
		}
		public virtual void GetAccessedLocalVariables(LogicAction downToChildAction, List<LogicLocalVariable> variables)
		{
		}
	}
}
