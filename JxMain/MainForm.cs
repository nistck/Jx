using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jx
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LabelInfo labelInfo = new LabelInfo()
            {
                Label = "New"
            };

            jxPropertyGrid1.SelectedObject = jxPropertyGrid1;
            jxPropertyGrid2.SelectedObject = labelInfo;

            jxPropertyGrid1.ReadOnly = true;  
        }

        public class LabelInfo : JxObject
        {
            private string label;
            private object tag;

            [JxProperty("中文标签")]
            [Description("...")]
            public string Label
            {
                get { return label; }
                set { this.label = value; }
            }


            public object Tag
            {
                get { return tag; }
                set { this.tag = value; }
            }

            private DateTime dt = DateTime.Now;

            [Editor(typeof(Jx.UI.PGEx.JxUIDateTimeEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public DateTime Dt
            {
                get { return dt; }
                set { this.dt = value; }
            }

            private DateTime t = DateTime.Now;
            [Editor(typeof(Jx.UI.PGEx.JxUITimeOnlyEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public DateTime T
            {
                get { return t; }
                set { this.t = value; }
            }

            private string filePath;
            [Editor(typeof(Jx.UI.PGEx.JxUIFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string FilePath
            {
                get { return filePath; }
                set { this.filePath = value; }
            }
        }
    }
}
