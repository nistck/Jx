using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Entities.Behaviors
{
    [JxName("行为树动作节点")]
    public class ActionNodeType : BehaviorNodeType
    {

    }

    [JxName("行为树动作节点")]
    public class ActionNode : BehaviorNode
    {
        private ActionNodeType _type = null; 
        public new ActionNodeType Type { get { return _type; } }
    }
}
