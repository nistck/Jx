using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicReturnActionType : LogicActionType
	{
		public LogicReturnActionType()
		{
			base.FullName = "Return";
		}
	}
}
