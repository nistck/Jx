using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogicSystemClassDisplayAttribute : Attribute
    {
        public LogicSystemClassDisplayAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }

        public string DisplayName { get; private set; }
    }
}
