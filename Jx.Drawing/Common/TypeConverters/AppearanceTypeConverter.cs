using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Visualizes the Appearance object in a PropertyGrid control.
    /// </summary>
    public class AppearanceTypeConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type.         /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A Type that represents the type from which to convert.</param>
        /// <returns>true if the AxHost.StateConverter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Appearance))
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
            if (destinationType == typeof(System.String) && value is Appearance)
            {
                Appearance appearance = (Appearance)value;

                return Jx.Drawing.Properties.Resources.ShapeAppearance;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
