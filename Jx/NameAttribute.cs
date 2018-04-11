using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {

        public NameAttribute(string name)
        {
            this.Name = name;
        }

        public NameAttribute(
            string name, string description, string category,
            Type type, bool readOnly, bool expandable)
        {
            this.Name = name;
            this.Description = description;
            this.Category = category;
            this.Type = type;
            this.ReadOnly = readOnly;
            this.Expandable = expandable;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public Type Type { get; private set; }
        public bool ReadOnly { get; private set; }
        public bool Expandable { get; private set; }
    }
}
