using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Jx.EntitySystem
{
	public class EntityTypePropertyDescriptor : PropertyDescriptor, _IWrappedPropertyDescriptor
	{
		public delegate void ValueChangeDelegate();
		private EntityType entityType;
		private PropertyInfo propertyInfo;
        public event ValueChangeDelegate ValueChange;

		public override Type ComponentType
		{
			get
			{
				return this.entityType.GetType();
			}
		}
		public override Type PropertyType
		{
			get
			{
				return this.propertyInfo.PropertyType;
			}
		}
		public override bool IsReadOnly
		{
			get
			{
				return !this.propertyInfo.CanWrite;
			}
		}
		public EntityTypePropertyDescriptor(EntityType entityType, PropertyInfo property, Attribute[] attrs) 
            : base(property.Name, attrs)
		{
			this.entityType = entityType;
			this.propertyInfo = property;
		}

		public override object GetValue(object component)
		{
			return this.propertyInfo.GetValue(this.entityType, null);
		}

		public override void SetValue(object component, object value)
		{
			this.propertyInfo.SetValue(this.entityType, value, null);
			if (this.ValueChange != null)
			{
				this.ValueChange();
			}
		}

        public override string DisplayName
        {
            get
            {
                if( propertyInfo != null )
                {
                    NameAttribute attrFound = propertyInfo.GetCustomAttribute<NameAttribute>();
                    if (attrFound != null && !string.IsNullOrEmpty(attrFound.Name))
                        return attrFound.Name;
                }
                return base.DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                if( propertyInfo != null )
                {
                    NameAttribute attrFound = propertyInfo.DeclaringType.GetCustomAttribute<NameAttribute>();
                    if (attrFound != null && !string.IsNullOrEmpty(attrFound.Name))
                    {
                        string categoryInfo = string.Format("{0} ({1})", attrFound.Name, base.Category);
                        return categoryInfo;
                    } 
                }
                return base.Category;
            }
        }

        public override bool CanResetValue(object component)
		{
			DefaultValueAttribute[] array = (DefaultValueAttribute[])this.propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
			return array.Length != 0 && !object.Equals(array[0].Value, this.GetValue(component));
		}

		public override void ResetValue(object component)
		{
			DefaultValueAttribute[] array = (DefaultValueAttribute[])this.propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
			if (array.Length != 0 && !object.Equals(array[0].Value, this.GetValue(component)))
			{
				this.SetValue(component, array[0].Value);
			}
		}
		public override bool ShouldSerializeValue(object component)
		{
			DefaultValueAttribute[] array = (DefaultValueAttribute[])this.propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
			return array.Length == 0 || !object.Equals(array[0].Value, this.GetValue(component));
		}

		public object GetWrappedOwner()
		{
			return this.entityType;
		}

		public PropertyInfo GetWrappedProperty()
		{
			return this.propertyInfo;
		}
	}
}
