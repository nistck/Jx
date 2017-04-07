using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel; 

namespace Jx
{
    public class JxObject : INotifyPropertyChanged, INotifyCollectionChanged,  ICustomTypeDescriptor
    {
        #region INotifyPropertyChanged 事件
        /// <summary>
        /// 属性值变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region INotifyCollectionChanged 事件
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region ICustomTypeDescriptor
        #endregion

        #region INotifyPropertyChanged 方法
        protected void OnPropertyChanged(string property, object old_value, object new_value)
        {
            if (PropertyChanged == null)
                return;
                        
            if (old_value == null && new_value == null)
                return;

            if (old_value != null && !old_value.Equals(new_value))
            {
                OnPropertyChanged(property);
                return;
            } 

            if (new_value != null && !new_value.Equals(old_value))
            {
                OnPropertyChanged(property);
                return;
            }
        }

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        #region INotifyCollectionChanged 方法
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged == null)
                return;
            CollectionChanged(this, e);
        }
        #endregion

        #region ICustomTypeDescriptor
        public virtual string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public virtual AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public virtual string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public virtual TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public virtual object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public virtual object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public virtual PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(this, true);
            PropertyDescriptorCollection r = new PropertyDescriptorCollection(null);

            foreach (PropertyDescriptor pd in pds)
            {
                JxPropertyAttribute attr = pd.Attributes.OfType<JxPropertyAttribute>().FirstOrDefault();
                string name = pd.Name;
                if (attr != null)
                    name = attr.Name;
                JxPropertyDescriptor bpd = new JxPropertyDescriptor(name, pd);
                r.Add(bpd);
            }

            return r;
        }

        [AttributeUsage(AttributeTargets.Property)]
        protected class JxPropertyAttribute : Attribute
        {

            public JxPropertyAttribute(string name)
            {
                this.Name = name;
            }

            public JxPropertyAttribute(
                string name, string description, string category,
                Type type, bool readOnly, bool expandable)
            {
                this.Name = name;
                this.Description = description;
                this.Category = category;
                this.Type = type;
                this.ReadOnly = readOnly;
                this.Expandable = expandable;
            }

            public string Name { get; private set; }
            public string Description { get; private set; }
            public string Category { get; private set; }
            public Type Type { get; private set; }
            public bool ReadOnly { get; private set; }
            public bool Expandable { get; private set; }
        }

        class JxPropertyDescriptor : PropertyDescriptor
        {
            private PropertyDescriptor descriptor;
            private string displayName;
            private string m_Desc = null;
            private bool m_DescSet = false;

            public JxPropertyDescriptor(PropertyDescriptor descriptor)
                : base(descriptor.Name, null)
            {
                this.displayName = descriptor.DisplayName;
                this.descriptor = descriptor;
            }

            public JxPropertyDescriptor(string displayName, PropertyDescriptor descriptor)
                : base(descriptor.Name, null)
            {
                this.displayName = displayName;
                this.descriptor = descriptor;
            }

            public JxPropertyDescriptor(string displayName, string desc, PropertyDescriptor descriptor)
                : base(descriptor.Name, null)
            {
                this.displayName = displayName;
                this.descriptor = descriptor;
                this.m_Desc = desc ?? descriptor.Description;
                this.m_DescSet = true;
            }

            public override AttributeCollection Attributes
            {
                get
                {
                    return descriptor.Attributes;
                }
            }

            public override bool CanResetValue(object component)
            {
                bool canResetValue = descriptor.CanResetValue(component);
                return canResetValue;
            }

            public override Type ComponentType
            {
                get { return descriptor.ComponentType; }
            }

            public override string DisplayName
            {
                get { return displayName; }
            }

            public override string Description
            {
                get
                {
                    if (m_DescSet)
                        return m_Desc;
                    return descriptor.Description;
                }
            }

            public override object GetValue(object component)
            {
                return descriptor.GetValue(component);
            }

            public override bool IsReadOnly
            {
                get { return descriptor.IsReadOnly; }
            }

            public override string Name
            {
                get { return descriptor.Name; }
            }

            public override Type PropertyType
            {
                get { return descriptor.PropertyType; }
            }

            public override void ResetValue(object component)
            {
                descriptor.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                descriptor.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return descriptor.ShouldSerializeValue(component);
            }

            public override void AddValueChanged(object component, EventHandler handler)
            {
                descriptor.AddValueChanged(component, handler);
            }

            public override string Category
            {
                get
                {
                    return descriptor.Category;
                }
            }

            public override TypeConverter Converter
            {
                get
                {
                    return descriptor.Converter;
                }
            }
        }
        #endregion
    }
}
