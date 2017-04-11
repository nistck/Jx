using System;

namespace Jx.EntitySystem
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class AllowToCreateTypeBasedOnThisClassAttribute : Attribute
	{
        public bool Allow { get; private set; }

		public AllowToCreateTypeBasedOnThisClassAttribute(bool allow)
		{
			Allow = allow;
		}
	}
}
