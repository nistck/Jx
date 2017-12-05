using System.Collections.Generic;
using Jx.Engine.Entity;

namespace Jx.Engine.Aspect
{
    public interface IAspectManager
    {
        IEnumerable<IAspect> Aspects { get; }
        IEnumerable<IAspect> ChannelAspects { get; }
        IAspect Get(IEntity entity);
    }
}