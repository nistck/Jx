using Jx.FileSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace Jx.EntitySystem
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class EntityExtendedProperties
	{
		private struct AT
		{
			public FieldInfo field;
			public EntityExtendedProperties.FieldSerializeSerializationTypes supportedSerializationTypes;
		}
		[Flags]
		public enum FieldSerializeSerializationTypes
		{
			Map = 1,
			World = 2
		}
		[AttributeUsage(AttributeTargets.Field)]
		public sealed class FieldSerializeAttribute : Attribute
		{
			private string aaW;
			private EntityExtendedProperties.FieldSerializeSerializationTypes aaw = EntityExtendedProperties.FieldSerializeSerializationTypes.Map | EntityExtendedProperties.FieldSerializeSerializationTypes.World;
			public string PropertyName
			{
				get
				{
					return this.aaW;
				}
			}
			public EntityExtendedProperties.FieldSerializeSerializationTypes SupportedSerializationTypes
			{
				get
				{
					return this.aaw;
				}
			}
			public FieldSerializeAttribute()
			{
			}
			public FieldSerializeAttribute(string propertyName, EntityExtendedProperties.FieldSerializeSerializationTypes supportedSerializationTypes)
			{
				this.aaW = propertyName;
				this.aaw = supportedSerializationTypes;
			}
			public FieldSerializeAttribute(string propertyName)
			{
				this.aaW = propertyName;
			}
			public FieldSerializeAttribute(EntityExtendedProperties.FieldSerializeSerializationTypes supportedSerializationTypes)
			{
				this.aaw = supportedSerializationTypes;
			}
		}
		private static Dictionary<Type, List<EntityExtendedProperties.AT>> zs = new Dictionary<Type, List<EntityExtendedProperties.AT>>();
		internal Entity zT;
		[Browsable(false)]
		public Entity Owner
		{
			get
			{
				return this.zT;
			}
		}
		private static List<EntityExtendedProperties.AT> A(Type type)
		{
			List<EntityExtendedProperties.AT> list;
			if (!EntityExtendedProperties.zs.TryGetValue(type, out list))
			{
				list = new List<EntityExtendedProperties.AT>();
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo[] array = fields;
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = array[i];
					EntityExtendedProperties.FieldSerializeAttribute fieldSerializeAttribute = null;
					EntityExtendedProperties.FieldSerializeAttribute[] array2 = (EntityExtendedProperties.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(EntityExtendedProperties.FieldSerializeAttribute), true);
					if (array2.Length != 0)
					{
						fieldSerializeAttribute = array2[0];
					}
					if (fieldSerializeAttribute != null)
					{
						EntityExtendedProperties.AT item = default(EntityExtendedProperties.AT);
						item.field = fieldInfo;
						item.supportedSerializationTypes = fieldSerializeAttribute.SupportedSerializationTypes;
						list.Add(item);
					}
				}
				EntityExtendedProperties.zs.Add(type, list);
			}
			return list;
		}
		protected internal virtual bool OnLoad(TextBlock block)
		{
			string text = this.Owner.Type.Name;
			if (this.Owner.Name != "")
			{
				text += string.Format(" ({0})", this.Owner.Name);
			}
			text = string.Format("Entity: \"{0}\"; ExtendedProperties", text);
			foreach (EntityExtendedProperties.AT current in EntityExtendedProperties.A(base.GetType()))
			{
				if (/*EntitySystemWorld.Instance.isEntityExtendedPropertiesSerializable(current.supportedSerializationTypes) &&*/ !Ci.LoadFieldValue(true, this, current.field, block, text))
				{
					return false;
				}
			}
			return true;
		}
		protected internal virtual void OnSave(TextBlock block)
		{
			string text = this.Owner.Type.Name;
			if (this.Owner.Name != "")
			{
				text += string.Format(" ({0})", this.Owner.Name);
			}
			text = string.Format("Entity: \"{0}\"; ExtendedProperties", text);
			foreach (EntityExtendedProperties.AT current in EntityExtendedProperties.A(base.GetType()))
			{
				if (/*EntitySystemWorld.Instance.isEntityExtendedPropertiesSerializable(current.supportedSerializationTypes) && */
                    !Ci.SaveFieldValue(true, this, current.field, block, null, text))
				{
					break;
				}
			}
		}
		protected internal virtual void OnDeleteSubscribedToDeletionEvent(Entity entity)
		{
		}
		private bool A(PropertyInfo propertyInfo)
		{
			BrowsableAttribute[] array = (BrowsableAttribute[])propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), true);
			BrowsableAttribute[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				BrowsableAttribute browsableAttribute = array2[i];
				if (!browsableAttribute.Browsable)
				{
					return false;
				}
			}
			return true;
		}
		protected internal virtual void OnDestroy()
		{
			PropertyInfo[] properties = base.GetType().GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.CanWrite && typeof(Entity).IsAssignableFrom(propertyInfo.PropertyType) && this.A(propertyInfo))
				{
					propertyInfo.SetValue(this, null, null);
				}
			}
		}
		public override string ToString()
		{
			return base.GetType().Name;
		}
	}
}
