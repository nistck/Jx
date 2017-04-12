using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
 
using Jx.Ext; 

namespace Jx.EntitySystem
{
	public class EntityTypeCustomTypeDescriptor : ExtendedFunctionalityDescriptorCustomTypeDescriptor, _IWrappedCustomTypeDescriptor
	{
		public delegate void ValueChangeDelegate();
		private EntityType entityType;
		private EntityTypeCustomTypeDescriptor.ValueChangeDelegate aar;
		private Type aaS;
		public event EntityTypeCustomTypeDescriptor.ValueChangeDelegate ValueChange
		{
			add
			{
				EntityTypeCustomTypeDescriptor.ValueChangeDelegate valueChangeDelegate = this.aar;
				EntityTypeCustomTypeDescriptor.ValueChangeDelegate valueChangeDelegate2;
				do
				{
					valueChangeDelegate2 = valueChangeDelegate;
					EntityTypeCustomTypeDescriptor.ValueChangeDelegate value2 = (EntityTypeCustomTypeDescriptor.ValueChangeDelegate)Delegate.Combine(valueChangeDelegate2, value);
					valueChangeDelegate = Interlocked.CompareExchange<EntityTypeCustomTypeDescriptor.ValueChangeDelegate>(ref this.aar, value2, valueChangeDelegate2);
				}
				while (valueChangeDelegate != valueChangeDelegate2);
			}
			remove
			{
				EntityTypeCustomTypeDescriptor.ValueChangeDelegate valueChangeDelegate = this.aar;
				EntityTypeCustomTypeDescriptor.ValueChangeDelegate valueChangeDelegate2;
				do
				{
					valueChangeDelegate2 = valueChangeDelegate;
					EntityTypeCustomTypeDescriptor.ValueChangeDelegate value2 = (EntityTypeCustomTypeDescriptor.ValueChangeDelegate)Delegate.Remove(valueChangeDelegate2, value);
					valueChangeDelegate = Interlocked.CompareExchange<EntityTypeCustomTypeDescriptor.ValueChangeDelegate>(ref this.aar, value2, valueChangeDelegate2);
				}
				while (valueChangeDelegate != valueChangeDelegate2);
			}
		}
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
			this.aaS = extendedFunctionalityDescriptorType;
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return this.GetProperties();
		}
		public override PropertyDescriptorCollection GetProperties()
		{
			EntityTypePropertyDescriptor.ValueChangeDelegate valueChangeDelegate = null;
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			Type type = this.entityType.GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
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
					string text = propertyInfo.DeclaringType.Name;
					text = text.Substring(0, text.Length - 4);
					list.Add(new CategoryAttribute(text));
				}
				EntityTypePropertyDescriptor entityTypePropertyDescriptor = new EntityTypePropertyDescriptor(this.entityType, propertyInfo, list.ToArray());
				EntityTypePropertyDescriptor arg_107_0 = entityTypePropertyDescriptor;
				if (valueChangeDelegate == null)
				{
					valueChangeDelegate = new EntityTypePropertyDescriptor.ValueChangeDelegate(this.A);
				}
				arg_107_0.ValueChange += valueChangeDelegate;
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
			if (this.aaS != null)
			{
				return this.aaS;
			}
			return base.GetExtendedFunctionalityDescriptorType(obj);
		}
		public object GetWrapperOwner()
		{
			return this.entityType;
		}
		[CompilerGenerated]
		private void A()
		{
			if (this.aar != null)
			{
				this.aar();
			}
		}
	}
}
