using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public class LogicSystemBrowsableAttribute : Attribute
    {
        private bool Cx;
        public bool Browsable
        {
            get
            {
                return this.Cx;
            }
        }
        public LogicSystemBrowsableAttribute(bool browsable)
        {
            this.Cx = browsable;
        }
    }
}
