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
            private object eX;
            private PropertyInfo ex;
            private object eY;
            public object Obj
            {
                get
                {
                    return this.eX;
                }
                set
                {
                    this.eX = value;
                }
            }
            public PropertyInfo Property
            {
                get
                {
                    return this.ex;
                }
                set
                {
                    this.ex = value;
                }
            }
            public object RestoreValue
            {
                get
                {
                    return this.eY;
                }
                set
                {
                    this.eY = value;
                }
            }
            public Item(object obj, PropertyInfo property, object restoreValue)
            {
                this.eX = obj;
                this.ex = property;
                this.eY = restoreValue;
            }
        }
        private UndoObjectsPropertyChangeAction.Item[] eT;
        public UndoObjectsPropertyChangeAction.Item[] Items
        {
            get
            {
                return this.eT;
            }
        }
        public UndoObjectsPropertyChangeAction(UndoObjectsPropertyChangeAction.Item[] items)
        {
            this.eT = items;
        }
        protected internal override void DoUndo()
        {
            for (int i = 0; i < this.eT.Length; i++)
            {
                UndoObjectsPropertyChangeAction.Item item = this.eT[i];
                object value = item.Property.GetValue(item.Obj, null);
                item.Property.SetValue(item.Obj, item.RestoreValue, null);
                item.RestoreValue = value;
            }
        }
        protected internal override void DoRedo()
        {
            this.DoUndo();
        }
        protected internal override void Destroy()
        {
        }
        public override string ToString()
        {
            return string.Format("Property Value: Items: {0}", this.eT.Length);
        }
    }

}
