using System;
namespace Jx.EntitySystem
{
	public class LogicClassMember : LogicComponent
	{
		public LogicClass ParentClass
		{
			get
			{
				return (LogicClass)base.Parent;
			}
		}
	}
}
