using System;
namespace Jx.EntitySystem.LogicSystem
{
	[ManualTypeCreate]
	public class LogicCommentActionType : LogicActionType
	{
		public LogicCommentActionType()
		{
			this.fullName = "Comment";
		}
	}
}
