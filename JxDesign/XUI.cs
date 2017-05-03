using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.EntitySystem;

namespace JxDesign
{
    internal class XUI
    {
        private XUI() { }
         
        public void InsertAddonMenuItem(ToolStripMenuItem menuItem)
        {
            //MainForm.Instance.AddonsMenuItem.DropDownItems.Add(menuItem);
        }

        public void ShowPropertiesForm()
        {
            MainForm.Instance.ShowPropertiesForm();
        }

        public void ShowSelectedEntitiesInObjectsWindow()
        {

            MainForm.Instance.ShowMapEntitiesForm();
            //MainForm.Instance.MapEntitiesForm.SetSelectedEntitiesEnsureItemsVisible();
        }

        public void UpdateEntityLayer(object mapObject)
        {
            //MainForm.Instance.MapEntitiesForm.UpdateEntityLayer(mapObject);
        }

        public void AddLayer(object editorLayer)
        {
            //MainForm.Instance.MapEntitiesForm.AddLayer(editorLayer);
        }

        public void RemoveLayer(object editorLayer)
        {
            //MainForm.Instance.MapEntitiesForm.RemoveLayer(editorLayer);
        }

        public void UpdateLayer(object editorLayer)
        {
            //MainForm.Instance.MapEntitiesForm.UpdateLayer(editorLayer);
        }

        public void TreeViewEndUpdate()
        {
            //MainForm.Instance.MapEntitiesForm.TreeViewBeginUpdate();
        }

        public void TreeViewBeginUpdate()
        {
            //MainForm.Instance.MapEntitiesForm.TreeViewEndUpdate();
        }

        public void SelectObjects(EntityCustomTypeDescriptor[] entities)
        {
#if _XUI_
            XLog.debug("XUI: MainForm.Instance.PropertiesForm.SelectObjects(array)");
#endif
            MainForm.Instance.PropertiesForm.SelectObjects(entities);
        }

        public void SetEntityEnsureItemVisible(Entity entity)
        {
            //MainForm.Instance.MapEntitiesForm.SetEntityEnsureItemVisible(entity);
        }

        public void AddEntity(Entity entity)
        {
            //MainForm.Instance.MapEntitiesForm.AddEntity(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            //MainForm.Instance.MapEntitiesForm.RemoveEntity(entity);
        }

        public void ClearEntityTypesSelection()
        {
            //MainForm.Instance.EntityTypesForm.ClearSelection();
        }

        public void SetEntitySelected(Entity entity, bool selected)
        {
            //MainForm.Instance.MapEntitiesForm.SetEntitySelected(entity, selected);
        }

        public void RefreshProperties()
        {
#if _XUI_
            XLog.debug("XUI: MainForm.Instance.PropertiesForm.RefreshProperties()");
#endif
            MainForm.Instance.PropertiesForm.RefreshProperties();
        }

        //-----------------
        static XUI instance;

        public static XUI Instance
        {
            get
            {
                if (instance == null)
                    instance = new XUI();
                return instance;
            }
        }
    }
}
