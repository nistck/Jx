using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Jx.Editors
{
    public class EntityUIEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        { 
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            MessageBox.Show("Editing!");
            return base.EditValue(context, provider, value);
        }
    }
}
