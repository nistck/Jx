using System;

using Jx.Engine.Entity;

namespace Jx.Engine.Component
{
    public interface IComponent
    {
        Guid ID { get; }
        IEntity Owner { get; }
        IComponent Clone();
        void Reset();
    }
}