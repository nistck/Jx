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

using JxRes.UI;
using WeifenLuo.WinFormsUI.Docking;

using Jx;
using Jx.UI.Forms;
using Jx.FileSystem;

namespace JxRes
{
    public partial class MainForm : Form
    {
        private static MainForm instance;
        public static MainForm Instance
        {
            get { return instance; }
        }

        private ResourcesForm resourcesForm = new ResourcesForm();
        private PropertiesForm propertiesForm = new PropertiesForm();
        private ConsoleForm consoleForm = new ConsoleForm();
        private ContentForm contentForm = new ContentForm();

        private DeserializeDockContent serializeContext;

        public MainForm()
        {
            InitializeComponent();
            instance = this;
        }

        public PropertiesForm PropertiesForm
        {
            get { return propertiesForm; }
        }

        public ResourcesForm ResourcesForm
        {
            get { return resourcesForm; }
        }
         
        private bool SaveLayoutFlag = true;

        private void Bootstrap()
        { 
            EngineApp.Init(new JxResApp());  

            bool created = EngineApp.Instance.Create();
            if (created)
            {
                EngineApp.Instance.Run();
            }
            EngineApp.Shutdown();
        }

        private string LayoutConfig
        {
            get {
                string _LayoutConfig = string.Format("Base/Constants/{0}Layout.xml", Program.ExecutableName);
                string layoutConfig = VirtualFileSystem.GetRealPathByVirtual(_LayoutConfig);
                return layoutConfig;
            }
        }

        public DockPanel DockPanel
        {
            get { return dockPanel; }
        }

        public MenuStrip MainMenu
        {
            get { return menuStrip; }
        }

        public ToolStrip ToolStripGeneral
        {
            get { return this.toolStrip1; }
        }

        public ToolStripMenuItem AddonsToolStripMenuItem
        {
            get { return addonsToolStripMenuItem; }
        }

        public void NewMessage(string text)
        {
            Log.Info(text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Bootstrap();
            //SetTheme(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015LightTheme1);

            AddonManager.Instance.PostInit();
             
            serializeContext = new DeserializeDockContent(GetContentFromPersistString);
            if( !LoadLayoutConfig() )
            {
                resourcesForm.Show(dockPanel, DockState.DockLeft);
                propertiesForm.Show(dockPanel, DockState.DockRight);
                contentForm.Show(dockPanel, DockState.Document);
                consoleForm.Show(dockPanel, DockState.DockBottomAutoHide);
            }

            resourcesForm.UpdateView();
        }

        public void UpdateLastSelectedResourcePath(string resourcePath)
        {
#if DEBUG_RES
            Log.Info(">> Update Last Selected ResourcePath: {0}", resourcePath);
#endif
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

        private void CloseAllContents()
        {
            // we don't want to create another instance of tool window, set DockPanel to null
            resourcesForm.DockPanel = null;
            propertiesForm.DockPanel = null;
            consoleForm.DockPanel = null;
            contentForm.DockPanel = null;

            // Close all other document windows
            //CloseAllDocuments();

            // IMPORTANT: dispose all float windows.
            foreach (var window in dockPanel.FloatWindows.ToList())
                window.Dispose();

            System.Diagnostics.Debug.Assert(dockPanel.Panes.Count == 0);
            System.Diagnostics.Debug.Assert(dockPanel.Contents.Count == 0);
            System.Diagnostics.Debug.Assert(dockPanel.FloatWindows.Count == 0);
        }

        private void CloseAllDocuments()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    // IMPORANT: dispose all panes.
                    document.DockHandler.DockPanel = null;
                    document.DockHandler.Close();
                }
            }
        }

        private void SetTheme(VisualStudioToolStripExtender.VsVersion version, ThemeBase theme)
        {
            this.dockPanel.Theme = theme;
            vsToolStripExtender1.SetStyle(this.menuStrip, version, theme);
            vsToolStripExtender1.SetStyle(this.toolStrip1, version, theme);
            vsToolStripExtender1.SetStyle(this.statusStrip1, version, theme);
        }


        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(ResourcesForm).ToString())
                return resourcesForm;
            else if (persistString == typeof(PropertiesForm).ToString())
                return propertiesForm;
            else if (persistString == typeof(ConsoleForm).ToString())
                return consoleForm;
            else if (persistString == typeof(ContentForm).ToString())
                return contentForm;   
            return null;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayoutConfig(SaveLayoutFlag);
        }

        private void UIStatus_TIMER_Tick(object sender, EventArgs e)
        {
            tsmiResourcesForm.Checked = resourcesForm.Visible;
            tsmiPropertiesForm.Checked = propertiesForm.Visible;
            tsmiContentForm.Checked = contentForm.Visible;
            tsmiConsoleForm.Checked = consoleForm.Visible;
        }

        private void tsmiPropertiesForm_Click(object sender, EventArgs e)
        {
            propertiesForm.Show(dockPanel);
        }

        private void tsmiResourcesForm_Click(object sender, EventArgs e)
        {
            resourcesForm.Show(dockPanel);
        }

        private void tsmiContentForm_Click(object sender, EventArgs e)
        {
            contentForm.Show(dockPanel);
        }

        private void tsmiConsoleForm_Click(object sender, EventArgs e)
        {
            consoleForm.Show(dockPanel);
        }
    }
}
