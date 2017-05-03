using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Jx.EntitySystem
{
    public abstract class DesignerInterface
    {
        public enum FontNames
        {
            MainMenu,
            ContextMenu,
            Form,
            TreeControl,
            PropertyGrid,
            SourceCodeEditor
        }
        private static DesignerInterface instace;
        public static DesignerInterface Instance
        {
            get
            {
                return DesignerInterface.instace;
            }
        }
        /// <summary>
        /// Gets or sets the current Map Editor functionality area.
        /// </summary>
        public abstract FunctionalityArea FunctionalityArea
        {
            get;
            set;
        }

        public static void Init(DesignerInterface overridedObject)
        {
            DesignerInterface.instace = overridedObject;
        }

        /// <summary>
        /// To select the list of entities in the Map Editor.
        /// </summary>
        /// <param name="entities">The array of entities.</param>
        public abstract void SelectEntities(Entity[] entities);
        /// <summary>
        /// Gets selected entities list in the Map Editor.
        /// </summary>
        /// <returns></returns>
        public abstract List<Entity> GetSelectedEntities();
        /// <summary>
        /// Checks the selection flag of the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract bool IsEntitySelected(Entity entity);
        /// <summary>
        /// Returns the current creating entity in the Map Editor.
        /// </summary>
        /// <returns>The current creating entity in the Map Editor.</returns>
        public abstract Entity GetCurrentCreatingEntity();
        public abstract void AddCustomCreatedEntity(Entity entity);
        public abstract void SetMapModified();
        public abstract void RefreshPropertiesForm();
        public abstract object SendCustomMessage(Entity sender, string message, object data); 
        public abstract bool EntityUITypeEditorEditValue(Entity ownerEntity, Type entityClassType, ref Entity entity);
        public abstract void EntityExtendedPropertiesUITypeEditorEditValue();
    }
}
