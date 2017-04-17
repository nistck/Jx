using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{
    public class EntityException : Exception
    {
        public EntityException(Entity entity, string message)
            : base(message)
        {
            this.Entity = entity;
        }

        public Entity Entity { get; private set; }
    }
}
