using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public class BTTrue : BTConstraint
    {
        protected override bool Evaluate(BTContext context)
        {
            return true;
        }
    }
}
