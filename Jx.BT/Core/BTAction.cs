using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.BT
{
    public abstract class BTActionType : BTLeafNodeType
    {

    }

    public abstract class BTAction : BTLeafNode
    {
        private BTActionType _type;
        public new BTActionType Type { get { return _type; } }            
    }
}
