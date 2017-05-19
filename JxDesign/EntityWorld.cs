using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.EntitySystem;
using Jx.MapSystem;

namespace JxDesign
{
    internal class EntityWorld
    {
        private static EntityWorld instance = null; 

        public static void Setup()
        {
            instance = new EntityWorld(); 
        }

        public static void Shutdown()
        {
            instance = null; 
        }

        public static EntityWorld Instance
        {
            get { return instance; }
        }
        

        private FunctionalityArea functionalityArea;
        private Entity creatingEntity = null;

        private EntityWorld() { }

        public FunctionalityArea FunctionalityArea
        {
            get { return functionalityArea; }
            set { functionalityArea = value; }
        }

        public Entity CreatingEntity
        {
            get { return creatingEntity; }
        }

        public Entity CreateEntity(EntityType entityType)
        {
            if (Map.Instance == null)
                return null; 
            creatingEntity = Entities.Instance.Create(entityType, Map.Instance);
            creatingEntity.PostCreate();

            MapWorld.Instance.Modified = true;
            return creatingEntity;
        }

        public List<Entity> SelectedEntities
        {
            get { return null; }
        }

        public void ResetBeforeMapSave()
        {

        }

        public void ClearEntitySelection(bool f1, bool f2)
        {

        }
    }
}
