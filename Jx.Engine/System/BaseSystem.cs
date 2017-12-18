using System;
using System.Collections;
using System.Collections.Generic;
using Jx.Engine.Events;
using Jx.Engine.Game;

using Jx.Engine.Entity;
using Jx.Engine.Component;

namespace Jx.Engine.System
{
    public abstract class BaseSystem : ISystem
    {
        public event EventHandler<TickEventArgs> Tick;

        private static bool _DefaultMatcher(IEntity _entity)
        {
            return false;
        }
         
        private readonly List<IEntity> _entities = new List<IEntity>(); 

        protected BaseSystem(EntityIncludeMatcher matcher = null, int priority = 0, bool startImmediately = true)
        {
            this.Matcher = matcher?? _DefaultMatcher;
            Priority = priority;

            if (startImmediately)
                Start();
        }

        public Guid ID { get; } = Guid.NewGuid();
         
        public IGameManager GameManager { get; set; }

        public EntityIncludeMatcher Matcher { get; private set; }

        public bool Actived { get; private set; }

        public int CompareTo(ISystem other)
        {
            return Priority.CompareTo(other.Priority);
        }

        public int Priority { get; } 

        public bool IsUpdating { get; private set; }

        public void AddToGameManager(IGameManager gameManager)
        {
            if (this.GameManager != null && this.GameManager == gameManager)
                return;

            if (this.GameManager != null)
                RemoveFromGameManager(this.GameManager);
            
            this.GameManager = gameManager;

            // Copy entities
            if( gameManager != null && gameManager.EntityManager != null && Matcher != null)
            {
                gameManager.EntityManager.EntityAdded -= EntityManager_EntityAdded;
                gameManager.EntityManager.EntityAdded += EntityManager_EntityAdded;

                _entities.Clear();
                foreach(var entity in gameManager.EntityManager)
                {
                    if (Matcher(entity))
                        _entities.Add(entity);
                }
            }

            OnAddedToGameManager(gameManager);
        }

        private void EntityManager_EntityAdded(Object sender, EntityChangedEventArgs e)
        {
            if (Matcher != null && Matcher(e.Entity))
                _entities.Add(e.Entity);
        }
 
        public void RemoveFromGameManager(IGameManager gameManager)
        {
            if (this.GameManager == null)
                return; 
            if (this.GameManager != null && this.GameManager != gameManager)
                return;

            GameManager.EntityManager.EntityAdded -= EntityManager_EntityAdded;
            _entities.Clear();

            OnRemovedFromGameManager();
        }

        public void Update(ITickEvent tickEvent)
        {
            try
            {
                IsUpdating = true;
                OnUpdate(tickEvent);

                Tick?.Invoke(this, new TickEventArgs(tickEvent));
            }catch(Exception e)
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

        public void Start()
        {
            Actived = true;
        }

        public void Stop()
        {
            Actived = false;
        }

        public event EventHandler<GameManagerChangedEventArgs> AddedToGameManager;

        protected virtual void OnAddedToGameManager(IGameManager gameManager)
        {
            AddedToGameManager?.Invoke(this, new GameManagerChangedEventArgs(gameManager));
        }

        public event EventHandler RemovedFromGameManager;

        protected virtual void OnRemovedFromGameManager()
        {
            RemovedFromGameManager?.Invoke(this, null);
        }

        public IEnumerator<IEntity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BaseSystem)) return base.Equals(obj);

            var other = (BaseSystem) obj;
            return ID == other.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        } 

        public void NotifyComponentChanged(IEntity entity, IComponent component, bool fAdd)
        {
            if (Matcher == null)
                return;

            _entities.Remove(entity);
            if (fAdd && Matcher(entity) )
                _entities.Add(entity);
        }
    }
}