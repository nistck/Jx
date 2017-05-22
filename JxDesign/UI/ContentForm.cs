using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JxDesign.UI
{
    public partial class ContentForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event EventHandler CanvasMouseDown;
        public ContentForm()
        {
            InitializeComponent();
        }

        private void MainViewForm_Load(object sender, EventArgs e)
        {

        }

        public void SetContentLabel(string format, params object[] args)
        {
            if( format == null )
            {
                ContentLabel.Text = "";
                return; 
            }

            string text = string.Format(format, args);
            ContentLabel.Text = text;
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (!MapWorld.MapLoaded)
                return;

            if (CanvasMouseDown != null)
                CanvasMouseDown(sender, e);
        }

        private bool AnySelected
        {
            get {
                if (!MapWorld.MapLoaded)
                    return false; 

                Tuple<EntityTypeSelectItemType, object>  t = MainForm.Instance.TypeSelected;
                return t != null && t.Item1 != EntityTypeSelectItemType.Null;
            }
        }
 
        private void Canvas_MouseEnter(object sender, EventArgs e)
        {
 
        }

        private void Canvas_MouseLeave(object sender, EventArgs e)
        {
 
        }

        private void Canvas_MouseHover(object sender, EventArgs e)
        {
 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Cursor = AnySelected ? Cursors.Hand : Cursors.Default;
        }
    }
}
