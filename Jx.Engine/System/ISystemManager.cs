using System;
using System.Collections.Generic;
using Jx.Engine.Events;
using Jx.Engine.Game;

using Jx.Engine.Entity;
using Jx.Engine.Component;

namespace Jx.Engine.System
{
    public interface ISystemManager : IUpdatable, IDrawable, IEnumerable<ISystem>
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
        void Clear(bool shouldNotify = false);
        int Count(); 
        
    }
}
