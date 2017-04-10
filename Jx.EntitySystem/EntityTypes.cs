using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Jx.Ext;
using Jx.IO;

namespace Jx.EntitySystem
{
    public class EntityTypes
    {
        /// <summary>
		/// Defines the class information of the entity type.
		/// </summary>
		public class ClassInfo
        {
            public class EntityTypeSerializableFieldItem
            {
                internal FieldInfo field;
                public FieldInfo Field
                {
                    get
                    {
                        return this.field;
                    }
                }
            }
            public class EntitySerializableFieldItem
            {
                internal FieldInfo field; 
                public FieldInfo Field
                {
                    get
                    {
                        return this.field;
                    }
                } 
            }
            internal class NetworkSynchronizedMeta
            {
                private Entity.NetworkDirections networkDirection;
                private MethodInfo method;
                public Entity.NetworkDirections Direction
                {
                    get
                    {
                        return this.networkDirection;
                    }
                }
                public MethodInfo Method
                {
                    get
                    {
                        return this.method;
                    }
                }
                public NetworkSynchronizedMeta(Entity.NetworkDirections direction, MethodInfo method)
                {
                    this.networkDirection = direction;
                    this.method = method;
                }
            }
            internal Type typeClassType;
            internal Type entityClassType;
            internal EntityTypes.ClassInfo baseClassInfo;
            internal List<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem> entityTypeSerializableFieldItemList;
            internal ReadOnlyCollection<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem> entityTypeSerializableFields;
            internal List<EntityTypes.ClassInfo.EntitySerializableFieldItem> entitySerializableFieldItemList;
            internal ReadOnlyCollection<EntityTypes.ClassInfo.EntitySerializableFieldItem> entitySerializableFields;
            internal FieldInfo fieldInfo;
            internal List<EntityTypes.ClassInfo.NetworkSynchronizedMeta> networkSynchronizedMetaBuffer = new List<EntityTypes.ClassInfo.NetworkSynchronizedMeta>();
            internal uint networkUIN;
            /// <summary>
            /// Gets the entity type class.
            /// </summary>
            public Type TypeClassType
            {
                get
                {
                    return this.typeClassType;
                }
            }
            /// <summary>
            /// Gets the entity class.
            /// </summary>
            public Type EntityClassType
            {
                get
                {
                    return this.entityClassType;
                }
            }
            /// <summary>
            /// Gets the base class information.
            /// </summary>
            public EntityTypes.ClassInfo BaseClassInfo
            {
                get
                {
                    return this.baseClassInfo;
                }
            }
            public IList<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem> EntityTypeSerializableFields
            {
                get
                {
                    return this.entityTypeSerializableFields;
                }
            }
            public IList<EntityTypes.ClassInfo.EntitySerializableFieldItem> EntitySerializableFields
            {
                get
                {
                    return this.entitySerializableFields;
                }
            }
            internal ClassInfo()
            {
            }
            /// <summary>
            /// Returns the entity class name.
            /// </summary>
            /// <returns>The entity class name.</returns>
            public override string ToString()
            {
                return this.entityClassType.Name;
            }
            internal EntityTypes.ClassInfo.NetworkSynchronizedMeta GetNetworkSynchronizedMeta(ushort num)
            {
                bool flag = (int)num >= this.networkSynchronizedMetaBuffer.Count;
                EntityTypes.ClassInfo.NetworkSynchronizedMeta result;
                if (flag)
                {
                    result = null;
                }
                else
                {
                    result = this.networkSynchronizedMetaBuffer[(int)num];
                }
                return result;
            }
        }

