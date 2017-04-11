using Jx.FileSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Jx.EntitySystem
{
	[LogicSystemBrowsable(true), LogicSystemCallStaticOverInstance]
	public class Entities
	{ 
		internal struct AT
		{
			public Entity entity;
			public TextBlock textBlock;

			public AT(Entity entity, TextBlock textBlock)
			{
				this.entity = entity;
				this.textBlock = textBlock;
			}
		}

		internal class aT
		{
			public string entityTypeName;
			public EntityType newEntityType;
		}

		public delegate void CreateDeleteEntityDelegate(Entity entity);
		private static Entities instance;
		private uint UINCounter = 1u;
		private Dictionary<uint, Entity> entitiesUINDictionary = new Dictionary<uint, Entity>();
		private uint entityNetworkUIN = 1u;
		private Dictionary<uint, Entity> entitySetByUIN = new Dictionary<uint, Entity>();
		internal Dictionary<string, Entity> entitySetByName = new Dictionary<string, Entity>();
		private uint UINOffset;
		private uint aAC;
		private float tickTime;
		internal LinkedList<Entity> entitiesSubscribedToOnTick = new LinkedList<Entity>();
		internal bool aAd;
		private int aAE;
		private OrderedDictionary aAe = new OrderedDictionary(32);
		internal List<Entities.AT> aAF;
		private List<Entities.aT> aAf;
		//private static RemoteEntityWorld[] remoteEntityWorlds = new RemoteEntityWorld[1]; 

        public event Entities.CreateDeleteEntityDelegate CreateEntity; 
        public event Entities.CreateDeleteEntityDelegate DeleteEntity;

		public static Entities Instance
		{
			get
			{
				return Entities.instance;
			}
		}

		public float TickTime
		{
			get
			{
				return this.tickTime;
			}
		}

		public ICollection<Entity> EntitiesCollection
		{
			get
			{
				return this.entitiesUINDictionary.Values;
			}
		}
		[LogicSystemBrowsable(true)]
		public static float TickDelta
		{
			get
			{
				return Entity.TickDelta;
			}
		}

		internal static void Init()
		{
			Trace.Assert(instance == null);
			instance = new Entities();
			instance._Init();
		}

		internal static void Shutdown()
		{
			if (instance != null)
			{
				instance._Shutdown();
				instance = null;
			}
		}

		private void _Init()
		{
			this.tickTime = EngineApp.Instance.Time;
		}

		private void _Shutdown()
		{
		}

		public Entity _CreateInternal(EntityType type, Entity parent, uint uin, uint networkUIN)
		{
			if (type == null)
			{
				Log.Fatal("Entities: _CreateInternal: type == null.");
			}
			EntityTypes.ClassInfo classInfo = type.ClassInfo;
			if (classInfo == null)
			{
				Log.Fatal("Entities: _CreateInternal: classInfo == null.");
			}
			Type entityClassType = classInfo.EntityClassType;
			ConstructorInfo constructor = entityClassType.GetConstructor(new Type[0]);
			Entity entity = (Entity)constructor.Invoke(new object[]{});
			entity.parent = parent;
			entity.createTime = this.TickTime;
			for (EntityTypes.ClassInfo classInfo2 = classInfo; classInfo2 != null; classInfo2 = classInfo2.baseClassInfo)
			{
				if (classInfo2.fieldInfo != null)
				{
					classInfo2.fieldInfo.SetValue(entity, type);
				}
			}
			this.AddToSystem(entity, uin, networkUIN);
			entity.OnPreCreate();
            if (CreateEntity != null)
			{
                CreateEntity(entity);
			}
			if (EntitySystemWorld.Instance.IsServer() && entity.Type.NetworkType == EntityNetworkTypes.Synchronized)
			{
				//SynchronizeEntityCreation(entity, null);
			}
			return entity;
		}
 
		[LogicSystemBrowsable(true)]
		public Entity Create(EntityType type, Entity parent)
		{
			if (type == null)
			{
				Log.Fatal("Entities: Create: type == null.");
			}
			if (EntitySystemWorld.Instance.IsDedicatedServer() && type.NetworkType == EntityNetworkTypes.ClientOnly)
			{
				Log.Fatal("Entities: Create: You cannot to create NetworkType.ClientOnly entity on the dedicated server. Entity type name: {0}.", type.Name);
			}
			if (EntitySystemWorld.Instance.IsClientOnly())
			{
				if (type.NetworkType == EntityNetworkTypes.ServerOnly)
				{
					Log.Fatal("Entities: Create: You cannot to create NetworkType.ServerOnly entity on the client. Entity type name: {0}.", type.Name);
				}
				if (type.NetworkType == EntityNetworkTypes.Synchronized)
				{
					Log.Fatal("Entities: Create: You cannot to create NetworkType.ClientServerSynchronized entity on the client. Entity type name: {0}.", type.Name);
				}
			}
			return this._CreateInternal(type, parent, 0u, 0u);
		}

		[LogicSystemBrowsable(true)]
		public Entity Create(string typeName, Entity parent)
		{
			EntityType byName = EntityTypes.Instance.GetByName(typeName);
			if (byName == null)
			{
				Log.Error("Entity Type not exists \"{0}\".", typeName);
				return null;
			}
			return this.Create(byName, parent);
		}

		public void CompleteEntityDelete(Entity entity)
		{
			while (true)
			{
				IL_00:
				foreach (Entity current in entity.Children)
				{
					if (LogicSystemManager.Instance != current || entity.Children.Count <= 1)
					{
						this.CompleteEntityDelete(current);
						goto IL_00;
					}
				}
				break;
			}
			if (!entity.isDestroyed) 
                entity.executeDestory(); 

			if (entity.parent != null)
			{
				entity.parent.OnRemoveChild(entity);
				entity.parent = null;
			}
			this.a(entity);
		}

		public void CompressUINs()
		{
			List<Entity> list = new List<Entity>(this.entitiesUINDictionary.Values.Count);
			foreach (Entity current in this.entitiesUINDictionary.Values)
			{
				list.Add(current);
				current.uin = (uint)list.Count;
			}
			this.entitiesUINDictionary.Clear();
			foreach (Entity current2 in list)
			{
				this.entitiesUINDictionary.Add(current2.uin, current2);
			}
			this.UINCounter = (uint)(list.Count + 1);
		}

		public void DeleteEntitiesQueuedForDeletion()
		{
			while (this.aAe.Count != 0)
			{
				IEnumerator enumerator = this.aAe.Keys.GetEnumerator();
				enumerator.MoveNext();
				Entity entity = (Entity)enumerator.Current;
				if (!entity.isDestroyed)
                    entity.executeDestory(); 

				if (entity.parent != null)
				{
					entity.parent.OnRemoveChild(entity);
					entity.parent = null;
				}
				this.a(entity);
			}
		}

		internal void OnSetSimulation(bool simulation)
		{
			foreach (KeyValuePair<uint, Entity> current in this.entitiesUINDictionary)
			{
				current.Value.OnSetSimulation(simulation);
			}
		}

		internal virtual void A(float num, bool flag)
		{
			this.tickTime = num;
			this.aAE++;
			if (this.aAE >= 2147483647)
			{
				this.aAE = 1;
				foreach (Entity current in this.entitiesSubscribedToOnTick)
				{
					current.zk = 0;
				}
			}
			while (true)
			{
				IL_5F:
				this.aAd = false;
				foreach (Entity current2 in this.entitiesSubscribedToOnTick)
				{
					if (!current2.IsSetForDeletion && current2.CreateTime != this.tickTime && this.aAE != current2.zk)
					{
						current2.zk = this.aAE;
						if (!flag)
						{
							current2.Ticking();
						}
						else
						{
							current2.ClientOnTick();
						}
						if (this.aAd)
						{
							goto IL_5F;
						}
					}
				}
				break;
			}
			this.DeleteEntitiesQueuedForDeletion();
		}

		public bool Internal_LoadEntityTreeFromTextBlock(Entity entity, TextBlock block, bool loadRootEntity, List<Entity> loadedEntities)
		{
			Trace.Assert(this.aAF == null);
			this.aAF = new List<Entities.AT>(1024);
			Trace.Assert(this.aAf == null);
			this.aAf = new List<Entities.aT>();
			if (!entity.Load(block))
			{
				this.aAF = null;
				this.aAf = null;
				return false;
			}
			if (loadRootEntity)
			{
				entity.loadingTextBlock = block;
				this.aAF.Add(new Entities.AT(entity, block));
			}
			this.aAf = null;
			foreach (Entities.AT current in this.aAF)
			{
				if (!current.entity.A(current.textBlock))
				{
					this.aAF = null;
					return false;
				}
				if (loadedEntities != null)
				{
					loadedEntities.Add(current.entity);
				}
			}
			this.Internal_ResetUINOffset();
			this.PostCreateInitLoadedEntities();
			this.aAF = null;
			return true;
		}

		public void WriteEntityTreeToTextBlock(Entity entity, TextBlock block)
		{
			entity.Save(block);
		}

		[LogicSystemBrowsable(true)]
		public Entity GetByName(string name)
		{
			Entity result;
			entitySetByName.TryGetValue(name, out result);
			return result;
		}

		[LogicSystemBrowsable(true)]
		public string GetUniqueName(string prefix)
		{
			int num = 0;
			string text;
			while (true)
			{
				text = prefix + num.ToString();
				if (this.GetByName(text) == null)
				{
					break;
				}
				num++;
			}
			return text;
		}

		public Entity GetByUIN(uint uin)
		{
			Entity result;
			this.entitiesUINDictionary.TryGetValue(uin, out result);
			return result;
		}

		public Entity GetLoadingEntityBySerializedUIN(uint uin)
		{
			return this.GetByUIN(uin + this.Internal_GetUINOffset());
		}

		public Entity GetByNetworkUIN(uint networkUIN)
		{
			Entity result;
			this.entitySetByUIN.TryGetValue(networkUIN, out result);
			return result;
		}

		public uint Internal_GetUINCounter()
		{
			return this.UINCounter;
		}

		public void Internal_SetUINOffset(uint offset)
		{
			this.UINOffset = offset;
			this.aAC = this.UINOffset;
		}

		public void Internal_ResetUINOffset()
		{
			this.UINOffset = 0u;
			this.UINCounter = this.aAC + 1u;
		}

		public uint Internal_GetUINOffset()
		{
			return this.UINOffset;
		}

		public void Internal_InitUINOffset()
		{
			this.Internal_SetUINOffset(this.Internal_GetUINCounter() + 1u);
		}

		private void AddToSystem(Entity entity, uint uin, uint networkUIN)
		{
			if (uin == 0u)
			{
				if (this.UINOffset != 0u)
				{
					Log.Fatal("Entities: AddToSystem: Internal error: uin == 0 && uinOffset != 0.");
				}
				entity.uin = this.UINCounter++;
			}
			else
			{
				entity.uin = uin + this.UINOffset;
				if (this.UINOffset != 0u)
				{
					if (uin + this.UINOffset > this.aAC)
					{
						this.aAC = uin + this.UINOffset;
					}
				}
				else if (uin + this.UINOffset >= this.UINCounter)
				{
					this.UINCounter = uin + this.UINOffset + 1u;
				}
			}
			if (entitiesUINDictionary.ContainsKey(entity.UIN))
			{
				Log.Fatal("Entities: AddToSystem: Internal error: entitiesUINDictionary.ContainsKey( entity.UIN ).");
			}
			entitiesUINDictionary.Add(entity.UIN, entity);
			if (EntitySystemWorld.Instance.IsClientOnly() && entity.Type.NetworkType == EntityNetworkTypes.Synchronized && networkUIN == 0u)
			{
				Log.Fatal("Entities: AddToSystem: EntitySystemWorld.Instance.IsClientOnly() && entity.Type.NetworkType == EntityNetworkTypes.ClientServerSynchronized && networkUIN == 0.");
			}
			entity.networkUIN = networkUIN;
			if (networkUIN == 0u && entity.Type.NetworkType == EntityNetworkTypes.Synchronized)
			{
				entity.networkUIN = this.entityNetworkUIN++;
			}
			if (entity.networkUIN != 0u)
			{
				if (entity.Type.NetworkType != EntityNetworkTypes.Synchronized)
				{
					Log.Fatal("Entities: AddToSystem: Internal error: entity.networkUIN != 0 && entity.Type.NetworkType != EntityNetworkTypes.ClientServerSynchronized.");
				}
				if (this.entitySetByUIN.ContainsKey(entity.networkUIN))
				{
					Log.Fatal("Entities: AddToSystem: Internal error: entity.networkUIN != 0 && entitiesNetworkUINDictionary.ContainsKey( entity.networkUIN )");
				}
				this.entitySetByUIN.Add(entity.networkUIN, entity);
				if (entity is World && entity.NetworkUIN != 1u)
				{
					Log.Fatal("Entities: AddToSystem: Internal error: entity is World && entity.NetworkUIN != 1.");
				}
			}
			if (this.UINCounter >= 4294967295u)
			{
				this.CompressUINs();
			}
		}

		private void A(Entity entity)
		{
			this.entitiesUINDictionary.Remove(entity.UIN);
			if (entity.NetworkUIN != 0u)
			{
				this.entitySetByUIN.Remove(entity.NetworkUIN);
			}
		}

		private void a(Entity entity)
		{
			this.aAe.Remove(entity);
			foreach (Entity current in entity.Children)
			{
				if (!current.isDestroyed) 
                    current.executeDestory(); 
			}

			if (!entity.isDestroyed) 
                entity.executeDestory(); 
			
			this.A(entity);
            if (DeleteEntity != null)
                DeleteEntity(entity);

			if (entity.parent != null)
			{
				entity.parent.OnRemoveChild(entity);
				entity.parent = null;
			}
		}

		internal void B(Entity key)
		{
			if (!this.aAe.Contains(key))
			{
				this.aAe.Add(key, null);
			}
		}

		private void PostCreateInitLoadedEntities()
		{
			LongOperationCallbackManager.CallCallback("Entities: PostCreateInitLoadedEntities");
			foreach (Entities.AT current in this.aAF)
			{
				if (current.entity.LogicObject != null)
				{
					current.entity.LogicObject.C();
				}
				current.entity.A(true);
			}
			foreach (Entities.AT current2 in this.aAF)
			{
				LongOperationCallbackManager.CallCallback("Entities: PostCreateInitLoadedEntities: OnPostCreate: " + current2.entity.ToString());
				if (current2.entity.isPostCreated)
				{
					Log.Fatal("Entities: PostCreateInitLoadedEntities : entity.postCreated.");
				}
				current2.entity.OnPostCreate(true);
			}
			foreach (Entities.AT current3 in this.aAF)
			{
				LongOperationCallbackManager.CallCallback("Entities: PostCreateInitLoadedEntities: OnPostCreate2: " + current3.entity.ToString());
				current3.entity.OnPostCreate2(true);
				current3.entity.loadingTextBlock = null;
				current3.entity.isPostCreated = true;
				if (EntitySystemWorld.Instance.IsServer() && current3.entity.Type.NetworkType == EntityNetworkTypes.Synchronized)
				{
					//current3.entity.SendEntityPostCreateMessage(EntitySystemWorld.Instance.RemoteEntityWorlds);
				}
			}
		}

		internal void A(string text, EntityType newEntityType)
		{
			Trace.Assert(this.A(text) == null);
			Entities.aT a = new Entities.aT();
			a.entityTypeName = text;
			a.newEntityType = newEntityType;
			this.aAf.Add(a);
		}

		internal Entities.aT A(string b)
		{
			foreach (Entities.aT current in this.aAf)
			{
				if (current.entityTypeName == b)
				{
					return current;
				}
			}
			return null;
		}

		public bool LoadEntitesFromSceneFileBlock(TextBlock block, Entity parent, out List<Entity> loadedEntities)
		{
			loadedEntities = new List<Entity>();
			this.Internal_InitUINOffset();
			return this.Internal_LoadEntityTreeFromTextBlock(parent, block, false, loadedEntities);
		}
		public void WriteEntitiesToSceneFileBlock(TextBlock block, IList<Entity> entities)
		{
			foreach (Entity current in entities)
			{
				TextBlock block2 = block.AddChild("entity");
				this.WriteEntityTreeToTextBlock(current, block2);
			}
		}
		public int GetAmountOfEntitiesSubscribedToOnTick()
		{
			return entitiesSubscribedToOnTick.Count;
		}
	}
}
