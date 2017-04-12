using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Jx.EntitySystem
{
	public class EntityTypePropertyDescriptor : PropertyDescriptor, _IWrappedPropertyDescriptor
	{
		public delegate void ValueChangeDelegate();
		private EntityType aap;
		private PropertyInfo aaQ;
		private EntityTypePropertyDescriptor.ValueChangeDelegate aaq;
		public event EntityTypePropertyDescriptor.ValueChangeDelegate ValueChange
		{
			add
			{
				EntityTypePropertyDescriptor.ValueChangeDelegate valueChangeDelegate = this.aaq;
				EntityTypePropertyDescriptor.ValueChangeDelegate valueChangeDelegate2;
				do
				{
					valueChangeDelegate2 = valueChangeDelegate;
					EntityTypePropertyDescriptor.ValueChangeDelegate value2 = (EntityTypePropertyDescriptor.ValueChangeDelegate)Delegate.Combine(valueChangeDelegate2, value);
					valueChangeDelegate = Interlocked.CompareExchange<EntityTypePropertyDescriptor.ValueChangeDelegate>(ref this.aaq, value2, valueChangeDelegate2);
				}
				while (valueChangeDelegate != valueChangeDelegate2);
			}
			remove
			{
				EntityTypePropertyDescriptor.ValueChangeDelegate valueChangeDelegate = this.aaq;
				EntityTypePropertyDescriptor.ValueChangeDelegate valueChangeDelegate2;
				do
				{
					valueChangeDelegate2 = valueChangeDelegate;
					EntityTypePropertyDescriptor.ValueChangeDelegate value2 = (EntityTypePropertyDescriptor.ValueChangeDelegate)Delegate.Remove(valueChangeDelegate2, value);
					valueChangeDelegate = Interlocked.CompareExchange<EntityTypePropertyDescriptor.ValueChangeDelegate>(ref this.aaq, value2, valueChangeDelegate2);
				}
				while (valueChangeDelegate != valueChangeDelegate2);
			}
		}
		public override Type ComponentType
		{
			get
			{
				return this.aap.GetType();
			}
		}
		public override Type PropertyType
		{
			get
			{
				return this.aaQ.PropertyType;
			}
		}
		public override bool IsReadOnly
		{
			get
			{
				return !this.aaQ.CanWrite;
			}
		}
		public EntityTypePropertyDescriptor(EntityType entityType, PropertyInfo property, Attribute[] attrs) : base(property.Name, attrs)
		{
			this.aap = entityType;
			this.aaQ = property;
		}
		public override object GetValue(object component)
		{
			return this.aaQ.GetValue(this.aap, null);
		}
		public override void SetValue(object component, object value)
		{
			this.aaQ.SetValue(this.aap, value, null);
			if (this.aaq != null)
			{
				this.aaq();
			}
		}
		public override bool CanResetValue(object component)
		{
			DefaultValueAttribute[] array = (DefaultValueAttribute[])this.aaQ.GetCustomAttributes(typeof(DefaultValueAttribute), true);
			return array.Length != 0 && !object.Equals(array[0].Value, this.GetValue(component));
		}
		public override void ResetValue(object component)
		{
			DefaultValueAttribute[] array = (DefaultValueAttribute[])this.aaQ.GetCustomAttributes(typeof(DefaultValueAttribute), true);
			if (array.Length != 0 && !object.Equals(array[0].Value, this.GetValue(component)))
			{
				this.SetValue(component, array[0].Value);
			}
		}
		public override bool ShouldSerializeValue(object component)
		{
			DefaultValueAttribute[] array = (DefaultValueAttribute[])this.aaQ.GetCustomAttributes(typeof(DefaultValueAttribute), true);
			return array.Length == 0 || !object.Equals(array[0].Value, this.GetValue(component));
		}
		public object GetWrappedOwner()
		{
			return this.aap;
		}
		public PropertyInfo GetWrappedProperty()
		{
			return this.aaQ;
		}
	}
}
