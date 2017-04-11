using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicAssignVariableActionType : LogicDotPathActionType
	{
		public LogicAssignVariableActionType()
		{
			this.fullName = "Assign";
		}
	}
}
