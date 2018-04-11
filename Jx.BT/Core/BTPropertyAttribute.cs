using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BTPropertyAttribute : Attribute
    {
        public BTPropertyAttribute(string label, string group  = null, string desc = null)
        {
            this.Label = label;
            this.Group = group;
            this.Desc = desc;
        }

        public string Label { get; private set; }
        public string Group { get; private set; }
        public string Desc { get; private set; }
    }
}
