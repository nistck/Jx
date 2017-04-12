using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigAttribute : Attribute
    {
        public ConfigAttribute(string domain, string name)
        {
            this.Domain = domain;
            this.Name = name;
        }

        public string Domain { get; private set; }
        public string Name { get; private set; }
    }
}
