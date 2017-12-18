using Jx.Engine.Entity;

namespace Jx.Engine.Collections
{
    public class EntityPool : ObjectPool<IEntity>
    {
        public EntityPool() : base(() => new BaseEntity(), entity => entity.Reset())
        {
        }
    }
}