using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using Jx;
using Jx.UI;
using Jx.UI.Forms;
using JxDesign.UI;
using Jx.FileSystem;
using Jx.MapSystem;

namespace JxDesign
{
    public partial class MainForm : Form
    {
        public const string APP_NAME = "地图设计器";

        private static MainForm instance = null; 

        public static MainForm Instance
        {
            get { return instance; }
        }

        private DeserializeDockContent serializeContext;
        private EntitiesForm entitiesForm = new EntitiesForm();
        private PropertiesForm propertiesForm = new PropertiesForm();
        private ConsoleForm consoleForm = new ConsoleForm();
        private ContentForm contentForm = new ContentForm();
        private EntityTypesForm entityTypesForm = new EntityTypesForm();

        private FileSystemWatcher fileSystemWatcher = null;
        private bool watchFileSystem = false;

        private ImageCache imageCache;

        public MainForm()
        {
            this.Hide();
            LongOperationNotifier.LongOperationNotify += LongOperationCallbackManager_LongOperationNotify;

            string p0 = Path.GetDirectoryName(Application.ExecutablePath);
            string filePath = Path.Combine(p0, @"Resources\Splash.jpg");
            SplashScreen.Show(filePath);

            InitializeComponent();
            instance = this;
        }

        private void LongOperationCallbackManager_LongOperationNotify(string text)
        {
            if (text == null)
                return;

            SplashScreen.UpdateStatusText(text);
        }

        
        public bool WatchFileSystem
        {
            get { return watchFileSystem; }
            set { this.watchFileSystem = value; }
        }

        private void SetTheme(VisualStudioToolStripExtender.VsVersion version, ThemeBase theme)
        {
            this.dockPanel.Theme = theme;
            vsToolStripExtender1.SetStyle(this.menuStrip1, version, theme);
            vsToolStripExtender1.SetStyle(this.toolStrip1, version, theme);
            vsToolStripExtender1.SetStyle(this.statusStrip1, version, theme);
        }

        public EntitiesForm EntitiesForm
        {
            get { return entitiesForm; }
        }

        public PropertiesForm PropertiesForm
        {
            get { return propertiesForm; }
        }

        public EntityTypesForm EntityTypesForm
        {
            get { return entityTypesForm; }
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

        private bool SaveLayoutFlag = true;

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
            if (persistString == typeof(EntitiesForm).ToString())
                return entitiesForm;
            else if (persistString == typeof(PropertiesForm).ToString())
                return propertiesForm;
            else if (persistString == typeof(ConsoleForm).ToString())
                return consoleForm;
            else if (persistString == typeof(ContentForm).ToString())
                return contentForm;
            else if (persistString == typeof(EntityTypesForm).ToString())
                return entityTypesForm;
            return null;
        } 

        private void MainForm_Load(object sender, EventArgs e)
        {
            imageCache = new ImageCache(IL16);
            tsmiNew.Image = imageCache["new"];
            tsmiOpen.Image = imageCache["open"];
            tsmiSave.Image = imageCache["save"];
            tsmiSaveAs.Image = imageCache["saveAs"];
            tsmiExit.Image = imageCache["exit"];
            tsbNew.Image = imageCache["new"];
            tsbOpen.Image = imageCache["open"];
            tsbSave.Image = imageCache["save"];
            tsbSaveAs.Image = imageCache["saveAs"];

            Bootstrap();

            //SetTheme(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015LightTheme1);

            AddonManager.Instance.PostInit();

            serializeContext = new DeserializeDockContent(GetContentFromPersistString);
            if (!LoadLayoutConfig())
            {
                entitiesForm.Show(dockPanel, DockState.DockLeft);
                entityTypesForm.Show(dockPanel, DockState.DockLeft);
                propertiesForm.Show(dockPanel, DockState.DockRight);
                contentForm.Show(dockPanel, DockState.Document);
                consoleForm.Show(dockPanel, DockState.DockBottomAutoHide);
            }

            #region Splash Screen

            LongOperationNotifier.LongOperationNotify -= LongOperationCallbackManager_LongOperationNotify;
            this.Show();
            SplashScreen.Hide();
            this.Activate();
            #endregion

            this.WindowState = FormWindowState.Maximized;

            #region File System Watcher
            this.fileSystemWatcher = new FileSystemWatcher();
            #endregion

            UpdateWindowTitle();
            Debug("准备就绪...");
        }

        public void Debug(string format, params object[] args)
        {
            if (ConsoleForm.DefaultInstance == null)
                return;
            ConsoleForm.DefaultInstance.WriteLine(format, args);
        }

        private void Bootstrap()
        {
            EngineApp.Init(new JxDesignApp());

            bool created = EngineApp.Instance.Create();
            if (created)
            {
                EngineApp.Instance.Run();
            }
            //EngineApp.Shutdown();
        }

        public MenuStrip MainMenu
        {
            get { return this.menuStrip1; }
        }

        public ToolStripMenuItem AddonsToolStripMenuItem
        {
            get { return this.addonsToolStripMenuItem; }
        }

        public ToolStrip ToolStripGeneral
        {
            get { return this.toolStrip1; }
        }

        public DockPanel DockPanel
        {
            get { return this.dockPanel; }
        }
 
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayoutConfig(SaveLayoutFlag);

            EngineApp.Shutdown();
        }

        public bool ToolsProcessKeyDownHotKeys(Keys keyCode, Keys modifiers, bool processCharactersWithoutModifiers)
        {
            bool shiftPressing = (modifiers & Keys.Shift) != Keys.None;
            bool ctrlPressing = (modifiers & Keys.Control) != Keys.None;
            bool altPressing = (modifiers & Keys.Alt) != Keys.None;
            bool noFuncKeyPressing = !shiftPressing && !ctrlPressing && !altPressing;
            bool onlyShiftPressing = shiftPressing && !ctrlPressing && !altPressing;
            bool onlyCtrlPressing = ctrlPressing && !shiftPressing && !altPressing;
            bool onlyAltPressing = altPressing && !shiftPressing && !ctrlPressing;

            if (keyCode == Keys.F9 && noFuncKeyPressing)
            {
                 
                return true;
            } 
            return false;
        }

        /// <summary>
        /// 更新窗口标题
        /// </summary>
        public void UpdateWindowTitle()
        {
            string title = !MapWorld.MapLoaded
                        ? APP_NAME
                        : string.Format("{0} - {1}{2}", APP_NAME, Map.Instance.VirtualFileName, MapWorld.Instance.Modified ? "*" : "");
            this.Text = title;
        }

        public void NotifyUpdate(bool entitiesNeedUpdate = true)
        {
            UpdateWindowTitle();

            if(entitiesNeedUpdate)
                EntitiesForm.UpdateData();
        }

        private void tsmiNew_Click(object sender, EventArgs e)
        {
            MapWorld.Instance.New();
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            if( MapWorld.MapLoaded )
                MapWorld.Instance.Save(Map.Instance.VirtualFileName);
            else
                MapWorld.Instance.SaveAs();
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            MapWorld.Instance.SaveAs();
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            MapWorld.Instance.Load();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tsbSave.Enabled = Map.Instance != null && MapWorld.Instance.Modified;
            tsbSaveAs.Enabled = Map.Instance != null;
            tsmiSave.Enabled = Map.Instance != null && MapWorld.Instance.Modified;
            tsmiSaveAs.Enabled = Map.Instance != null;
        }
    }
}
