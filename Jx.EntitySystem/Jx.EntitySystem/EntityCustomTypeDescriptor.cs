using Jx.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Jx.EntitySystem
{
	public class EntityCustomTypeDescriptor : ExtendedFunctionalityDescriptorCustomTypeDescriptor, _IWrappedCustomTypeDescriptor
	{
		private Entity aaa;
		private PropertyDescriptorCollection aaB;
		public Entity Entity
		{
			get
			{
				return this.aaa;
			}
		}
		public EntityCustomTypeDescriptor(Entity entity)
		{
			this.aaa = entity;
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return this.GetProperties();
		}
		public override PropertyDescriptorCollection GetProperties()
		{
			if (this.aaB == null)
			{
				this.aaB = new PropertyDescriptorCollection(null);
				Type type = this.aaa.GetType();
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				PropertyInfo[] array = properties;
				for (int i = 0; i < array.Length; i++)
				{
					PropertyInfo propertyInfo = array[i];
					if (!(propertyInfo.Name == "Type") || !(propertyInfo.DeclaringType != typeof(Entity)))
					{
						List<Attribute> list = new List<Attribute>();
						object[] customAttributes = propertyInfo.GetCustomAttributes(true);
						object[] array2 = customAttributes;
						for (int j = 0; j < array2.Length; j++)
						{
							object obj = array2[j];
							list.Add((Attribute)obj);
						}
						bool flag = false;
						foreach (Attribute current in list)
						{
							if (current is CategoryAttribute)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							string format = ToolsLocalization.Translate("EntityCustomTypeDescriptor", "class {0}");
							string category = string.Format(format, propertyInfo.DeclaringType.Name);
							list.Add(new CategoryAttribute(category));
						}
						this.aaB.Add(new EntityPropertyDescriptor(this.aaa, propertyInfo, list.ToArray()));
					}
				}
			}
			return this.aaB;
		}
		public override object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public override object GetExtendedFunctionalityDescriptorObject()
		{
			return this.aaa;
		}

		public EntityPropertyDescriptor GetProperty(string propertyName)
		{
			foreach (PropertyDescriptor propertyDescriptor in this.aaB)
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
			return this.aaa;
		}
	}
}
