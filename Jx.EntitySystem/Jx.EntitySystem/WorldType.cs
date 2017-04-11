using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{
    [AllowToCreateTypeBasedOnThisClass(false)]
    public class WorldType : EntityType
    {
        public WorldType()
        {
            base.NetworkType = EntityNetworkTypes.Synchronized;
            base.AllowEmptyName = true;
            base.CreatableInMapEditor = false;
        }
    }
}
