using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Jx.EntitiesCommon.Behaviors
{
    public abstract class LinearNodeType : CompositeNodeType
    {
        [FieldSerialize]
        protected bool firstAnswer; 

        [JxName("终止符")]
        [Description("遍历子节点时，返回什么值就终止遍历.")]
        public bool FirstAnswer
        {
            get { return firstAnswer; }
        }
 
    }

    public abstract class LinearNode : CompositeNode
    {
        private LinearNodeType _type = null;
        public new LinearNodeType Type { get { return _type; } }
    }
}
 