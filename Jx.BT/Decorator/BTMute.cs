using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    [BTProperty("静默", BTConstants.GROUP_DECORATOR)]
    public class BTMute : BTDecorator
    {
        private BTResult result = BTResult.Running; 

        public BTResult Result
        {
            get { return result; }
            set { this.result = value; }
        }

        public BTMute(BTNode child = null)
            : base(child)
        {
        }

        protected override BTResult OnTick(BTContext context)
        {
            m_Child.Tick(context); 
            return result; 
        }

        public override string ToString()
        {
            string text = string.Format("{0}", result);
            return text;
        }
    }
}
