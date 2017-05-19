using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jx.Ext
{
    public static class ReflectionUtil
    {
        /// <summary>
        /// 获得成员上标记的DefaultValue缺省值
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this MemberInfo member)
        {
            if (member == null)
                return null;

            DefaultValueAttribute defaultValueAttribute = member.GetCustomAttribute<DefaultValueAttribute>(true);
            if (defaultValueAttribute == null)
                return null;

            return defaultValueAttribute.Value;
        }
    }
}
