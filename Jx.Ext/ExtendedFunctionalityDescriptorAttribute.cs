using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public class ExtendedFunctionalityDescriptorAttribute : Attribute
    {
        private Type descriptorType;
        private string descriptorTypeName;
        public Type DescriptorType
        {
            get
            {
                return this.descriptorType;
            }
        }
        public string DescriptorTypeName
        {
            get
            {
                return this.descriptorTypeName;
            }
        }
        public ExtendedFunctionalityDescriptorAttribute(Type descriptorType)
        {
            this.descriptorType = descriptorType;
        }
        public ExtendedFunctionalityDescriptorAttribute(string descriptorTypeName)
        {
            this.descriptorTypeName = descriptorTypeName;
        }
    }
}
