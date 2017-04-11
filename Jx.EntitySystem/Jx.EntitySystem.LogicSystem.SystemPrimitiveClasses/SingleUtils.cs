using System;
namespace Jx.EntitySystem.LogicSystem.SystemPrimitiveClasses
{
	[LogicSystemBrowsable(true), LogicSystemClassDisplay("Single")]
	public static class SingleUtils
	{
		[LogicSystemMethodDisplay("Single + Single", "( {0} + {1} )")]
		public static float Addition(float a, float b)
		{
			return a + b;
		}
		[LogicSystemMethodDisplay("Single - Single", "( {0} - {1} )")]
		public static float Subtraction(float a, float b)
		{
			return a - b;
		}
		[LogicSystemMethodDisplay("Single * Single", "( {0} * {1} )")]
		public static float Multiplication(float a, float b)
		{
			return a * b;
		}
		[LogicSystemMethodDisplay("Single / Single", "( {0} / {1} )")]
		public static float Division(float a, float b)
		{
			return a / b;
		}
		[LogicSystemMethodDisplay("Single % Single", "( {0} % {1} )")]
		public static float Remainder(float a, float b)
		{
			return a % b;
		}
		[LogicSystemMethodDisplay("- Single", "( - {0} )")]
		public static float Negate(float a)
		{
			return -a;
		}
		[LogicSystemMethodDisplay("Single == Single", "( {0} == {1} )")]
		public static bool Equal(float a, float b)
		{
			return a == b;
		}
		[LogicSystemMethodDisplay("Single != Single", "( {0} != {1} )")]
		public static bool NotEqual(float a, float b)
		{
			return a != b;
		}
		[LogicSystemMethodDisplay("Single < Single", "( {0} < {1} )")]
		public static bool Less(float a, float b)
		{
			return a < b;
		}
		[LogicSystemMethodDisplay("Single > Single", "( {0} > {1} )")]
		public static bool Greater(float a, float b)
		{
			return a > b;
		}
		[LogicSystemMethodDisplay("Single <= Single", "( {0} <= {1} )")]
		public static bool LessOrEqual(float a, float b)
		{
			return a <= b;
		}
		[LogicSystemMethodDisplay("Single >= Single", "( {0} >= {1} )")]
		public static bool GreaterOrEqual(float a, float b)
		{
			return a >= b;
		}
		[LogicSystemMethodDisplay("Abs( Single )", "Abs( {0} )")]
		public static float Absolute(float a)
		{
			return Math.Abs(a);
		}
		[LogicSystemMethodDisplay("Min( Single, Single )", "Min( {0} >= {1} )")]
		public static float Minimum(float a, float b)
		{
			return Math.Min(a, b);
		}
		[LogicSystemMethodDisplay("Max( Single, Single )", "Max( {0} >= {1} )")]
		public static float Maximum(float a, float b)
		{
			return Math.Max(a, b);
		}
	}
}
