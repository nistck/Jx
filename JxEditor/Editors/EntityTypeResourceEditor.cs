using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx;
using Jx.Editors;
using Jx.EntitySystem; 

using JxEditor.UI;

namespace JxEditor.Editors
{
    internal class EntityTypeResourceEditor : ResourceObjectEditor
    {
        private static EntityTypeResourceEditor currentEditor;
        private EntityTypeEditorObjectsForm awo;
        private ToolStripMenuItem awP;
        private ToolStripMenuItem awp;
        private EntityType currentEntityType;
        private Entity awq;
        private bool awR;
        internal List<object> awr = new List<object>();
 
        private string aws;
        private int awT; 
        private List<int> awU = new List<int>(); 

        public static EntityTypeResourceEditor CurrentEditor
        {
            get
            {
                return EntityTypeResourceEditor.currentEditor;
            }
        }

        public EntityType EntityType
        {
            get
            {
                return this.currentEntityType;
            }
        }

        public Entity Entity
        {
            get
            {
                return this.awq;
            }
        }

        public string FilterObjectsByAlias
        {
            get
            {
                return this.aws;
            }
            set
            {
                this.aws = value;
            }
        }

        public override void Create(ResourceType resourceType, string fileName)
        {
            base.Create(resourceType, fileName);
            EntityTypeResourceEditor.currentEditor = this;
            base.AllowEditMode = true;
            this.currentEntityType = EntityTypes.Instance.FindByFilePath(fileName);
            if (this.currentEntityType != null)
            {
                try
                {
                    EntityTypes.Instance.DestroyType(this.currentEntityType);
                    EntityType entityType = EntityTypes.Instance.LoadTypeFromFile(fileName);
                    if (entityType == null)
                    {
                        throw new Exception(string.Format(this.Translate("Unable to load type \"{0}\"."), fileName));
                    }
                    EntityTypes.Instance.Editor_ChangeAllReferencesToType(this.currentEntityType, entityType);
                    this.currentEntityType = entityType;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex.Message);
                    this.currentEntityType = null;
                }
            }
            if (this.currentEntityType != null)
            {
                this.SetSelectObject(this.currentEntityType, true, true);
            }
            else
            {
                MainForm.Instance.PropertiesForm.SelectObjects(null);
            }

            /*
            if (this.currentEntityType != null)
            {
                //MainForm.Instance.PropertiesForm.CreateExtendedFunctionalityDescriptor(typeof(EntityTypeExtendedFunctionalityDescriptor), this);
            }
            else
            {
                //MainForm.Instance.PropertiesForm.DestroyExtendedFunctionalityDescriptor();
            }
            
            if (this.F())
            {
                MapHelper.CreateMap();
                this.A();
                MapObject mapObject = this.awq as MapObject;
                if (mapObject != null)
                {
                    base.SetCameraByBounds(mapObject.MapBounds);
                }
                else
                {
                    base.SetCameraByBounds(new Bounds(new Vec3(-3f, -3f, -3f), new Vec3(3f, 3f, 3f)));
                }
            }
            else
            {
                base.SetCameraByBounds(new Bounds(new Vec3(-10000f, -10000f, -10000f), new Vec3(-10001f, -10001f, -10001f)));
            }
            Gizmo.Instance.CommitModify += new Gizmo.ChangeMofidyStateDelegate(this.d);
            Gizmo.Instance.CloneAndSelectObjects += new Gizmo.CloneAndSelectObjectsDelegate(this.e);
            //*/
        }

        public override void Dispose()
        {
            /*
            Gizmo.Instance.CommitModify -= new Gizmo.ChangeMofidyStateDelegate(this.d);
            Gizmo.Instance.CloneAndSelectObjects -= new Gizmo.CloneAndSelectObjectsDelegate(this.e);
            Gizmo.Instance.Objects.Clear();
            //*/

            this.a();
            //MapHelper.DestroyMap();
            base.Dispose();
            EntityTypeResourceEditor.currentEditor = null;
        }

        protected override bool OnSave()
        {
            bool flag;
            try
            {
                MainForm.Instance.ResourcesForm.WatchFileSystem = false;
                flag = EntityTypes.Instance.SaveTypeToFile(this.currentEntityType);
            }
            finally
            {
                MainForm.Instance.ResourcesForm.WatchFileSystem = true;
            }
            return flag && base.OnSave();
        }

        protected override void OnBeginEditMode()
        {
            base.OnBeginEditMode();
            /*
            this.awo = MainForm.Instance.EntityTypeEditorObjectsForm;
            this.awo.SetEditor(this);
            if (this.currentEntityType is MapObjectType)
            {
                this.awo.Show(MainForm.Instance.DockPanel);
                this.awo.UpdateData(this.currentEntityType);
            }
            //*/
            this.SetSelectObject(this.currentEntityType, true, true);
            this.B();
        }

