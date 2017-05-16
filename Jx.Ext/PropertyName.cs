using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Linq.Expressions;

namespace Jx.Ext
{
    /// <summary>
    /// <para>Returns the name of the property using lambda expression.</para>
    /// <para>URL: https://www.codeproject.com/Articles/251886/Using-Property-Name-Without-Typos</para> 
    /// </summary>
    public sealed class PropertyName
    {
        /// <summary>
        /// Error message in case lambda expression does not contain a property.
        /// </summary>
        private const string _ErrorMessage = "Expression '{0}' does not contain a property.";

        /// <summary>
        /// Returns the name of the property using lambda expression.
        /// </summary>
        /// <typeparam name="T">Type containing the property.</typeparam>
        /// <param name="propertyExpression">
        /// Expression containing the property.</param>
        /// <returns>The name of the property.</returns>
        public static string For<T>(Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            var body = propertyExpression.Body;
            return GetPropertyName(body);
        }

        /// <summary>
        /// Returns the name of the property using lambda expression.
        /// </summary>
        /// <param name="propertyExpression">Expression 
        ///           containing the property.</param>
        /// <returns>The name of the property.</returns>
        public static string For(Expression<Func<object>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            var body = propertyExpression.Body;
            return GetPropertyName(body);
        }

        /// <summary>
        /// Returns the name of the property using lambda expression.
        /// </summary>
        /// <param name="propertyExpression">
        /// Expression containing the property.</param>
        /// <param name="nested">Is it a recurrent invocation?</param>
        /// <returns>The name of the property.</returns>
        private static string GetPropertyName(
                Expression propertyExpression, bool nested = false)
        {
            MemberExpression memberExpression;

            if (propertyExpression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)propertyExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = propertyExpression as MemberExpression;
            }

            if (memberExpression == null)
            {
                if (nested) return string.Empty;
                throw new ArgumentException(
                  string.Format(_ErrorMessage, propertyExpression),
                  "propertyExpression");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                if (nested) return string.Empty;
                throw new ArgumentException(
                  string.Format(_ErrorMessage, propertyExpression),
                  "propertyExpression");
            }

            return (memberExpression.Expression != null &&
               memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                 ? GetPropertyName(memberExpression.Expression, true) +
                 propertyInfo.Name : propertyInfo.Name +
                 (nested ? "." : string.Empty);
        }
    }
}
