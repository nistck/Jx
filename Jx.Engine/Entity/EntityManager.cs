using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Jx.Engine.Collections;
using Jx.Engine.Events;
using Jx.Engine.Game;

namespace Jx.Engine.Entity
{
    internal class EntityManager : IEntityManager
    { 
        private readonly IObjectPool<IEntity> _entityPool;
        private IGameManager _gameManager;

        public EntityManager(IObjectPool<IEntity> objectPool = null)
        { 
            _entityPool = objectPool?? new EntityPool();
        }

        public event EventHandler<EntityChangedEventArgs> EntityAdded;
        public event EventHandler<EntityRemovedEventArgs> EntityRemoved;
        public event EventHandler<ComponentChangedEventArgs> ComponentAdded;
        public event EventHandler<ComponentRemovedEventArgs> ComponentRemoved;

        public void Register(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public IDictionary<Guid, IEntity> Entities { get; } = new Dictionary<Guid, IEntity>();

        public IEntity Get(string name)
        {
            var entity = _entityPool.Get(); 
            entity.Name = name;
 
            entity.Deleted += CleanupDeleted; 
            Entities[entity.ID] = entity;

            return entity;
        }

        public void Clear()
        {
            foreach (var entity in Entities)
            {
                entity.Value.Delete();
            }

            Entities.Clear();
        }

        public void Add(IEntity entity)
        {
            Entities[entity.ID] = entity;

            entity.ComponentAdded += ComponentAdded;
            entity.ComponentRemoved += ComponentRemoved;

            EntityAdded?.Invoke(this, new EntityChangedEventArgs(entity));
        }

        public void Add(IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public void Remove(IEntity entity)
        {
            if (!Entities.ContainsKey(entity.ID)) return;

            entity = Entities[entity.ID];
            entity.ComponentAdded -= ComponentAdded;
            entity.ComponentRemoved -= ComponentRemoved;

            Entities.Remove(entity.ID);

            EntityRemoved?.Invoke(this, new EntityRemovedEventArgs(entity));
        }

        private void CleanupDeleted(object sender, EventArgs eargs)
        {
            var entity = sender as IEntity;

            if (entity == null) return;

            entity.Deleted -= CleanupDeleted;

            _entityPool.Put(entity);

            Entities.Remove(entity.ID);

            _gameManager.RemoveEntity(entity);
        }

        public IEnumerator<IEntity> GetEnumerator()
        {
            return Entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(string name)
        {
            var q = Entities.Where(_kvp => _kvp.Value != null && _kvp.Value.Name == name).Select(_kvp => _kvp.Value).FirstOrDefault();
            Remove(q);
        }
    }
}