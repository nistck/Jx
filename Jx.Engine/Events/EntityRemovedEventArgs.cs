using System;
using Jx.Engine.Entity;

namespace Jx.Engine.Events
{
    public class EntityRemovedEventArgs : EventArgs
    {
        public EntityRemovedEventArgs(IEntity entity)
        {
            Entity = entity;
        }

        public IEntity Entity { get; set; }
    }
}