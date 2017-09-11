using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.EntitiesCommon
{
    public class DynamicType : EntityType
    {
        public DynamicType()
        {
            this.NetworkType = EntityNetworkTypes.Synchronized;
        }
    }

    public class Dynamic : Entity
    {
        private DynamicType _type = null; 
        public new DynamicType Type { get { return _type; } }
    }
}
