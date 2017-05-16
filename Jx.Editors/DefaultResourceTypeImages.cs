using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Jx.Editors.Properties;

namespace Jx.Editors
{
    public class DefaultResourceTypeImages
    {
        public static Bitmap GetByName(string name)
        {
            object @object = Resources.ResourceManager.GetObject(name);
            return (Bitmap)@object;
        }

        public static Bitmap Config_16
        {
            get { return GetByName("Config_16"); }
        }

        public static Bitmap EntityType_16
        {
            get { return GetByName("EntityType_16");  }
        }
    }
}
