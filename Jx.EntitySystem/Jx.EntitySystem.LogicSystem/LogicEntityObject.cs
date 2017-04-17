using Jx.FileSystem;
using System;
using System.Collections.Generic;
namespace Jx.EntitySystem.LogicSystem
{
	[LogicSystemBrowsable(true)]
	public class LogicEntityObject
	{
		public class WaitingThreadItem
		{
			private string aBC = "";
			private float aBc;
			internal List<LogicExecuteMethodInformation> aBD;

			public string ThreadName
			{
				get
				{
					return this.aBC;
				}
				set
				{
					this.aBC = value;
				}
			}
			public float RemainingTime
			{
				get
				{
					return this.aBc;
				}
				set
				{
					this.aBc = value;
				}
			}
		}

		private Entity aAx;
		private int aAY = -1;
		private List<LogicExecuteMethodInformation> aAy;
		private List<LogicEntityObject.WaitingThreadItem> aAZ = new List<LogicEntityObject.WaitingThreadItem>();
		public IList<LogicEntityObject.WaitingThreadItem> WaitingThreads
		{
			get
			{
				return this.aAZ.AsReadOnly();
			}
		}

		protected LogicEntityObject(Entity ownerEntity)
		{
			this.aAx = ownerEntity;
		}

		internal Entity A()
		{
			return this.aAx;
		}

		internal void TickWaitItems()
		{
			int num = this.aAZ.Count;
			for (int i = 0; i < num; i++)
			{
				LogicEntityObject.WaitingThreadItem waitingThreadItem = this.aAZ[i];
				waitingThreadItem.RemainingTime -= Entity.TickDelta;
				if (waitingThreadItem.RemainingTime <= 0f)
				{
					this.aAZ.RemoveAt(i);
					i--;
					num--;
					this.aAx.UnsubscribeToTickEvent();
					if (this.B() != -1)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodLevel != -1");
					}
					if (this.aAy != null)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodInformations != null");
					}
					this.aAy = waitingThreadItem.aBD;
					foreach (LogicExecuteMethodInformation current in this.aAy)
					{
						current.NeedReturn = false;
						current.NeedReturnForWait = false;
					}
					((LogicDesignerMethod)this.aAy[0].Method).A(this.aAy[0]);
					if (this.aAy == null)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodInformations == null");
					}
					if (this.B() != -1)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodLevel != -1");
					}
					this.aAy = null;
				}
			}
		}

		internal void A(string threadName, float remainingTime)
		{
			LogicEntityObject.WaitingThreadItem waitingThreadItem = new LogicEntityObject.WaitingThreadItem();
			waitingThreadItem.ThreadName = threadName;
			waitingThreadItem.RemainingTime = remainingTime;
			waitingThreadItem.aBD = this.aAy;
			this.aAZ.Add(waitingThreadItem);
			foreach (LogicExecuteMethodInformation current in this.aAy)
			{
				current.NeedReturn = true;
				current.NeedReturnForWait = true;
			}
			this.aAx.SubscribeToTickEvent();
		}

		internal int B()
		{
			return this.aAY;
		}

		internal void A(int num)
		{
			this.aAY = num;
		}

		internal List<LogicExecuteMethodInformation> b()
		{
			return this.aAy;
		}

		internal void A(List<LogicExecuteMethodInformation> list)
		{
			this.aAy = list;
		}

		internal bool A(TextBlock textBlock)
		{
			TextBlock textBlock2 = textBlock.FindChild("waitItems");
			if (textBlock2 != null)
			{
				foreach (TextBlock current in textBlock2.Children)
				{
					LogicEntityObject.WaitingThreadItem waitingThreadItem = new LogicEntityObject.WaitingThreadItem();
					if (current.IsAttributeExist("threadName"))
					{
						waitingThreadItem.ThreadName = current.GetAttribute("threadName");
					}
					if (current.IsAttributeExist("remainingTime"))
					{
						waitingThreadItem.RemainingTime = float.Parse(current.GetAttribute("remainingTime"));
					}
					TextBlock textBlock3 = current.FindChild("executeMethodInformations");
					if (textBlock3 != null)
					{
						waitingThreadItem.aBD = new List<LogicExecuteMethodInformation>();
						foreach (TextBlock current2 in textBlock3.Children)
						{
							LogicExecuteMethodInformation logicExecuteMethodInformation = new LogicExecuteMethodInformation();
							if (!logicExecuteMethodInformation.A(current2))
							{
								return false;
							}
							waitingThreadItem.aBD.Add(logicExecuteMethodInformation);
						}
					}
					this.aAZ.Add(waitingThreadItem);
				}
			}
			if (this.aAZ.Count != 0)
			{
				this.aAx.SubscribeToTickEvent();
			}
			return true;
		}

		internal void C()
		{
			foreach (LogicEntityObject.WaitingThreadItem current in this.aAZ)
			{
				if (current.aBD != null)
				{
					foreach (LogicExecuteMethodInformation current2 in current.aBD)
					{
						current2.A();
					}
				}
			}
		}

		internal void OnSave(TextBlock textBlock)
		{
			if (this.aAZ.Count != 0)
			{
				TextBlock textBlock2 = textBlock.AddChild("waitItems");
				foreach (LogicEntityObject.WaitingThreadItem current in this.aAZ)
				{
					TextBlock textBlock3 = textBlock2.AddChild("item");
					if (!string.IsNullOrEmpty(current.ThreadName))
					{
						textBlock3.SetAttribute("threadName", current.ThreadName);
					}
					textBlock3.SetAttribute("remainingTime", current.RemainingTime.ToString());
					if (current.aBD != null)
					{
						TextBlock textBlock4 = textBlock3.AddChild("executeMethodInformations");
						foreach (LogicExecuteMethodInformation current2 in current.aBD)
						{
							TextBlock textBlock5 = textBlock4.AddChild("item");
							current2.a(textBlock5);
						}
					}
				}
			}
		}

		public LogicEntityObject.WaitingThreadItem[] GetWaitingThreadsByName(string threadName)
		{
			if (threadName == null)
			{
				threadName = "";
			}
			List<LogicEntityObject.WaitingThreadItem> list = new List<LogicEntityObject.WaitingThreadItem>();
			foreach (LogicEntityObject.WaitingThreadItem current in this.aAZ)
			{
				if (current.ThreadName == threadName)
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}
	}
}
