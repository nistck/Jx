﻿using System;
using System.Collections.Generic;
using Jx.Engine.Component;
using Jx.Engine.Events;

namespace Jx.Engine.Entity
{
    public interface IEntity : Identifier
    {
        /// <summary>
        /// Entity是否已经被删除 （标记为删除）
        /// </summary>
        bool IsDeleted { get; set; }
        IDictionary<Type, IComponent> Components { get; }
        string Name { get; set; }
        event EventHandler Deleted;
        void Delete();        
        IEntity Clone();
        void Reset();
        bool HasComponent(Type componentType);
        bool HasComponents(IEnumerable<Type> types);
        bool HasComponents(params Type[] componentTypes);

        TComponent GetComponent<TComponent>() where TComponent : IComponent, new();

        event EventHandler<ComponentChangedEventArgs> ComponentAdded;
        IEntity AddComponent(IComponent component, bool shouldOverwrite = false);        
        event EventHandler<ComponentRemovedEventArgs> ComponentRemoved;
        bool RemoveComponent(Type componentType);
    }
}