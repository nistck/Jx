using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.EntitySystem;

namespace Jx.Entities.Behaviors
{
    [JxName("行为树")]
    public class BehaviorTreeType : EntityType
    {
        [FieldSerialize]
        private BehaviorTreeNodeType rootNodeType = null;

        [JxName("根节点")]
        public BehaviorTreeNodeType RootNodeType
        { 
            get { return rootNodeType; }
            set { this.rootNodeType = value; }
        }
    }

    [JxName("行为树")]
    public class BehaviorTree : Entity
    {
        private BehaviorTreeType _type = null; 
        public new BehaviorTreeType Type { get { return _type; } } 

    }
}
