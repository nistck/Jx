using System;
using System.Collections.Generic;
using Jx.Engine.Entity;

namespace Jx.Engine.Aspect
{
    public interface IAspectMatchingFamily
    {
        IEnumerable<IAspect> ActiveAspectList { get; }
        IEnumerable<IAspect> EntireAspectList { get; }
        
        void NewEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
        void ComponentAddedToEntity(IEntity entity, Type componentType);
        void ComponentRemovedFromEntity(IEntity entity, Type componentType);
        void CleanUp();
    }
}