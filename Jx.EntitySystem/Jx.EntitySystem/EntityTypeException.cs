using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{
    public class EntityTypeException : Exception
    {
        public EntityTypeException(EntityType entityType, string message)
            : base(message)
        {
            this.EntityType = entityType;
        }

        public EntityType EntityType { get; private set; }
    }
}
