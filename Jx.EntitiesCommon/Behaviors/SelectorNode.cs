using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitiesCommon.Behaviors
{
    public class SelectorNodeType : LinearNodeType
    {
        public SelectorNodeType()
        {
            this.firstAnswer = true;
        }
    }

    public class SelectorNode : LinearNode
    {
        private SelectorNodeType _type = null;
        public new SelectorNodeType Type { get { return _type; } }
    }
}
