using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public abstract class BTDecorator : BTNode
    {
        protected BTNode m_Child; 

        public BTDecorator(BTNode child = null)
            : base()
        {
            this.m_Child = child;
            if (child != null)
                child.Parent = this; 
        }

        public BTNode Child
        {
            get { return m_Child; }
            set {
                if (this.m_Child != null)
                    this.m_Child.Parent = null; 

                this.m_Child = value;
                if (this.m_Child != null)
                    this.m_Child.Parent = this; 
            }
        }
    }
}
