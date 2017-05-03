using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{ 
    [ManualTypeCreate]
    public class DefaultWorldType : WorldType
    {
    }

    public class DefaultWorld : World
    {
        private DefaultWorldType _type = null;
        public new DefaultWorldType Type { get { return _type; } }
    }
}
