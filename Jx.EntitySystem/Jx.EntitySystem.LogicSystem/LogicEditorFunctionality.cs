using System;
namespace Jx.EntitySystem.LogicSystem
{
	public abstract class LogicEditorFunctionality
	{
		private static LogicEditorFunctionality aab;
		public static LogicEditorFunctionality Instance
		{
			get
			{
				return LogicEditorFunctionality.aab;
			}
		}
		public static void Init(LogicEditorFunctionality overridedObject)
		{
			if (LogicEditorFunctionality.aab != null)
			{
				Log.Fatal("LogicEditorMethods: Instance already created");
			}
			LogicEditorFunctionality.aab = overridedObject;
			LogicEditorFunctionality.aab.A();
		}
		public static void Shutdown()
		{
			if (LogicEditorFunctionality.aab != null)
			{
				LogicEditorFunctionality.aab.a();
				LogicEditorFunctionality.aab = null;
			}
		}
		private void A()
		{
		}
		private void a()
		{
		}
		public abstract bool ShowActionDialog(LogicAction parentAction, Type needType, bool dotPathAction, ref LogicAction action);
		public abstract bool ShowTypeDialog(Type needBaseType, ref Type type);
	}
}
