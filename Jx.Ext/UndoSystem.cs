using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public class UndoSystem
    {
        public abstract class Action
        {
            protected internal abstract void DoUndo();
            protected internal abstract void DoRedo();
            protected internal abstract void Destroy();
        }
        public delegate void ActionDelegate(Action action);
        private static UndoSystem instance;
        private int maxLevel;
        private List<Action> redoActions = new List<Action>();
        private List<Action> undoActions = new List<Action>();  

        public event EventHandler ChangeActionLists;
        public event EventHandler ClearEvent;
        public event ActionDelegate ActionUndo;
        public event ActionDelegate ActionRedo;
        public event ActionDelegate ActionDestroy;

        public static UndoSystem Instance
        {
            get { return instance; }
        }

        public static void Init(int maxLevel)
        {
            Trace.Assert(instance == null);
            instance = new UndoSystem(maxLevel);
        }
        public static void Shutdown()
        {
            if (instance != null)
            {
                instance.Clear();
                instance = null;
            }
        }

        internal UndoSystem(int maxLevel)
        {
            this.maxLevel = maxLevel;
        }

        public void Clear()
        {
            bool flag = undoActions.Count != 0 || this.redoActions.Count != 0;
            foreach (Action current in redoActions)
            {
                current.Destroy();
                if (ActionDestroy != null)
                    ActionDestroy(current);
            }
            redoActions.Clear();
            foreach (Action current2 in this.undoActions)
            {
                current2.Destroy();
                if (ActionDestroy != null)
                    ActionDestroy(current2);
            }
            undoActions.Clear();
            if (flag && this.ChangeActionLists != null)
                ChangeActionLists(this, EventArgs.Empty);
            if (ClearEvent != null)
                ClearEvent(this, EventArgs.Empty);
        }

        public void CommitAction(Action action)
        {
            foreach (Action current in redoActions)
            {
                current.Destroy();
                if (ActionDestroy != null)
                    ActionDestroy(current);
            }
            redoActions.Clear();
            if (undoActions.Count + 1 >= this.maxLevel)
            {
                undoActions[0].Destroy();
                if (ActionDestroy != null)
                    ActionDestroy(undoActions[0]);
                undoActions.RemoveAt(0);
            }
            undoActions.Add(action);
            if (ChangeActionLists != null)
                ChangeActionLists(this, EventArgs.Empty);
        }

        public Action GetTopUndoAction()
        {
            if (undoActions.Count == 0)
                return null;
            return undoActions[this.undoActions.Count - 1];
        }

        public Action GetTopRedoAction()
        {
            if (redoActions.Count == 0)
                return null;
            
            return redoActions[redoActions.Count - 1];
        }

        public void DoUndo()
        {
            if (undoActions.Count == 0)
                return;

            Action action = undoActions[undoActions.Count - 1];
            this.undoActions.RemoveAt(undoActions.Count - 1);
            action.DoUndo();
            if (ActionUndo != null)
                ActionUndo(action);

            if (redoActions.Count + 1 >= maxLevel)
            {
                redoActions[0].Destroy();
                if (ActionDestroy != null)
                    ActionDestroy(redoActions[0]);
                
                redoActions.RemoveAt(0);
            }
            redoActions.Add(action);
            if (ChangeActionLists != null)
                ChangeActionLists(this, EventArgs.Empty);
        }

        public void DoRedo()
        {
            if (redoActions.Count == 0)
            {
                return;
            }
            Action action = redoActions[redoActions.Count - 1];
            this.redoActions.RemoveAt(redoActions.Count - 1);
            action.DoRedo();
            if (ActionRedo != null)
                ActionRedo(action);

            if (undoActions.Count + 1 >= maxLevel)
            {
                this.undoActions[0].Destroy();
                if (ActionDestroy != null)
                    ActionDestroy(undoActions[0]);
                
                undoActions.RemoveAt(0);
            }
            undoActions.Add(action);
            if (ChangeActionLists != null)
                ChangeActionLists(this, EventArgs.Empty);
        }

        public string[] DumpDebugToLines()
        {
            List<string> list = new List<string>();
            list.Add("UndoSystem");
            list.Add("");
            list.Add("Undo actions:");
            for (int i = 0; i < this.undoActions.Count; i++)
            {
                list.Add(this.undoActions[i].ToString());
            }
            list.Add("");
            list.Add("Redo actions:");
            for (int j = this.redoActions.Count - 1; j >= 0; j--)
            {
                list.Add(this.redoActions[j].ToString());
            }
            return list.ToArray();
        }
    }

}
