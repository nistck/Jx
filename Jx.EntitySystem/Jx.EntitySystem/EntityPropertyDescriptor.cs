using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Jx.EntitySystem
{
	public class EntityPropertyDescriptor : PropertyDescriptor, _IWrappedPropertyDescriptor
	{
		private Entity entity;
		private PropertyInfo propertyInfo;

		public override Type ComponentType
		{
			get
			{
				return entity.GetType();
			}
		}

		public override Type PropertyType
		{
			get
			{
				return propertyInfo.PropertyType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return !propertyInfo.CanWrite;
			}
		}

		public Entity Entity
		{
			get
			{
				return this.entity;
			}
		}

		public EntityPropertyDescriptor(Entity entity, PropertyInfo property, Attribute[] attrs) : base(property.Name, attrs)
		{
			this.entity = entity;
			this.propertyInfo = property;
		}

		public override object GetValue(object component)
		{
			return this.propertyInfo.GetValue(this.entity, null);
		}
		public override void SetValue(object component, object value)
		{
			this.propertyInfo.SetValue(this.entity, value, null);
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
			return this.entity;
		}

		public PropertyInfo GetWrappedProperty()
		{
			return this.propertyInfo;
		}

        public override string DisplayName
        {
            get
            {
                if (propertyInfo != null)
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
                List<Type> types = new List<Type>();

                Type typeOwner = propertyInfo.DeclaringType;
                if (propertyInfo != null)
                    types.Add(typeOwner);

                if( Entity != null && Entity.Type != null )
                {
                    if (Entity.Type.ClassInfo.EntityClassType == typeOwner)
                        types.Add(Entity.Type.GetType());
                } 

                string categoryInfo = null;
                while( types.Count > 0 )
                {
                    Type type = types[0];
                    types.RemoveAt(0);

                    NameAttribute attrFound = type.GetCustomAttribute<NameAttribute>();
                    if (attrFound != null && !string.IsNullOrEmpty(attrFound.Name) )
                    {
                        categoryInfo = string.Format("{0} ({1})", attrFound.Name, base.Category);
                        return categoryInfo;
                    }
                }
                 
                return base.Category;
            }
        }
    }
}
