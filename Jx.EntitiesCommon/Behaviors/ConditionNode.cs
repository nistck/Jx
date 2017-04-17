using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitiesCommon.Behaviors
{
    public abstract class ConditionNodeType : BehaviorNodeType
    {
    }

    public abstract class ConditionNode : BehaviorNode
    {
        private ConditionNodeType _type = null; 
        public new ConditionNodeType Type { get { return _type; } } 

    }
}
