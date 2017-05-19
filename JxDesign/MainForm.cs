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
using Jx.Ext;
using Jx.UI.Forms;
using JxDesign.UI;
using Jx.FileSystem;
using Jx.MapSystem;
using Jx.EntitySystem;

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

        private bool Bjc { get; set; }

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
            tsbUndo.Image = imageCache["undo"];
            tsbRedo.Image = imageCache["redo"];

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

            InitializeEntityTypesForm();
            InitializeContentForm();
            InitializeEntitiesForm();
            InitializePropertiesForm();

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

            DestroyEntityTypesForm();
            DestroyContentForm();
            DestroyEntitiesForm();
            DestroyPropertiesForm();
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

        #region 属性窗口
        private void InitializePropertiesForm()
        {
            if (PropertiesForm == null)
                return;
            PropertiesForm.PropertyItemDoubleClick += new EventHandler(propertiesForm_PropertyItemDoubleClick);
            PropertiesForm.SelectedGridItemChanged += new PropertiesForm.SelectedGridItemChangedDelegate(propertiesForm_SelectedGridItemChanged);
            PropertiesForm.ProcessCmdKeyEvent += new PropertiesForm.ProcessCmdKeyEventDelegate(propertiesForm_ProcessCmdKeyEvent);
            PropertiesForm.ContextMenuOpening += new PropertiesForm.ContextMenuOpeningDelegate(propertiesForm_ContextMenuOpening);
            PropertiesForm.ModalDialogCollectionEditorOK += new PropertiesForm.ModalDialogCollectionEditorOKDelegate(propertiesForm_ModalDialogCollectionEditorOK);
            PropertiesForm.PropertyValueChanged += new PropertiesForm.PropertyValueChangeDelegate(propertiesForm_PropertyValueChanged);
        }

        private void DestroyPropertiesForm()
        {
            if (PropertiesForm == null)
                return;
            PropertiesForm.PropertyItemDoubleClick -= new EventHandler(propertiesForm_PropertyItemDoubleClick);
            PropertiesForm.SelectedGridItemChanged -= new PropertiesForm.SelectedGridItemChangedDelegate(propertiesForm_SelectedGridItemChanged);
            PropertiesForm.ProcessCmdKeyEvent -= new PropertiesForm.ProcessCmdKeyEventDelegate(propertiesForm_ProcessCmdKeyEvent);
            PropertiesForm.ContextMenuOpening -= new PropertiesForm.ContextMenuOpeningDelegate(propertiesForm_ContextMenuOpening);
            PropertiesForm.ModalDialogCollectionEditorOK -= new PropertiesForm.ModalDialogCollectionEditorOKDelegate(propertiesForm_ModalDialogCollectionEditorOK);
            PropertiesForm.PropertyValueChanged -= new PropertiesForm.PropertyValueChangeDelegate(propertiesForm_PropertyValueChanged);

        }

        void propertiesForm_PropertyValueChanged(object obj, GridItem gridItem, object obj2)
        {
            if (!this.Bjc)
            {
                List<object> list = this.A(gridItem);
                if (list.Count != 0 && gridItem.Tag != null)
                {
                    object[] array = (object[])gridItem.Tag;
                    List<UndoObjectsPropertyChangeAction.Item> list2 = new List<UndoObjectsPropertyChangeAction.Item>(list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        object obj3 = list[i];
                        object obj4 = array[i];
                        PropertyInfo property = list[i].GetType().GetProperty(gridItem.PropertyDescriptor.Name, gridItem.PropertyDescriptor.PropertyType);
                        if (property != null)
                        {
                            object value = property.GetValue(obj3, null);
                            if (!object.Equals(value, obj4))
                            {
                                list2.Add(new UndoObjectsPropertyChangeAction.Item(obj3, property, obj4));
                            }
                            if (obj3 is MapObject)
                            {
                                PropertyInfo property2 = obj3.GetType().GetProperty("AutoVerticalAlignment");
                                if (property2 == property)
                                {
                                    PropertyInfo property3 = typeof(MapObject).GetProperty("Position");
                                    PropertyInfo property4 = typeof(MapObject).GetProperty("Rotation");
                                    MapObject mapObject = (MapObject)obj3; 
                                }
                            }
                            if (obj3 is MapObject)
                            {
                                PropertyInfo property5 = obj3.GetType().GetProperty("EditorLayer");
                                if (property5 == property)
                                {
                                    foreach (Entity current in EntityWorld.Instance.SelectedEntities)
                                    {
                                        MapObject mapObject2 = current as MapObject;
                                        if (mapObject2 != null)
                                        {
                                            throw new Exception("this.MapEntitiesForm.UpdateEntityLayer(mapObject2);");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (list2.Count != 0)
                    {
                        UndoObjectsPropertyChangeAction action = new UndoObjectsPropertyChangeAction(list2.ToArray());
                        UndoSystem.Instance.CommitAction(action);
                        MapWorld.Instance.Modified = true;
                    }
                    this.a(gridItem);
                    return;
                }
            }
            else
            {
                this.Bjc = false;
            }
        }

        void propertiesForm_ModalDialogCollectionEditorOK()
        {
            MapWorld.Instance.Modified = true;
        }

        void propertiesForm_ContextMenuOpening(ContextMenuStrip contextMenuStrip)
        {
            if (this.PropertiesForm.SelectedGridItem != null && this.PropertiesForm.SelectedGridItem.Label == "Tags")
            {
                System.Collections.Generic.List<Entity> entities = EntityWorld.Instance.SelectedEntities;
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                //*
                string text = ToolsLocalization.Translate("Various", "Add Tag");
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(text, null, delegate (object s, System.EventArgs e2)
                {
                    EntityAddTagDialog entityAddTagDialog = new EntityAddTagDialog();
                    if (entityAddTagDialog.ShowDialog() == DialogResult.OK)
                    {
                        List<UndoObjectsPropertyChangeAction.Item> list = new List<UndoObjectsPropertyChangeAction.Item>();
                        foreach (Entity current4 in entities)
                        {
                            List<Entity.TagInfo> list2 = new List<Entity.TagInfo>();
                            foreach (Entity.TagInfo current5 in current4.Tags)
                            {
                                list2.Add(new Entity.TagInfo(current5.Name, current5.Value));
                            }
                            list.Add(new UndoObjectsPropertyChangeAction.Item(current4, typeof(Entity).GetProperty("Tags"), list2));
                        }
                        UndoSystem.Instance.CommitAction(new UndoObjectsPropertyChangeAction(list.ToArray()));
                        foreach (Entity current6 in entities)
                        {
                            current6.SetTag(entityAddTagDialog.TagName, entityAddTagDialog.TagValue);
                        }
                        MapWorld.Instance.Modified = true;
                    }
                });
                toolStripMenuItem.Enabled = true;
                contextMenuStrip.Items.Add(toolStripMenuItem);
 
                List<string> set = entities.SelectMany(_entity => _entity.Tags.Select(_tag => _tag.Name)).Distinct().ToList();
                 
                string text2 = ToolsLocalization.Translate("Various", "Remove Tag");
                ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(text2);
                toolStripMenuItem2.Enabled = (set.Count != 0);
                contextMenuStrip.Items.Add(toolStripMenuItem2);

                foreach (string current3 in set)
                {
                    ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem(current3, null, delegate (object s, System.EventArgs e2)
                    {
                        string name = (string)((ToolStripMenuItem)s).Tag;
                        System.Collections.Generic.List<UndoObjectsPropertyChangeAction.Item> list = new System.Collections.Generic.List<UndoObjectsPropertyChangeAction.Item>();
                        foreach (Entity current4 in entities)
                        {
                            System.Collections.Generic.List<Entity.TagInfo> list2 = new System.Collections.Generic.List<Entity.TagInfo>();
                            foreach (Entity.TagInfo current5 in current4.Tags)
                            {
                                list2.Add(new Entity.TagInfo(current5.Name, current5.Value));
                            }
                            list.Add(new UndoObjectsPropertyChangeAction.Item(current4, typeof(Entity).GetProperty("Tags"), list2));
                        }
                        UndoSystem.Instance.CommitAction(new UndoObjectsPropertyChangeAction(list.ToArray()));
                        foreach (Entity current6 in entities)
                        {
                            current6.RemoveTag(name);
                        }
                        MapWorld.Instance.Modified = true;
                    });
                    toolStripMenuItem3.Tag = current3;
                    contextMenuStrip.Items.Add(toolStripMenuItem2);

                    toolStripMenuItem2.DropDownItems.Add(toolStripMenuItem3);
                }
            }
        }

        void propertiesForm_ProcessCmdKeyEvent(PropertiesForm sender, ref Message ptr, Keys keys, ref bool handled)
        {
            if ((long)ptr.Msg == 256L)
            {
                Keys keys2 = keys & Keys.KeyCode;
                Keys modifiers = keys & ~keys2;
                if ((PropertiesForm.SelectedGridItem == null || PropertiesForm.SelectedGridItem.PropertyDescriptor == null || PropertiesForm.SelectedGridItem.PropertyDescriptor.IsReadOnly) && this.ToolsProcessKeyDownHotKeys(keys2, modifiers, true))
                {
                    handled = true;
                }
            }
        }

        void propertiesForm_SelectedGridItemChanged(object sender, GridItem newSelection)
        {
            if (newSelection == null)
                return;
            this.a(newSelection);
        }

        void propertiesForm_PropertyItemDoubleClick(object sender, EventArgs e)
        {
            if (PropertiesForm.SelectedGridItem != null && PropertiesForm.SelectedGridItem.Label == "LogicClass" && EntityWorld.Instance.SelectedEntities.Count == 1)
            {
                Entity entity = EntityWorld.Instance.SelectedEntities[0];
                if (entity.LogicClass == null)
                {
                    string text = ToolsLocalization.Translate("Various", "Tick you want create the class of Logic Editor for selected object?");
                    string caption = ToolsLocalization.Translate("Various", "Map Editor");
                    if (MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                A(entity.LogicClass, entity.LogicClass == null);
            }
        }

        private void A(LogicClass bja, bool bjB)
        {
            /*
            this.Bja = bja;
            this.BjB = bjB;
            this.Bjv.Start();
            //*/
        }

        
        private System.Collections.Generic.List<object> A(GridItem gridItem)
        {
            System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
            try
            {
                System.Reflection.FieldInfo field = gridItem.GetType().GetField("objs", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                {
                    object[] array = (object[])field.GetValue(gridItem);
                    object[] array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        object obj = array2[i];
                        object obj2 = obj;
                        EntityCustomTypeDescriptor entityCustomTypeDescriptor = obj2 as EntityCustomTypeDescriptor;
                        if (entityCustomTypeDescriptor != null)
                        {
                            obj2 = entityCustomTypeDescriptor.Entity;
                        }
                        list.Add(obj2);
                    }
                }
            }
            catch
            {
            }
            if (list.Count == 0)
            {
                EntityPropertyDescriptor entityPropertyDescriptor = gridItem.PropertyDescriptor as EntityPropertyDescriptor;
                if (entityPropertyDescriptor != null)
                {
                    list.Add(entityPropertyDescriptor.Entity);
                }
            }
            if (list.Count == 0)
            {
                object value = gridItem.Parent.Value;
                if (!(value is System.Collections.ICollection))
                {
                    list.Add(gridItem.Parent.Value);
                }
            }
            return list;
        }


        private void a(GridItem gridItem)
        {
            GridItem gridItem2 = gridItem;
            while (gridItem2 != null && gridItem2.PropertyDescriptor != null)
            {
                System.Collections.Generic.List<object> list = this.A(gridItem2);
                if (list.Count != 0)
                {
                    object[] array = new object[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            PropertyInfo property = list[i].GetType().GetProperty(gridItem2.PropertyDescriptor.Name, gridItem2.PropertyDescriptor.PropertyType);
                            if (property != null)
                            {
                                array[i] = property.GetValue(list[i], null);
                            }
                        }
                    }
                    gridItem2.Tag = array;
                }
                gridItem2 = gridItem2.Parent;
            }
        }
        #endregion

        #region Entity Types Form
        private void InitializeEntityTypesForm()
        {
            EntityTypesForm.SelectedItemChange += EntityTypesForm_SelectedItemChange;            
        } 

        private void DestroyEntityTypesForm()
        {
            EntityTypesForm.SelectedItemChange -= EntityTypesForm_SelectedItemChange;
        }

        private EntityType EntityTypeSelected { get; set; }

        private void EntityTypesForm_SelectedItemChange(EntityTypesForm sender)
        {            
            Tuple<EntityTypesForm.ItemTypes, object> item = sender.SelectedItem;
            string label = ""; 
            if( item.Item1 == EntityTypesForm.ItemTypes.Entity )
            {
                EntityTypeSelected = item.Item2 as EntityType;
                label = string.Format("点击创建 实体: {0}", item.Item2);
            }
            else
            {
                label = string.Format("路径: {1}", item.Item2);
            }
            contentForm.SetContentLabel(label);
        }
        #endregion

        #region Content Form
        private void InitializeContentForm()
        {
            contentForm.CanvasMouseDown += ContentForm_CanvasMouseDown;
        }

        private void DestroyContentForm()
        {
            contentForm.CanvasMouseDown -= ContentForm_CanvasMouseDown;
        } 

        private void ContentForm_CanvasMouseDown(object sender, EventArgs e)
        {
            if(EntityTypeSelected != null)
            {
                EntityType entityType = EntityTypeSelected;
                EntityTypeSelected = null;
                Log.Info("创建实体类: {0}", entityType);

                Entity entity = EntityWorld.Instance.CreateEntity(entityType);
                TreeNode parentNode = EntitiesForm.GetCurrentNodeLayer();
                EntitiesForm.CreateEntityNode(entity, parentNode);

                contentForm.SetContentLabel("实体创建: {0}", entity);
            }
        }
        #endregion

        #region Entities Form
        private void InitializeEntitiesForm()
        {
            EntitiesForm.NodeSelectChanged += EntitiesForm_NodeSelectChanged;
        }

        private void DestroyEntitiesForm()
        {
            EntitiesForm.NodeSelectChanged -= EntitiesForm_NodeSelectChanged;
        }

        private void EntitiesForm_NodeSelectChanged(TreeNode nodeNew, TreeNode nodeOld)
        {
            PropertiesForm.ReadOnly = true;
            if ( nodeNew == null )
            {
                PropertiesForm.SelectObject(null);
                PropertiesForm.RefreshProperties();
                return; 
            }

            Entity entity = nodeNew.Tag as Entity;
            if( entity != null )
            {
                EntityCustomTypeDescriptor customTypeDescriptor = new EntityCustomTypeDescriptor(entity);
                PropertiesForm.SelectObjects(customTypeDescriptor);
                PropertiesForm.RefreshProperties();
                return;
            }
        }


        #endregion

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            UndoSystem.Instance.DoUndo();
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            UndoSystem.Instance.DoRedo();
        }
    }
}
