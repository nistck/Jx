using System;
using Jx.Engine.Entity;

namespace Jx.Engine.Events
{
    public class EntityChangedEventArgs : EventArgs
    {
        public EntityChangedEventArgs(IEntity entity)
        {
            Entity = entity;
        }

        public IEntity Entity { get; set; }
    }
}