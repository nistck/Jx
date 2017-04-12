using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.EntitySystem;

namespace JxEditor.UI
{
    public partial class EntityTypeNewResourceDialog : Form
    {
        public EntityTypeNewResourceDialog(string directory)
        {
            InitializeComponent();
        }

        public string TypeName { get; private set; }

        public EntityTypes.ClassInfo TypeClass { get; }
    }
}