        private static EntityTypes instance;
        private List<EntityTypes.ClassInfo> classes = new List<EntityTypes.ClassInfo>();
        private ReadOnlyCollection<EntityTypes.ClassInfo> readonlyClasses;
        private Dictionary<Type, EntityTypes.ClassInfo> typeClassInfoDic = new Dictionary<Type, EntityTypes.ClassInfo>();
        private Dictionary<string, EntityTypes.ClassInfo> typeNameClassInfoDic = new Dictionary<string, EntityTypes.ClassInfo>();
        private List<EntityType> types = new List<EntityType>();
        private ReadOnlyCollection<EntityType> readonlyTypes;
        private Dictionary<string, EntityType> entityTypeNameDic = new Dictionary<string, EntityType>();
        private uint classInfoNetworkUIN;
        private uint entityTypeNetworkUIN;
        /// <summary>
        /// Gets an instance of the <see cref="T:Engine.EntitySystem.EntityTypes" />.
        /// </summary>
        public static EntityTypes Instance
        {
            get
            {
                return EntityTypes.instance;
            }
        }
        /// <summary>
        /// Gets the entity types list.
        /// </summary>
        public IList<EntityType> Types
        {
            get
            {
                return this.readonlyTypes;
            }
        }
        /// <summary>
        /// Gets the class informations list.
        /// </summary>
        public IList<EntityTypes.ClassInfo> Classes
        {
            get
            {
                return this.readonlyClasses;
            }
        }
        internal static bool Init()
        { 
            EntityTypes.instance = new EntityTypes();
            bool flag = EntityTypes.instance.Setup();
            bool flag2 = !flag;
            if (flag2)
            {
                EntityTypes.Shutdown();
            }
            return flag;
        }
        internal static void Shutdown()
        {
            bool flag = EntityTypes.instance != null;
            if (flag)
            {
                EntityTypes.instance.Free();
                EntityTypes.instance = null;
            }
        }
        internal EntityTypes()
        {
            this.readonlyClasses = this.classes.AsReadOnly();
            this.readonlyTypes = this.types.AsReadOnly();
        }
        private bool Setup()
        {
            this.loadEntityFromAssembly();
            List<EntityType> list;
            return this.ManualCreateTypes() && this.LoadGroupOfTypes("", SearchOption.AllDirectories, out list);
        }
        private void Free()
        {
            foreach (EntityType current in this.types)
            {
                current.Dispose();
            }
        }
        /// <summary>
        /// Finds the entity type by the name.
        /// </summary>
        /// <param name="name">The entity type name.</param>
        /// <returns>The entity type or <b>null</b>.</returns>
        [LogicSystemBrowsable(true)]
        public EntityType GetByName(string name)
        {
            EntityType result = null;
            this.entityTypeNameDic.TryGetValue(name, out result);
            return result;
        }
        private EntityType CreateEntityType(string typeName, string entityClassTypeName, string filePath)
        {
            EntityType byName = this.GetByName(typeName);
            bool flag = byName != null;
            EntityType result;
            if (flag)
            {
                Log.Error(string.Format("EntityTypes: Entity type with name \"{0}\" is already exists.\nFiles:\n\"{1}\",\n\"{2}\".", typeName, byName.FilePath, filePath), typeName);
                result = null;
            }
            else
            {
                EntityTypes.ClassInfo classInfoByEntityClassName = this.GetClassInfoByEntityClassName(entityClassTypeName);
                bool flag2 = classInfoByEntityClassName == null;
                if (flag2)
                {
                    Log.Error("EntityTypes: The class with name \"{0}\" is not exists. File \"{1}\".", entityClassTypeName, filePath);
                    result = null;
                }
                else
                {
                    ConstructorInfo constructor = classInfoByEntityClassName.TypeClassType.GetConstructor(new Type[0]);
                    EntityType entityType = (EntityType)constructor.Invoke(null);
                    for (EntityTypes.ClassInfo classInfo = classInfoByEntityClassName; classInfo != null; classInfo = classInfo.BaseClassInfo)
                    {
                        foreach (EntityTypes.ClassInfo.EntityTypeSerializableFieldItem current in classInfo.EntityTypeSerializableFields)
                        {
                            object value = current.Field.GetValue(entityType);
                            entityType.za.Add(current, value);
                        }
                    }
                    entityType.classInfo = classInfoByEntityClassName;
                    entityType.name = typeName;
                    this.types.Add(entityType);
                    this.entityTypeNameDic.Add(typeName, entityType);
                    this.entityTypeNetworkUIN += 1u;
                    entityType.networkUIN = this.entityTypeNetworkUIN;
                    result = entityType;
                }
            }
            return result;
        }
        private bool ManualCreateTypes()
        {
            bool result;
            foreach (EntityTypes.ClassInfo current in this.classes)
            {
                ManualTypeCreateAttribute[] array = (ManualTypeCreateAttribute[])current.typeClassType.GetCustomAttributes(typeof(ManualTypeCreateAttribute), false);
                for (int i = 0; i < array.Length; i++)
                {
                    ManualTypeCreateAttribute manualTypeCreateAttribute = array[i];
                    string text = manualTypeCreateAttribute.TypeName;
                    bool flag = text == null || text == "";
                    if (flag)
                    {
                        text = current.entityClassType.Name;
                    }
                    bool flag2 = this.ManualCreateType(text, current) == null;
                    if (flag2)
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }
        public EntityType ManualCreateType(string typeName, EntityTypes.ClassInfo classInfo)
        {
            bool flag = this.GetByName(typeName) != null;
            if (flag)
            {
                Log.Fatal("EntityTypes: ManualCreateType: type with name \"{0}\" already created.", typeName);
            }
            EntityType entityType = this.CreateEntityType(typeName, classInfo.EntityClassType.Name, null);
            bool flag2 = entityType == null;
            EntityType result;
            if (flag2)
            {
                result = null;
            }
            else
            {
                entityType.filePath = "";
                bool flag3 = string.IsNullOrEmpty(entityType.fullName);
                if (flag3)
                {
                    entityType.fullName = entityType.name;
                }
                entityType.manualCreated = true;
                result = entityType;
            }
            return result;
        }
        public EntityType LoadType(string p)
        {
            LongOperationCallbackManager.CallCallback("EntityTypes: PreLoadTypeFromFile: " + p);
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(p);
            bool flag = textBlock == null;
            EntityType result;
            if (flag)
            {
                result = null;
            }
            else
            {
                bool flag2 = textBlock.Children.Count != 1;
                if (flag2)
                {
                    result = null;
                }
                else
                {
                    TextBlock textBlock2 = textBlock.Children[0];
                    bool flag3 = textBlock2.Name != "type";
                    if (flag3)
                    {
                        result = null;
                    }
                    else
                    {
                        string data = textBlock2.Data;
                        EntityType entityType = this.loadEntityType(textBlock, p, p);
                        result = entityType;
                    }
                }
            }
            return result;
        }
        public void DestroyType(string p)
        {
            EntityType entityTypeByPath = this.GetEntityTypeByPath(p);
            this.DestroyType(entityTypeByPath);
        }
        public EntityType GetEntityTypeByPath(string p)
        {
            bool flag = p == null;
            EntityType result;
            if (flag)
            {
                result = null;
            }
            else
            {
                foreach (EntityType current in this.types)
                {
                    bool flag2 = p.Equals(current.FilePath, StringComparison.CurrentCultureIgnoreCase);
                    if (flag2)
                    {
                        result = current;
                        return result;
                    }
                }
                result = null;
            }
            return result;
        }
        public void DestroyType(EntityType type)
        {
            this.types.Remove(type);
            this.entityTypeNameDic.Remove(type.name);
            type.Dispose();
        }
        public bool LoadGroupOfTypes(string virtualDirectory, SearchOption searchOption, out List<EntityType> loadedTypes)
        {
            LongOperationCallbackManager.CallCallback("EntityTypes: LoadGroupOfTypes: " + virtualDirectory);
            loadedTypes = null;
            string[] array = new string[0];
            bool result;
            try
            {
                array = VirtualDirectory.GetFiles(virtualDirectory, "*.type", searchOption);
            }
            catch
            {
                Log.Error("EntityTypes: Getting list of type files failed.");
                bool flag = false;
                result = flag;
                return result;
            }
            loadedTypes = new List<EntityType>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                EntityType entityType = this.loadEntityTypeFromFile(text);
                bool flag2 = entityType == null;
                if (flag2)
                {
                    Log.Error("EntityTypes: Entity type loading failed \"{0}\".", text);
                    bool flag3 = false;
                    result = flag3;
                    return result;
                }
                loadedTypes.Add(entityType);
            }
            foreach (EntityType current in loadedTypes)
            {
                bool flag4 = !this.loadTypeFromLoadedTextBlock(current);
                if (flag4)
                {
                    bool flag5 = false;
                    result = flag5;
                    return result;
                }
            }
            result = true;
            return result;
        }
        public bool LoadGroupOfTypes(IList<TextBlock> blocks, out List<EntityType> loadedTypes)
        {
            LongOperationCallbackManager.CallCallback("EntityTypes: LoadGroupOfTypes");
            loadedTypes = new List<EntityType>(blocks.Count);
            bool result;
            foreach (TextBlock current in blocks)
            {
                EntityType entityType = this.loadEntityType(current, "", "");
                bool flag = entityType == null;
                if (flag)
                {
                    bool flag2 = false;
                    result = flag2;
                    return result;
                }
                loadedTypes.Add(entityType);
            }
            foreach (EntityType current2 in loadedTypes)
            {
                bool flag3 = !this.loadTypeFromLoadedTextBlock(current2);
                if (flag3)
                {
                    bool flag4 = false;
                    result = flag4;
                    return result;
                }
            }
            result = true;
            return result;
        }
        public EntityType LoadTypeFromFile(string virtualFileName)
        {
            EntityType entityType = this.loadEntityTypeFromFile(virtualFileName);
            bool flag = entityType == null;
            EntityType result;
            if (flag)
            {
                Log.Error("EntityTypes: LoadTypeFromFile: Entity type loading failed \"{0}\".", virtualFileName);
                result = null;
            }
            else
            {
                bool flag2 = !this.loadTypeFromLoadedTextBlock(entityType);
                if (flag2)
                {
                    result = null;
                }
                else
                {
                    result = entityType;
                }
            }
            return result;
        }
        public EntityType LoadTypeFromTextBlock(TextBlock block)
        {
            EntityType entityType = this.loadEntityType(block, "", "");
            bool flag = entityType == null;
            EntityType result;
            if (flag)
            {
                result = null;
            }
            else
            {
                bool flag2 = !this.loadTypeFromLoadedTextBlock(entityType);
                if (flag2)
                {
                    result = null;
                }
                else
                {
                    result = entityType;
                }
            }
            return result;
        }
        public List<EntityType> Editor_FindTypesWhoHasReferenceToType(EntityType type)
        {
            List<EntityType> list = new List<EntityType>();
            foreach (EntityType current in this.Types)
            {
                bool flag = current.OnIsExistsReferenceToObject(type);
                if (flag)
                {
                    list.Add(current);
                }
            }
            return list;
        }
        private EntityType loadEntityType(TextBlock textBlock, string text, string filePath)
        {
            bool flag = textBlock.Children.Count != 1;
            EntityType result;
            if (flag)
            {
                Log.Error(string.Format("EntityTypes: Need one root block \"{0}\".", filePath));
                result = null;
            }
            else
            {
                TextBlock textBlock2 = textBlock.Children[0];
                bool flag2 = textBlock2.Name != "type";
                if (flag2)
                {
                    Log.Error(string.Format("EntityTypes: Need block named \"type\" \"{0}\".", filePath));
                    result = null;
                }
                else
                {
                    bool flag3 = string.IsNullOrEmpty(textBlock2.Data);
                    if (flag3)
                    {
                        Log.Error(string.Format("EntityTypes: Need define entity type name \"{0}\".", filePath));
                        result = null;
                    }
                    else
                    {
                        string data = textBlock2.Data;
                        string attribute = textBlock2.GetAttribute("class");
                        EntityType entityType = this.CreateEntityType(data, attribute, filePath);
                        bool flag4 = entityType == null;
                        if (flag4)
                        {
                            result = null;
                        }
                        else
                        {
                            entityType.filePath = text;
                            entityType.fullName = entityType.name;
                            entityType.textBlock = textBlock2;
                            result = entityType;
                        }
                    }
                }
            }
            return result;
        }
        private EntityType loadEntityTypeFromFile(string p)
        {
            LongOperationCallbackManager.CallCallback("EntityTypes: PreLoadTypeFromFile: " + p);
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(p);
            bool flag = textBlock == null;
            EntityType result;
            if (flag)
            {
                result = null;
            }
            else
            {
                result = this.loadEntityType(textBlock, p, p);
            }
            return result;
        }
        private bool loadTypeFromLoadedTextBlock(EntityType entityType)
        {
            LongOperationCallbackManager.CallCallback("EntityTypes: LoadTypeFromLoadTextBlock: " + entityType.FilePath);
            bool flag = !entityType.loadEntityTypeFromTextBlock(entityType.textBlock);
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = !entityType.OnLoad(entityType.textBlock);
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    entityType.OnLoaded();
                    entityType.textBlock = null;
                    result = true;
                }
            }
            return result;
        }

