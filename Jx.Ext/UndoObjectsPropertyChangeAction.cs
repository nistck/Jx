using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace Jx.Ext
{
    public class UndoObjectsPropertyChangeAction : UndoSystem.Action
    {
        public class Item
        { 
            private object eY;
            public object Target { get; set; }
            public PropertyInfo Property { get; set; }
            public object RestoreValue { get; set; }
            public Item(object obj, PropertyInfo property, object restoreValue)
            {
                this.Target = obj;
                this.Property = property;
                this.RestoreValue = restoreValue;
            }
        }

        private readonly List<Item> items = new List<Item>();
        public List<Item> Items
        {
            get {
                List<Item> result = new List<Item>();
                result.AddRange(items);
                return result; 
            }
        }
        public UndoObjectsPropertyChangeAction(params Item[] items)
        {
            if (items != null)
                this.items.AddRange(items.ToList());
        }
        protected internal override void DoUndo()
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                object value = item.Property.GetValue(item.Target, null);
                item.Property.SetValue(item.Target, item.RestoreValue, null);
                item.RestoreValue = value;
            }
        }
        protected internal override void DoRedo()
        {
            DoUndo();
        }
        protected internal override void Destroy()
        {
        }
        public override string ToString()
        {
            return string.Format("Property Value: Items: {0}", items.Count);
        }
    }

}
