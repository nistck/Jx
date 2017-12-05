using System.Collections.Generic;

namespace Jx.Engine.Channel
{
    public interface IChannelFilterable
    {
        IList<string> Channels { get; }
    }
}