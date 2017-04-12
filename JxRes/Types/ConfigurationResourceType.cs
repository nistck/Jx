using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Jx.Editors;
using JxRes.Editors;

namespace JxRes.Types
{
    internal class ConfigurationResourceType : ResourceType
    {
        public override Type ResourceObjectEditorType
        {
            get
            { 
                return typeof(ConfigurationEditor);
            }
        }
        public ConfigurationResourceType(string name, string displayName, string[] extensions, Image icon)
            : base(name, displayName, extensions, icon)
        {
        }
    }
}
