using System;
using Jx.Engine.System;

namespace Jx.Engine.Events
{
    public class SystemStoppedEventArgs : EventArgs
    {
        public SystemStoppedEventArgs(ISystem system)
        {
            System = system;
        }

        public ISystem System { get; set; }
    }
}