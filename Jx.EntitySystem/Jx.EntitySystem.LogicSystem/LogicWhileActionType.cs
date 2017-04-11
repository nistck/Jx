using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicWhileActionType : LogicActionType
	{
		public LogicWhileActionType()
		{
			this.fullName = "While";
		}
	}
}