        protected override bool OnEndEditMode()
        {
            if (!base.OnEndEditMode())
            {
                //PhysicsModelResourceEditor.PreviewMapObjectType = null;
                return false;
            }
            this.awr.Clear();
            this.b();
            if (this.awo != null)
            {
                this.awo.Hide();
                //this.awo.Clear();
                this.awo = null;
            }
            if (base.Modified)
            {
                this.ClearObjectSelection(true);
                this.a();
                string filePath = this.currentEntityType.FilePath;
                EntityTypes.Instance.DestroyType(this.currentEntityType);
                EntityType entityType = EntityTypes.Instance.LoadTypeFromFile(filePath);
                if (entityType != null)
                {
                    EntityTypes.Instance.Editor_ChangeAllReferencesToType(this.currentEntityType, entityType);
                    this.currentEntityType = entityType;
                    this.SetSelectObject(this.currentEntityType, true, true);
 
                }
                else
                {
                    //Gizmo.Instance.Objects.Clear();
                    this.ClearObjectSelection(true);
                    this.currentEntityType = null;
                    Log.Warning("EntityTypeResourceEditor: OnEndEditMode: Unable to load entity type.");
                }
            }
            else
            {
                this.SetSelectObject(this.currentEntityType, true, true);
            }
            return true;
        }
        protected override void OnModifiedSet()
        {
            base.OnModifiedSet();
            this.SetNeedRecreateEntity();
        }
        private void A()
        {
            this.a();
            this.awT--; 
        }
        private void a()
        {
 
            this.awq = null;
        }
        public override void OnContextMenuCreate(ContextMenuStrip menu)
        {

            base.OnContextMenuCreate(menu);
        } 

        private void B()
        {
            /*
            this.awP = MainForm.Instance.AddMainMenuExtendItem(this.Translate("&Type Editor"));
            this.awp = new ToolStripMenuItem(this.Translate("&Objects Window"));
            this.awp.Click += new EventHandler(this.b);
            this.awP.DropDownItems.Add(this.awp);
            //*/
        }
        private void b()
        {
            /*
            if (this.awP != null)
            {
                MainForm.Instance.RemoveMainMenuExtendItem(this.awP);
                this.awP = null;
                this.awp = null;
            }
            //*/
        }
         
        public bool IsObjectSelected(object obj)
        {
            return this.awr.Contains(obj);
        }

        public void SetSelectObject(object obj, bool select, bool updatePropertiesForm)
        {
            if (this.currentEntityType == null)
            {
                return;
            }
            if (select)
            {
                if (!this.awr.Contains(obj))
                {
                    this.awr.Add(obj); 
                }
            }
            else
            {
                this.awr.Remove(obj); 
            }
            if (this.awo != null)
            {
                //this.awo.SetObjectSelected(obj, select);
            }
            if (updatePropertiesForm)
            {
                this.C();
            }
        }
        private void C()
        {
            List<object> list = new List<object>(this.awr.Count);
            foreach (object current in this.awr)
            {
                object item = current;
                if (current is EntityType)
                {
                    item = new EntityTypeCustomTypeDescriptor(this.currentEntityType, null);
                }
                list.Add(item);
            }
            if (list.Count != 0)
            {
                MainForm.Instance.PropertiesForm.SelectObjects(list.ToArray(), false);
                return;
            }
            MainForm.Instance.PropertiesForm.SelectObjects(null, false);
        }
        public void ClearObjectSelection(bool refreshPropertiesForm)
        {
            while (this.awr.Count != 0)
            {
                this.SetSelectObject(this.awr[this.awr.Count - 1], false, false);
            }
            if (refreshPropertiesForm)
            {
                MainForm.Instance.PropertiesForm.SelectObjects(new object[]
                {
                    new object()
                }, false);
            }
        }

        protected override bool OnToolsProcessKeyDownHotKeys(Keys keyCode, Keys modifiers, bool processCharactersWithoutModifiers)
        {
            if (base.EditModeActive && keyCode == Keys.Delete)
            {
               
                return true;
            }
            return base.OnToolsProcessKeyDownHotKeys(keyCode, modifiers, processCharactersWithoutModifiers);
        }
           
        protected override void OnTick(float delta)
        {
            base.OnTick(delta); 
            if (this.awp != null && this.awo != null)
            {
                this.awp.Checked = this.awo.Visible;
            }
        }
        private void d()
        {
            MainForm.Instance.PropertiesForm.RefreshProperties();
        }
         
        public void SetNeedRecreateEntity()
        {
            this.awR = true;
        } 
        private string Translate(string text)
        {
            return ToolsLocalization.Translate("EntityTypeResourceEditor", text);
        }
    }

}
