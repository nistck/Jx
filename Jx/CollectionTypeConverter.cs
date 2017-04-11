using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Jx
{
    public class CollectionTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return "(Collection)";
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            Type type = value.GetType();
            PropertyInfo property = type.GetProperty("Count");
            int num = (int)property.GetValue(value, null);
            PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
            for (int i = 0; i < num; i++)
            {
                propertyDescriptorCollection.Add(new CollectionItemPropertyDescriptor(value, i));
            }
            return propertyDescriptorCollection;
        }
    }
}
