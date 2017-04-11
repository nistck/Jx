using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicDeclareLocalVariableActionType : LogicActionType
	{
		public LogicDeclareLocalVariableActionType()
		{
			this.fullName = "Declare local variable";
		}
	}
}
