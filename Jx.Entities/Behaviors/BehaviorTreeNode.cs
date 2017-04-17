using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.EntitySystem;

namespace Jx.Entities.Behaviors
{
    public abstract class BehaviorTreeNodeType : EntityType
    {
        [FieldSerialize]
        private string desc; 

        [JxName("节点描述")]
        public string Desc
        {
            get { return desc; }
            set { this.desc = value; }
        } 
    }

    public abstract class BehaviorTreeNode : Entity
    {
        private BehaviorTreeNodeType _type = null; 
        public new BehaviorTreeNodeType Type { get { return _type; } }
    }
}
