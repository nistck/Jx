using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace Jx
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(string description, string localizationGroup)
            : base(description)
        {
            this.LocalizationGroup = localizationGroup;
        }
         
        public string LocalizationGroup { get; private set; }
    }
}
