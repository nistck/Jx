using System;
using System.Reflection;

#if WINDOWS_STORE
using TP = System.Reflection.TypeInfo;
#else
using TP = System.Type;
#endif

using TCU = Jx.Serialization.Json.TypeCoercionUtility;

namespace Jx.Serialization.Json
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class JsonMemberAttribute : Attribute
	{
		public string MemberName { get; set; }

		public JsonMemberAttribute()
		{
			MemberName = null;
		}

		public JsonMemberAttribute(string memberName)
		{
			MemberName = memberName;
		}

		/// <summary>
		/// Gets the name specified for use in serialization.
		/// </summary>
		/// <returns></returns>
		public static string GetMemberName(object value)
		{
			if(value == null)
			{
				return null;
			}

			Type type = value.GetType();
			MemberInfo memberInfo = null;

			if(TCU.GetTypeInfo(type).IsEnum)
			{
				string name = Enum.GetName(type, value);
				if(String.IsNullOrEmpty(name))
				{
					return null;
				}
				memberInfo = TCU.GetTypeInfo(type).GetField(name);
			}
			else
			{
				memberInfo = value as MemberInfo;
			}

			if(MemberInfo.Equals(memberInfo, null))
			{
				throw new ArgumentException();
			}

#if WINDOWS_STORE
			BTPropertyAttribute attribute = memberInfo.GetCustomAttribute<BTPropertyAttribute>(true);
#else
			JsonMemberAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(JsonMemberAttribute)) as JsonMemberAttribute;
#endif
			return attribute != null ? attribute.MemberName : null;
		}
	}
}