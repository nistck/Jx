using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.BT
{
    public abstract class BTLeafNodeType : BTNodeType
    {

    }

    /// <summary>
    /// 叶子节点
    /// </summary>
    public abstract class BTLeafNode : BTNode
    {
        private BTLeafNodeType _type;
        public new BTLeafNodeType Type { get { return _type; } }
    }
}
