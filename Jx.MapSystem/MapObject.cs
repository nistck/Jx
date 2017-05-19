using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.MapSystem
{
    public class MapObjectType : MapGeneralObjectType
    {
    }

    public class MapObject : MapGeneralObject
    {
        private MapObjectType _type = null; 
        public new MapObjectType Type { get { return _type; } }
    }
}
