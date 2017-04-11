using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogicSystemCallStaticOverInstanceAttribute : Attribute
    {
        public LogicSystemCallStaticOverInstanceAttribute()
        {

        }
    }
}
