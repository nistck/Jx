using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

using Jx.FileSystem;
using Jx.FileSystem.Internals;

using Jx.EntitySystem.LogicSystem;

namespace Jx.EntitySystem
{
	public class EntitySystemWorld
	{
 
		private static EntitySystemWorld instance;
 
		private List<Assembly> entityClassAssemblies;
 
		private List<string> logicSystemSystemClassesAssemblies = new List<string>();

        private WorldSimulationTypes worldSimulationType;
        private bool isDedicatedServer;
		private bool isServer;
		private bool isClientOnly;
		private bool isSingle;
		private bool isEditor;
 
		private float engineTime;
		private bool simulation;
		private bool systemPauseOfSimulation;
 
		private Assembly logicSystemScriptsAssembly;
		private Dictionary<string, Type> logicSystemScriptsAssemblyClassNameDic;
 
		private int networkTickCounter;
		private float clientTickTime;
		private float clientTimeWhenTickTimeWasUpdated;

        private WorldType defaultWorldType;

		public static EntitySystemWorld Instance
		{
			get
			{
				return instance;
			}
		}

        public WorldType DefaultWorldType
        {
            get { return defaultWorldType; }
        }

        public WorldSimulationTypes WorldSimulationType
        {
            get
            {
                return worldSimulationType;
            }
        }

        public ReadOnlyCollection<Assembly> EntityClassAssemblies
		{
			get
			{
				return entityClassAssemblies.AsReadOnly();
			}
		}

		public Assembly LogicSystemScriptsAssembly
		{
			get
			{
				return logicSystemScriptsAssembly;
			}
		}

		public List<string> LogicSystemSystemClassesAssemblies
		{
			get
			{
				return logicSystemSystemClassesAssemblies;
			}
			set
			{
				logicSystemSystemClassesAssemblies = value;
			}
		}

		public static bool Init(EntitySystemWorld overridedObject)
		{
            if (instance != null)
                return false;
            if (overridedObject == null)
                return false; 

			instance = overridedObject;
			bool flag = instance._Startup();
			if (!flag)
			{
				Shutdown();
			}
			return flag;
		}

		public static void Shutdown()
		{
			if (instance != null)
			{
				instance._Shutdown();
				instance = null;
			}
		}

        private void CreateEntityClassAssembly(Assembly assembly)
        {
            if (assembly == null)
                return; 
            if( entityClassAssemblies == null )
                entityClassAssemblies = new List<Assembly>();

            if (entityClassAssemblies.Contains(assembly))
                return;
            entityClassAssemblies.Add(assembly);
        }

		private bool _Startup()
		{			
			TextBlock textBlock = null;
			if (VirtualFile.Exists("Base/Constants/EntitySystem.config"))
			{
				textBlock = TextBlockUtils.LoadFromVirtualFile("Base/Constants/EntitySystem.config");
				if (textBlock == null)
				{
					return false;
				}
			}

            CreateEntityClassAssembly(typeof(EntitySystemWorld).Assembly);
            CreateEntityClassAssembly(Assembly.GetExecutingAssembly());

            /*
			Assembly item = AssemblyUtils.LoadAssemblyByRealFileName("MapSystem.dll", false);
			entityClassAssemblies.Add(item);
            //*/

			EngineComponentManager.ComponentInfo[] componentsByType = EngineComponentManager.Instance.GetComponentsByType(EngineComponentManager.ComponentTypeFlags.EntityClasses, true);
			for (int i = 0; i < componentsByType.Length; i++)
			{
				EngineComponentManager.ComponentInfo componentInfo = componentsByType[i];
				EngineComponentManager.ComponentInfo.PathInfo[] allEntryPointsForThisPlatform = componentInfo.GetAllEntryPointsForThisPlatform();
				for (int j = 0; j < allEntryPointsForThisPlatform.Length; j++)
				{
					EngineComponentManager.ComponentInfo.PathInfo pathInfo = allEntryPointsForThisPlatform[j];
					Assembly assembly = AssemblyUtils.LoadAssemblyByRealFileName(pathInfo.Path, false);
					if (assembly != null && !entityClassAssemblies.Contains(assembly))
					{
						entityClassAssemblies.Add(assembly);
					}
				}
			}
			if (textBlock != null)
			{
				TextBlock textBlock2 = textBlock.FindChild("logicSystem");
				if (textBlock2 != null)
				{
					TextBlock textBlock3 = textBlock2.FindChild("systemClassesAssemblies");
					if (textBlock3 != null)
					{
						foreach (TextBlock current in textBlock3.Children)
						{
							string attribute = current.GetAttribute("file");
							this.logicSystemSystemClassesAssemblies.Add(attribute);
						}
					}
				}
			}
			LogicSystemClasses.Init();
			if (!EntityTypes.Init()) 
				return false;
			
			if (textBlock != null)
			{
				string defaultWorldType = textBlock.GetAttribute("defaultWorldType");
				if (string.IsNullOrEmpty(defaultWorldType))
				{
					Log.Fatal("EntitySystemWorld: Init: Default world type is not defined. (Base\\Constants\\EntitySystem.config: \"defaultWorldType\" attribute)");
                    return false;
				}
				this.defaultWorldType = (EntityTypes.Instance.GetByName(defaultWorldType) as WorldType);
				if (this.defaultWorldType == null)
				{
					Log.Fatal("EntitySystemWorld: Init: World type \"{0}\" is not defined or it is not a WorldType (Base\\Constants\\EntitySystem.config: \"defaultWorldType\" attribute).", defaultWorldType);
                    return false;
				}
			}

            Log.Info(">> Ĭ��WorldType: {0}", defaultWorldType);
			return true;
		}

