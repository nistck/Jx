﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using JxRes.UI;
using WeifenLuo.WinFormsUI.Docking;

using Jx;
using Jx.UI;
using Jx.UI.Forms;
using Jx.FileSystem;

using Jx.Drawing;
using Jx.Drawing.Base;
using Jx.Drawing.Common;
using Jx.Drawing.Utilities;

using JxRes.Editors;
 

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

        [Config("Loading", "showSplashScreen")]
        private static bool showSplashScreen = false; 

        public MainForm()
        {
            if (showSplashScreen)
            {
                this.Hide();
                LongOperationNotifier.LongOperationNotify += LongOperationCallbackManager_LongOperationNotify;

                string p0 = Path.GetDirectoryName(Application.ExecutablePath);
                string filePath = Path.Combine(p0, @"Resources\Splash.jpg");
                SplashScreen.Show(filePath);
            }

            InitializeComponent();
            instance = this;
        }

        private void LongOperationCallbackManager_LongOperationNotify(string text)
        {
            if (text == null)
                return; 
             
            SplashScreen.UpdateStatusText(text); 
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

        private void Bootstrap(int loopInterval = 20)
        { 
            JxEngineApp.Init(new JxResApp(loopInterval));  

            bool created = JxEngineApp.Instance.Create();
            if (created)
            {
                JxEngineApp.Instance.Run();
            }
            //EngineApp.Shutdown();
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

            #region Splash Screen
            if (showSplashScreen)
            {
                LongOperationNotifier.LongOperationNotify -= LongOperationCallbackManager_LongOperationNotify;
                this.Show();
                SplashScreen.Hide();
                this.Activate();
            }
            #endregion

            this.WindowState = FormWindowState.Maximized;

            Debug("准备就绪...");
        }

        public void Debug(string format, params object[] args)
        {
            if (ConsoleForm.DefaultInstance == null)
                return;
            ConsoleForm.DefaultInstance.WriteLine(format, args);
        }

        private DrawingPanel Canvas
        {
            get { return contentForm.Canvas; }
        }

        private Text CreateCanvasText(string text, PointF pt)
        {
            Jx.Drawing.Base.Text textObj = new Jx.Drawing.Base.Text();
            textObj.DisplayedText = text??"";
            textObj.Location = pt;
            textObj.Font = new Font("Courier New", 12, FontStyle.Regular);
            Canvas.Shapes.Add(textObj);
            return textObj;
        }

        public void UpdateLastSelectedResourcePath(string resourcePath, TreeNode node = null)
        {
            Canvas.Enabled = true;
            Canvas.Shapes.Clear();

            int X0 = 20;
            int Y0 = 20;
            int textLineHeight = 20;

            ResourceObjectEditor objectEditor = JxResApp.Instance.ResourceObjectEditor;
            if( objectEditor is EntityTypeResourceEditor )
            {
                EntityTypeResourceEditor entityTypeEditor = objectEditor as EntityTypeResourceEditor;

                Jx.Drawing.Base.Text text = new Jx.Drawing.Base.Text();
                text.DisplayedText = resourcePath;
                text.Location = new PointF(10, 10); 
                text.Font = new Font("Courier New", 10, FontStyle.Regular);
                Canvas.Shapes.Add(text);

            }
            else
            {
                int x = X0, y = Y0; 
                List<string> textMessages = new List<string>();
                textMessages.Add(
                        string.Format("路径: {0}", resourcePath)
                    );
                textMessages.Add(
                        "Ok"
                    );
 
                for(int i = 0; i < textMessages.Count; i ++)
                {
                    PointF pt = new PointF(x, y);
                    Text tx = CreateCanvasText(textMessages[i], pt);
                    if (tx != null)
                        tx.Transformer.Translate(x, y);

                   y += textLineHeight;
                }

                //Canvas.Enabled = false;
            }

#if DEBUG_RES
            Log.Info(">> Update Last Selected ResourcePath: {0}", resourcePath);
#endif
            Canvas.Invalidate();
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

            JxEngineApp.Shutdown();
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
