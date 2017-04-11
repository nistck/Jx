using System;
namespace Jx.EntitySystem
{
	[LogicSystemBrowsable(false)]
	public class LogicComponentType : EntityType
	{
		public LogicComponentType()
		{
			base.AllowEmptyName = true;
			base.CreatableInMapEditor = false;
		}
	}
}
