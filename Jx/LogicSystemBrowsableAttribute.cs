using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public class LogicSystemBrowsableAttribute : Attribute
    {
        private bool browsable;
        public bool Browsable
        {
            get
            {
                return this.browsable;
            }
        }
        public LogicSystemBrowsableAttribute(bool browsable)
        {
            this.browsable = browsable;
        }
    }
}
