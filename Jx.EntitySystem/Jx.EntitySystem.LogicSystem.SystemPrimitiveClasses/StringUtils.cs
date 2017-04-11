using System;
namespace Jx.EntitySystem.LogicSystem.SystemPrimitiveClasses
{
	[LogicSystemBrowsable(true), LogicSystemClassDisplay("String")]
	public static class StringUtils
	{
		[LogicSystemMethodDisplay("String + String", " {0} + {1} ")]
		public static string Concatenation(string a, string b)
		{
			return a + b;
		}
		[LogicSystemMethodDisplay("String == String", "( {0} == {1} )")]
		public static bool Equal(string a, string b)
		{
			return a == b;
		}
		[LogicSystemMethodDisplay("String != String", "( {0} != {1} )")]
		public static bool NotEqual(string a, string b)
		{
			return a != b;
		}
	}
}
