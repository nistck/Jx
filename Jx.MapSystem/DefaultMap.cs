using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.MapSystem
{
    [ManualTypeCreate]
    public sealed class DefaultMapType : MapType
    {
    }

    public sealed class DefaultMap : Map
    {
        private DefaultMapType _type = null;
        public new DefaultMapType Type { get { return _type; } }
    }
}
