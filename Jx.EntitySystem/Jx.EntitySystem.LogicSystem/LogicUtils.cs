using System;
namespace Jx.EntitySystem.LogicSystem
{
	[LogicSystemBrowsable(true), LogicSystemClassDisplay("Logic")]
	public static class LogicUtils
	{
		private static float aaC;
		private static string aac;

		internal static float A()
		{
			return LogicUtils.aaC;
		}
		internal static void A(float num)
		{
			LogicUtils.aaC = num;
		}
		internal static string a()
		{
			return LogicUtils.aac;
		}
		internal static void A(string text)
		{
			LogicUtils.aac = text;
		}
		[LogicSystemBrowsable(true)]
		public static void Wait(float seconds)
		{
			if (LogicUtils.A() != 0f)
			{
				Log.Fatal("LogicSystem: Internal error: LogicUtils.Wait: WaitCalledSeconds != 0");
			}
			LogicUtils.A(seconds);
			LogicUtils.A("");
		}
		[LogicSystemBrowsable(true)]
		public static void Wait(float seconds, string threadName)
		{
			if (threadName == null)
			{
				threadName = "";
			}
			if (LogicUtils.A() != 0f)
			{
				Log.Fatal("LogicSystem: Internal error: LogicUtils.Wait: WaitCalledSeconds != 0");
			}
			LogicUtils.A(seconds);
			LogicUtils.A(threadName);
		}

		[LogicSystemBrowsable(true)]
		public static void ResumeWaitingThread(Entity entity, string threadName)
		{
			if (entity.LogicObject != null)
			{
				LogicEntityObject.WaitingThreadItem[] waitingThreadsByName = entity.LogicObject.GetWaitingThreadsByName(threadName);
				for (int i = 0; i < waitingThreadsByName.Length; i++)
				{
					LogicEntityObject.WaitingThreadItem waitingThreadItem = waitingThreadsByName[i];
					waitingThreadItem.RemainingTime = 0f;
				}
			}
		}
	}
}
