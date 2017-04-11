using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicIfThenElseActionType : LogicActionType
	{
		public LogicIfThenElseActionType()
		{
			this.fullName = "If/Then/Else";
		}
	}
}
