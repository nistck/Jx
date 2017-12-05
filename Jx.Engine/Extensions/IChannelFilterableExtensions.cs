using System.Linq;
using Jx.Engine.Channel;

namespace Jx.Engine.Extensions
{
    public static class ChannelFilterableExtensions
    {
        public static bool IsInChannel(this IChannelFilterable obj, params string[] channels)
        {
            return channels.Any(c => obj.Channels.Contains(c));
        }
    }
}