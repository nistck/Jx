using System;
using Jx.Engine.Events;

namespace Jx.Engine.Channel
{
    public class ChannelManager : IChannelManager
    {
        private string _channel = "default";

        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                ChannelChanged?.Invoke(this, new ChannelChangedEventArgs(_channel));
            }
        }

        public event EventHandler<ChannelChangedEventArgs> ChannelChanged;
    }
}