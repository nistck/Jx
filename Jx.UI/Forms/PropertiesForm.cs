using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jx.UI.Forms
{
    public partial class PropertiesForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public PropertiesForm()
        {
            InitializeComponent();
        }

        private void PropertiesForm_Load(object sender, EventArgs e)
        {
            
        }

        public void SelectObjects(object[] objects, bool invalidate = true)
        {
            jxPropertyGrid.SelectedObjects = objects;
        }

        public void SelectObject(object obj)
        {
            jxPropertyGrid.SelectedObject = obj;
        }

        public void RefreshProperties()
        {

        }
    }
}
