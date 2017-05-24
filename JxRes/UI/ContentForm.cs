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
    public partial class ContentForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ContentForm()
        {
            InitializeComponent();
        }

        public Jx.Drawing.Base.DrawingPanel Canvas
        {
            get { return drawingPanel; }
        }

        private void MainViewForm_Load(object sender, EventArgs e)
        {

        }
    }
}
