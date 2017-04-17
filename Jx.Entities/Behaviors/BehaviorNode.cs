using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Entities.Behaviors
{
    public abstract class BehaviorNodeType : BehaviorTreeNodeType
    {
    }

    public abstract class BehaviorNode : BehaviorTreeNode
    {
        private BehaviorNodeType _type = null; 
        public new BehaviorNodeType Type { get { return _type; } }
    }
}
