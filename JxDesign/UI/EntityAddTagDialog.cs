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

namespace JxDesign.UI
{
    public partial class EntityAddTagDialog : Form
    {
        public EntityAddTagDialog(Entity entity = null)
        {
            this.Entity = entity;

            InitializeComponent();
        }

        public Entity Entity { get; private set; }

        private void EntityAddTagDialog_Load(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            string name = textName.Text.Trim();
            string value = textValue.Text.Trim(); 
            if( string.IsNullOrEmpty(name) )
            {
                MessageBox.Show("请输入名称", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textName.Text = "";
                textName.Focus();
                return;
            }

            if( Entity != null)
            {
                Entity.TagInfo tagFound = Entity.Tags.Where(_tag => _tag.Name == name).FirstOrDefault();
                if( tagFound != null)
                {
                }
            }

            TagName = name;
            TagValue = value;
            DialogResult = DialogResult.OK;
            Close();
        } 

        public string TagName { get; private set; }
        public string TagValue { get; private set; }
    }
}
