using System.Collections.Generic;
using Jx.Engine.Aspect;
using Jx.Engine.Component;

namespace Jx.Engine.Entity
{
    public interface IEntityAspectManager
    {        
        IEnumerable<TAspectType> GetAspectList<TAspectType>() where TAspectType : IAspect, new();
        IEnumerable<TAspectType> GetUnfilteredAspectList<TAspectType>() where TAspectType : IAspect, new();
        void RegisterEntity(IEntity e);
        void UnregisterEntity(IEntity e);
        void ComponentRemovedFromEntity(IEntity e, IComponent component);
        void ComponentAddedToEntity(IEntity e, IComponent component);
        void ReleaseAspectList<TAspectType>();              
    }
}
