using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.MapSystem
{
    public class MapGeneralObjectType : EntityType
    {
        public MapGeneralObjectType()
        {
            base.CreatableInMapEditor = false; 
        }

        [Browsable(false)]
        public new bool CreatableInMapEditor
        {
            get { return base.CreatableInMapEditor; }
            set { }
        }
    }

    public class MapGeneralObject : Entity
    {
        private MapGeneralObjectType _type = null; 
        public new MapGeneralObjectType Type { get { return _type; } }
    }
}
