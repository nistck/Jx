using System;
namespace Jx.EntitySystem.LogicSystem.SystemPrimitiveClasses
{
	[LogicSystemBrowsable(true), LogicSystemClassDisplay("Int32")]
	public static class Int32Utils
	{
		[LogicSystemMethodDisplay("Int32 + Int32", "( {0} + {1} )")]
		public static int Addition(int a, int b)
		{
			return a + b;
		}
		[LogicSystemMethodDisplay("Int32 - Int32", "( {0} - {1} )")]
		public static int Subtraction(int a, int b)
		{
			return a - b;
		}
		[LogicSystemMethodDisplay("Int32 * Int32", "( {0} * {1} )")]
		public static int Multiplication(int a, int b)
		{
			return a * b;
		}
		[LogicSystemMethodDisplay("Int32 / Int32", "( {0} / {1} )")]
		public static int Division(int a, int b)
		{
			return a / b;
		}
		[LogicSystemMethodDisplay("Int32 % Int32", "( {0} % {1} )")]
		public static int Remainder(int a, int b)
		{
			return a % b;
		}
		[LogicSystemMethodDisplay("- Int32", "( - {0} )")]
		public static int Negate(int a)
		{
			return -a;
		}
		[LogicSystemMethodDisplay("Int32 == Int32", "( {0} == {1} )")]
		public static bool Equal(int a, int b)
		{
			return a == b;
		}
		[LogicSystemMethodDisplay("Int32 != Int32", "( {0} != {1} )")]
		public static bool NotEqual(int a, int b)
		{
			return a != b;
		}
		[LogicSystemMethodDisplay("Int32 < Int32", "( {0} < {1} )")]
		public static bool Less(int a, int b)
		{
			return a < b;
		}
		[LogicSystemMethodDisplay("Int32 > Int32", "( {0} > {1} )")]
		public static bool Greater(int a, int b)
		{
			return a > b;
		}
		[LogicSystemMethodDisplay("Int32 <= Int32", "( {0} <= {1} )")]
		public static bool LessOrEqual(int a, int b)
		{
			return a <= b;
		}
		[LogicSystemMethodDisplay("Int32 >= Int32", "( {0} >= {1} )")]
		public static bool GreaterOrEqual(int a, int b)
		{
			return a >= b;
		}
		[LogicSystemMethodDisplay("Abs( Int32 )", "Abs( {0} )")]
		public static int Absolute(int a)
		{
			return Math.Abs(a);
		}
		[LogicSystemMethodDisplay("Min( Int32, Int32 )", "Min( {0} >= {1} )")]
		public static int Minimum(int a, int b)
		{
			return Math.Min(a, b);
		}
		[LogicSystemMethodDisplay("Max( Int32, Int32 )", "Max( {0} >= {1} )")]
		public static int Maximum(int a, int b)
		{
			return Math.Max(a, b);
		}
	}
}
