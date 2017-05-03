using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using Jx.UI.Editors;
using JxRes.Editors;

namespace JxRes.Descriptors
{
    internal class EntityTypeExtendedFunctionalityDescriptor : ExtendedFunctionalityDescriptor
    {
        private EntityTypeResourceEditor editor;

        private Button helpButton;

        public EntityTypeExtendedFunctionalityDescriptor(Control parentControl, object owner)
            : base(parentControl, owner)
        {
            this.editor = owner as EntityTypeResourceEditor;

            this.helpButton = new Button();
            this.helpButton.Text = "&Help";
            parentControl.Controls.Add(this.helpButton);

            this.helpButton.Location = new Point(10, 10);
            this.helpButton.Size = new Size(90, 32);
            this.helpButton.Click += HelpButton_Click;
            this.helpButton.Enabled = true; 
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            if (this.editor == null)
                return;

            string message = string.Format("Edit: {0}", this.editor.EntityType);
            MessageBox.Show(message);
        }
    }
}
