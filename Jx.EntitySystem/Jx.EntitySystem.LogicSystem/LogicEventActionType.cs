using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicEventActionType : LogicDotPathActionType
	{
		public LogicEventActionType()
		{
			this.fullName = "Event";
		}
	}
}
