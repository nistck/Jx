using System;

namespace Jx.Engine.Events
{
    public class ChannelChangedEventArgs : EventArgs
    {
        public ChannelChangedEventArgs(string channel)
        {
            Channel = channel;
        }

        public string Channel { get; set; }
    }
}