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
using WeifenLuo.WinFormsUI.Docking;

using Jx.UI.Forms;
using JxDesign.UI;
using Jx.FileSystem;

namespace JxDesign
{
    public partial class MainForm : Form
    {
        private static MainForm instance = null; 

        public static MainForm Instance
        {
            get { return instance; }
        }

        private DeserializeDockContent serializeContext;
        private DesignerEntitiesForm entitiesForm = new DesignerEntitiesForm();
        private PropertiesForm propertiesForm = new PropertiesForm();
        private ConsoleForm consoleForm = new ConsoleForm();
        private ContentForm contentForm = new ContentForm();


        public MainForm()
        {
            InitializeComponent();
            instance = this;
        }

        public DesignerEntitiesForm MapEntitiesForm
        {
            get { return entitiesForm; }
        }

        public PropertiesForm PropertiesForm
        {
            get { return propertiesForm; }
        }


        public void ShowPropertiesForm()
        {

        }

        public void ShowMapEntitiesForm()
        {

        }

        private string LayoutConfig
        {
            get
            {
                string _LayoutConfig = string.Format("Base/Constants/{0}Layout.xml", Program.ExecutableName);
                string layoutConfig = VirtualFileSystem.GetRealPathByVirtual(_LayoutConfig);
                return layoutConfig;
            }
        }

        private bool LoadLayoutConfig()
        {
            if (string.IsNullOrEmpty(LayoutConfig))
                return false;

            if (File.Exists(LayoutConfig))
            {
                dockPanel.LoadFromXml(LayoutConfig, serializeContext);
                return true;
            }
            return false;
        }

        private void SaveLayoutConfig(bool saveLayout = true)
        {
            if (string.IsNullOrEmpty(LayoutConfig))
                return;

            if (saveLayout)
                dockPanel.SaveAsXml(LayoutConfig);
            else if (File.Exists(LayoutConfig))
                File.Delete(LayoutConfig);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DesignerEntitiesForm).ToString())
                return entitiesForm;
            else if (persistString == typeof(PropertiesForm).ToString())
                return propertiesForm;
            else if (persistString == typeof(ConsoleForm).ToString())
                return consoleForm;
            else if (persistString == typeof(ContentForm).ToString())
                return contentForm;
            return null;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            serializeContext = new DeserializeDockContent(GetContentFromPersistString);
            if (!LoadLayoutConfig())
            {
                entitiesForm.Show(dockPanel, DockState.DockLeft);
                propertiesForm.Show(dockPanel, DockState.DockRight);
                contentForm.Show(dockPanel, DockState.Document);
                consoleForm.Show(dockPanel, DockState.DockBottomAutoHide);
            }
        }

        public MenuStrip MainMenu
        {
            get { return this.menuStrip1; }
        }

        public ToolStripMenuItem AddonsToolStripMenuItem
        {
            get { return null; }
        }

        public ToolStrip ToolStripGeneral
        {
            get { return this.toolStrip1; }
        }

        public DockPanel DockPanel
        {
            get { return this.dockPanel; }
        }

        public bool MapModified { get; set; }
    }
}
