using System;
using Jx.Engine.Component;

namespace Jx.Engine.Events
{
    public class ComponentRemovedEventArgs : EventArgs
    {
        public ComponentRemovedEventArgs(IComponent component)
        {
            Component = component;
        }

        public IComponent Component { get; set; }
    }
}