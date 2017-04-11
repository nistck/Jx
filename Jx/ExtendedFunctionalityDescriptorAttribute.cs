using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public class ExtendedFunctionalityDescriptorAttribute : Attribute
    {
        private Type Ep;
        private string EQ;
        public Type DescriptorType
        {
            get
            {
                return this.Ep;
            }
        }
        public string DescriptorTypeName
        {
            get
            {
                return this.EQ;
            }
        }
        public ExtendedFunctionalityDescriptorAttribute(Type descriptorType)
        {
            this.Ep = descriptorType;
        }
        public ExtendedFunctionalityDescriptorAttribute(string descriptorTypeName)
        {
            this.EQ = descriptorTypeName;
        }
    }
}
