using System;
using System.Linq;
using System.Collections.Generic; 
using Jx.Engine.Entity;
using Jx.Engine.Events;
using Jx.Engine.System;
using Jx.Engine.Collections;

namespace Jx.Engine.Game
{
    public abstract class BaseGameManager : IGameManager
    {        
        protected BaseGameManager( 
            IEntityManager entityManager = null,
            ISystemManager systemManager = null)
        {
            this.StartupTime = DateTime.Now;

            EntityManager = entityManager ?? new EntityManager();
            Systems = systemManager ?? new SystemManager();           

            RegisterManagers();
        }

        public Guid ID { get; } = Guid.NewGuid();

        public DateTime StartupTime { get; private set; }

        public IEntityManager EntityManager { get; }
        public ISystemManager Systems { get; }

        public bool IsUpdating { get; private set; }
        public bool IsDrawing { get; private set; }

        public event EventHandler<TickEventArgs> Tick;
        public event EventHandler<SystemChangedEventArgs> SystemAdded;
        public event EventHandler<SystemRemovedEventArgs> SystemRemoved;
        public event EventHandler<SystemStoppedEventArgs> SystemStopped;
        public event EventHandler<SystemStartedEventArgs> SystemStarted;
        public event EventHandler<EntityChangedEventArgs> EntityAdded;
        public event EventHandler<EntityRemovedEventArgs> EntityRemoved;

        public void AddEntities(IEnumerable<IEntity> entities)
        {
            EntityManager.Add(entities);
        }

        public IEntity CreateEntity(string name)
        {
            IEntity entity = new BaseEntity(name);
            EntityManager.Add(entity);
            entity.ComponentAdded += Entity_ComponentAdded;
            entity.ComponentRemoved += Entity_ComponentRemoved;
            return entity;
        }

        private void Entity_ComponentRemoved(object sender, ComponentRemovedEventArgs e)
        {
            IEntity entity = sender as IEntity;
            foreach (var system in Systems)
                system.NotifyComponentChanged(entity, e.Component, false);
        }

        private void Entity_ComponentAdded(object sender, ComponentChangedEventArgs e)
        {
            IEntity entity = sender as IEntity;
            foreach (var system in Systems)
                system.NotifyComponentChanged(entity, e.Component, true);
        }

        public void AddSystem(ISystem system)
        {
            Systems.Add(system);
        }
         
        public void RemoveAllSystems(bool shouldNotify)
        {
            Systems.Clear(shouldNotify);
        }

        public void RemoveAllEntities()
        {
            EntityManager.Clear();
        }

        public void RemoveEntity(IEntity entity)
        {
            if( entity != null)
            {
                entity.ComponentAdded -= Entity_ComponentAdded;
                entity.ComponentRemoved -= Entity_ComponentRemoved;
            }
            EntityManager.Remove(entity);
        }

        public void RemoveSystem<TSystemType>(bool shouldNotify) where TSystemType : ISystem
        {
            RemoveSystem(typeof(TSystemType), shouldNotify);
        }

        public void StartSystem<TSystemType>() where TSystemType : ISystem
        {
            Systems.Start(typeof(TSystemType));
        }

        public void SuspendSystem<TSystemType>() where TSystemType : ISystem
        {
            Systems.Stop(typeof(TSystemType));
        }

        private List<ItfType> GetEntitiesImplementItf<ItfType>()
        {
            List<ItfType> result = new List<ItfType>();
            if (EntityManager == null)
                return result;

            var q = EntityManager.Where(_entity => _entity != null && typeof(ItfType).IsAssignableFrom(_entity.GetType()))
                .OfType<ItfType>().Where(_x => _x != null);
            result.AddRange(q); 
            return result;
        }
        
