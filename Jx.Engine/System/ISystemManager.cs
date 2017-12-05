using System;
using Jx.Engine.Events;
using Jx.Engine.Game;

namespace Jx.Engine.System
{
    public interface ISystemManager
    {
        event EventHandler<SystemRemovedEventArgs> SystemRemoved;
        event EventHandler<SystemChangedEventArgs> SystemAdded;
        event EventHandler<SystemStartedEventArgs> SystemStarted;
        event EventHandler<SystemStoppedEventArgs> SystemStopped;

        void Register(IGameManager gameManager);
        void Add(ISystem system);
        void Remove(Type systemType, bool shouldNotify = false);
        void Start(Type systemType);
        void Stop(Type systemType);
        ISystem GetBySystemType(Type systemType);
        void Update(ITickEvent tickEvent);
        void Draw(ITickEvent tickEvent);
        void Clear(bool shouldNotify = false);
        int Count();
    }
}