using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManualTypeCreateAttribute : Attribute
    {
        private string typeName;

        public ManualTypeCreateAttribute()
        {

        }

        public ManualTypeCreateAttribute(string typeName)
        {
            this.typeName = typeName;
        }

        public string TypeName
        {
            get { return typeName; }
        }
    }
}
