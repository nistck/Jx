using System;

using Jx.Engine.Entity;

namespace Jx.Engine.Component
{
    public abstract class BaseComponent : IComponent
    {
        public Guid ID { get; } = Guid.NewGuid();

        public IEntity Owner { get; internal set; }

        public virtual IComponent Clone()
        {
            var x = MemberwiseClone() as IComponent;
            return x;
        }

        public virtual void Reset()
        {

        }
    }
}