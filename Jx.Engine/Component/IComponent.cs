using System;

namespace Jx.Engine.Component
{
    public interface IComponent : Identifier
    {
        IComponent Clone();
        void Reset();
    }
}