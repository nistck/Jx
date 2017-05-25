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
using Jx.FileSystem;

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
            internal ClassInfo baseClassInfo;
            internal List<EntityTypeSerializableFieldItem> entityTypeSerializableFieldItemList;
            internal ReadOnlyCollection<EntityTypeSerializableFieldItem> entityTypeSerializableFields;
            internal List<EntitySerializableFieldItem> entitySerializableFieldItemList;
            internal ReadOnlyCollection<EntitySerializableFieldItem> entitySerializableFields;
            internal FieldInfo fieldInfo;
            internal List<NetworkSynchronizedMeta> networkSynchronizedMetaBuffer = new List<NetworkSynchronizedMeta>();
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
            public ClassInfo BaseClassInfo
            {
                get
                {
                    return this.baseClassInfo;
                }
            }
            public IList<EntityTypeSerializableFieldItem> EntityTypeSerializableFields
            {
                get
                {
                    return this.entityTypeSerializableFields;
                }
            }
            public IList<EntitySerializableFieldItem> EntitySerializableFields
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

        public class ReferenceInfo
        {
            private readonly List<ClassInfo.EntityTypeSerializableFieldItem> items = new List<ClassInfo.EntityTypeSerializableFieldItem>();
            public ReferenceInfo(EntityType sourceType, EntityType entityType)
            {
                this.SourceType = sourceType;
                this.EntityType = entityType; 
            }

            /// <summary>
            /// 被引用类型
            /// </summary>
            public EntityType SourceType { get; private set; }
            public EntityType EntityType { get; private set; }
            /// <summary>
            /// 引用
            /// </summary>
            public object Target { get; set; }

            public void Add(ClassInfo.EntityTypeSerializableFieldItem item)
            {
                if (item == null)
                    return;

                items.Add(item); 
            }

            public void AddRange(List<ClassInfo.EntityTypeSerializableFieldItem> r)
            {
                if (r == null)
                    return;

                items.AddRange(r);
            }

            public IReadOnlyList<ClassInfo.EntityTypeSerializableFieldItem> Items
            {
                get { return items.AsReadOnly(); }
            }

            public bool Update(EntityType newType = null)
            {
                if (newType == null || SourceType == null)
                    return false;

                List<ClassInfo.EntityTypeSerializableFieldItem> L = new List<ClassInfo.EntityTypeSerializableFieldItem>();
                List<ClassInfo.EntityTypeSerializableFieldItem> fields = new List<ClassInfo.EntityTypeSerializableFieldItem>();
                fields.AddRange(items);

                while(fields.Count > 0)
                {
                    ClassInfo.EntityTypeSerializableFieldItem item = fields[0];
                    fields.RemoveAt(0);
                    try
                    {
                        item.Field.SetValue(Target, newType);
                        L.Add(item);
                    }
                    catch (Exception) { }

                }
                 
                if( fields.Count > 0 )
                {   // 有失败的

                    while (L.Count > 0)
                    {
                        ClassInfo.EntityTypeSerializableFieldItem item = L[0];
                        L.RemoveAt(0);
                        try
                        {
                            item.Field.SetValue(Target, SourceType); 
                        }
                        catch (Exception) { } 
                    }

                    return false;
                }

                return true;
            }
        }

        private static EntityTypes instance;
        private List<ClassInfo> classes = new List<ClassInfo>();
        private ReadOnlyCollection<ClassInfo> readonlyClasses;
        private Dictionary<Type, ClassInfo> typeClassInfoDic = new Dictionary<Type, ClassInfo>();
        private Dictionary<string, ClassInfo> typeNameClassInfoDic = new Dictionary<string, ClassInfo>();
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
                return instance;
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
        public IList<ClassInfo> Classes
        {
            get
            {
                return readonlyClasses;
            }
        }

        internal static bool Init()
        { 
            instance = new EntityTypes();
            bool ok = instance.Setup(); 
            if (!ok)
            {
                Shutdown();
            }
            return ok;
        }

        internal static void Shutdown()
        {
            if (instance != null)
            {
                instance.Free();
                instance = null;
            }
        }

        internal EntityTypes()
        {
            this.readonlyClasses = this.classes.AsReadOnly();
            this.readonlyTypes = this.types.AsReadOnly();
        }

        private bool Setup()
        {
            loadEntityFromAssembly();
            List<EntityType> list;

            bool b1 = ManualCreateTypes(); 
            bool b2 = LoadGroupOfTypes("", SearchOption.AllDirectories, out list);

            return b1 && b2;
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
            entityTypeNameDic.TryGetValue(name, out result);
            return result;
        }

        private EntityType CreateEntityType(string typeName, string entityClassTypeName, string filePath)
        {
            EntityType byName = GetByName(typeName);
            if( byName != null )
            {
                Log.Error(string.Format("EntityTypes: Entity type with name \"{0}\" is already exists.\nFiles:\n\"{1}\",\n\"{2}\".", typeName, byName.FilePath, filePath), typeName);
                return null; 
            }

            ClassInfo classInfoByEntityClassName = GetClassInfoByEntityClassName(entityClassTypeName);
            if (classInfoByEntityClassName == null)
            {
                Log.Error("EntityTypes: The class with name \"{0}\" is not exists. File \"{1}\".", entityClassTypeName, filePath);
                return null;
            }

#if DEBUG_ENTITY
            long _ts0 = DateTime.Now.Ticks;
#endif

            ConstructorInfo constructor = classInfoByEntityClassName.TypeClassType.GetConstructor(new Type[0]);
            EntityType entityType = (EntityType)constructor.Invoke(null);
            for (ClassInfo classInfo = classInfoByEntityClassName; classInfo != null; classInfo = classInfo.BaseClassInfo)
            {
                foreach (ClassInfo.EntityTypeSerializableFieldItem current in classInfo.EntityTypeSerializableFields)
                {
                    object value = current.Field.GetValue(entityType);
                    entityType.entityTypeSerializableFields.Add(current, value);
                }
            }
            entityType.classInfo = classInfoByEntityClassName;
            entityType.name = typeName;
            types.Add(entityType);
            entityTypeNameDic.Add(typeName, entityType);
            entityTypeNetworkUIN += 1u;
            entityType.networkUIN = this.entityTypeNetworkUIN;
#if DEBUG_ENTITY
            long _ts1 = DateTime.Now.Ticks;
            Log.Info(">> 创建EntityType: {0}, TypeName: {1}, 路径: {2}, 用时: {3} ms",
                entityType, entityClassTypeName, filePath, (_ts1 - _ts0) / 10000);
#endif
            return entityType;
        }

        private bool ManualCreateTypes()
        {
            var q = classes
                .Select(_clazz => new Tuple<ClassInfo, ManualTypeCreateAttribute>(_clazz, _clazz.TypeClassType.GetCustomAttribute<ManualTypeCreateAttribute>(false)))
                .Where(_t => _t.Item2 != null)
                .Select(_t => new Tuple<ClassInfo, string>(_t.Item1, _t.Item2.TypeName?? _t.Item1.EntityClassType.Name))
                ;

            int totalCount = q.Count();
            int readyCount = 0;
            foreach(Tuple<ClassInfo, string> tx in q)
            {
                EntityType entityType = ManualCreateType(tx.Item2, tx.Item1);
                readyCount++;
                LongOperationNotifier.Notify("手工创建EntityType ({0}/{1}): {2}", readyCount, totalCount, entityType);
                if (entityType == null)
                {
                    LongOperationNotifier.Notify("手工创建EntityType失败, ClassInfo: {0}", tx.Item1);
                    return false;
                }
            }
            
            return true;
        }

        public EntityType ManualCreateType(string typeName, ClassInfo classInfo)
        {
            EntityType entityTypeExists = GetByName(typeName);
            if( entityTypeExists != null ) 
            {
                Log.Fatal("EntityTypes: ManualCreateType: type with name \"{0}\" already created.", typeName);
                return null; 
            }

            EntityType entityType = CreateEntityType(typeName, classInfo.EntityClassType.Name, null);
            if (entityType == null)
                return null; 

            entityType.filePath = "";
            if(string.IsNullOrEmpty(entityType.fullName) )
                entityType.fullName = entityType.name;
            entityType.manualCreated = true;

            return entityType; 
        }

        public EntityType LoadType(string p)
        {
            LongOperationNotifier.Notify("加载EntityType: {0}", p);
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(p);
            if (textBlock == null || textBlock.Children.Count != 1)
                return null; 

            TextBlock tc = textBlock.Children[0];
            if (tc.Name != "type")
                return null;

            EntityType entityType = loadEntityType(textBlock, p, p);    // LoadType
            return entityType;
        }

        public void DestroyType(string p)
        {
            EntityType entityTypeByPath = GetEntityTypeByPath(p);
            DestroyType(entityTypeByPath);
        }

        public EntityType GetEntityTypeByPath(string p)
        {
            if (p == null)
                return null;

            string p1 = VirtualFileSystem.GetRealPathByVirtual(p);

            EntityType entityType = types.Where(_t => {
                bool b1 = p.Equals(_t.FilePath, StringComparison.CurrentCultureIgnoreCase);
                bool b2 = p1.Equals(_t.FilePath, StringComparison.CurrentCultureIgnoreCase);
                return b1 || b2;
            }).FirstOrDefault();
            return entityType;
        }

        public void DestroyType(EntityType type)
        {
            types.Remove(type);
            entityTypeNameDic.Remove(type.name);
            type.Dispose();
        }

        public bool LoadGroupOfTypes(string virtualDirectory, SearchOption searchOption, out List<EntityType> loadedTypes)
        {
            LongOperationNotifier.Notify("从资源目录中搜索type文件: {0}", virtualDirectory);
            loadedTypes = null;
            string[] typeFiles = new string[0]; 
            try
            {
                typeFiles = VirtualDirectory.GetFiles(virtualDirectory, "*.type", searchOption);
            }
            catch
            {
                Log.Error("EntityTypes: Getting list of type files failed.");
                return false;
            }

#if DEBUG_ENTITY
            string _targetDirectory = VirtualFileSystem.GetRealPathByVirtual(virtualDirectory);
            Log.Info(">> 搜索资源目录: {0}, 找到{1}个type文件", _targetDirectory, typeFiles.Length);
#endif
            loadedTypes = new List<EntityType>(typeFiles.Length);
            for (int i = 0; i < typeFiles.Length; i++)
            {
                string typeFile = typeFiles[i];
                EntityType entityType = loadEntityTypeFromFile(typeFile);
                if (entityType == null)
                {
                    Log.Error("EntityTypes: Entity type loading failed \"{0}\".", typeFile);

                    LongOperationNotifier.Notify("加载type文件失败: {0}", typeFile);
                    return false;
                }
                loadedTypes.Add(entityType);

                LongOperationNotifier.Notify("加载type文件 ({0}/{1}): {2}", i + 1, typeFiles.Length, typeFile);

#if DEBUG_ENTITY
                Log.Info(">> #{0:000} EntityType: {1}, 文件: {2}", i + 1, entityType, typeFile);
#endif
            }

            foreach (EntityType type in loadedTypes)
            {
                bool loadFailure = !loadTypeFromLoadedTextBlock(type);
                if (loadFailure)
                    return false; 
            }
            return true;
        }

        public bool LoadGroupOfTypes(IList<TextBlock> blocks, out List<EntityType> loadedTypes)
        {
            loadedTypes = new List<EntityType>(blocks.Count);
            
            foreach (TextBlock current in blocks)
            {
                EntityType entityType = loadEntityType(current, "", "");    // LoadGroupOfTypes
                if (entityType == null)
                    return false;  

                loadedTypes.Add(entityType);
            }
            foreach (EntityType entityType in loadedTypes)
            {
                bool state = loadTypeFromLoadedTextBlock(entityType);
                if (!state)
                    return false;  
            }
            return true;
        }

        public EntityType LoadTypeFromFile(string virtualFileName)
        {
            EntityType entityType = loadEntityTypeFromFile(virtualFileName);
            if( entityType == null)
            {
                Log.Error("EntityTypes: LoadTypeFromFile: Entity type loading failed \"{0}\".", virtualFileName);
                return null; 
            }

            if (!loadTypeFromLoadedTextBlock(entityType))
                return null;

            return entityType; 
        }

        public EntityType LoadTypeFromTextBlock(TextBlock block)
        {
            EntityType entityType = loadEntityType(block, "", "");  // LoadTypeFromTextBlock
            if (entityType == null)
                return null;

            if (!loadTypeFromLoadedTextBlock(entityType))
                return null;

            return entityType;
        }
         
        public List<EntityType> FindTypesWhoHasReferenceToType(EntityType type)
        {
            List<EntityType> list = new List<EntityType>();
            foreach (EntityType current in this.Types)
            {
                if (current.OnIsExistsReferenceToObject(type))
                {
                    list.Add(current);
                    continue;
                }
            }
            return list;
        }

        private EntityType loadEntityType(TextBlock textBlock, string text, string filePath)
        {
            if( textBlock == null || textBlock.Children.Count != 1 )
            {
                Log.Error(string.Format("EntityTypes: Need one root block \"{0}\".", filePath));
                return null; 
            }

            TextBlock tc = textBlock.Children[0];
            if( tc.Name != "type" )
            {
                Log.Error(string.Format("EntityTypes: Need block named \"type\" \"{0}\".", filePath));
                return null;
            }

            if( string.IsNullOrEmpty(tc.Data))
            {
                Log.Error(string.Format("EntityTypes: Need define entity type name \"{0}\".", filePath));
                return null; 
            }

            string data = tc.Data;
            string attribute = tc.GetAttribute("class");
            EntityType entityType = CreateEntityType(data, attribute, filePath);
            if (entityType == null)
                return null;

            entityType.filePath = text;
            entityType.fullName = entityType.name;
            entityType.textBlock = tc;
            return entityType;
        }

        private EntityType loadEntityTypeFromFile(string p)
        {
            LongOperationNotifier.Notify("加载type文件: {0}", p);
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(p);
            if (textBlock == null)
                return null; 
                        
            EntityType result = loadEntityType(textBlock, p, p);    // loadEntityTypeFromFile
            return result;
        }

        private bool loadTypeFromLoadedTextBlock(EntityType entityType)
        {
            LongOperationNotifier.Notify("从TextBlock初始化EntityType: {0}", entityType.FilePath);
            bool loadFromTextBlockFailure = !entityType.loadEntityTypeFromTextBlock(entityType.textBlock);
            if (loadFromTextBlockFailure)
                return false;

            bool initByTextBlockFailure = !entityType._OnLoad(entityType.textBlock);
            if (initByTextBlockFailure)
                return false;

            entityType._OnLoaded();
            entityType.textBlock = null;
            return true;
        }

        public bool SaveTypeToFile(EntityType type)
        {
            TextBlock textBlock = new TextBlock();
            TextBlock typeBlock = textBlock.AddChild("type", type.Name);
            typeBlock.SetAttribute("class", type.ClassInfo.EntityClassType.Name);
 
            if (!type.Save(typeBlock))
                return false;

            if (!type._OnSave(typeBlock))
                return false;

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
                return false; 
            }
            return true;
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
            typeNameClassInfoDic.TryGetValue(entityClassName, out result);
            return result;
        }

        private ClassInfo addClassInfo(Type type)
        {
            if (type == null)
                return null; 

            ClassInfo classInfo = GetClassInfoByEntityClassType(type);
            if (classInfo != null)
                return classInfo; 
            
            string typeName = type.FullName + "Type";
            Type typeEntityType = EntitySystemWorld.Instance.FindEntityClassType(typeName);

            if (typeEntityType == null)
            {
                Log.Fatal(string.Format("Type class not defined for \"{0}\"", type.FullName));
                return null;
            }

            if (!typeof(EntityType).IsAssignableFrom(typeEntityType))
            {
                Log.Fatal(string.Format("Type class not inherited by EntityType \"{0}\"", typeEntityType.FullName));
                return null;
            }

            Type baseType = type.BaseType;
            ClassInfo baseClassInfo = null;
            if (baseType != null && typeof(Entity).IsAssignableFrom(baseType))
            {
                baseClassInfo = addClassInfo(baseType);
            }

            classInfo = new ClassInfo();
            classInfo.baseClassInfo = baseClassInfo;
            classInfo.typeClassType = typeEntityType;
            classInfo.entityClassType = type;

            #region EntityType 序列化字段
            classInfo.entityTypeSerializableFieldItemList = new List<ClassInfo.EntityTypeSerializableFieldItem>();
            classInfo.entityTypeSerializableFields = new ReadOnlyCollection<ClassInfo.EntityTypeSerializableFieldItem>(classInfo.entityTypeSerializableFieldItemList);
            FieldInfo[] entityTypeFields = classInfo.typeClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < entityTypeFields.Length; i++)
            {
                FieldInfo fieldInfo = entityTypeFields[i];
                if (fieldInfo.DeclaringType != typeEntityType)
                    continue;

                EntityType.FieldSerializeAttribute fieldSerializeAttribute = null;
                EntityType.FieldSerializeAttribute[] rAttrs = (EntityType.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(EntityType.FieldSerializeAttribute), true);
                if (rAttrs.Length > 0)
                    fieldSerializeAttribute = rAttrs[0];

                if (fieldSerializeAttribute == null)
                    continue;

                ClassInfo.EntityTypeSerializableFieldItem entityTypeSerializableFieldItem = new ClassInfo.EntityTypeSerializableFieldItem();
                entityTypeSerializableFieldItem.field = fieldInfo;
                classInfo.entityTypeSerializableFieldItemList.Add(entityTypeSerializableFieldItem);
            }
            #endregion

            #region Entity 序列化字段
            classInfo.entitySerializableFieldItemList = new List<ClassInfo.EntitySerializableFieldItem>();
            classInfo.entitySerializableFields = new ReadOnlyCollection<EntityTypes.ClassInfo.EntitySerializableFieldItem>(classInfo.entitySerializableFieldItemList);
            FieldInfo[] entityFields = classInfo.entityClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int j = 0; j < entityFields.Length; j++)
            {
                FieldInfo fieldInfo = entityFields[j];
                if (fieldInfo.DeclaringType == typeEntityType)
                    continue;

                Entity.FieldSerializeAttribute fieldSerializeAttribute = null;
                Entity.FieldSerializeAttribute[] rAttrs = (Entity.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(Entity.FieldSerializeAttribute), true);
                if (rAttrs.Length > 0)
                    fieldSerializeAttribute = rAttrs[0];

                if (fieldSerializeAttribute == null)
                    continue;

                ClassInfo.EntitySerializableFieldItem entitySerializableFieldItem = new ClassInfo.EntitySerializableFieldItem();
                entitySerializableFieldItem.field = fieldInfo;
                classInfo.entitySerializableFieldItemList.Add(entitySerializableFieldItem);
            }
            #endregion

            #region _type字段
            FieldInfo[] typeFields = classInfo.entityClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int k = 0; k < typeFields.Length; k++)
            {
                FieldInfo fieldInfo = typeFields[k];
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(Entity.TypeFieldAttribute), false);
                if (attrs.Length == 0)
                    continue;
                classInfo.fieldInfo = fieldInfo;
                break;
            }
            
            if (classInfo.fieldInfo == null)
                classInfo.fieldInfo = classInfo.entityClassType.GetField("_type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (classInfo.fieldInfo == null && !typeof(LogicComponent).IsAssignableFrom(type))
            {
                Log.Fatal("Field \"_type\" not defined for \"{0}\"", type.FullName);
                return null;
            }
            #endregion

            #region 网络处理函数
            MethodInfo[] methods = classInfo.entityClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int l = 0; l < methods.Length; l++)
            {
                MethodInfo methodInfo = methods[l];
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
                        return null;
                    }
                    classInfo.networkSynchronizedMetaBuffer[(int)messageIdentifier] = new EntityTypes.ClassInfo.NetworkSynchronizedMeta(direction, methodInfo);
                }
            }
            #endregion

            classInfoNetworkUIN += 1u;
            classInfo.networkUIN = classInfoNetworkUIN;
            classes.Add(classInfo);
            typeClassInfoDic.Add(type, classInfo);
            typeNameClassInfoDic.Add(type.Name, classInfo);
            return classInfo;
        }

        private void loadEntityFromAssembly()
        {            
            var q = EntitySystemWorld.Instance.EntityClassTypes.Where(_type => typeof(Entity).IsAssignableFrom(_type));

            int totalCount = q.Count();
            int readyCount = 0; 
            foreach (Type type in q)
            {
                addClassInfo(type);
                readyCount++;
                LongOperationNotifier.Notify("初始化EntityType类型信息 ({0}/{1}): {2}", readyCount, totalCount, type);
#if DEBUG_ENTITY
                Log.Info(">> Entity类型: {0} ", type);
#endif
            }
        }

        /// <summary>
        /// Finds the type on his file path.
        /// </summary>
        /// <param name="virtualFileName"></param>
        /// <returns>The entity type or <b>null</b>.</returns>
        public EntityType FindByFilePath(string virtualFileName)
        {
            string filePath = VirtualFileSystem.NormalizePath(virtualFileName);
            foreach (EntityType current in EntityTypes.Instance.Types)
            {
                if (string.Compare(current.FilePath, filePath, true) == 0)
                    return current;
            }
            return null;
        }

        public void ChangeAllReferencesToType(string oldVirtualPathByReal, string newVirtualPathByReal)
        {
            EntityType entityType = this.GetEntityTypeByPath(newVirtualPathByReal);
            if (entityType == null)
                entityType = LoadType(newVirtualPathByReal);
            
            EntityType entityTypeByPath = GetEntityTypeByPath(oldVirtualPathByReal);
            if (entityType != null && entityTypeByPath != null)
                ChangeAllReferencesToType(entityTypeByPath, entityType);

        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newValue"></param>
        public List<ReferenceInfo> ChangeAllReferencesToType(EntityType type, EntityType newValue)
        {
            List<ReferenceInfo> result = new List<ReferenceInfo>();
            foreach (EntityType current in Types)
            {
                List<ClassInfo.EntityTypeSerializableFieldItem> r = current.OnChangeReferencesToObject(type, newValue);
                ReferenceInfo ri = new ReferenceInfo(type, current);
                ri.AddRange(r);
                result.Add(ri);
            }
            return result;
        } 

        /// <summary>
        /// Returns the list of types which are based on set class information.
        /// </summary>
        /// <param name="classInfo">The class information.</param>
        /// <returns>The list of types which are based on set class information.</returns>
        public List<EntityType> GetTypesBasedOnClass(ClassInfo classInfo)
        {
            List<EntityType> list = new List<EntityType>();
            foreach (EntityType current in types)
            {
                for (ClassInfo classInfo2 = current.classInfo; classInfo2 != null; classInfo2 = classInfo2.BaseClassInfo)
                {
                    if (classInfo2 == classInfo)
                    {
                        list.Add(current);
                        break;
                    }
                }
            }
            return list;
        }

        public static List<ClassInfo> GetClassInfo(EntityType entityType)
        {
            List<ClassInfo> classInfoLink = new List<ClassInfo>();
            if (entityType == null)
                return classInfoLink;
            return entityType.GetClassInfo(); 
        }
    }
}
