using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Reflection;

namespace Jx
{
    public class CollectionItemPropertyDescriptor : PropertyDescriptor
    {
        private object Ed;
        private int EE;
        public override Type ComponentType
        {
            get
            {
                return this.Ed.GetType();
            }
        }
        public override Type PropertyType
        {
            get
            {
                Type[] genericArguments = this.Ed.GetType().GetGenericArguments();
                if (genericArguments.Length == 0)
                {
                    genericArguments = this.Ed.GetType().BaseType.GetGenericArguments();
                    if (genericArguments == null || genericArguments.Length == 0)
                    {
                        Type[] interfaces = this.Ed.GetType().GetInterfaces();
                        for (int i = 0; i < interfaces.Length; i++)
                        {
                            Type type = interfaces[i];
                            genericArguments = type.GetGenericArguments();
                            if (genericArguments != null && genericArguments.Length != 0)
                            {
                                break;
                            }
                        }
                    }
                }
                return genericArguments[0];
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }
        public override TypeConverter Converter
        {
            get
            {
                return new ExpandableObjectConverter();
            }
        }
        public CollectionItemPropertyDescriptor(object list, int index) : base(index.ToString(), null)
        {
            this.Ed = list;
            this.EE = index;
        }
        public override object GetValue(object component)
        {
            Type type = this.Ed.GetType();
            PropertyInfo property = type.GetProperty("Item", new Type[]
            {
                typeof(int)
            });
            object[] index = new object[]
            {
                this.EE
            };
            return property.GetValue(this.Ed, index);
        }
        public override void SetValue(object component, object value)
        {
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override void ResetValue(object component)
        {
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
