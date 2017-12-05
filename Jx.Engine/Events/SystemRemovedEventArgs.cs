using System;
using Jx.Engine.System;

namespace Jx.Engine.Events
{
    public class SystemRemovedEventArgs : EventArgs
    {
        public SystemRemovedEventArgs(ISystem system)
        {
            System = system;
        }

        public ISystem System { get; set; }
    }
}