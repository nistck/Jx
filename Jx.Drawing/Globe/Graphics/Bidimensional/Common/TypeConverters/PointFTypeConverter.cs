using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Visualizes the PointF value in a PropertyGrid control.
    /// </summary>
    public class PointFTypeConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type.         /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A Type that represents the type from which to convert.</param>
        /// <returns>true if the AxHost.StateConverter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.Drawing.PointF))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A CultureInfo. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
        /// <param name="value">The Object to convert.</param>
        /// <param name="destinationType">The Type to convert the value parameter to.</param>
        /// <returns>An Object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is System.Drawing.PointF)
            {
                System.Drawing.PointF point = (System.Drawing.PointF)value;

                return point.X + "; " + point.Y;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Determinates whether changing a value on this object should require a call to the CreateInstance method to create a new value.
        /// </summary>
        /// <param name="context">A type descriptor through which additional context can be provided.</param>
        /// <returns>This method returns true if the CreateInstance object should be called when a change is made to one or more properties of this object; otherwise, false.</returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Creates a instance of the type that this TypeConverter is associated with, using the specified context, given a set of property values for the object.
        /// </summary>
        /// <param name="context">A ITypeDescriptorContext that provides a format context.</param>
        /// <param name="propertyValues">An IDictionary of new property values.</param>
        /// <returns>An object rapresenting the given IDictionary, or a null reference (Nothing in Visual Basic) if the object cannot be created. This method always returns a null reference (Nothing in Visual Basic).</returns>
        public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
        {
            if (propertyValues != null)
                return new System.Drawing.PointF((float)propertyValues["X"], (float)propertyValues["Y"]);

            return null;
        }
    }
}
