using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.EntitySystem;

namespace Jx.EntitiesCommon.Behaviors
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

        private BehaviorNode rootNode;

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);
            rootNode = Entities.Instance.Create(Type.RootNodeType, World.Instance) as BehaviorNode;
        }

        public virtual bool Evaluate()
        {
            if (rootNode == null)
                return false;

            bool result = rootNode.Evaluate();
            return result;
        }
    }
}
