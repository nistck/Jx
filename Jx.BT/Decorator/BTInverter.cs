using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    [BTProperty("反转", BTConstants.GROUP_DECORATOR)]
    public class BTInverter : BTDecorator
    {
        public BTInverter(BTNode child = null) 
            : base(child)
        {
 
        }

        public BTInverter() : this(null) { }
        

        protected override BTResult OnTick(BTContext context)
        {
            BTResult result = m_Child.Tick(context);
            switch (result.Code)
            {
                case BTResultCode.Running:
                    return BTResult.Running;
                case BTResultCode.Success:
                    return BTResult.Failed;
                case BTResultCode.Failed:
                    return BTResult.Success;
            }
            return BTResult.Failed;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