        public bool SaveTypeToFile(EntityType type)
        {
            TextBlock textBlock = new TextBlock();
            TextBlock textBlock2 = textBlock.AddChild("type", type.Name);
            textBlock2.SetAttribute("class", type.ClassInfo.EntityClassType.Name);
            bool flag = !type.a(textBlock2);
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = !type.OnSave(textBlock2);
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(VirtualFileSystem.GetRealPathByVirtual(type.FilePath), FileMode.Create))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(fileStream))
                            {
                                streamWriter.Write(textBlock.DumpToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Save type to file failed: {0}", ex.ToString());
                        result = false;
                        return result;
                    }
                    result = true;
                }
            }
            return result;
        }

        public ClassInfo GetClassInfoByEntityClassType(Type entityClassType)
        {
            ClassInfo result;
            this.typeClassInfoDic.TryGetValue(entityClassType, out result);
            return result;
        }
        /// <summary>
        /// Finds the class information by the entity class name.
        /// </summary>
        /// <param name="entityClassName">The entity class name.</param>
        /// <returns>The class information or <b>null</b>..</returns>
        public ClassInfo GetClassInfoByEntityClassName(string entityClassName)
        {
            ClassInfo result;
            this.typeNameClassInfoDic.TryGetValue(entityClassName, out result);
            return result;
        }
        private EntityTypes.ClassInfo addClassInfo(Type type)
        {
            ClassInfo classInfo = this.GetClassInfoByEntityClassType(type);
            bool flag = classInfo != null;
            ClassInfo result;
            if (flag)
            {
                result = classInfo;
            }
            else
            {
                Type type2 = null;
                string name = type.FullName + "Type";
                foreach (Assembly current in EntitySystemWorld.Instance.EntityClassAssemblies)
                {
                    Type type3 = current.GetType(name);
                    bool flag2 = type3 != null;
                    if (flag2)
                    {
                        type2 = type3;
                        break;
                    }
                }
                bool flag3 = type2 == null;
                if (flag3)
                {
                    Log.Fatal(string.Format("Type class not defined for \"{0}\"", type.FullName));
                }
                bool flag4 = !typeof(EntityType).IsAssignableFrom(type2);
                if (flag4)
                {
                    Log.Fatal(string.Format("Type class not inherited by EntityType \"{0}\"", type2.FullName));
                }
                Type baseType = type.BaseType;
                EntityTypes.ClassInfo baseClassInfo = null;
                bool flag5 = baseType != null && typeof(Entity).IsAssignableFrom(baseType);
                if (flag5)
                {
                    baseClassInfo = this.addClassInfo(baseType);
                }
                classInfo = new EntityTypes.ClassInfo();
                classInfo.baseClassInfo = baseClassInfo;
                classInfo.typeClassType = type2;
                classInfo.entityClassType = type;
                classInfo.entityTypeSerializableFieldItemList = new List<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem>();
                classInfo.entityTypeSerializableFields = new ReadOnlyCollection<EntityTypes.ClassInfo.EntityTypeSerializableFieldItem>(classInfo.entityTypeSerializableFieldItemList);
                FieldInfo[] fields = classInfo.typeClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] array = fields;
                for (int i = 0; i < array.Length; i++)
                {
                    FieldInfo fieldInfo = array[i];
                    bool flag6 = !(fieldInfo.DeclaringType != type2);
                    if (flag6)
                    {
                        EntityType.FieldSerializeAttribute fieldSerializeAttribute = null;
                        EntityType.FieldSerializeAttribute[] array2 = (EntityType.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(EntityType.FieldSerializeAttribute), true);
                        bool flag7 = array2.Length != 0;
                        if (flag7)
                        {
                            fieldSerializeAttribute = array2[0];
                        }
                        bool flag8 = fieldSerializeAttribute != null;
                        if (flag8)
                        {
                            EntityTypes.ClassInfo.EntityTypeSerializableFieldItem entityTypeSerializableFieldItem = new EntityTypes.ClassInfo.EntityTypeSerializableFieldItem();
                            entityTypeSerializableFieldItem.field = fieldInfo;
                            classInfo.entityTypeSerializableFieldItemList.Add(entityTypeSerializableFieldItem);
                        }
                    }
                }
                classInfo.entitySerializableFieldItemList = new List<EntityTypes.ClassInfo.EntitySerializableFieldItem>();
                classInfo.entitySerializableFields = new ReadOnlyCollection<EntityTypes.ClassInfo.EntitySerializableFieldItem>(classInfo.entitySerializableFieldItemList);
                FieldInfo[] fields2 = classInfo.entityClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] array3 = fields2;
                for (int j = 0; j < array3.Length; j++)
                {
                    FieldInfo fieldInfo2 = array3[j];
                    bool flag9 = !(fieldInfo2.DeclaringType != type);
                    if (flag9)
                    {
                        Entity.FieldSerializeAttribute fieldSerializeAttribute2 = null;
                        Entity.FieldSerializeAttribute[] array4 = (Entity.FieldSerializeAttribute[])fieldInfo2.GetCustomAttributes(typeof(Entity.FieldSerializeAttribute), true);
                        bool flag10 = array4.Length != 0;
                        if (flag10)
                        {
                            fieldSerializeAttribute2 = array4[0];
                        }
                        bool flag11 = fieldSerializeAttribute2 != null;
                        if (flag11)
                        {
                            EntityTypes.ClassInfo.EntitySerializableFieldItem entitySerializableFieldItem = new EntityTypes.ClassInfo.EntitySerializableFieldItem();
                            entitySerializableFieldItem.field = fieldInfo2; 
                            classInfo.entitySerializableFieldItemList.Add(entitySerializableFieldItem);
                        }
                    }
                }
                classInfo.fieldInfo = classInfo.entityClassType.GetField("_type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                bool flag12 = classInfo.fieldInfo == null;
                if (flag12)
                {
                    FieldInfo[] fields3 = classInfo.entityClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array5 = fields3;
                    for (int k = 0; k < array5.Length; k++)
                    {
                        FieldInfo fieldInfo3 = array5[k];
                        bool flag13 = fieldInfo3.GetCustomAttributes(typeof(Entity.TypeFieldAttribute), false).Length != 0;
                        if (flag13)
                        {
                            classInfo.fieldInfo = fieldInfo3;
                            break;
                        }
                    }
                }
                bool flag14 = classInfo.fieldInfo == null && !typeof(LogicComponent).IsAssignableFrom(type);
                if (flag14)
                {
                    Log.Fatal("Field \"_type\" not defined for \"{0}\"", type.FullName);
                }
                MethodInfo[] methods = classInfo.entityClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo[] array6 = methods;
                for (int l = 0; l < array6.Length; l++)
                {
                    MethodInfo methodInfo = array6[l];
                    Entity.NetworkReceiveAttribute[] array7 = (Entity.NetworkReceiveAttribute[])methodInfo.GetCustomAttributes(typeof(Entity.NetworkReceiveAttribute), false);
                    bool flag15 = array7.Length != 0;
                    if (flag15)
                    {
                        Entity.NetworkReceiveAttribute networkReceiveAttribute = array7[0];
                        Entity.NetworkDirections direction = networkReceiveAttribute.Direction;
                        ushort messageIdentifier = networkReceiveAttribute.MessageIdentifier;
                        while (classInfo.networkSynchronizedMetaBuffer.Count <= (int)messageIdentifier)
                        {
                            classInfo.networkSynchronizedMetaBuffer.Add(null);
                        }
                        bool flag16 = classInfo.networkSynchronizedMetaBuffer[(int)messageIdentifier] != null;
                        if (flag16)
                        {
                            Log.Fatal("EntitySystem: NetworkSynchronizedAttribute is already defined for network message \"{0}\" for entity class \"{0}\". Method name is a \"{2}\"", messageIdentifier, classInfo.entityClassType.Name, methodInfo.Name);
                        }
                        classInfo.networkSynchronizedMetaBuffer[(int)messageIdentifier] = new EntityTypes.ClassInfo.NetworkSynchronizedMeta(direction, methodInfo);
                    }
                }
                this.classInfoNetworkUIN += 1u;
                classInfo.networkUIN = this.classInfoNetworkUIN;
                this.classes.Add(classInfo);
                this.typeClassInfoDic.Add(type, classInfo);
                this.typeNameClassInfoDic.Add(type.Name, classInfo);
                result = classInfo;
            }
            return result;
        }
        private void loadEntityFromAssembly()
        {
            foreach (Assembly current in EntitySystemWorld.Instance.EntityClassAssemblies)
            {
                Type[] array = current.GetTypes();
                Type[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    Type type = array2[i];
                    bool flag = typeof(Entity).IsAssignableFrom(type);
                    if (flag)
                    {
                        this.addClassInfo(type);
                    }
                }
            }
        }
        /// <summary>
        /// Finds the type on his file path.
        /// </summary>
        /// <param name="virtualFileName"></param>
        /// <returns>The entity type or <b>null</b>.</returns>
        public EntityType FindByFilePath(string virtualFileName)
        {
            string strB = VirtualFileSystem.NormalizePath(virtualFileName);
            EntityType result;
            foreach (EntityType current in EntityTypes.Instance.Types)
            {
                bool flag = string.Compare(current.FilePath, strB, true) == 0;
                if (flag)
                {
                    result = current;
                    return result;
                }
            }
            result = null;
            return result;
        }
        public void Editor_ChangeAllReferencesToType(string oldVirtualPathByReal, string newVirtualPathByReal)
        {
            EntityType entityType = this.GetEntityTypeByPath(newVirtualPathByReal);
            bool flag = entityType == null;
            if (flag)
            {
                entityType = this.LoadType(newVirtualPathByReal);
            }
            EntityType entityTypeByPath = this.GetEntityTypeByPath(oldVirtualPathByReal);
            bool flag2 = entityType == null || entityTypeByPath == null;
            if (!flag2)
            {
                this.Editor_ChangeAllReferencesToType(entityTypeByPath, entityType);
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newValue"></param>
        public void Editor_ChangeAllReferencesToType(EntityType type, EntityType newValue)
        {
            foreach (EntityType current in this.Types)
            {
                current.OnChangeReferencesToObject(type, newValue);
            }
        }
        /// <summary>
        /// Returns the list of types which are based on set class information.
        /// </summary>
        /// <param name="classInfo">The class information.</param>
        /// <returns>The list of types which are based on set class information.</returns>
        public List<EntityType> GetTypesBasedOnClass(EntityTypes.ClassInfo classInfo)
        {
            List<EntityType> list = new List<EntityType>();
            foreach (EntityType current in this.types)
            {
                for (EntityTypes.ClassInfo classInfo2 = current.classInfo; classInfo2 != null; classInfo2 = classInfo2.BaseClassInfo)
                {
                    bool flag = classInfo2 == classInfo;
                    if (flag)
                    {
                        list.Add(current);
                        break;
                    }
                }
            }
            return list;
        }
    }
}