		private void _Shutdown()
		{
			WorldDestroy();
			EntityTypes.Shutdown();
			LogicSystemClasses.Shutdown(); 
		}

		protected internal virtual bool OnLoadNotDefinedEntityType(string entityTypeName, string entityClassName, ref EntityType changedType, ref bool changeAllSameTypes)
		{
			string text = string.Format("Entity Type not defined \"{0}\", Class: {1}", entityTypeName, entityClassName);
			Log.Error(text);
			return false;
		}

		private bool WorldCreateWithoutPostCreate(WorldSimulationTypes worldSimulationType, WorldType worldType)
		{
			WorldDestroy();
            if (worldType == null)
                return false; 

			Entities.Init();
			uint networkUIN = 0u;
			Entities.Instance._CreateInternal(worldType, null, 0u, networkUIN);
			return true;
		}

		public bool WorldCreate(WorldSimulationTypes worldSimulationType, WorldType worldType)
		{
            if (worldType == null)
            {
                Log.Fatal("EntitySystemWorld: WorldCreate: worldType == null.");
                return false;
            }
			
			if (!WorldCreateWithoutPostCreate(worldSimulationType, worldType))
			{
				return false;
			}
			World.Instance.PostCreate();
			return true;
		}

		public void WorldDestroy()
		{
			if (World.Instance == null)
			{
				return;
			}
			World.Instance.DeleteEntitiesQueuedForDeletion();
			Entities.Instance.CompleteEntityDelete(World.Instance);
			Entities.Shutdown();
			this.networkTickCounter = 0;
			this.clientTickTime = 0f;
			this.clientTimeWhenTickTimeWasUpdated = 0f;
		}
 
		private void Simulate()
		{
			//EntitySystemWorld.entitySystemTimeCounter.Start();
			Entities.Instance.A(this.engineTime, /*clientOnly*/true);
			//EntitySystemWorld.entitySystemTimeCounter.End();
		}

		public void WorldTick(int tick)
		{
			this.networkTickCounter = tick;
            float time = EngineApp.Instance.Time;
			float timeElipsed = (float)this.networkTickCounter * Entity.TickDelta;
			this.clientTimeWhenTickTimeWasUpdated = time;
			this.clientTickTime += Entity.TickDelta;
			if (this.clientTickTime > timeElipsed + 0.5f)
			{
				this.clientTickTime = timeElipsed + 0.5f;
			}
			if (this.clientTickTime > timeElipsed + Entity.TickDelta * 2f)
			{
				float delta = (this.clientTickTime - timeElipsed) * 0.05f;
				this.clientTickTime -= delta;
			}
			if (this.clientTickTime < timeElipsed)
			{
				this.clientTickTime = timeElipsed;
			}
			this.engineTime = time;
			Simulate();
		}

		public void Tick()
		{
			if (Entities.Instance == null)
			{
				return;
			} 
            float time = EngineApp.Instance.Time;
			if (!this.simulation || this.systemPauseOfSimulation)
			{
				this.engineTime = time;
				return;
			}
			while (time > this.engineTime + Entity.TickDelta)
			{
				this.engineTime += Entity.TickDelta;
				Simulate();
			}
		}

		internal bool isEntitySerializable(Entity.FieldSerializeSerializationTypes fieldSerializeSerializationTypes)
		{
            return true;
            /*
			switch (this.SerializationMode)
			{
			case SerializationModes.Map:
			case SerializationModes.MapSceneFile:
				return (fieldSerializeSerializationTypes & Entity.FieldSerializeSerializationTypes.Map) != (Entity.FieldSerializeSerializationTypes)0;
			case SerializationModes.World:
				return (fieldSerializeSerializationTypes & Entity.FieldSerializeSerializationTypes.World) != (Entity.FieldSerializeSerializationTypes)0;
			default:
				return false;
			}
            //*/
		} 

		public void ResetExecutedTime()
		{
            this.engineTime = EngineApp.Instance.Time;
			//RendererWorld.Instance._ResetFrameRenderTimeAndRenderTimeStep();
		}

		public void Internal_SetLogicSystemScriptsAssembly(Assembly assembly)
		{
			this.logicSystemScriptsAssembly = assembly;
			this.logicSystemScriptsAssemblyClassNameDic = null;
			if (this.logicSystemScriptsAssembly != null)
			{
				logicSystemScriptsAssemblyClassNameDic = new Dictionary<string, Type>();
				Type[] types = this.logicSystemScriptsAssembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (!type.Name.Contains("<>c__DisplayClass"))
					{
						logicSystemScriptsAssemblyClassNameDic.Add(type.Name, type);
					}
				}
			}
		}

		public Type GetLogicSystemScriptsAssemblyClassByClassName(string className)
		{
			Type result;
			if (logicSystemScriptsAssemblyClassNameDic != null && logicSystemScriptsAssemblyClassNameDic.TryGetValue(className, out result))
			{
				return result;
			}
			return null;
		}

		public bool IsDedicatedServer()
		{
			return isDedicatedServer;
		}

		public bool IsServer()
		{
			return isServer;
		}

		public bool IsClientOnly()
		{
			return isClientOnly;
		}

		public bool IsSingle()
		{
			return isSingle;
		}

		public bool IsEditor()
		{
			return isEditor;
		}
         
	}
}