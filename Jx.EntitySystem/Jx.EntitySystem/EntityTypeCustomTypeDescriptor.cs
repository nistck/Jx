using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Linq;
 
using Jx.Ext; 

namespace Jx.EntitySystem
{
	public class EntityTypeCustomTypeDescriptor : ExtendedFunctionalityDescriptorCustomTypeDescriptor, _IWrappedCustomTypeDescriptor
	{
		public delegate void ValueChangeDelegate();
		private EntityType entityType; 
		private Type extendedFunctionalityDescriptorType;

        public event ValueChangeDelegate ValueChange;

		public EntityType EntityType
		{
			get
			{
				return this.entityType;
			}
		}

		public EntityTypeCustomTypeDescriptor(EntityType entityType, Type extendedFunctionalityDescriptorType)
		{
			this.entityType = entityType;
			this.extendedFunctionalityDescriptorType = extendedFunctionalityDescriptorType;
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return this.GetProperties();
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			Type type = this.entityType.GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				List<Attribute> attributes = propertyInfo.GetCustomAttributes(true).OfType<Attribute>().ToList();
				bool categoryFound = attributes.OfType<CategoryAttribute>().Count() > 0;                
				if (!categoryFound)
				{
					string text = propertyInfo.DeclaringType.Name;
					text = text.Substring(0, text.Length - 4);
					attributes.Add(new CategoryAttribute(text));
				}

				EntityTypePropertyDescriptor entityTypePropertyDescriptor = new EntityTypePropertyDescriptor(this.entityType, propertyInfo, attributes.ToArray());
                entityTypePropertyDescriptor.ValueChange += OnValueChange;
				propertyDescriptorCollection.Add(entityTypePropertyDescriptor);
			}
			return propertyDescriptorCollection;
		}

		public override object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public override object GetExtendedFunctionalityDescriptorObject()
		{
			return this.entityType;
		}

		public override Type GetExtendedFunctionalityDescriptorType(object obj)
		{
			if (this.extendedFunctionalityDescriptorType != null)
			{
				return this.extendedFunctionalityDescriptorType;
			}
			return base.GetExtendedFunctionalityDescriptorType(obj);
		}

		public object GetWrapperOwner()
		{
			return this.entityType;
		} 
 
		private void OnValueChange()
		{
			if (this.ValueChange != null)
			{
				this.ValueChange();
			}
		}
	}
}
