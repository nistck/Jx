using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.UI.Editors
{
    public abstract class ResourceEditorInterface
    {
        private static ResourceEditorInterface instance = null; 

        public static ResourceEditorInterface Instance
        {
            get { return instance; }
        }

        public static void Init(ResourceEditorInterface overridedObject)
        {
            instance = overridedObject;
        }

        public abstract object SendCustomMessage(string message, object param);
    }
}
