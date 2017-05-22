using Jx.Ext;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Jx.EntitySystem
{
	public class EntityCustomTypeDescriptor : ExtendedFunctionalityDescriptorCustomTypeDescriptor, _IWrappedCustomTypeDescriptor
	{
		private Entity entity;
		private PropertyDescriptorCollection propertyDescriptorCollection;
		public Entity Entity
		{
			get
			{
				return this.entity;
			}
		}
		public EntityCustomTypeDescriptor(Entity entity)
		{
			this.entity = entity;
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return this.GetProperties();
		}
		public override PropertyDescriptorCollection GetProperties()
		{
			if (this.propertyDescriptorCollection == null)
			{
				this.propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				Type type = this.entity.GetType();
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); 
				for (int i = 0; i < properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
                    bool propertyIgnore = propertyInfo.Name == "Type" || propertyInfo.DeclaringType == typeof(Entity);
					if (!propertyIgnore)
					{
                        List<Attribute> list = propertyInfo.GetCustomAttributes(true).OfType<Attribute>().ToList();
                        bool categoryFound = list.Where(_attr => _attr is CategoryAttribute).Count() > 0; 
						if (!categoryFound)
						{
							string format = ToolsLocalization.Translate("EntityCustomTypeDescriptor", "class {0}");
							string category = string.Format(format, propertyInfo.DeclaringType.Name);
							list.Add(new CategoryAttribute(category));
						}
						propertyDescriptorCollection.Add(new EntityPropertyDescriptor(entity, propertyInfo, list.ToArray()));
					}
				}
			}
			return this.propertyDescriptorCollection;
		}
		public override object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public override object GetExtendedFunctionalityDescriptorObject()
		{
			return this.entity;
		}

		public EntityPropertyDescriptor GetProperty(string propertyName)
		{
			foreach (PropertyDescriptor propertyDescriptor in this.propertyDescriptorCollection)
			{
				EntityPropertyDescriptor entityPropertyDescriptor = (EntityPropertyDescriptor)propertyDescriptor;
				if (entityPropertyDescriptor.Name == propertyName)
				{
					return entityPropertyDescriptor;
				}
			}
			return null;
		}
		public object GetWrapperOwner()
		{
			return this.entity;
		}
	}
}
