using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.EntitySystem;

namespace JxDesign
{
    internal class DesignerInterfaceImpl : DesignerInterface
    {
        private static DesignerInterfaceImpl instance; 

        public new DesignerInterfaceImpl Instance
        {
            get { return instance; }
        }

        public DesignerInterfaceImpl()
        {
            instance = this;
        }


        public override FunctionalityArea FunctionalityArea
        {
            get
            {
                return EntityWorld.Instance.FunctionalityArea;
            }
            set
            {
                EntityWorld.Instance.FunctionalityArea = value;
            }
        }

        public override void SelectEntities(Entity[] entities)
        {
            /*
            EntityWorld.Instance.ClearEntitySelection(true, true);
            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];
                    EntityWorld.Instance.SetEntitySelected(entity, true, true);
                }
            }
            EntityWorld.Instance.UpdatePropertiesFormEntities();
            //*/
        }

        public override List<Entity> GetSelectedEntities()
        {
            return EntityWorld.Instance.SelectedEntities;
        }

        public override bool IsEntitySelected(Entity entity)
        {
            return true;
        }

        public override Entity GetCurrentCreatingEntity()
        {
            return EntityWorld.Instance.CreatingEntity;
        }

        public override void AddCustomCreatedEntity(Entity entity)
        {
            /*
            //MainForm.Instance.MapEntitiesForm.AddEntity(entity);
            //MainForm.Instance.MapEntitiesForm.SetEntityEnsureItemVisible(entity);
            XUI.Instance.AddEntity(entity);
            XUI.Instance.SetEntityEnsureItemVisible(entity);
            Eh action = new Eh(new Entity[]
            {
                entity
            }, true);
            UndoSystem.Instance.CommitAction(action);
            //*/

            MapWorld.Instance.Modified = true;
        }
        public override void RefreshPropertiesForm()
        {
            //MainForm.Instance.PropertiesForm.RefreshProperties();
            XUI.Instance.RefreshProperties();
        }
        public override void SetMapModified()
        {
            MapWorld.Instance.Modified = true;
        }
        public override object SendCustomMessage(Entity sender, string message, object data)
        {
            /*
            if (sender != null && sender == StaticLightingManager.Instance)
            {
                if (message == "Calculate")
                {
                    if (StaticLightingCalculationWorld.Instance == null && !StaticLightingCalculationWorld.Init())
                    {
                        return null;
                    }
                    bool flag;  
                    dt dt = new dt(out flag);
                    if (!flag)
                    {
                        dt.Close();
                        return null;
                    }
                    dt.ShowDialog(); 
                }
                return null;
            }
            RecastNavigationSystem recastNavigationSystem = sender as RecastNavigationSystem;
            if (recastNavigationSystem != null)
            {
                if (message == "Geometries")
                {
                    if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is RecastGeometriesArea)
                    {
                        MapEditorInterface.Instance.FunctionalityArea = null;
                    }
                    else
                    {
                        MapEditorInterface.Instance.FunctionalityArea = null;
                        MapEditorInterface.Instance.FunctionalityArea = new RecastGeometriesArea(recastNavigationSystem);
                    }
                }
                if (message == "Test")
                {
                    if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is RecastTestArea)
                    {
                        MapEditorInterface.Instance.FunctionalityArea = null;
                    }
                    else
                    {
                        MapEditorInterface.Instance.FunctionalityArea = null;
                        MapEditorInterface.Instance.FunctionalityArea = new RecastTestArea(recastNavigationSystem);
                    }
                }
                if (message == "IsAllowToRenderNavigationMesh")
                {
                    bool flag2 = false;
                    if (EntityWorld.Instance.IsEntitySelected(sender))
                    {
                        flag2 = true;
                        if (this.FunctionalityArea != null && this.FunctionalityArea is RecastGeometriesArea)
                        {
                            flag2 = false;
                        }
                    }
                    return flag2;
                }
                return null;
            }
            else
            {
                HeightmapTerrain heightmapTerrain = sender as HeightmapTerrain;
                if (heightmapTerrain != null)
                {
                    if (message == "Edit")
                    {
                        if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is HeightmapTerrainArea)
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                        }
                        else
                        {
                            MapEditorInterface.Instance.FunctionalityArea = new HeightmapTerrainArea(heightmapTerrain);
                        }
                    }
                    if (message == "AmbientOcclusion")
                    {
                        if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is HeightmapTerrainArea)
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                        }
                        HeightmapTerrainAOForm heightmapTerrainAOForm = new HeightmapTerrainAOForm(heightmapTerrain);
                        heightmapTerrainAOForm.ShowDialog();
                    }
                    return null;
                }
                HeightmapTerrainManager heightmapTerrainManager = sender as HeightmapTerrainManager;
                if (heightmapTerrainManager != null)
                {
                    if (message == "Configure")
                    {
                        if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is HeightmapTerrainManagerConfigureArea)
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                        }
                        else
                        {
                            MapEditorInterface.Instance.FunctionalityArea = new HeightmapTerrainManagerConfigureArea(heightmapTerrainManager);
                        }
                    }
                    if (message == "Select Terrain")
                    {
                        if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is HeightmapTerrainManagerSelectTerrainArea)
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                        }
                        else
                        {
                            MapEditorInterface.Instance.FunctionalityArea = new HeightmapTerrainManagerSelectTerrainArea(heightmapTerrainManager);
                        }
                    }
                    return null;
                }
                DecorativeObjectManager decorativeObjectManager = sender as DecorativeObjectManager;
                if (decorativeObjectManager != null)
                {
                    if (message == "Edit")
                    {
                        if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is DecorativeObjectManagerArea)
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                        }
                        else
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                            MapEditorInterface.Instance.FunctionalityArea = new DecorativeObjectManagerArea();
                        }
                    }
                    if (message == "Geometries")
                    {
                        if (MapEditorInterface.Instance.FunctionalityArea != null && MapEditorInterface.Instance.FunctionalityArea is DecorativeObjectManagerGeometriesArea)
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                        }
                        else
                        {
                            MapEditorInterface.Instance.FunctionalityArea = null;
                            MapEditorInterface.Instance.FunctionalityArea = new DecorativeObjectManagerGeometriesArea(DecorativeObjectManager.Instance);
                        }
                    }
                    if (message == "Update")
                    {
                        GeometryManager.UpdateAllDecorativeObjectsProperties(null);
                    }
                    return null;
                }
                if (message == "UndoEntitiesCreateDeleteAction_Create")
                {
                    Entity[] entities = (Entity[])data;
                    Eh action = new Eh(entities, true);
                    UndoSystem.Instance.CommitAction(action);
                } 
                return null;
            }
            //*/
            return null;
        }
         
