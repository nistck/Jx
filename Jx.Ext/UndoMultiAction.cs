using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public class UndoMultiAction : UndoSystem.Action
    {
        private List<UndoSystem.Action> et;
        public List<UndoSystem.Action> Actions
        {
            get
            {
                return this.et;
            }
        }
        public UndoMultiAction()
        {
            this.et = new List<UndoSystem.Action>();
        }
        public UndoMultiAction(ICollection<UndoSystem.Action> actions)
        {
            this.et = new List<UndoSystem.Action>(actions);
        }
        public void AddAction(UndoSystem.Action action)
        {
            this.et.Add(action);
        }
        protected internal override void Destroy()
        {
            for (int i = 0; i < this.et.Count; i++)
            {
                this.et[i].Destroy();
            }
        }
        protected internal override void DoRedo()
        {
            for (int i = 0; i < this.et.Count; i++)
            {
                this.et[i].DoRedo();
            }
        }
        protected internal override void DoUndo()
        {
            for (int i = 0; i < this.et.Count; i++)
            {
                this.et[i].DoUndo();
            }
        }
    }
}
