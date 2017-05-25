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

        private LogicClass currentLogicClass = null;
        private bool currentLogicClassState = false;

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
            tsbLogic.Image = imageCache["logic"];

            Bootstrap(); 
            timerEntitySystemWorld.Enabled = true;

            //SetTheme(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015LightTheme1);

            AddonManager.Instance.PostInit();

            serializeContext = new DeserializeDockContent(GetContentFromPersistString);
            if (!LoadLayoutConfig())
            {
                ShowMapEntitiesForm();
                ShowEntityTypesForm();
                ShowPropertiesForm();
                ShowContentForm();
                ShowConsoleForm();
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

        #region 视图
        public void ShowConsoleForm()
        {
            consoleForm.Show(dockPanel, DockState.DockBottomAutoHide);
        }
        public void ShowContentForm()
        {
            contentForm.Show(dockPanel, DockState.Document);
        }
        public void ShowPropertiesForm()
        {
            propertiesForm.Show(dockPanel, DockState.DockRight);
        }
        public void ShowEntityTypesForm()
        {
            entityTypesForm.Show(dockPanel, DockState.DockLeft);
        }
        public void ShowMapEntitiesForm()
        {
            entitiesForm.Show(dockPanel, DockState.DockLeft);
        }
        #endregion

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
            if( MapWorld.Instance.Load() )
                NotifyUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tsbSave.Enabled = Map.Instance != null && MapWorld.Instance.Modified;
            tsbSaveAs.Enabled = Map.Instance != null;
            tsmiSave.Enabled = Map.Instance != null && MapWorld.Instance.Modified;
            tsmiSaveAs.Enabled = Map.Instance != null;

            tsmiEntitiesWindow.Checked = EntitiesForm.Visible;
            tsmiEntityTypesWindow.Checked = EntityTypesForm.Visible;
            tsmiContentWindow.Checked = contentForm.Visible;
            tsmiPropertiesWindow.Checked = propertiesForm.Visible;
            tsmiConsoleWindow.Checked = consoleForm.Visible;

            UpdateTypeSelectionLabel();
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
                List<object> list = this.ListGridItemValues(gridItem);
                if (list.Count != 0 && gridItem.Tag != null)
                {
                    object[] array = (object[])gridItem.Tag;
                    List<UndoObjectsPropertyChangeAction.Item> undoItems = new List<UndoObjectsPropertyChangeAction.Item>(list.Count);
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
                                undoItems.Add(new UndoObjectsPropertyChangeAction.Item(obj3, property, obj4));
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
                                MapObject mapObject = obj3 as MapObject;
                                string editorLayerName = ReflectionUtil.GetMemberName(() => mapObject.EditorLayer);
                                PropertyInfo propertyEditorLayer = obj3.GetType().GetProperty(editorLayerName);
                                if (propertyEditorLayer == property )
                                {
                                    foreach (Entity current in EntityWorld.Instance.SelectedEntities)
                                    {
                                        MapObject mapObject2 = current as MapObject;
                                        if (mapObject2 != null)
                                            EntitiesForm.UpdateMapObjectLayer(mapObject2);
                                    }
                                }
                            }
                        }
                    }
                    if (undoItems.Count != 0)
                    {
                        UndoObjectsPropertyChangeAction action = new UndoObjectsPropertyChangeAction(undoItems.ToArray());
                        UndoSystem.Instance.CommitAction(action);
                        MapWorld.Instance.Modified = true;
                    }
                    this.a(gridItem); 
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
            if (PropertiesForm.SelectedGridItem != null && PropertiesForm.SelectedGridItem.Label == "Tags")
            {
                List<Entity> entitiesSelected = EntityWorld.Instance.SelectedEntities;

                var q = entitiesSelected.Select(_entity =>
                {
                    var _tags = _entity.Tags.Select(_tag => new Entity.TagInfo(_tag.Name, _tag.Value));
                    return new UndoObjectsPropertyChangeAction.Item(_entity, typeof(Entity).GetProperty("Tags"), _tags);
                });

                contextMenuStrip.Items.Add(new ToolStripSeparator());
                //*
                string textAddTag = ToolsLocalization.Translate("Various", "Add Tag");
                ToolStripMenuItem tsmiAddTag = new ToolStripMenuItem(textAddTag, Properties.Resources.new_16, delegate (object s, EventArgs e2)
                {
                    EntityAddTagDialog entityAddTagDialog = new EntityAddTagDialog();
                    if (entityAddTagDialog.ShowDialog() == DialogResult.OK)
                    {
                        UndoSystem.Instance.CommitAction(new UndoObjectsPropertyChangeAction(q.ToArray()));
                        foreach (Entity current in entitiesSelected)
                            current.SetTag(entityAddTagDialog.TagName, entityAddTagDialog.TagValue);

                        PropertiesForm.RefreshProperties();
                        MapWorld.Instance.Modified = true;
                    }
                });
                tsmiAddTag.Enabled = true;
                contextMenuStrip.Items.Add(tsmiAddTag);
 
                List<string> tagsSet = entitiesSelected.SelectMany(_entity => _entity.Tags.Select(_tag => _tag.Name)).Distinct().ToList();
                 
                string textRemoveTag = ToolsLocalization.Translate("Various", "Remove Tag");
                ToolStripMenuItem tsmiRemoveTag = new ToolStripMenuItem(textRemoveTag, Properties.Resources.delete_16);
                tsmiRemoveTag.Enabled = (tagsSet.Count != 0);
                contextMenuStrip.Items.Add(tsmiRemoveTag);

                foreach (string tag in tagsSet)
                {
                    ToolStripMenuItem tsmiDeleteTag = new ToolStripMenuItem(tag, Properties.Resources.item_16, delegate (object s, EventArgs e2)
                    {
                        string name = (string)((ToolStripMenuItem)s).Tag; 
                        UndoSystem.Instance.CommitAction(new UndoObjectsPropertyChangeAction(q.ToArray()));
                        foreach (Entity current in entitiesSelected)
                            current.RemoveTag(name);

                        PropertiesForm.RefreshProperties();
                        MapWorld.Instance.Modified = true;
                    });
                    tsmiDeleteTag.Tag = tag;
                    contextMenuStrip.Items.Add(tsmiRemoveTag);

                    tsmiRemoveTag.DropDownItems.Add(tsmiDeleteTag); 
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
                        return;
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

            this.currentLogicClass = bja;
            this.currentLogicClassState = bjB;
            this.timerLogicEditor.Enabled = true;
        } 
        
        private List<object> ListGridItemValues(GridItem gridItem)
        {
            List<object> list = new List<object>();
            try
            {
                FieldInfo field = gridItem.GetType().GetField("objs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    object[] objs = (object[])field.GetValue(gridItem);
                    for (int i = 0; i < objs.Length; i++)
                    {
                        object obj = objs[i];
                        EntityCustomTypeDescriptor entityCustomTypeDescriptor = obj as EntityCustomTypeDescriptor;
                        if (entityCustomTypeDescriptor != null)
                            obj = entityCustomTypeDescriptor.Entity;
                        list.Add(obj);
                    }
                }
            }
            catch { }

            if (list.Count == 0)
            {
                EntityPropertyDescriptor entityPropertyDescriptor = gridItem.PropertyDescriptor as EntityPropertyDescriptor;
                if (entityPropertyDescriptor != null) 
                    list.Add(entityPropertyDescriptor.Entity); 
            }

            if (list.Count == 0)
            {
                object value = gridItem.Parent.Value;
                if (!(value is System.Collections.ICollection))
                    list.Add(gridItem.Parent.Value);
            }
            return list;
        }


        private void a(GridItem gridItem)
        {
            GridItem current = gridItem;
            while (current != null && current.PropertyDescriptor != null)
            {
                List<object> list = ListGridItemValues(current);
                if (list.Count != 0)
                {
                    object[] array = new object[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            PropertyInfo property = list[i].GetType().GetProperty(current.PropertyDescriptor.Name, current.PropertyDescriptor.PropertyType);
                            if (property != null)
                            {
                                array[i] = property.GetValue(list[i], null);
                            }
                        }
                    }
                    current.Tag = array;
                }
                current = current.Parent;
            }
        }
        #endregion

        #region Entity Types Form
        private void InitializeEntityTypesForm()
        {
            EntityTypesForm.SelectedItemChange += EntityTypesForm_SelectedItemChange;
            EntityTypesForm.ObjectNodeSelectChanged += EntityTypesForm_ObjectNodeSelectChanged;
            EntityTypesForm.ModelNodeSelectChanged += EntityTypesForm_ModelNodeSelectChanged;
        }

        private void DestroyEntityTypesForm()
        {
            EntityTypesForm.SelectedItemChange -= EntityTypesForm_SelectedItemChange;
            EntityTypesForm.ObjectNodeSelectChanged -= EntityTypesForm_ObjectNodeSelectChanged;
            EntityTypesForm.ModelNodeSelectChanged -= EntityTypesForm_ModelNodeSelectChanged;
        }

        private static readonly Tuple<EntityTypeSelectItemType, object> NULL_SELECTION = new Tuple<EntityTypeSelectItemType, object>(EntityTypeSelectItemType.Null, null);
        private Tuple<EntityTypeSelectItemType, object> typeSelected = NULL_SELECTION;
        public Tuple<EntityTypeSelectItemType, object> TypeSelected
        {
            get { return typeSelected; }
            set { typeSelected = value ?? NULL_SELECTION; }
        }
        public EntityType EntityTypeSelected { get; private set; }

        private void UpdateEntityTypeSelected()
        {
            Tuple<EntityTypeSelectItemType, object> item = EntityTypesForm.SelectedItem;
            TypeSelected = item;

            EntityTypeSelected = null;
            if (item.Item1 == EntityTypeSelectItemType.Entity)
                EntityTypeSelected = item.Item2 as EntityType;
        }

        internal void ClearEntityTypeSelected()
        {
            EntityTypeSelected = null;
            TypeSelected = NULL_SELECTION;
            EntityTypesForm.ClearSelection();
        }

        private void OnEntityTypesNodeSelectionChanged()
        {
            UpdateEntityTypeSelected();
            UpdateTypeSelectionLabel();
        }

        private void EntityTypesForm_SelectedItemChange(EntityTypesForm sender)
        {
 
        }

        private void EntityTypesForm_ModelNodeSelectChanged(TreeNode nodeNew, TreeNode nodeOld)
        {
            OnEntityTypesNodeSelectionChanged();
        }

        private void EntityTypesForm_ObjectNodeSelectChanged(TreeNode nodeNew, TreeNode nodeOld)
        {
            OnEntityTypesNodeSelectionChanged();
        }

        private void UpdateTypeSelectionLabel()
        {
            string label = "";
            if( !MapWorld.MapLoaded )
            {
                contentForm.SetContentLabel("地图未加载");
                return; 
            }

            if( TypeSelected == null)
                return;

            if (TypeSelected.Item1 == EntityTypeSelectItemType.Entity)
            {
                label = string.Format("点击创建 实体: {0}", EntityTypeSelected);
            }
            else if( TypeSelected.Item1 == EntityTypeSelectItemType.Mesh )
            {
                label = string.Format("路径: {0}", TypeSelected.Item2);
            }
            else
            {
                label = string.Format("-", TypeSelected.Item2);
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
                ClearEntityTypeSelected();
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

        private void UpdatePropertiesView(params object[] objects)
        {
            PropertiesForm.SelectObjects(objects);
            PropertiesForm.RefreshProperties();
        }

        private void EntitiesForm_NodeSelectChanged(TreeNode nodeNew, TreeNode nodeOld)
        {
            PropertiesForm.ReadOnly = false;
            if ( nodeNew == null )
            {
                UpdatePropertiesView(null);
                return; 
            }

            Entity entity = nodeNew.Tag as Entity;
            if( entity != null )
            {
                EntityCustomTypeDescriptor customTypeDescriptor = new EntityCustomTypeDescriptor(entity);
                UpdatePropertiesView(customTypeDescriptor);
                return;
            }

            UpdatePropertiesView();
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

        private void tsmiEntityTypesWindow_Click(object sender, EventArgs e)
        {
            ShowEntityTypesForm();
        }

        private void tsmiEntitiesWindow_Click(object sender, EventArgs e)
        {
            ShowMapEntitiesForm();
        }

        private void tsmiContentWindow_Click(object sender, EventArgs e)
        {
            ShowContentForm();
        }

        private void tsmiPropertiesWindow_Click(object sender, EventArgs e)
        {
            ShowPropertiesForm();
        }

        private void tsmiOutputWindow_Click(object sender, EventArgs e)
        {
            ShowConsoleForm();
        }

        private void tsbLogic_Click(object sender, EventArgs e)
        {
            A(null, false);
            //EngineApp.Instance.SetMainLoopQuit();
        }

        private void timerEntitySystemWorld_Tick(object sender, EventArgs e)
        {
            if (EntitySystemWorld.Instance != null)
                EntitySystemWorld.Instance.Tick();
        }
    }
}
