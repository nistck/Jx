using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{ 
    [ManualTypeCreate]
    public sealed class DefaultWorldType : WorldType
    {
    }

    public sealed class DefaultWorld : World
    {
        private DefaultWorldType _type = null;
        public new DefaultWorldType Type { get { return _type; } }
    }
}
