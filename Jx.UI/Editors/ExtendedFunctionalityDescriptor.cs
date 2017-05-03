using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jx.UI.Editors
{
    public class ExtendedFunctionalityDescriptor
    {
        public ExtendedFunctionalityDescriptor(Control parentControl, object owner)
        {
            this.ParentControl = parentControl;
            this.Owner = owner;
        }

        public object Owner { get; private set; }
        public Control ParentControl { get; private set; }

        public virtual void Dispose()
        {

        }
    }
}
