using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitiesCommon.Behaviors
{
    public class SequenceNodeType : SerialNodeType
    {
        public SequenceNodeType()
        {
            this.firstAnswer = false;
        }
    }

    public class SequenceNode : SerialNode
    {
        private SequenceNodeType _type = null;
        public new SequenceNodeType Type { get { return _type; } }
    }
}
