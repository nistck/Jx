using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Entities.Behaviors
{
    public abstract class DeciderNodeType : BehaviorTreeNodeType
    {
    }

    public abstract class DeciderNode : BehaviorTreeNode
    {
        private DeciderNodeType _type = null; 
        public new DeciderNodeType Type { get { return _type; } }
    }
}
