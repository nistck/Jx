using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JxEditor.UI;
using WeifenLuo.WinFormsUI.Docking;

using Jx.UI.Forms;

namespace JxEditor
{
    public partial class MainForm : Form
    {
        private ResourcesForm resourcesForm;
        private PropertiesForm propertiesForm;
        private ConsoleForm consoleForm;
        private MainViewForm mainViewForm;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            resourcesForm = new ResourcesForm();
            resourcesForm.Show(dockPanel, DockState.DockLeft);

            propertiesForm = new PropertiesForm();
            propertiesForm.Show(dockPanel, DockState.DockRight);

            consoleForm = new ConsoleForm();
            consoleForm.Show(dockPanel, DockState.DockBottomAutoHide);

            mainViewForm = new MainViewForm();
            mainViewForm.Show(dockPanel, DockState.Document);

            SetTheme(VisualStudioToolStripExtender.VsVersion.Vs2015, this.vS2015LightTheme1);
        }

        private void SetTheme(VisualStudioToolStripExtender.VsVersion version, ThemeBase theme)
        {
            this.dockPanel.Theme = theme;
            vsToolStripExtender1.SetStyle(this.menuStrip1, version, theme);
            vsToolStripExtender1.SetStyle(this.toolStrip1, version, theme);
            vsToolStripExtender1.SetStyle(this.statusStrip1, version, theme);
        }
    }
}
