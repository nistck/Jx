using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JxRes.UI
{
    public partial class ResourcesForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ResourcesForm()
        {
            InitializeComponent();
        }

        public bool WatchFileSystem { get; set; }

        public void UpdateAddResource(string p)
        {

        }

        public void SelectNodeByPath(string p)
        {

        }
    }
}
