using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jx.Engine.Component;
using Jx.Engine.Entity;
using Jx.Engine.Events;
using Jx.Engine.Game;

namespace Jx.Engine.System
{
    internal class SystemManager : ISystemManager
    {
        private IGameManager _gameManager;
        private readonly List<ISystem> _systems = new List<ISystem>();
        private readonly List<IDrawableSystem> _drawableSystems = new List<IDrawableSystem>();
        
        public SystemManager()
        {
        }

        public event EventHandler<SystemRemovedEventArgs> SystemRemoved;
        public event EventHandler<SystemChangedEventArgs> SystemAdded;
        public event EventHandler<SystemStartedEventArgs> SystemStarted;
        public event EventHandler<SystemStoppedEventArgs> SystemStopped;

        public void Register(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Add(ISystem system)
        {
            if (ContainsSystem(system))
                throw new InvalidOperationException($"System {system.GetType().Name} already added to game manager.");

            _systems.Add(system);
            _systems.Sort();

            var drawableSystem = system as IDrawableSystem;
            if (drawableSystem != null)
            {
                _drawableSystems.Add(drawableSystem);
                _drawableSystems.Sort();
            }

            system.AddToGameManager(_gameManager);

            SystemAdded?.Invoke(this, new SystemChangedEventArgs(system));
        }

        public void Stop(Type systemType)
        {
            var system = GetBySystemType(systemType);

            if (system == null) return;

            system.Stop();

            SystemStopped?.Invoke(this, new SystemStoppedEventArgs(system));
        }

        public ISystem GetBySystemType(Type systemType)
        {
            return _systems.SingleOrDefault(s => s.GetType() == systemType);
        }

        public bool IsUpdating { get; private set; }
        public void Update(ITickEvent tickEvent)
        {
            try
            {
                IsUpdating = true;

                var qSystems = _systems.Where(_sys => _sys.Actived);
                foreach (var system in qSystems)
                {
                    system.Update(tickEvent);
                }
            }catch(Exception e)
            {
                throw e;
            }
            finally
            {
                IsUpdating = false;
            }
        }

        public bool IsDrawing { get; private set; }
        public void Draw(ITickEvent tickEvent)
        {
            try
            {
                IsDrawing = true;

                var qSystems = _drawableSystems.Where(_sys => _sys.Actived);
                foreach (var system in qSystems)
                {
                    system.Draw(tickEvent);
                }
            }
            catch (Exception e) {
                throw e;
            }
            finally
            {
                IsDrawing = false; 
            }
        }

        public void Clear(bool shouldNotify = false)
        {
            while (_systems.Count > 0)
            {
                Remove(_systems.First().GetType(), shouldNotify);
            }
        }

        public int Count()
        {
            return _systems.Count;
        }

        public void Remove(Type systemType, bool shouldNotify = false)
        {
            var system = GetBySystemType(systemType);

            if (system == null) return;

            _systems.Remove(system);
            _systems.Sort();

            var drawableSystem = system as IDrawableSystem;
            if (drawableSystem != null)
            {
                _drawableSystems.Remove(drawableSystem);
                _drawableSystems.Sort();
            }

            system.RemoveFromGameManager(_gameManager);

            if (!shouldNotify) return;

            SystemRemoved?.Invoke(this, new SystemRemovedEventArgs(system));
        }

        public void Start(Type systemType)
        {
            var system = GetBySystemType(systemType);

            if (system == null) return;

            system.Start();

            SystemStarted?.Invoke(this, new SystemStartedEventArgs(system));
        }

        private bool ContainsSystem(ISystem system)
        {
            return _systems.Any(s => s.ID == system.ID);
        }

        public IEnumerator<ISystem> GetEnumerator()
        {
            return _systems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}