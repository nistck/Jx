using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.EntitiesCommon
{
    public class SimpleWorldType : WorldType
    {
    }

    public class SimpleWorld : World
    {
        private SimpleWorldType _type = null; 
        public new SimpleWorldType Type { get { return _type; } }
    }
}
