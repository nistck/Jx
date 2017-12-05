﻿using System;
using System.Collections.Generic;
using Jx.Engine.Channel;
using Jx.Engine.Collections;
using Jx.Engine.Entity;
using Jx.Engine.Extensions;

namespace Jx.Engine.Aspect
{
    public class DefaultAspectMatchingFamily<TAspectType> : IAspectMatchingFamily where TAspectType : IAspect, new()
    {
        private readonly IAspectManager _aspectManager;
        private readonly List<Type> _componentTypes = new List<Type>();
        private readonly IDictionary<Guid, IAspect> _entities = new Dictionary<Guid, IAspect>();

        public DefaultAspectMatchingFamily(IChannelManager channelManager)
        {
            _aspectManager = new AspectManager(channelManager, new AspectPool<TAspectType>());
            var componentTypes = AssociatedComponentsAttributeExtension.GetAssociatedComponentTypes(typeof (TAspectType));
            _componentTypes.AddRange(componentTypes);
        }

        public void CleanUp()
        {
            foreach (var kvp in _entities)
            {
                kvp.Value.Delete();
            }

            _entities.Clear();
        }

        public void ComponentAddedToEntity(IEntity entity, Type componentType)
        {
            if (!_componentTypes.Contains(componentType) || !IsMatch(entity)) return;

            Add(entity);
        }

        public void ComponentRemovedFromEntity(IEntity entity, Type componentType)
        {
            if (!ContainsEntity(entity) || !_componentTypes.Contains(componentType)) return;

            Remove(entity);
        }

        public IEnumerable<IAspect> ActiveAspectList => _aspectManager.ChannelAspects;

        public IEnumerable<IAspect> EntireAspectList => _aspectManager.Aspects;

        public void NewEntity(IEntity entity)
        {
            if (ContainsEntity(entity) || !IsMatch(entity)) return;

            Add(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            if (!ContainsEntity(entity)) return;

            Remove(entity);
        }

        private bool IsMatch(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return entity.HasComponents(_componentTypes);
        }

        private bool ContainsEntity(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return _entities.ContainsKey(entity.ID);
        }

        private void Add(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var aspect = _aspectManager.Get(entity);

            _entities.Add(entity.ID, aspect);
        }

        private void Remove(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var aspect = _entities[entity.ID];

            if (aspect == null) return;

            aspect.Delete();

            _entities.Remove(entity.ID);
        }
    }
}