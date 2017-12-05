using System;

namespace Jx.Engine.Component
{
    public interface IComponent
    {
        Guid ID { get; }
        IComponent Clone();
        void Reset();
    }
}