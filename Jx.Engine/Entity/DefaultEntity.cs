using System;
using System.Collections.Generic;
using System.Linq;
using Jx.Engine.Component;
using Jx.Engine.Events;

namespace Jx.Engine.Entity
{
    internal class DefaultEntity : IEntity
    {
        public DefaultEntity(string name = "")
        {
            Name = name;
        }

        public event EventHandler Deleted;
        public event EventHandler<ComponentChangedEventArgs> ComponentAdded;
        public event EventHandler<ComponentRemovedEventArgs> ComponentRemoved;

        public Guid ID { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public bool IsDeleted { get; set; } 
        public IDictionary<Type, IComponent> Components { get; } = new Dictionary<Type, IComponent>();

        public void Reset()
        {
            IsDeleted = false;
        }        

        public void Delete()
        {
            IsDeleted = true;
            OnDeleted();
        }

        public IEntity Clone()
        {
            var e = new DefaultEntity(Name);
            foreach (var c in Components.Values)
            {
                e.Components.Add(c.GetType(), c.Clone());
            }
            return e;
        }

        public IEntity AddComponent(IComponent c, bool overwriteIfExists = false)
        {
            var componentType = c.GetType();

            if (HasComponent(componentType) && !overwriteIfExists)
            {
                throw new InvalidOperationException("Component already exists on this entity");
            }

            Components[componentType] = c;
            if (c is BaseComponent)
                ((BaseComponent)c).Owner = this;
            OnComponentAdded(c);
            return this;
        }

        public bool RemoveComponent(Type componentType)
        {
            if (!HasComponent(componentType)) return false;
            var component = Components[componentType];
            Components.Remove(componentType);
            OnComponentRemoved(component);
            component = null;
            return true;
        }

        public bool HasComponent(Type componentType)
        {
            return Components.ContainsKey(componentType);
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent, new()
        {
            Type componentType = typeof(TComponent);
            if (!HasComponent(componentType))
                return default(TComponent);

            IComponent x = Components[componentType];
            if (!componentType.IsAssignableFrom(x.GetType()))
                return default(TComponent);
            return (TComponent)x;
        }

        public bool HasComponents(IEnumerable<Type> types)
        {
            return types.All(HasComponent);
        }

        public bool HasComponents(params Type[] componentTypes)
        {
            return componentTypes.All(HasComponent);
        }

        public virtual void OnDeleted()
        {
            Deleted?.Invoke(this, null);
        }

        protected void OnComponentAdded(IComponent c)
        {
            ComponentAdded?.Invoke(this, new ComponentChangedEventArgs(c));
        }

        protected void OnComponentRemoved(IComponent c)
        {
            ComponentRemoved?.Invoke(this, new ComponentRemovedEventArgs(c));
        } 
 
        public override string ToString()
        {
            string result = string.Format("Entity({0}, ID: {1})", Name, ID);
            return result;
        }
    }
}