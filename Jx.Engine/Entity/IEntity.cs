using System;
using System.Collections.Generic;
using Jx.Engine.Channel;
using Jx.Engine.Component;
using Jx.Engine.Events;

namespace Jx.Engine.Entity
{
    public interface IEntity : IChannelFilterable
    {
        Guid ID { get; }
        bool IsDeleted { get; set; }
        IDictionary<Type, IComponent> Components { get; }
        string Name { get; set; }
        event EventHandler Deleted;
        void Delete();        
        IEntity Clone();
        void Reset();
        bool HasComponent(Type componentType);
        bool HasComponents(IEnumerable<Type> types);
        event EventHandler<ComponentChangedEventArgs> ComponentAdded;
        IEntity AddComponent(IComponent component, bool shouldOverwrite = false);        
        event EventHandler<ComponentRemovedEventArgs> ComponentRemoved;
        bool RemoveComponent(Type componentType);
    }
}