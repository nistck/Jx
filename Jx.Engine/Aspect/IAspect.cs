using System;
using System.Collections.Generic;
using Jx.Engine.Channel;
using Jx.Engine.Component;
using Jx.Engine.Entity;

namespace Jx.Engine.Aspect
{
    public interface IAspect : Identifier, IChannelFilterable
    {
        IDictionary<Type, IComponent> Components { get; }
        event EventHandler Deleted;
        void Delete();
        void Reset();
        void Init(IEntity e);
    }
}