using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicGetConstantValueActionType : LogicActionType
	{
		public LogicGetConstantValueActionType()
		{
			this.fullName = "Constant value";
		}
	}
}
