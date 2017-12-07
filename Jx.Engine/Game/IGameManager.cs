using System;
using System.Collections.Generic; 
using Jx.Engine.Entity;
using Jx.Engine.Events;
using Jx.Engine.System;

namespace Jx.Engine.Game
{
    public interface IGameManager : IUpdatable, IDrawable, Identifier
    {
        event EventHandler<TickEventArgs> Tick;
        event EventHandler<SystemChangedEventArgs> SystemAdded;
        event EventHandler<SystemRemovedEventArgs> SystemRemoved;
        event EventHandler<SystemStoppedEventArgs> SystemStopped;
        event EventHandler<SystemStartedEventArgs> SystemStarted;
        event EventHandler<EntityChangedEventArgs> EntityAdded;
        event EventHandler<EntityRemovedEventArgs> EntityRemoved;

        DateTime StartupTime { get; }
         
        IEntityManager EntityManager { get; }
        ISystemManager Systems { get; }
        IEntity CreateEntity(string name);
        IEntity GetEntity(string name);
        void AddEntities(IEnumerable<IEntity> entities);
        void RemoveAllEntities();        
        void RemoveEntity(IEntity e);
        void RemoveEntity(string name);  
        void AddSystem(ISystem system);        
        void RemoveSystem<TSystemType>(bool shouldNotify) where TSystemType : ISystem;
        void RemoveAllSystems(bool shouldNotify);        
        void SuspendSystem<TSystemType>() where TSystemType : ISystem;        
        void StartSystem<TSystemType>() where TSystemType : ISystem;
    }
}