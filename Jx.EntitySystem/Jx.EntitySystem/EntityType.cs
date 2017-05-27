using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;

using Jx.FileSystem;

namespace Jx.EntitySystem
{
    [LogicSystemBrowsable(true), Editor(typeof(EditorEntityTypeUITypeEditor), typeof(UITypeEditor))]
    public class EntityType : IDisposable
    {

        /// <summary>
        /// Specifies that a field will be serialized. This class cannot be inherited.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public sealed class FieldSerializeAttribute : Attribute
        {
            private string propertyName;
            /// <summary>
            /// Gets the property name.
            /// </summary>
            public string PropertyName
            {
                get
                {
                    return this.propertyName;
                }
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <remarks>
            /// The property name will be taken by Reflection from property. If it will be used obfuscator there can be problems with renamed names of properties later.
            /// </remarks>
            public FieldSerializeAttribute()
            {
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            public FieldSerializeAttribute(string propertyName)
            {
                this.propertyName = propertyName;
            }
        }
        internal EntityTypes.ClassInfo classInfo;
        internal string name;
        internal string fullName = "";
        internal string filePath = "";
        [EntityType.FieldSerializeAttribute("creatableInMapEditor")]
        private bool creatableInMapEditor = true;
        [EntityType.FieldSerializeAttribute("allowEmptyName")]
        private bool allowEmptyName;
        [EntityType.FieldSerializeAttribute("uniqueEntityInstance")]
        private bool uniqueEntityInstance;
        private EntityNetworkTypes entityNetworkType;
        internal TextBlock textBlock;
        internal bool manualCreated;
        internal Dictionary<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem, object> entityTypeSerializableFields = 
            new Dictionary<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem, object>();
        internal uint networkUIN;

        [FieldSerialize]
        private uint taskTimeout = 0;

        /// <summary>
        /// Gets the class information of type.
        /// </summary>
        [Description("The class information of type.")]
        public EntityTypes.ClassInfo ClassInfo
        {
            get
            {
                return this.classInfo;
            }
        }
        /// <summary>
        /// Gets the name of type.
        /// </summary>
        [Description("The name of type.")]
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        /// <summary>
        /// Gets or sets the full name of type.
        /// </summary>
        [Browsable(false)]
        public string FullName
        {
            get
            {
                return this.fullName;
            }
            set
            {
                this.fullName = value;
            }
        }

        [Description("任务超时时间, 单位: 毫秒。取值0，不限定。")]
        [DefaultValue(0)]
        public uint TaskTimeout
        {
            get { return taskTimeout; }
            set { taskTimeout = value; }
        }

        /// <summary>
        /// Gets the file path of the .type file, which defines the given type.
        /// </summary>
        /// <remarks>
        /// If type to create manually file path it will be empty.
        /// </remarks>
        [Browsable(false)]
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the entity can created in the Map Editor.
        /// </summary>
        [DefaultValue(true), Description("Whether the entity can created in the Map Editor.")]
        [Browsable(false)]
        public bool CreatableInMapEditor
        {
            get
            {
                return this.creatableInMapEditor;
            }
            set
            {
                this.creatableInMapEditor = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the entity can have an empty name.
        /// </summary>
        /// <seealso cref="P:Engine.EntitySystem.Entity.Name" />
        [DefaultValue(false), Description("Whether the entity can have an empty name.")]
        public bool AllowEmptyName
        {
            get
            {
                return this.allowEmptyName;
            }
            set
            {
                this.allowEmptyName = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the network type of entity.
        /// </summary>
        [DefaultValue(EntityNetworkTypes.NotSynchronized), Description("The network type of entity.")]
        [Browsable(false)]
        public EntityNetworkTypes NetworkType
        {
            get
            {
                return this.entityNetworkType;
            }
            set
            {
                this.entityNetworkType = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to be created some instances of this type.
        /// </summary>
        /// <remarks>
        /// This value is applied, when it is necessary to forbid creation more one instance.
        /// This value is convenient for using to control users at creation of entities in the Map Editor.
        /// For example the given property is used <b>MapSystem.Collision</b> which one can be only on all map.
        /// </remarks>
        [Browsable(false), DefaultValue(false)]
        public bool UniqueEntityInstance
        {
            get
            {
                return this.uniqueEntityInstance;
            }
            set
            {
                this.uniqueEntityInstance = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the type manually is created.
        /// </summary>
        [Browsable(false)]
        public bool ManualCreated
        {
            get
            {
                return this.manualCreated;
            }
        }

        internal bool loadEntityTypeFromTextBlock(TextBlock block)
        {
            string errorString = string.Format("File path: \"{0}\"", this.FilePath); 
            for (EntityTypes.ClassInfo baseClassInfo = this.ClassInfo; baseClassInfo != null; baseClassInfo = baseClassInfo.BaseClassInfo)
            {
                foreach (EntityTypes.ClassInfo.EntityTypeSerializableFieldItem current in baseClassInfo.EntityTypeSerializableFields)
                {
                    bool failure = !EntityHelper.LoadFieldValue(false, this, current.Field, block, errorString);
                    if (failure)
                        return false; 
                }
            }
            return true;
        }

        internal bool Save(TextBlock block)
        {
            string errorString = string.Format("File path: \"{0}\"", this.FilePath); 
            for (EntityTypes.ClassInfo baseClassInfo = this.ClassInfo; baseClassInfo != null; baseClassInfo = baseClassInfo.BaseClassInfo)
            {
                foreach (EntityTypes.ClassInfo.EntityTypeSerializableFieldItem current in baseClassInfo.EntityTypeSerializableFields)
                {
                    object defaultValue = entityTypeSerializableFields[current];
                    bool failure = !EntityHelper.SaveFieldValue(false, this, current.Field, block, defaultValue, errorString);
                    if (failure)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="T:Engine.EntitySystem.EntityType" />.
        /// </summary>
        public virtual void Dispose()
        {
        }

        internal bool _OnLoad(TextBlock block)
        {
            return OnLoad(block);
        }

        /// <summary>
        /// Called when the type during loading.
        /// </summary>
        /// <param name="block">The text block in which data of type will be loaded.</param>
        /// <returns><b>true</b> if the data are correct; otherwise, <b>false</b>.</returns>
        protected virtual bool OnLoad(TextBlock block)
        {
            if (block.IsAttributeExist("networkType"))
            {
                try
                {
                    this.entityNetworkType = (EntityNetworkTypes)Enum.Parse(typeof(EntityNetworkTypes), block.GetAttribute("networkType"));
                }
                catch { }
            }

            if (block.IsAttributeExist("allowEditorCreate"))
            {
                this.CreatableInMapEditor = bool.Parse(block.GetAttribute("allowEditorCreate"));
            }
            return true;
        }

        internal void _OnLoaded()
        {
            OnLoaded();
        }

        /// <summary>
        /// Called when the type is loaded.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        internal bool _OnSave(TextBlock block)
        {
            return OnSave(block);
        }

        /// <summary>
        /// Called when the type during saving.
        /// </summary>
        /// <param name="block">The text block in which data of type will be saved.</param>
        /// <returns><b>true</b> if the data are correct; otherwise, <b>false</b>.</returns>
        protected virtual bool OnSave(TextBlock block)
        {
            if (this.entityNetworkType > EntityNetworkTypes.NotSynchronized)
                block.SetAttribute("networkType", this.entityNetworkType.ToString());
            return true;
        }
 
        /// <summary>
        /// Returns a string containing the type class name and name of the entity type.
        /// </summary>
        /// <returns>Returns a string containing the type class name and name of the entity type.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.ClassInfo.entityClassType.Name);
        }
        /// <summary>
        /// Whether to check up there is a link to object.
        /// </summary>
        /// <remarks>
        /// This method it is necessary to redirect to all of you created 
        /// <b>MapObject.MapObjectCreateObjectCollection</b> objects and 
        /// for all custom not serializable <b>EntityType</b> fields. 
        /// It is necessary for normal work of Resource Editor.
        /// </remarks>
        /// <param name="obj">The cheched object.</param>
        /// <returns><b>true</b> if a link to object is exists; otherwise, <b>false</b>.</returns>
        protected internal virtual bool OnIsExistsReferenceToObject(object obj)
        {
            for (EntityTypes.ClassInfo baseClassInfo = this.ClassInfo; baseClassInfo != null; baseClassInfo = baseClassInfo.BaseClassInfo)
            {
                foreach (EntityTypes.ClassInfo.EntityTypeSerializableFieldItem current in baseClassInfo.EntityTypeSerializableFields)
                {
                    bool flag = current.Field.GetValue(this) == obj;
                    if (flag)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Replaces the link of all objects to another.
        /// </summary>
        /// <remarks>
        /// This method it is necessary to redirect to all of you created 
        /// <b>MapObject.MapObjectCreateObjectCollection</b> objects and 
        /// for all custom not serializable <b>EntityType</b> fields. 
        /// It is necessary for normal work of Resource Editor.
        /// </remarks>
        /// <param name="obj">The source link to object.</param>
        /// <param name="newValue">The new link to object.</param>
        protected internal virtual List<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem> OnChangeReferencesToObject(object obj, object newValue)
        {
            List<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem> result = new List<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem>(); 

            for (EntityTypes.ClassInfo baseClassInfo = this.ClassInfo; baseClassInfo != null; baseClassInfo = baseClassInfo.BaseClassInfo)
            {
                foreach (EntityTypes.ClassInfo.EntityTypeSerializableFieldItem current in baseClassInfo.EntityTypeSerializableFields)
                {
                    bool flag = current.Field.GetValue(this) == obj;
                    if (flag)
                    {
                        current.Field.SetValue(this, newValue);
                        result.Add(current);
                    }
                }
            }
            return result;
        }
        protected virtual void OnPreloadResources()
        {
        }
        public void PreloadResources()
        {
            this.OnPreloadResources();
        }
    }
}
