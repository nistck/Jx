using System;
using Jx.Engine.System;

namespace Jx.Engine.Events
{
    public class SystemChangedEventArgs : EventArgs
    {
        public SystemChangedEventArgs(ISystem system)
        {
            System = system;
        }

        public ISystem System { get; set; }
    }
}