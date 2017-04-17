using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Jx.EntitiesCommon.Behaviors
{
    public abstract class SerialNodeType : CompositeNodeType
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

    public abstract class SerialNode : CompositeNode
    {
        private SerialNodeType _type = null;
        public new SerialNodeType Type { get { return _type; } }
    }
}
 