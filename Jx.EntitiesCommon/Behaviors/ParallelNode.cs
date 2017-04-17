using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitiesCommon.Behaviors
{
    public class ParallelNodeType : CompositeNodeType
    {
        public enum ParallelPolicy
        {
            Selector,   // 全true -> true
            Sequence,   // 全false -> false
            Hibird      // 指定数量true -> true, 指定数量false -> false
        }

        [FieldSerialize]
        private ParallelPolicy policy = ParallelPolicy.Selector;

        public ParallelPolicy Policy
        {
            get { return policy; }
            set { this.policy = value; }
        }
    }

    public class ParallelNode : CompositeNode
    {
        private ParallelNodeType _type = null; 
        public new ParallelNodeType Type { get { return _type; } }
    }
}
