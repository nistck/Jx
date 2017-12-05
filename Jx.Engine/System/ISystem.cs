using System;
using Jx.Engine.Channel;
using Jx.Engine.Game;

namespace Jx.Engine.System
{
    public interface ISystem : IChannelFilterable, IComparable<ISystem>
    {
        Guid ID { get; }
        int Priority { get; }
        void Start();
        void Stop();
        void AddToGameManager(IGameManager gameManager);
        void RemoveFromGameManager(IGameManager gameManager);
        void Update(ITickEvent tickEvent);
    }
}