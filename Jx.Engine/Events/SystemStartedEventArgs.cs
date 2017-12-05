using System;
using Jx.Engine.System;

namespace Jx.Engine.Events
{
    public class SystemStartedEventArgs : EventArgs
    {
        public SystemStartedEventArgs(ISystem system)
        {
            System = system;
        }

        public ISystem System { get; set; }
    }
}