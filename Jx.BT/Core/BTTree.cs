using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public class BTTree
    {
        protected BTNode _root; 

        public BTNode Root
        {
            get { return _root; }
            set { this._root = value; }
        }
 
        public BTResult Tick(BTContext context)
        {
            context.OnSessionStart(); 
            BTResult result = BTResult.Running;

            try
            {
                if (Root != null)
                    result = Root.Tick(context);
            }
            finally {
                context.OnSessionEnd(); 
            }
            return result; 
        }
    }
}
