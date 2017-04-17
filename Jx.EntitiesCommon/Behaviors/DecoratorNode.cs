using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitiesCommon.Behaviors
{
    public abstract class DecoratorNodeType : DeciderNodeType
    {
    }

    public abstract class DecoratorNode : DeciderNode
    {
        private DecoratorNodeType _type = null;
        public new DecoratorNodeType Type { get { return _type; } }
    }
}
