using System;

using Jx.Engine.Entity;

namespace Jx.Engine.Component
{
    public interface IComponent : Identifier
    {
        IEntity Owner { get; }
        IComponent Clone();
        void Reset();
    }
}