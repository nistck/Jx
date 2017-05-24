using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

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

        public static MemberInfo GetMember<T>(Expression<Func<T>> memberExpression)
        {
            if (memberExpression == null)
                return null;
            MemberExpression body = memberExpression.Body as MemberExpression;
            if (body == null )
                return null;

            MemberInfo m = body.Member;
            return m;
        }

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            if (memberExpression == null)
                return null;
            MemberExpression body = memberExpression.Body as MemberExpression;
            if (body == null || body.Member == null)
                return null;

            string property = body.Member.Name;
            return property;
        }

        public static bool SetMemberValue<T>(Expression<Func<T>> memberExpression, T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, GetMemberValue(memberExpression)))
                return false;

            MemberExpression body = memberExpression.Body as MemberExpression;
            if (body == null)
                return false;

            string property = body.Member.Name;

            object source = null;
            ConstantExpression exp = body.Expression as ConstantExpression;
            if (exp != null)
                source = exp.Value;


            Type sourceType = source.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // IsProperty? 
            PropertyInfo propertyInfo = sourceType.GetProperties(bindingFlags)
                .Where(_property => _property.Name == property).FirstOrDefault();
            if (propertyInfo != null)
            {
                try
                {
                    propertyInfo.GetSetMethod().Invoke(source, new object[] { value });
                    return true;
                }
                catch (Exception) { }
            }

            // IsField?
            FieldInfo fieldInfo = sourceType.GetFields(bindingFlags)
                .Where(_field => _field.Name == property).FirstOrDefault();
            if (fieldInfo != null)
            {
                try
                {
                    fieldInfo.SetValue(source, value);
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        public static T GetMemberValue<T>(Expression<Func<T>> memberExpression)
        {
            object value = null;
            if (GetMemberValue<T>(memberExpression, out value))
            {
                if (value == null)
                    return default(T);

                if (typeof(T).IsAssignableFrom(value.GetType()))
                    return (T)value;
                return default(T);
            }
            return default(T);
        }

        /// <summary>
        /// 获得指定表达式的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberExpression"></param>
        /// <param name="value"></param>
        /// <returns>true, <paramref name="value"/>有效; false, <paramref name="value"/>无效；</returns>
        public static bool GetMemberValue<T>(Expression<Func<T>> memberExpression, out object value)
        {
            value = null;

            MemberExpression body = memberExpression.Body as MemberExpression;
            if (body == null)
                return false;

            string property = body.Member.Name;

            object source = null;
            ConstantExpression exp = body.Expression as ConstantExpression;
            if (exp != null)
                source = exp.Value;

            if (source == null)
                return false;

            bool bx = GetMemberValue(source, property, out value);
            return bx;
        }

        public static bool GetMemberValue<TResult>(object source, string member, out TResult result)
        {
            result = default(TResult);
            if (source == null || string.IsNullOrEmpty(member))
                return false;

            object value = null;
            Type sourceType = source.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // IsProperty? 
            PropertyInfo propertyInfo = sourceType.GetProperties(bindingFlags)
                .Where(_property => _property.Name == member).FirstOrDefault();
            if (propertyInfo != null)
            {
                try
                {
                    value = propertyInfo.GetGetMethod().Invoke(source, new object[] { });
                    return Cast(value, out result);
                }
                catch (Exception) { }
            }

            // IsField?
            FieldInfo fieldInfo = sourceType.GetFields(bindingFlags)
                .Where(_field => _field.Name == member).FirstOrDefault();
            if (fieldInfo != null)
            {
                try
                {
                    value = fieldInfo.GetValue(source);
                    return Cast(value, out result);
                }
                catch (Exception) { }
            }
            return false;
        }

        public static bool Cast<T>(object o, out T result)
        {
            result = default(T);
            if (o == null)
                return false;

            if (typeof(T).IsAssignableFrom(o.GetType()))
            {
                result = (T)o;
                return true;
            }
            return false;
        }

        public static T Cast<T>(object o)
        {
            T t = default(T);
            Cast(o, out t);
            return t;
        }
    }
}
