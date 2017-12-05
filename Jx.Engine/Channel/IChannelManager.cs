using System;
using Jx.Engine.Events;

namespace Jx.Engine.Channel
{
    public interface IChannelManager
    {
        string Channel { get; set; }
        event EventHandler<ChannelChangedEventArgs> ChannelChanged;
    }
}