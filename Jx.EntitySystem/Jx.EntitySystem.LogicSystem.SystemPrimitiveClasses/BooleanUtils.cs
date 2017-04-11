using System;
using Jx;

namespace Jx.EntitySystem.LogicSystem.SystemPrimitiveClasses
{
	[LogicSystemBrowsable(true), LogicSystemClassDisplay("Boolean")]
	public static class BooleanUtils
	{
		[LogicSystemMethodDisplay("Boolean == Boolean", "( {0} == {1} )")]
		public static bool Equal(bool a, bool b)
		{
			return a == b;
		}
		[LogicSystemMethodDisplay("Boolean != Boolean", "( {0} != {1} )")]
		public static bool NotEqual(bool a, bool b)
		{
			return a != b;
		}
		[LogicSystemMethodDisplay("! Boolean", " ( ! {0} )")]
		public static bool Negation(bool a)
		{
			return !a;
		}
		[LogicSystemMethodDisplay("Boolean and Boolean", "( {0} and {1} )")]
		public static bool And(bool a, bool b)
		{
			return a && b;
		}
		[LogicSystemMethodDisplay("Boolean or Boolean", "( {0} or {1} )")]
		public static bool Or(bool a, bool b)
		{
			return a || b;
		}
	}
}
