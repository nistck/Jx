using System;
using System.Collections.Generic;
using Jx.Engine.Events;
using Jx.Engine.Game;

namespace Jx.Engine.Entity
{
    public interface IEntityManager
    {
        event EventHandler<EntityChangedEventArgs> EntityAdded;
        event EventHandler<EntityRemovedEventArgs> EntityRemoved;
        event EventHandler<ComponentChangedEventArgs> ComponentAdded;
        event EventHandler<ComponentRemovedEventArgs> ComponentRemoved;

        IDictionary<Guid, IEntity> Entities { get; }        
        void Register(IGameManager gameManager);
        IEntity Get(string name = "", string[] channels = null);
        void Clear();
        void Add(IEntity entity);
        void Add(IEnumerable<IEntity> entities);
        void Remove(IEntity entity);
    }
}