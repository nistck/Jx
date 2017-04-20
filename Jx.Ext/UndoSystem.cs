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
        public delegate void ActionDelegate(UndoSystem.Action action);
        private static UndoSystem dT;
        private int dt;
        private List<UndoSystem.Action> dU = new List<UndoSystem.Action>();
        private List<UndoSystem.Action> du = new List<UndoSystem.Action>();
        private EventHandler dV;
        private EventHandler dv;
        private UndoSystem.ActionDelegate dW;
        private UndoSystem.ActionDelegate dw;
        private UndoSystem.ActionDelegate dX;
        public event EventHandler ChangeActionLists
        {
            add
            {
                EventHandler eventHandler = this.dV;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.dV, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            remove
            {
                EventHandler eventHandler = this.dV;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.dV, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }
        public event EventHandler ClearEvent
        {
            add
            {
                EventHandler eventHandler = this.dv;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.dv, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
            remove
            {
                EventHandler eventHandler = this.dv;
                EventHandler eventHandler2;
                do
                {
                    eventHandler2 = eventHandler;
                    EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.dv, value2, eventHandler2);
                }
                while (eventHandler != eventHandler2);
            }
        }
        public event UndoSystem.ActionDelegate ActionUndo
        {
            add
            {
                UndoSystem.ActionDelegate actionDelegate = this.dW;
                UndoSystem.ActionDelegate actionDelegate2;
                do
                {
                    actionDelegate2 = actionDelegate;
                    UndoSystem.ActionDelegate value2 = (UndoSystem.ActionDelegate)Delegate.Combine(actionDelegate2, value);
                    actionDelegate = Interlocked.CompareExchange<UndoSystem.ActionDelegate>(ref this.dW, value2, actionDelegate2);
                }
                while (actionDelegate != actionDelegate2);
            }
            remove
            {
                UndoSystem.ActionDelegate actionDelegate = this.dW;
                UndoSystem.ActionDelegate actionDelegate2;
                do
                {
                    actionDelegate2 = actionDelegate;
                    UndoSystem.ActionDelegate value2 = (UndoSystem.ActionDelegate)Delegate.Remove(actionDelegate2, value);
                    actionDelegate = Interlocked.CompareExchange<UndoSystem.ActionDelegate>(ref this.dW, value2, actionDelegate2);
                }
                while (actionDelegate != actionDelegate2);
            }
        }
        public event UndoSystem.ActionDelegate ActionRedo
        {
            add
            {
                UndoSystem.ActionDelegate actionDelegate = this.dw;
                UndoSystem.ActionDelegate actionDelegate2;
                do
                {
                    actionDelegate2 = actionDelegate;
                    UndoSystem.ActionDelegate value2 = (UndoSystem.ActionDelegate)Delegate.Combine(actionDelegate2, value);
                    actionDelegate = Interlocked.CompareExchange<UndoSystem.ActionDelegate>(ref this.dw, value2, actionDelegate2);
                }
                while (actionDelegate != actionDelegate2);
            }
            remove
            {
                UndoSystem.ActionDelegate actionDelegate = this.dw;
                UndoSystem.ActionDelegate actionDelegate2;
                do
                {
                    actionDelegate2 = actionDelegate;
                    UndoSystem.ActionDelegate value2 = (UndoSystem.ActionDelegate)Delegate.Remove(actionDelegate2, value);
                    actionDelegate = Interlocked.CompareExchange<UndoSystem.ActionDelegate>(ref this.dw, value2, actionDelegate2);
                }
                while (actionDelegate != actionDelegate2);
            }
        }
        public event UndoSystem.ActionDelegate ActionDestroy
        {
            add
            {
                UndoSystem.ActionDelegate actionDelegate = this.dX;
                UndoSystem.ActionDelegate actionDelegate2;
                do
                {
                    actionDelegate2 = actionDelegate;
                    UndoSystem.ActionDelegate value2 = (UndoSystem.ActionDelegate)Delegate.Combine(actionDelegate2, value);
                    actionDelegate = Interlocked.CompareExchange<UndoSystem.ActionDelegate>(ref this.dX, value2, actionDelegate2);
                }
                while (actionDelegate != actionDelegate2);
            }
            remove
            {
                UndoSystem.ActionDelegate actionDelegate = this.dX;
                UndoSystem.ActionDelegate actionDelegate2;
                do
                {
                    actionDelegate2 = actionDelegate;
                    UndoSystem.ActionDelegate value2 = (UndoSystem.ActionDelegate)Delegate.Remove(actionDelegate2, value);
                    actionDelegate = Interlocked.CompareExchange<UndoSystem.ActionDelegate>(ref this.dX, value2, actionDelegate2);
                }
                while (actionDelegate != actionDelegate2);
            }
        }
        public static UndoSystem Instance
        {
            get
            {
                return UndoSystem.dT;
            }
        }
        public static void Init(int maxLevel)
        {
            Trace.Assert(UndoSystem.dT == null);
            UndoSystem.dT = new UndoSystem(maxLevel);
        }
        public static void Shutdown()
        {
            if (UndoSystem.dT != null)
            {
                UndoSystem.dT.Clear();
                UndoSystem.dT = null;
            }
        }
        internal UndoSystem(int num)
        {
            this.dt = num;
        }
        public void Clear()
        {
            bool flag = this.du.Count != 0 || this.dU.Count != 0;
            foreach (UndoSystem.Action current in this.dU)
            {
                current.Destroy();
                if (this.dX != null)
                {
                    this.dX(current);
                }
            }
            this.dU.Clear();
            foreach (UndoSystem.Action current2 in this.du)
            {
                current2.Destroy();
                if (this.dX != null)
                {
                    this.dX(current2);
                }
            }
            this.du.Clear();
            if (flag && this.dV != null)
            {
                this.dV(this, EventArgs.Empty);
            }
            if (this.dv != null)
            {
                this.dv(this, EventArgs.Empty);
            }
        }
        public void CommitAction(UndoSystem.Action action)
        {
            foreach (UndoSystem.Action current in this.dU)
            {
                current.Destroy();
                if (this.dX != null)
                {
                    this.dX(current);
                }
            }
            this.dU.Clear();
            if (this.du.Count + 1 >= this.dt)
            {
                this.du[0].Destroy();
                if (this.dX != null)
                {
                    this.dX(this.du[0]);
                }
                this.du.RemoveAt(0);
            }
            this.du.Add(action);
            if (this.dV != null)
            {
                this.dV(this, EventArgs.Empty);
            }
        }
        public UndoSystem.Action GetTopUndoAction()
        {
            if (this.du.Count == 0)
            {
                return null;
            }
            return this.du[this.du.Count - 1];
        }
        public UndoSystem.Action GetTopRedoAction()
        {
            if (this.dU.Count == 0)
            {
                return null;
            }
            return this.dU[this.dU.Count - 1];
        }
        public void DoUndo()
        {
            if (this.du.Count == 0)
            {
                return;
            }
            UndoSystem.Action action = this.du[this.du.Count - 1];
            this.du.RemoveAt(this.du.Count - 1);
            action.DoUndo();
            if (this.dW != null)
            {
                this.dW(action);
            }
            if (this.dU.Count + 1 >= this.dt)
            {
                this.dU[0].Destroy();
                if (this.dX != null)
                {
                    this.dX(this.dU[0]);
                }
                this.dU.RemoveAt(0);
            }
            this.dU.Add(action);
            if (this.dV != null)
            {
                this.dV(this, EventArgs.Empty);
            }
        }
        public void DoRedo()
        {
            if (this.dU.Count == 0)
            {
                return;
            }
            UndoSystem.Action action = this.dU[this.dU.Count - 1];
            this.dU.RemoveAt(this.dU.Count - 1);
            action.DoRedo();
            if (this.dw != null)
            {
                this.dw(action);
            }
            if (this.du.Count + 1 >= this.dt)
            {
                this.du[0].Destroy();
                if (this.dX != null)
                {
                    this.dX(this.du[0]);
                }
                this.du.RemoveAt(0);
            }
            this.du.Add(action);
            if (this.dV != null)
            {
                this.dV(this, EventArgs.Empty);
            }
        }
        public string[] DumpDebugToLines()
        {
            List<string> list = new List<string>();
            list.Add("UndoSystem");
            list.Add("");
            list.Add("Undo actions:");
            for (int i = 0; i < this.du.Count; i++)
            {
                list.Add(this.du[i].ToString());
            }
            list.Add("");
            list.Add("Redo actions:");
            for (int j = this.dU.Count - 1; j >= 0; j--)
            {
                list.Add(this.dU[j].ToString());
            }
            return list.ToArray();
        }
    }

}