        public override bool EntityUITypeEditorEditValue(Entity ownerEntity, System.Type entityClassType, ref Entity entity)
        {
            /*
#if DEBUG
            XLog.debug(
                "MapEditorInterfaceImpl.EntityUITypeEditorEditValue: entity = {0}, entityClassType = {1}", ownerEntity, entityClassType);
#endif
            ///*
            dy dy = new dy(ownerEntity, entityClassType, true, entity);
            if (dy.ShowDialog() == DialogResult.OK)
            {
                entity = dy.Entity;
                return true;
            }
            //*/
            return false;
        }
        public override void EntityExtendedPropertiesUITypeEditorEditValue()
        {
            /*
#if DEBUG
            XLog.debug("MapEditorInterfaceImpl.EntityExtendedPropertiesUITypeEditorEditValue");
#endif
            ///*
            System.Collections.Generic.List<Entity> selectedEntities = EntityWorld.Instance.SelectedEntities;
            System.Type type = null;
            if (selectedEntities[0].ExtendedProperties != null)
            {
                type = selectedEntities[0].ExtendedProperties.GetType();
            }
            for (int i = 1; i < selectedEntities.Count; i++)
            {
                Entity entity = selectedEntities[i];
                System.Type left = null;
                if (entity.ExtendedProperties != null)
                {
                    left = entity.ExtendedProperties.GetType();
                }
                if (left != type)
                {
                    type = null;
                }
            }
            dw dw = new dw(type);
            if (dw.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            System.Collections.Generic.List<EI.A> list = new System.Collections.Generic.List<EI.A>();
            foreach (Entity current in selectedEntities)
            {
                System.Type left2 = null;
                if (current.ExtendedProperties != null)
                {
                    left2 = current.ExtendedProperties.GetType();
                }
                if (left2 != dw.ExtendedPropertiesClass)
                {
                    list.Add(new EI.A(current, current.ExtendedProperties));
                    if (current.ExtendedProperties != null)
                    {
                        this.entityExtendedProperties.Add(current.ExtendedProperties);
                    }
                    current.Editor_ReplaceExtendedProperties(null);
                    if (dw.ExtendedPropertiesClass != null)
                    {
                        current.CreateExtendedProperties(dw.ExtendedPropertiesClass);
                    }
                }
            }
            if (list.Count != 0)
            {
                EI action = new EI(list);
                UndoSystem.Instance.CommitAction(action);
                SetMapModified();
            }
            //*/
        }
    }
}
