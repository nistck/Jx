using System;
using Jx.Engine.Component;

namespace Jx.Engine.Events
{
    public class ComponentChangedEventArgs : EventArgs
    {
        public ComponentChangedEventArgs(IComponent component)
        {
            Component = component;
        }

        public IComponent Component { get; set; }
    }
}