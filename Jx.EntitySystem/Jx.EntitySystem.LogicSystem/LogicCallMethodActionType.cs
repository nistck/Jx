using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicCallMethodActionType : LogicAllowCallDotPathActionType
	{
		public LogicCallMethodActionType()
		{
			this.fullName = "Call method/get property";
		}
	}
}
