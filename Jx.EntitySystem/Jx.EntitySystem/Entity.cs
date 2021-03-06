﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Jx.FileSystem;

using System.Reflection;

using Jx.EntitySystem.LogicSystem;
using Jx.Ext;

namespace Jx.EntitySystem
{
    [LogicSystemBrowsable(true), Editor(typeof(EditorEntityUITypeEditor), typeof(UITypeEditor))]
    public class Entity 
    {
 
        [Flags]
        public enum FieldSerializeSerializationTypes
        {
            Map = 1,
            World = 2
        }
        /// <summary>
        /// Specifies that a field will be serialized. This class cannot be inherited.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public sealed class FieldSerializeAttribute : Attribute
        {
            private string propertyName;
            private FieldSerializeSerializationTypes serializationTypes = FieldSerializeSerializationTypes.Map | FieldSerializeSerializationTypes.World;
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
            public FieldSerializeSerializationTypes SupportedSerializationTypes
            {
                get
                {
                    return this.serializationTypes;
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
            /// <param name="supportedSerializationTypes">The supported serialization types.</param>
            public FieldSerializeAttribute(string propertyName, FieldSerializeSerializationTypes supportedSerializationTypes)
            {
                this.propertyName = propertyName;
                this.serializationTypes = supportedSerializationTypes;
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            public FieldSerializeAttribute(string propertyName)
            {
                this.propertyName = propertyName;
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="supportedSerializationTypes">The supported serialization types.</param>
            public FieldSerializeAttribute(Entity.FieldSerializeSerializationTypes supportedSerializationTypes)
            {
                this.serializationTypes = supportedSerializationTypes;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public sealed class TypeFieldAttribute : Attribute
        {
        }

        public enum NetworkDirections
        {
            ToClient,
            ToServer
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class NetworkReceiveAttribute : Attribute
        {
            private NetworkDirections networkDirection;
            private ushort messageIdentifier;
            public NetworkDirections Direction
            {
                get
                {
                    return this.networkDirection;
                }
            }
            public ushort MessageIdentifier
            {
                get
                {
                    return this.messageIdentifier;
                }
            }
            public NetworkReceiveAttribute(Entity.NetworkDirections direction, ushort messageIdentifier)
            {
                this.networkDirection = direction;
                this.messageIdentifier = messageIdentifier;
            }
        }

        public class TagInfo
        {
            [FieldSerialize("name")]
            private string name = "";
            [FieldSerialize("value")]
            private string value = "";
            public string Name
            {
                get { return name; }
                set { this.name = value; }
            }
            public string Value
            {
                get { return value; }
                set { this.value = value; }
            }

            public TagInfo()
            {
            }

            public TagInfo(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public override string ToString()
            {
                return string.Format("{0} = {1}", this.name, this.value);
            }
        } 


        public delegate void NameChangedDelegate(Entity entity);
        public delegate void CreateDelegate(Entity entity);
        public delegate void PostCreateDelegate(Entity entity, bool loaded);
        public delegate void DestroyingDelegate(Entity entity);
        public delegate void DestroyedDelegate(Entity entity);
        public delegate void TickDelegate(Entity entity);
        public delegate void DeleteSubscribedToDeletionEventDelegate(Entity entity, Entity deletedEntity);

        internal static float tickDelta;

        [TypeField]
        private EntityType entityType = null;
        internal Entity parent;
        internal LinkedListNode<Entity> zD;
        internal uint uin;
        internal uint networkUIN;
        private LogicClass logicClass;

        [FieldSerialize("logicObject")]
        private LogicEntityObject logicObject;

        private bool allowSave = true;
        private static LinkedList<Entity> emptyChildren = new LinkedList<Entity>();
        private LinkedList<Entity> children;
        internal bool isPostCreated;
        internal bool isSetForDeletion;
        internal bool isDestroyed;

        [FieldSerialize("name")]
        private string name = ""; 

        internal List<Entity> subscriptionsToDeletionEvent;
        private LinkedListNode<Entity> zj;
        private int subscribeToTickEventCount;
        internal int tickRound;
        private object userData;
        [FieldSerialize("tags")]
        private List<TagInfo> tagInfos = new List<TagInfo>();
        private bool editorSelectable = true;
        private bool editor_excludeEntityFromWorld;
        internal float createTime;
        private EntityExtendedProperties extendedProperties;
        internal TextBlock loadingTextBlock;
        //private static RemoteEntityWorld[] toRemoteEntityWorlds = new RemoteEntityWorld[1];

        [LogicSystemBrowsable(true)]
        public event NameChangedDelegate NameChanged;

        [LogicSystemBrowsable(true)]
        public event CreateDelegate Create;

        [LogicSystemBrowsable(true)]
        public event PostCreateDelegate PostCreated;

        [LogicSystemBrowsable(true)]
        public event DestroyingDelegate Destroying;

        [LogicSystemBrowsable(true)]
        public event DestroyedDelegate Destroyed;

        [LogicSystemBrowsable(true)]
        public event TickDelegate Tick;

        [LogicSystemBrowsable(true)]
        public event DeleteSubscribedToDeletionEventDelegate DeleteSubscribedToDeletionEvent;


        [LocalizedDescription("The type of this object.", "Entity")]
        public EntityType Type
        {
            get
            {
                return this.entityType;
            }
        }

        [Browsable(false)]
        public Entity Parent
        {
            get
            {
                return this.parent;
            }
        }

        [Browsable(false)]
        public uint UIN
        {
            get
            {
                return this.uin;
            }
        }
 
        [Browsable(false)]
        public uint NetworkUIN
        {
            get
            {
                return this.networkUIN;
            }
        }

        [Browsable(false)]
        public float CreateTime
        {
            get
            {
                return this.createTime;
            }
        }

        [Browsable(false)]
        public bool IsSetForDeletion
        {
            get
            {
                return this.isSetForDeletion;
            }
        }


        [LogicSystemBrowsable(true), LocalizedDescription("The name of the object. The name of the object is always unique on the map. The name can be empty, when the property AllowEmptyName of the object type is enabled.", "Entity")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (this.isSetForDeletion || this.isDestroyed)
                {
                    return;
                }
                if (!this.Type.AllowEmptyName && value == "")
                {
                    throw new InvalidOperationException("The empty name of entity is forbidden.");
                }
                if (value != "")
                {
                    Entity byName = Entities.Instance.GetByName(value);
                    if (byName != null && byName != this)
                    {
                        throw new InvalidOperationException(string.Format("The name of entity is already occupied by \"{0}\".", byName.ToString()));
                    }
                }
                if (!this.Editor_IsExcludedFromWorld())
                {
                    if (this.name != "")
                    {
                        Entities.Instance.entitySetByName.Remove(this.name);
                    }
                    if (value != "")
                    {
                        Entities.Instance.entitySetByName[value] = this;
                    }
                }
                this.name = value;
                this.OnNameChange();
            }
        }

        [Browsable(false)]
        public LinkedList<Entity> Children
        {
            get
            {
                if (this.children == null)
                {
                    return Entity.emptyChildren;
                }
                return this.children;
            }
        }

        [Browsable(false)]
        public bool IsPostCreated
        {
            get
            {
                return this.isPostCreated;
            }
        }

        [Browsable(false)]
        public bool IsDestroyed
        {
            get
            {
                return this.isDestroyed;
            }
        }

        [LocalizedDescription("Link to the class of Logic Editor. Using the editor you can add logic event handlers, algorithms in a visual style, as well as additional code in C#. For creation the class of Logic Editor use double clicking of the left mouse button.", "Entity"), DefaultValue(null)]
        public LogicClass LogicClass
        {
            get
            {
                return logicClass;
            }
            set
            {
                if (logicClass != null)
                {
                    resetLogicObject();
                    if (!EntitySystemWorld.Instance.IsEditor())
                        UnsubscribeToDeletionEvent(logicClass);
                }
                logicClass = value;
                if (logicClass != null)
                {
                    SubscribeToDeletionEvent(logicClass);
                    if (!EntitySystemWorld.Instance.IsEditor())
                        CreateLogicObject();
                }
            }
        }

        [Browsable(false)]
        public static float TickDelta
        {
            get
            { 
                return tickDelta;
            }
        }

        [Browsable(false)]
        public object UserData
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        [Browsable(false)]
        public bool AllowSave
        {
            get
            {
                return this.allowSave;
            }
            set
            {
                this.allowSave = value;
            }
        }

        [Browsable(false)]
        public virtual bool EditorSelectable
        {
            get
            {
                return this.editorSelectable;
            }
            set
            {
                this.editorSelectable = value;
            }
        }

        [LogicSystemBrowsable(true), Browsable(false)]
        public LogicEntityObject LogicObject
        {
            get
            {
                return logicObject;
            }
        }

        [LocalizedDescription("The list of additional properties of the object. The programmer can create special class with the list of properties to expand the object.", "Entity"), Editor(typeof(EditorEntityExtendedPropertiesUITypeEditor), typeof(UITypeEditor))]
        public EntityExtendedProperties ExtendedProperties
        {
            get
            {
                return this.extendedProperties;
            }
        }

        [Browsable(false)]
        public TextBlock LoadingTextBlock
        {
            get
            {
                return this.loadingTextBlock;
            }
        }

        [LocalizedDescription("Any additional text information that can be useful to developer.", "Entity"), Editor("Engine.EntitySystem.Editor.EntityTagsCollectionEditor, EntitySystem.Editor", typeof(UITypeEditor)), TypeConverter(typeof(CollectionTypeConverter))]
        public List<TagInfo> Tags
        {
            get
            {
                return this.tagInfos;
            }
            set
            {
                this.tagInfos = value;
            }
        }

        protected Entity()
        {
            this.zD = new LinkedListNode<Entity>(this);
        }

        [LogicSystemBrowsable(true)]
        public void SetForDeletion(bool allowToCancelDeletion)
        {
            if (!this.isSetForDeletion)
            {
                if (allowToCancelDeletion)
                { 
                    if (this.OnCancelDeletion())
                    {
                        return;
                    }
                }
                Entities.Instance.SetForDeletion(this);
                foreach (Entity current in this.Children)
                {
                    current.SetForDeletion(true);
                } 
                this.isSetForDeletion = true;
            }
        }

        [Obsolete]
        public void SetDeleted()
        {
            this.SetForDeletion(false);
        }
         
        protected virtual bool OnCancelDeletion()
        {
            return false;
        }

        protected virtual void OnNameChange()
        {
            if (NameChanged != null)
                NameChanged(this);
        }

        public void SubscribeToDeletionEvent(Entity entity)
        {
            if (entity == this)
                Log.Fatal("Entity: SubscribeToDeletionEvent: entity == this.");

            if (this.subscriptionsToDeletionEvent == null)
                this.subscriptionsToDeletionEvent = new List<Entity>();

            this.subscriptionsToDeletionEvent.Add(entity);
            if (entity.subscriptionsToDeletionEvent == null)
                entity.subscriptionsToDeletionEvent = new List<Entity>();

            entity.subscriptionsToDeletionEvent.Add(this);
        }

        public void UnsubscribeToDeletionEvent(Entity entity)
        {
            if (this.subscriptionsToDeletionEvent != null)
            {
                for (int i = this.subscriptionsToDeletionEvent.Count - 1; i >= 0; i--)
                {
                    if (this.subscriptionsToDeletionEvent[i] == entity)
                    {
                        this.subscriptionsToDeletionEvent.RemoveAt(i);
                        break;
                    }
                }
            }
            if (entity.subscriptionsToDeletionEvent != null)
            {
                for (int j = entity.subscriptionsToDeletionEvent.Count - 1; j >= 0; j--)
                {
                    if (entity.subscriptionsToDeletionEvent[j] == this)
                    {
                        entity.subscriptionsToDeletionEvent.RemoveAt(j);
                        return;
                    }
                }
            }
        }

        [LogicSystemBrowsable(true)]
        public void PostCreate()
        {
            if (this.parent == null && World.Instance != this)
            {
                Log.Fatal("Entity: PostCreate: parent == null");
            }
            if (this.IsPostCreated)
            {
                Log.Fatal("Entity: PostCreate: Already post created.");
            }
            this.A(false);
            _OnCreate();
            this.EntityCreated();
            this.OnPostCreate(false);
            this.OnPostCreate2(false);
            this.isPostCreated = true;
            this._OnPostCreated(false);

            if (EntitySystemWorld.Instance.IsServer() && this.Type.NetworkType == EntityNetworkTypes.Synchronized)
            {
                //SendEntityPostCreateMessage(EntitySystemWorld.Instance.RemoteEntityWorlds);
            }
        }

        private void _OnCreate()
        {
            OnCreate();
        }

        protected virtual void OnCreate()
        {
        }

        private void EntityCreated()
        {
            if (Create != null)
                Create(this);
        }

        protected internal virtual void OnPreCreate()
        {
        }

        internal void A(bool flag)
        {
            Entity entity;
            Entities.Instance.entitySetByName.TryGetValue(this.Name, out entity);
            if (entity != null)
            {
                if (entity != this)
                {
                    if (flag )
                    {
                        Log.Info("Entity: Error: Another entity has duplicate name \"{0}\".", this.Name);
                        this.name = "";
                    }
                    else
                    {
                        Log.Error("Entity: Another entity has duplicate name \"{0}\".", this.Name);
                    }
                }
                else
                {
                    Entities.Instance.entitySetByName.Remove(this.name);
                }
            }
            if (this.name != "")
            {
                Entities.Instance.entitySetByName.Add(this.Name, this);
                return;
            }
            if (!this.Type.AllowEmptyName)
            {
                this.Name = Entities.Instance.GetUniqueName(this.Type.Name + "_");
            }
        }

        internal void _OnPostCreated(bool loaded)
        {
            if (PostCreated != null)
                PostCreated(this, loaded);
        }

        internal void OnPostCreate_(bool loaded)
        {
            OnPostCreate(loaded);
        }

        internal void OnPostCreate2_(bool loaded)
        {
            OnPostCreate2(loaded);
        }

        protected virtual void OnPostCreate(bool loaded)
        {
            if (editor_excludeEntityFromWorld)
            {
                return;
            }
            if (parent != null && zD.List == null)
            {
                parent.OnAddChild(this);
            }
        }

        protected virtual void OnPostCreate2(bool loaded)
        {
            if (editor_excludeEntityFromWorld)
                return;

            if (logicObject == null && logicClass != null && !EntitySystemWorld.Instance.IsEditor())
                CreateLogicObject();

            if (loaded)
                _OnPostCreated(true);
        }

        internal void executeDestory()
        {
            _OnDestroying();
            OnDestroy();
            _OnDestroyed();
        }

        internal void _OnDestroying()
        {
            if (Destroying != null)
                Destroying(this);
        }

        internal void _OnDestroyed()
        {
            if (Destroyed != null)
                Destroyed(this);
        }

        protected virtual void OnDestroy()
        {
            if (editor_excludeEntityFromWorld)
                return;
            
            if (extendedProperties != null && !Editor_IsExcludedFromWorld())
                DestroyExtendedProperties();

            if (Name != "" && IsPostCreated)
            {
                Entity entity = Entities.Instance.entitySetByName[Name];
                if (entity == this)
                    Entities.Instance.entitySetByName.Remove(Name);
            }

            if (LogicSystemManager.Instance != null && LogicSystemManager.Instance.Parent == this)
            {
                LogicSystemManager logicSystemManager = null;
                foreach (Entity current in Children)
                {
                    if (!current.isDestroyed)
                    {
                        if (logicSystemManager == null && current == LogicSystemManager.Instance)
                        {
                            logicSystemManager = (LogicSystemManager)current;
                        }
                        else
                        {
                            current.executeDestory();
                        }
                    }
                }
                if (logicSystemManager != null)
                    logicSystemManager.executeDestory();

            }
            IL_F5:
            while (subscriptionsToDeletionEvent != null && subscriptionsToDeletionEvent.Count != 0)
            {
                for (int i = 0; i < subscriptionsToDeletionEvent.Count; i++)
                {
                    int count = subscriptionsToDeletionEvent.Count;
                    if (subscriptionsToDeletionEvent[i] != null)
                        subscriptionsToDeletionEvent[i].DeleteSubscribedToDeletion(this);
                    
                    if (count != subscriptionsToDeletionEvent.Count)
                        goto IL_F5;
                }
                while (subscriptionsToDeletionEvent.Count != 0)
                {
                    if (subscriptionsToDeletionEvent[0] == null)
                        subscriptionsToDeletionEvent.RemoveAt(0);
                    else
                        UnsubscribeToDeletionEvent(subscriptionsToDeletionEvent[0]);
                }
                break;
            }
            this.c();
            isDestroyed = true;
        }
        protected virtual void OnAddChild(Entity entity)
        {
            if (entity.zD.List != null)
            {
                Log.Fatal("Entity: OnAddChild: entity.parentLinkedListNode.List != null.");
            }
            if (this.children == null)
            {
                this.children = new LinkedList<Entity>();
            }
            this.children.AddLast(entity.zD);
        }

        protected internal virtual void OnRemoveChild(Entity entity)
        {
            if (entity.Parent != this)
            {
                Log.Fatal("Entity: OnRemoveChild: entity.Parent != this.");
            }
            if (entity.zD.List != null)
            {
                this.children.Remove(entity.zD);
            }
        }

        internal void NotifySetSimulation(bool simulation)
        {
            OnSetSimulation(simulation);
        }

        protected virtual void OnSetSimulation(bool simulation)
        {
        }

        internal readonly object tickingLock = new object(); 
        internal bool InTicking { get; private set; }
        private long lastTick = 0; 
        internal void Ticking()
        {
            lock (tickingLock)
            {
                if (InTicking)
                    return;

                InTicking = true;
                try
                {
                    OnTick();
                    if (Tick != null)
                        Tick(this);
                }
                finally
                {
                    lastTick = DateTime.Now.Ticks;
                    InTicking = false;
                }
            }
        }

        public long LastTick
        {
            get { return lastTick; }
        }
 
        internal void ClientOnTick()
        {
            this.Client_OnTick();
        }

        protected virtual void OnTick()
        {
            if (this.LogicObject != null)
            {
                this.LogicObject.TickWaitItems();
            }
        }

        protected virtual void Client_OnTick()
        {
        }

        [LogicSystemBrowsable(true)]
        public void SubscribeToTickEvent()
        {
            if (subscribeToTickEventCount == 0)
            {
                if (this.zj == null)
                {
                    this.zj = new LinkedListNode<Entity>(this);
                }
                Entities.Instance.entitiesSubscribedToOnTick.AddLast(this.zj);
                Entities.Instance.entitiesSubscribedToOnTickChanged = true;
            }
            subscribeToTickEventCount++;
        }

        [LogicSystemBrowsable(true)]
        public void UnsubscribeToTickEvent()
        {
            if (subscribeToTickEventCount <= 0)
            {
                Log.Error(string.Format("Entity: RemoveTimer: Amount of subscribtions to the timer <= 0. Entity name: \"{0}\".", this.Name));
            }
            subscribeToTickEventCount--;
            if (subscribeToTickEventCount == 0)
            {
                Entities.Instance.entitiesSubscribedToOnTick.Remove(this.zj);
                Entities.Instance.entitiesSubscribedToOnTickChanged = true;
                this.tickRound = 0;
            }
        }

        private void c()
        {
            if (this.subscribeToTickEventCount != 0)
            {
                Entities.Instance.entitiesSubscribedToOnTick.Remove(this.zj);
            }
            this.subscribeToTickEventCount = 0;
        }

        internal void DeleteSubscribedToDeletion(Entity entity)
        {
            OnDeleteSubscribedToDeletionEvent(entity);

            if (DeleteSubscribedToDeletionEvent != null)
                DeleteSubscribedToDeletionEvent(this, entity);
        }

        protected virtual void OnDeleteSubscribedToDeletionEvent(Entity entity)
        {
            if (entity == this.logicClass)
            {
                this.logicClass = null;
            }
            if (this.extendedProperties != null)
            {
                this.extendedProperties.OnDeleteSubscribedToDeletionEvent(entity);
            }
        }

        protected virtual bool OnLoad(TextBlock block)
        {
            if (block.IsAttributeExist("logicClass"))
            {
                this.logicClass = (Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(block.GetAttribute("logicClass"))) as LogicClass);
            }
            if (this.logicObject == null && this.logicClass != null )
            {
                this.CreateLogicObject();
                TextBlock textBlock = block.FindChild("logicObject");
                if (textBlock != null && !this.logicObject.A(textBlock))
                {
                    return false;
                }
            }
            string text = this.Type.Name;
            if (this.name != "")
            {
                text += string.Format(" ({0})", this.name);
            }
            text = string.Format("Entity: \"{0}\"", text);
            for (EntityTypes.ClassInfo classInfo = this.Type.ClassInfo; classInfo != null; classInfo = classInfo.BaseClassInfo)
            {
                foreach (EntityTypes.ClassInfo.EntitySerializableFieldItem current in classInfo.EntitySerializableFields)
                {
                    if (/*EntitySystemWorld.Instance.isEntitySerializable(current.SupportedSerializationTypes) &&*/ !EntityHelper.LoadFieldValue(true, this, current.Field, block, text))
                    {
                        return false;
                    }
                }
            }
            string text2 = null;
            if (block.IsAttributeExist("subscriptionsToDeletionEvent"))
            {
                text2 = block.GetAttribute("subscriptionsToDeletionEvent");
            }
            else if (block.IsAttributeExist("relationships"))
            {
                text2 = block.GetAttribute("relationships");
            }
            else if (block.IsAttributeExist("relations"))
            {
                text2 = block.GetAttribute("relations");
            }
            if (text2 != null)
            {
                string[] array = text2.Split(new char[]
                {
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries);
                this.subscriptionsToDeletionEvent = new List<Entity>(Math.Max(array.Length, 4));
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string s = array2[i];
                    Entity loadingEntityBySerializedUIN = Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(s));
                    if (loadingEntityBySerializedUIN != null)
                    {
                        this.subscriptionsToDeletionEvent.Add(loadingEntityBySerializedUIN);
                    }
                }
            }
            TextBlock textBlock2 = block.FindChild("extendedProperties");
            if (textBlock2 != null)
            {
                string typeClass = textBlock2.GetAttribute("class");
                Type type = EntitySystemWorld.Instance.FindEntityClassType(typeClass); 
                if (type == null)
                {
                    Log.Error("Extended properties class \"{0}\" not exists.", typeClass);
                    return false;
                }
                this.CreateExtendedProperties(type);
                if (!this.extendedProperties.OnLoad(textBlock2))
                {
                    return false;
                }
            }
            if (block.IsAttributeExist("textUserData"))
            {
                string attribute2 = block.GetAttribute("textUserData");
                if (!string.IsNullOrEmpty(attribute2))
                {
                    this.SetTag("TextUserData", attribute2);
                }
            }
            return true;
        }

        internal bool LoadEntityFromBlock(TextBlock block)
        {
            LongOperationNotifier.Notify("Entity: Load: " + this.ToString());
            return this.OnLoad(block);
        }

        protected internal virtual void OnPreLoadChildEntity(EntityType childType, TextBlock childBlock, ref bool needLoad)
        {
            if (EntitySystemWorld.Instance.IsDedicatedServer() && childType.NetworkType == EntityNetworkTypes.ClientOnly)
            {
                needLoad = false;
            }
            if (EntitySystemWorld.Instance.IsClientOnly())
            {
                if (childType.NetworkType == EntityNetworkTypes.ServerOnly)
                {
                    needLoad = false;
                }
                if (childType.NetworkType == EntityNetworkTypes.Synchronized)
                {
                    needLoad = false;
                }
            }
        }

        internal bool Load(TextBlock textBlock)
        {
            LongOperationNotifier.Notify("Entity: LoadingGenerateHierarchy: " + this.ToString());
            foreach (TextBlock current in textBlock.Children)
            {
                if (!(current.Name != "entity"))
                {
                    EntityType entityType = EntityTypes.Instance.GetByName(current.GetAttribute("type"));
                    if (entityType == null)
                    {
                        EntityType entityType2 = null;
                        string typeName = current.GetAttribute("type");
                        string className = current.GetAttribute("classPrompt");
                        Entities.EntityTypeInfo a = Entities.Instance.FindEntityTypeInfo(typeName);
                        if (a == null)
                        {
                            bool flag = false;
                            if (!EntitySystemWorld.Instance.OnLoadNotDefinedEntityType(typeName, className, ref entityType2, ref flag))
                            {
                                if (!EntitySystemWorld.Instance.IsEditor())
                                {
                                    Log.Error("Entity: Load: not defined type \"{0}\".", typeName);
                                }
                                bool result = false;
                                return result;
                            }
                            if (flag)
                            {
                                Entities.Instance.A(typeName, entityType2);
                            }
                        }
                        else
                        {
                            entityType2 = a.newEntityType;
                        }
                        if (entityType2 == null)
                        {
                            continue;
                        }
                        entityType = entityType2;
                    }
                    bool flag2 = true;
                    this.OnPreLoadChildEntity(entityType, current, ref flag2);
                    if (flag2)
                    {
                        uint uin = uint.Parse(current.GetAttribute("uin"));
                        Entity entity = Entities.Instance._CreateInternal(entityType, this, uin, 0u);
                        if (!entity.Load(current))
                        {
                            bool result = false;
                            return result;
                        }
                        entity.loadingTextBlock = current;
                        Entities.Instance.aAF.Add(new Entities.EntityTextBlock(entity, current));
                        this.OnAddChild(entity);
                    }
                }
            }
            return true;
        }

        protected virtual void OnSave(TextBlock block)
        {
            block.SetAttribute("type", Type.Name);
            block.SetAttribute("uin", uin.ToString());

            if (!(this is LogicComponent))
                block.SetAttribute("classPrompt", Type.ClassInfo.entityClassType.Name);

            if (subscriptionsToDeletionEvent != null && subscriptionsToDeletionEvent.Count != 0)
            {
                string _subscriptionsToDeletionEvent = string.Join(" ", subscriptionsToDeletionEvent.Select(_x => _x.UIN)); 
                block.SetAttribute("subscriptionsToDeletionEvent", _subscriptionsToDeletionEvent);
            }

            string text = Type.Name;
            if (this.name != "")
                text += string.Format(" ({0})", this.name);

            text = string.Format("Entity: \"{0}\"", text);

            for (EntityTypes.ClassInfo classInfo = Type.ClassInfo; classInfo != null; classInfo = classInfo.BaseClassInfo)
            {
                foreach (EntityTypes.ClassInfo.EntitySerializableFieldItem current2 in classInfo.EntitySerializableFields)
                {
                    //if (EntitySystemWorld.Instance.IsEntityFieldSerializable(current2.SupportedSerializationTypes))
                    {
                        if (!EntityHelper.SaveFieldValue(true, this, current2.Field, block, text))
                            return;
                    }
                }
            }
            if (logicClass != null)
                block.SetAttribute("logicClass", logicClass.UIN.ToString());
            
            if (extendedProperties != null)
            {
                TextBlock textBlock = block.AddChild("extendedProperties");
                textBlock.SetAttribute("class", this.extendedProperties.GetType().Name);
                extendedProperties.OnSave(textBlock);
            }

            if (logicObject != null)
            {
                TextBlock logicObjectBlock = block.FindChild("logicObject");
                if (logicObjectBlock == null)
                    logicObjectBlock = block.AddChild("logicObject");
                
                logicObject.OnSave(logicObjectBlock);
                return;
            }
        }

        internal void Save(TextBlock textBlock)
        {
            this.OnSave(textBlock);
            foreach (Entity current in this.Children)
            {
                if (current.AllowSave)
                {
                    TextBlock entityBlock = textBlock.AddChild("entity");
                    current.Save(entityBlock);
                }
            }
        }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.name))
            {
                return string.Format("{0} ({1})", this.name, this.Type.Name);
            }
            return this.Type.Name;
        }

        internal bool AllowSaveHerit()
        {
            for (Entity entity = this; entity != null; entity = entity.Parent)
            {
                if (!entity.AllowSave)
                {
                    return false;
                }
            }
            return true;
        }

        private void CopyBrowsablePropertiesFrom(Entity entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo propertyInfo = array[i];
                if (propertyInfo.CanWrite)
                {
                    BrowsableAttribute[] array2 = (BrowsableAttribute[])propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (array2.Length != 0)
                    {
                        bool flag = true;
                        BrowsableAttribute[] array3 = array2;
                        for (int j = 0; j < array3.Length; j++)
                        {
                            BrowsableAttribute browsableAttribute = array3[j];
                            if (!browsableAttribute.Browsable)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            goto IL_96;
                        }
                    }
                    if (!(propertyInfo.Name == "Name"))
                    {
                        object value = propertyInfo.GetValue(entity, null);
                        propertyInfo.SetValue(this, value, null);
                    }
                }
                IL_96:;
            }
        }

        public Entity CloneWithCopyBrowsableProperties(bool cloneChildren, Entity clonedParent)
        {
            // create an entity, set its parent to clonedParent
            Entity entity = Entities.Instance.Create(this.Type, clonedParent);
            if (cloneChildren)
            {
                foreach (Entity current in this.Children)
                {
                    current.CloneWithCopyBrowsableProperties(cloneChildren, entity);
                }
            }
            entity.CopyBrowsablePropertiesFrom(this);
            entity.OnClone(this);
            entity.PostCreate();
            return entity;
        }

        public Entity CloneWithCopyBrowsableProperties(bool cloneChildren)
        {
            return this.CloneWithCopyBrowsableProperties(cloneChildren, this.Parent);
        }

        protected virtual void OnClone(Entity source)
        {
        }

        public void _CallOnClone(Entity source)
        {
            this.OnClone(source);
        }

        private void CreateLogicObject()
        {
            if (EntitySystemWorld.Instance.LogicSystemScriptsAssembly == null)
            {
                Log.Error("Entity: CreateLogicObject: Dynamin link library with compiled code from Logic Editor is not loaded.");
                return;
            }
            if (this.logicClass.loadingTextBlock != null && string.IsNullOrEmpty(this.logicClass.ClassName) && this.logicClass.loadingTextBlock.IsAttributeExist("className"))
            {
                this.logicClass.className = this.logicClass.loadingTextBlock.GetAttribute("className");
            }
            Type logicSystemScriptsAssemblyClassByClassName = EntitySystemWorld.Instance.GetLogicSystemScriptsAssemblyClassByClassName(this.logicClass.ClassName);
            if (logicSystemScriptsAssemblyClassByClassName == null)
            {
                Log.Error("Entity: CreateLogicObject: classType = null. \"{0}\"", this.logicClass.ClassName);
                return;
            }
            ConstructorInfo constructorInfo = null;
            ConstructorInfo[] constructors = logicSystemScriptsAssemblyClassByClassName.GetConstructors();
            for (int i = 0; i < constructors.Length; i++)
            {
                ConstructorInfo constructorInfo2 = constructors[i];
                if (constructorInfo2.GetParameters().Length == 1)
                {
                    constructorInfo = constructorInfo2;
                    break;
                }
            }
            this.logicObject = (LogicEntityObject)constructorInfo.Invoke(new object[]
            {
                this
            });
        }

        private void resetLogicObject()
        {
            this.logicObject = null;
        }

        public virtual string Editor_GetShowInformation()
        {
            return null;
        }

        public void Editor_IncludeToWorld()
        {
            if (!this.editor_excludeEntityFromWorld)
            {
                Log.Fatal("Entity: Editor_IncludeToWorld: The entity is already included into the world.");
            }
            this.Parent.OnAddChild(this);
            if (this.name != "")
            {
                Entities.Instance.entitySetByName[this.name] = this;
            }
            this.OnPostCreate(false);
            this.OnPostCreate2(false);
            this.editor_excludeEntityFromWorld = false;
        }

        public void Editor_ExcludeFromWorld()
        {
            if (this.editor_excludeEntityFromWorld)
            {
                Log.Fatal("Entity: Editor_ExcludeFromWorld: The entity is already excluded into the world.");
            }
            this.editor_excludeEntityFromWorld = true;
            this.Parent.OnRemoveChild(this);
            if (this.name != "")
            {
                Entities.Instance.entitySetByName.Remove(this.name);
            }
            executeDestory();
        }

        public bool Editor_IsExcludedFromWorld()
        {
            return this.editor_excludeEntityFromWorld;
        }

        protected virtual bool OnIsExistsReferenceToObject(object obj)
        {
            return false;
        }

        public bool IsExistsReferenceToObject(object obj)
        {
            if (obj == null)
                return false;

            bool r = OnIsExistsReferenceToObject(obj);
            if (r)
                return true;

            Type type = obj.GetType();

            List<EntityTypes.ClassInfo.EntitySerializableFieldItem> fields = Type.GetClassInfo()
                .SelectMany(_clazz => _clazz.EntitySerializableFields)
                .Where(_f => _f.Field.FieldType.IsAssignableFrom(type))
                .ToList();

            EntityTypes.ClassInfo.EntitySerializableFieldItem anyOne = fields.Where(_f =>
            {
                object _v = _f.Field.GetValue(this);
                return obj.Equals(_v);
            }).FirstOrDefault();  
            return anyOne != null;
        }

        public EntityExtendedProperties CreateExtendedProperties(Type extendedPropertiesClass)
        {
            this.DestroyExtendedProperties();
            ConstructorInfo constructor = extendedPropertiesClass.GetConstructor(new Type[0]);
            this.extendedProperties = (EntityExtendedProperties)constructor.Invoke(null);
            this.extendedProperties.zT = this;
            return this.extendedProperties;
        }

        public void DestroyExtendedProperties()
        {
            if (this.extendedProperties != null)
            {
                this.extendedProperties.OnDestroy();
                this.extendedProperties = null;
            }
        }

        public void Editor_ReplaceExtendedProperties(EntityExtendedProperties extendedProperties)
        {
            this.extendedProperties = extendedProperties;
        } 

        protected virtual void OnPreloadResources()
        {
            this.Type.PreloadResources();
            foreach (Entity current in this.Children)
            {
                current.PreloadResources();
            }
        }
        public void PreloadResources()
        {
            this.OnPreloadResources();
        }
        public void SetTag(string name, string value)
        {
            for (int i = 0; i < this.tagInfos.Count; i++)
            {
                if (this.tagInfos[i].Name == name)
                {
                    this.tagInfos[i].Value = value;
                    return;
                }
            }
            Entity.TagInfo tagInfo = new Entity.TagInfo();
            tagInfo.Name = name;
            tagInfo.Value = value;
            this.tagInfos.Add(tagInfo);
        }
        public void RemoveTag(string name)
        {
            for (int i = this.tagInfos.Count - 1; i >= 0; i--)
            {
                if (this.tagInfos[i].Name == name)
                {
                    this.tagInfos.RemoveAt(i);
                }
            }
        }
        public bool GetTag(string name, out string value)
        {
            for (int i = 0; i < this.tagInfos.Count; i++)
            {
                if (this.tagInfos[i].Name == name)
                {
                    value = this.tagInfos[i].Value;
                    return true;
                }
            }
            value = "";
            return false;
        }
        public string GetTag(string name)
        {
            for (int i = 0; i < this.tagInfos.Count; i++)
            {
                if (this.tagInfos[i].Name == name)
                {
                    return this.tagInfos[i].Value;
                }
            }
            return "";
        }
    }
}
