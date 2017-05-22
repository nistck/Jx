using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.FileSystem;

namespace JxDesign.UI
{
    public partial class NewMapForm : Form
    {
        public NewMapForm()
        {
            InitializeComponent();
        }

        public NewConfig Config { get; private set; }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            string p = textDir.Text.Trim();
            if( string.IsNullOrEmpty(p) )
            {
                MessageBox.Show("请设定保存路径", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textDir.Text = "";
                buttonBrowse.Focus();
                return;
            }

            Config.MapDirectory = p;
            Config.State = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            string p = VirtualFileSystem.GetRealPathByVirtual(@"Maps");
            try
            {
                Directory.CreateDirectory(p);
            }
            catch (Exception) { }
            //FBD.RootFolder = Environment.SpecialFolder.Windows;
            FBD.SelectedPath = p;
            FBD.ShowNewFolderButton = true;
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                string p1 = FBD.SelectedPath;
                string d = VirtualFileSystem.GetVirtualPathByReal(p1);
                if (string.IsNullOrEmpty(d))
                    d = p1;
                textDir.Text = d;
            }
            else
                textDir.Text = ""; 
        }

        private void NewMapForm_Load(object sender, EventArgs e)
        {
            Config = new NewConfig();
        }
    }
}
