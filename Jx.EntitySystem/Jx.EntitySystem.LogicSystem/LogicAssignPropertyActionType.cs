using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicAssignPropertyActionType : LogicDotPathActionType
	{
		public LogicAssignPropertyActionType()
		{
			this.fullName = "Assign";
		}
	}
}
