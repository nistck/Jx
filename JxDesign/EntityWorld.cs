using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.EntitySystem;

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

        private EntityWorld() { }

        public FunctionalityArea FunctionalityArea
        {
            get { return functionalityArea; }
            set { functionalityArea = value; }
        }

        public Entity CreatingEntity
        {
            get { return null; }
        }

        public List<Entity> SelectedEntities
        {
            get { return null; }
        }
    }
}
