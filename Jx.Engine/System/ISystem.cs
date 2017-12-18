using System;
using System.Collections.Generic;

using Jx.Engine.Game;
using Jx.Engine.Entity;
using Jx.Engine.Events;
using Jx.Engine.Component;

namespace Jx.Engine.System
{
    public delegate bool EntityIncludeMatcher(IEntity entity);

    public interface ISystem : IUpdatable, IComparable<ISystem>, IEnumerable<IEntity>
    {
        event EventHandler<TickEventArgs> Tick;

        Guid ID { get; }

        EntityIncludeMatcher Matcher { get; }        
        int Priority { get; }
        IGameManager GameManager { get; }
        void Start();
        void Stop();
        bool Actived { get; }
        void AddToGameManager(IGameManager gameManager);
        void RemoveFromGameManager(IGameManager gameManager);

        void NotifyComponentChanged(IEntity entity, IComponent component, bool fAdd);
    }
}