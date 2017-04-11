using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicGetVariableActionType : LogicAllowCallDotPathActionType
	{
		public LogicGetVariableActionType()
		{
			this.fullName = "Get variable";
		}
	}
}
