using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogicSystemMethodDisplayAttribute : Attribute
    {
        public LogicSystemMethodDisplayAttribute(string displayText, string formatText)
        {
            this.DisplayText = displayText;
            this.FormatText = formatText;
        }

        public string DisplayText { get; private set; }
        public string FormatText { get; private set; }
    }
}
