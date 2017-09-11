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
			private string threadName = "";
			private float remainingTime;
			internal List<LogicExecuteMethodInformation> aBD;

			public string ThreadName
			{
				get
				{
					return this.threadName;
				}
				set
				{
					this.threadName = value;
				}
			}
			public float RemainingTime
			{
				get
				{
					return this.remainingTime;
				}
				set
				{
					this.remainingTime = value;
				}
			}
		}

		private Entity ownerEntity;
		private int aAY = -1;
		private List<LogicExecuteMethodInformation> currentExecutingMethodInformations;
		private List<LogicEntityObject.WaitingThreadItem> waitintThreads = new List<LogicEntityObject.WaitingThreadItem>();
		public IList<LogicEntityObject.WaitingThreadItem> WaitingThreads
		{
			get
			{
				return this.waitintThreads.AsReadOnly();
			}
		}

		protected LogicEntityObject(Entity ownerEntity)
		{
			this.ownerEntity = ownerEntity;
		}

		internal Entity OwnerEntity()
		{
			return this.ownerEntity;
		}

		internal void TickWaitItems()
		{
			int num = this.waitintThreads.Count;
			for (int i = 0; i < num; i++)
			{
				LogicEntityObject.WaitingThreadItem waitingThreadItem = this.waitintThreads[i];
				waitingThreadItem.RemainingTime -= Entity.TickDelta;
				if (waitingThreadItem.RemainingTime <= 0f)
				{
					this.waitintThreads.RemoveAt(i);
					i--;
					num--;
					this.ownerEntity.UnsubscribeToTickEvent();
					if (this.B() != -1)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodLevel != -1");
					}
					if (this.currentExecutingMethodInformations != null)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodInformations != null");
					}

					this.currentExecutingMethodInformations = waitingThreadItem.aBD;
					foreach (LogicExecuteMethodInformation current in this.currentExecutingMethodInformations)
					{
						current.NeedReturn = false;
						current.NeedReturnForWait = false;
					}
					((LogicDesignerMethod)this.currentExecutingMethodInformations[0].Method).A(this.currentExecutingMethodInformations[0]);
					if (this.currentExecutingMethodInformations == null)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodInformations == null");
					}
					if (this.B() != -1)
					{
						Log.Fatal("LogicEntityObject: Internal error: TickWaitItems: currentExecutingMethodLevel != -1");
					}
					this.currentExecutingMethodInformations = null;
				}
			}
		}

		internal void A(string threadName, float remainingTime)
		{
			LogicEntityObject.WaitingThreadItem waitingThreadItem = new LogicEntityObject.WaitingThreadItem();
			waitingThreadItem.ThreadName = threadName;
			waitingThreadItem.RemainingTime = remainingTime;
			waitingThreadItem.aBD = this.currentExecutingMethodInformations;
			this.waitintThreads.Add(waitingThreadItem);
			foreach (LogicExecuteMethodInformation current in this.currentExecutingMethodInformations)
			{
				current.NeedReturn = true;
				current.NeedReturnForWait = true;
			}
			this.ownerEntity.SubscribeToTickEvent();
		}

		internal int B()
		{
			return this.aAY;
		}

		internal void A(int num)
		{
			this.aAY = num;
		}

		internal List<LogicExecuteMethodInformation> GetCurrentExecutingMethodInformations()
		{
			return this.currentExecutingMethodInformations;
		}

		internal void SetCurrentExecutingMethodInformations(List<LogicExecuteMethodInformation> list)
		{
			this.currentExecutingMethodInformations = list;
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
					this.waitintThreads.Add(waitingThreadItem);
				}
			}
			if (this.waitintThreads.Count != 0)
			{
				this.ownerEntity.SubscribeToTickEvent();
			}
			return true;
		}

		internal void C()
		{
			foreach (WaitingThreadItem current in this.waitintThreads)
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
			if (this.waitintThreads.Count != 0)
			{
				TextBlock textBlock2 = textBlock.AddChild("waitItems");
				foreach (WaitingThreadItem current in this.waitintThreads)
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

		public WaitingThreadItem[] GetWaitingThreadsByName(string threadName)
		{
			if (threadName == null)
			{
				threadName = "";
			}
			List<WaitingThreadItem> list = new List<WaitingThreadItem>();
			foreach (WaitingThreadItem current in this.waitintThreads)
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
