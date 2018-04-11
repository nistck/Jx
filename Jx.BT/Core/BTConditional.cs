using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public abstract class BTConditional : BTLeafNode
    {
        private bool inverseResult = false; 
        public bool InverseResult
        {
            get { return inverseResult; }
            set { this.inverseResult = value; }
        }

        protected override BTResult OnTick(BTContext context)
        {
            bool r = Check(context);
            if (inverseResult)
                r = !r;
            if (r)
            {
                return BTResult.Success;
            }
            else
            {
                return BTResult.Failed;
            }
        }

        protected virtual bool Check(BTContext context)
        {
            return false;
        }
    }
}