        private List<ItfType> GetComponentsImplementItf<ItfType>()
        {
            List<ItfType> result = new List<ItfType>();
            if (EntityManager == null)
                return result;

            var q = EntityManager.SelectMany(_entity => _entity.Components.Values)
                .Where(_component => _component != null && typeof(ItfType).IsAssignableFrom(_component.GetType()))
                .OfType<ItfType>().Where(_x => _x != null);
            result.AddRange(q);
            return result;
        }

        private void OnUpdateEntities(ITickEvent tickEvent)
        {
            foreach(var u in GetEntitiesImplementItf<IUpdatable>())
                u.Update(tickEvent);
        }

        private void OnUpdateComponents(ITickEvent tickEvent)
        {
            foreach (var u in GetComponentsImplementItf<IUpdatable>())
                u.Update(tickEvent);
        }

        public virtual void Update(ITickEvent tickEvent)
        {
            try
            {
                IsUpdating = true;

                OnUpdateEntities(tickEvent);
                OnUpdateComponents(tickEvent);

                Systems.Update(tickEvent); 

                OnSystemsUpdated(tickEvent);

                Tick?.Invoke(this, new TickEventArgs(tickEvent));
                OnUpdate(tickEvent);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                IsUpdating = false;
            }
        }

        protected virtual void OnUpdate(ITickEvent tickEvent)
        {

        }


        private void OnDrawEntities(ITickEvent tickEvent)
        {
            foreach (var u in GetEntitiesImplementItf<IDrawable>())
                u.Draw(tickEvent);
        }

        private void OnDrawComponents(ITickEvent tickEvent)
        {
            foreach (var u in GetComponentsImplementItf<IDrawable>())
                u.Draw(tickEvent);
        }


        public void Draw(ITickEvent tickEvent)
        {
            try
            {
                IsDrawing = true;

                OnDrawEntities(tickEvent);
                OnDrawComponents(tickEvent);

                Systems.Draw(tickEvent);
                OnDraw(tickEvent); 
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                IsDrawing = false;
            }
        }

        protected virtual void OnDraw(ITickEvent tickEvent)
        {

        }

        public void RemoveSystem(Type systemType, bool shouldNotify)
        {
            Systems.Remove(systemType, shouldNotify);
        }

        protected virtual void OnSystemsUpdated(ITickEvent tickEvent)
        {
        }

        protected virtual void OnManagersUpdated(ITickEvent tickEvent)
        {
        }

        private void RegisterManagers()
        {
            
            // Entity events
            EntityManager.Register(this);
            EntityManager.EntityAdded += EntityManagerOnEntityAdded;
            EntityManager.EntityRemoved += EntityManagerOnEntityRemoved;
            EntityManager.ComponentAdded += EntityManagerOnComponentAdded;
            EntityManager.ComponentRemoved += EntityManagerOnComponentRemoved;

            // Systems events
            Systems.Register(this);
            Systems.SystemAdded += (sender, args) => SystemAdded?.Invoke(sender, args);
            Systems.SystemStarted += (sender, args) => SystemStarted?.Invoke(sender, args);
            Systems.SystemRemoved += (sender, args) => SystemRemoved?.Invoke(sender, args);
            Systems.SystemStopped += (sender, args) => SystemStopped?.Invoke(sender, args);
        }

        private void EntityManagerOnComponentRemoved(object sender, ComponentRemovedEventArgs args)
        {
        }

        private void EntityManagerOnComponentAdded(object sender, ComponentChangedEventArgs args)
        {
        }

        private void EntityManagerOnEntityRemoved(object sender, EntityRemovedEventArgs args)
        {
            EntityRemoved?.Invoke(sender, args);
        }

        private void EntityManagerOnEntityAdded(object sender, EntityChangedEventArgs args)
        {
            EntityAdded?.Invoke(sender, args);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BaseGameManager)) return base.Equals(obj);

            var other = (BaseGameManager)obj;
            return ID == other.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public IEntity GetEntity(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(string name)
        {
            throw new NotImplementedException();
        }
    }
}