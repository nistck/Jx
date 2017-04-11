using System;
namespace Jx.EntitySystem
{
	[ManualTypeCreate]
	public class LogicSystemManagerType : LogicComponentType
	{
		public LogicSystemManagerType()
		{
			base.NetworkType = EntityNetworkTypes.ServerOnly;
		}
	}
}
