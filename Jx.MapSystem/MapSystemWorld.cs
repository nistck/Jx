using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Jx.FileSystem;
using Jx.EntitySystem;

namespace Jx.MapSystem
{
    public static class MapSystemWorld
    {
        public sealed class MapObjectAttachedObjectClassItem
        {
            private string ahQ;
            private Type ahq;
            private Type ahR;
            internal ConstructorInfo ahr;
            private Type ahS;
            internal ConstructorInfo ahs;
            private string ahT;
            public string TextBlockName
            {
                get
                {
                    return this.ahQ;
                }
            }
            public Type TypeClassType
            {
                get
                {
                    return this.ahq;
                }
            }
            public Type PreLoadingEntityClassType
            {
                get
                {
                    return this.ahR;
                }
            }
            public Type EntityClassType
            {
                get
                {
                    return this.ahS;
                }
            }
            public string DisplayName
            {
                get
                {
                    return this.ahT;
                }
            }
            internal MapObjectAttachedObjectClassItem(string text, Type type, Type type2, Type type3, string text2)
            {
                this.ahQ = text;
                this.ahq = type;
                this.ahR = type2;
                this.ahS = type3;
                this.ahT = text2;
                if (type2 != null)
                {
                    this.ahr = type2.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                    Trace.Assert(this.ahr != null);
                }
                this.ahs = type3.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
                {
                    typeof(bool)
                }, null);
                Trace.Assert(this.ahs != null);
            }
        }
        public sealed class MapObjectCreateObjectClassItem
        {
            private string aht;
            private Type ahU;
            public string TextBlockName
            {
                get
                {
                    return this.aht;
                }
            }
            public Type TypeClassType
            {
                get
                {
                    return this.ahU;
                }
            }
            internal MapObjectCreateObjectClassItem(string text, Type type)
            {
                this.aht = text;
                this.ahU = type;
            }
        }
        private static List<MapObjectAttachedObjectClassItem> mapObjectAttachedObjectClasses;
        private static List<MapObjectCreateObjectClassItem> mapObjectCreateObjectClasses;
        private static Dictionary<string, Assembly> loadedAssemblyDic;
        public static List<MapObjectAttachedObjectClassItem> MapObjectAttachedObjectClasses
        {
            get
            {
                return mapObjectAttachedObjectClasses;
            }
        }
        public static List<MapObjectCreateObjectClassItem> MapObjectCreateObjectClasses
        {
            get
            {
                return MapSystemWorld.mapObjectCreateObjectClasses;
            }
        }

        /// <summary>
        /// 创建<paramref name="mapType"/>类型的地图 (需要先初始化 World)
        /// </summary>
        /// <param name="mapType"></param>
        /// <returns></returns>
        public static bool MapCreate(MapType mapType)
        {
            if (World.Instance == null)
            {
                Log.Fatal("MapSystemWorld: MapCreate: World.Instance == null.");
                return false;
            }
            MapDestroy();
            Map map = Entities.Instance.Create(mapType, World.Instance) as Map;
            map.PostCreate();
            return true;
        }

        /// <summary>
        /// 销毁地图
        /// </summary>
        public static void MapDestroy()
        {
            if (Map.Instance == null)
                return;

            Entities.Instance.CompleteEntityDelete(Map.Instance);
            Entities.Instance.DeleteEntitiesQueuedForDeletion();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        internal static Assembly TryLoadAssembly(string key)
        {
            Assembly result;
            if (!loadedAssemblyDic.TryGetValue(key, out result))
            {
                return null;
            }
            return result;
        }

        internal static Assembly LoadAssembly(string p)
        {
            Assembly assembly = TryLoadAssembly(p);
            if (assembly != null)
                return assembly;
            
            try
            {
                byte[] array;
                using (Stream stream = VirtualFile.Open(p))
                {
                    array = new byte[stream.Length];
                    if (stream.Read(array, 0, array.Length) != array.Length)
                    {
                        throw new Exception();
                    }
                }
                assembly = AppDomain.CurrentDomain.Load(array);
                loadedAssemblyDic[p] = assembly;
            }
            catch
            {
                Log.Fatal("Load assembly failed \"{0}\".", p);
                return null;
            }
            return assembly;
        } 

        static MapSystemWorld()
        {
            mapObjectAttachedObjectClasses = new List<MapObjectAttachedObjectClassItem>();
            mapObjectCreateObjectClasses = new List<MapObjectCreateObjectClassItem>();
            loadedAssemblyDic = new Dictionary<string, Assembly>();
            /*
            MapSystemWorld.A("mesh", typeof(MapObjectTypeAttachedMesh), typeof(CS), typeof(MapObjectAttachedMesh), "3D Model");
            MapSystemWorld.A("mapObject", typeof(MapObjectTypeAttachedMapObject), null, typeof(MapObjectAttachedMapObject), "Object Type");
            MapSystemWorld.A("particle", typeof(MapObjectTypeAttachedParticle), null, typeof(MapObjectAttachedParticle), "Particle System");
            MapSystemWorld.A("light", typeof(MapObjectTypeAttachedLight), null, typeof(MapObjectAttachedLight), "Light Source");
            MapSystemWorld.A("sound", typeof(MapObjectTypeAttachedSound), null, typeof(MapObjectAttachedSound), "Sound Source");
            MapSystemWorld.A("billboard", typeof(MapObjectTypeAttachedBillboard), null, typeof(MapObjectAttachedBillboard), "Billboard");
            MapSystemWorld.A("ribbonTrail", typeof(MapObjectTypeAttachedRibbonTrail), null, typeof(MapObjectAttachedRibbonTrail), "Ribbon Trail");
            MapSystemWorld.A("gui", typeof(MapObjectTypeAttachedGui), null, typeof(MapObjectAttachedGui), "In-Game 3D GUI");
            MapSystemWorld.A("helper", typeof(MapObjectTypeAttachedHelper), null, typeof(MapObjectAttachedHelper), "Helper");
            MapSystemWorld.A("mapObject", typeof(MapObjectCreateMapObject));
            MapSystemWorld.A("particle", typeof(MapObjectCreateParticle));
            MapSystemWorld.A("light", typeof(MapObjectCreateLight));
            MapSystemWorld.A("billboard", typeof(MapObjectCreateBillboard));
            MapSystemWorld.A("sound", typeof(MapObjectCreateSound));
            MapSystemWorld.A("mesh", typeof(MapObjectCreateMesh));
            //*/
        }

        internal static void A(string text, Type type, Type type2, Type type3, string text2)
        {
            Trace.Assert(MapSystemWorld.B(text) == null, "GetMapObjectAttachedObjectClassByTextBlockName( textBlockName ) == null");
            MapSystemWorld.MapObjectAttachedObjectClassItem item = new MapSystemWorld.MapObjectAttachedObjectClassItem(text, type, type2, type3, text2);
            MapSystemWorld.mapObjectAttachedObjectClasses.Add(item);
        }

        internal static MapSystemWorld.MapObjectAttachedObjectClassItem B(string b)
        {
            foreach (MapSystemWorld.MapObjectAttachedObjectClassItem current in MapSystemWorld.mapObjectAttachedObjectClasses)
            {
                if (string.Equals(current.TextBlockName, b, StringComparison.OrdinalIgnoreCase))
                {
                    return current;
                }
            }
            return null;
        }

        internal static void A(string text, Type type)
        {
            Trace.Assert(GetMapObjectCreateObjectClassByTextBlockName(text) == null, "GetMapObjectCreateObjectClassByTextBlockName( textBlockName ) == null");
            MapObjectCreateObjectClassItem item = new MapObjectCreateObjectClassItem(text, type);
            mapObjectCreateObjectClasses.Add(item);
        }

        internal static MapObjectCreateObjectClassItem GetMapObjectCreateObjectClassByTextBlockName(string b)
        {
            foreach (MapObjectCreateObjectClassItem current in mapObjectCreateObjectClasses)
            {
                if (string.Equals(current.TextBlockName, b, StringComparison.OrdinalIgnoreCase))
                {
                    return current;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a map from file.
        /// </summary>
        /// <param name="virtualFileName">The file name of virtual file system.</param>
        /// <returns><b>true</b> if the map has been loaded; otherwise, <b>false</b>.</returns>
        public static bool MapLoad(string virtualFileName)
        {
            virtualFileName = PathUtils.NormalizeSlashes(virtualFileName);
            LongOperationNotifier.Notify("加载地图: {0}", virtualFileName);

            if (World.Instance == null)
            {
                Log.Fatal("MapSystemWorld: MapLoad: World.Instance == null.");
                return false;
            }

            MapDestroy();
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(virtualFileName);
            if (textBlock == null)
                return false;

            string mapTypeName = textBlock.GetAttribute("type");
            MapType mapType = EntityTypes.Instance.GetByName(mapTypeName) as MapType;
            if (mapType == null)
            {
                Log.Error("地图加载错误: 地图类型 \"{0}\" 未定义。", mapTypeName);
                return false;
            }

            Entities.Instance.Internal_InitUINOffset();
            uint uin = uint.Parse(textBlock.GetAttribute("uin"));
            Map map = (Map)Entities.Instance._CreateInternal(mapType, World.Instance, uin, 0u);
            map.virtualFileName = virtualFileName;
            if (!Entities.Instance.Internal_LoadEntityTreeFromTextBlock(map, textBlock, true, null))
            {
                MapDestroy();
                return false;
            }
            //TODO
            /*
            if (EntitySystemWorld.Instance.WorldSimulationType != WorldSimulationTypes.Editor)
            {
                map.GetDataForEditor().ClearAll();
            }
            //*/

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            EntitySystemWorld.Instance.ResetExecutedTime();
            return true;
        }

        internal static bool C(string p)
        {
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(p);
            if (textBlock == null)
            {
                return false;
            }
            Entities.Instance.Internal_InitUINOffset();
            Map.Instance.virtualFileName = p;
            if (!Entities.Instance.Internal_LoadEntityTreeFromTextBlock(Map.Instance, textBlock, true, null))
            {
                MapSystemWorld.MapDestroy();
                return false;
            }
            /*
            if (EntitySystemWorld.Instance.WorldSimulationType != WorldSimulationTypes.Editor)
            {
                Map.Instance.GetDataForEditor().ClearAll();
            }
            //*/

            return true;
        }

        /// <summary>
        /// Saves a map to file.
        /// </summary>
        /// <param name="virtualFileName">The file name of virtual file system.</param>
        /// <param name="compressEntityUINs"></param>
        /// <returns><b>true</b> if the map has been saved; otherwise, <b>false</b>.</returns>
        public static bool MapSave(string virtualFileName, bool compressEntityUINs)
        {
            if (Map.Instance == null)
            {
                Log.Fatal("MapSystemWorld: MapSave: Map instance is not created.");
            }
            virtualFileName = PathUtils.NormalizeSlashes(virtualFileName);
            Entities.Instance.DeleteEntitiesQueuedForDeletion();
            if (compressEntityUINs)
                Entities.Instance.CompressUINs();

            Map.Instance.virtualFileName = virtualFileName;
            TextBlock textBlock = new TextBlock();
            Entities.Instance.WriteEntityTreeToTextBlock(Map.Instance, textBlock);
            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(virtualFileName);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(realPathByVirtual))
                {
                    streamWriter.Write(textBlock.DumpToString());
                }
            }
            catch
            {
                Log.Error("Unable to save file \"{0}\".", realPathByVirtual);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads a world from file.
        /// </summary>
        /// <param name="worldSimulationType">The world similation type.</param>
        /// <param name="virtualFileName">The file name of virtual file system.</param>
        /// <returns><b>true</b> if the world has been loaded; otherwise, <b>false</b>.</returns>
        public static bool WorldLoad(WorldSimulationTypes worldSimulationType, string virtualFileName)
        {
            if (EntitySystemWorld.Instance == null)
            {
                Log.Fatal("MapSystemWorld: WorldLoad: EntitySystemWorld.Instance == null");
                return false;
            }
            virtualFileName = PathUtils.NormalizeSlashes(virtualFileName);
            MapDestroy();
            EntitySystemWorld.Instance.WorldDestroy();
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(virtualFileName);
            if (textBlock == null)
                return false;

            string worldTypeName = textBlock.GetAttribute("type");
            WorldType worldType = EntityTypes.Instance.GetByName(worldTypeName) as WorldType;
            if (worldType == null)
            {
                Log.Error("世界加载失败: 世界类型 \"{0}\" 未定义。", worldTypeName);
                return false;
            }

            WorldLoad(textBlock);
            if (!EntitySystemWorld.Instance.WorldCreateWithoutPostCreate(worldSimulationType, worldType))
            {
                MapDestroy();
                EntitySystemWorld.Instance.WorldDestroy();
                return false;
            }
            Entities.Instance.Internal_InitUINOffset();
            Map.WorldFileName = virtualFileName;
            if (!Entities.Instance.Internal_LoadEntityTreeFromTextBlock(World.Instance, textBlock, true, null))
            {
                MapDestroy();
                EntitySystemWorld.Instance.WorldDestroy();
                return false;
            }
            /*
            if (Map.Instance != null && EntitySystemWorld.Instance.WorldSimulationType != WorldSimulationTypes.Editor)
            {
                Map.Instance.GetDataForEditor().ClearAll();
            }
            //*/
            Map.WorldFileName = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            EntitySystemWorld.Instance.ResetExecutedTime();
            return true;
        }

        private static void WorldLoad(TextBlock textBlock)
        {
            string text = null;
            foreach (TextBlock current in textBlock.Children)
            {
                if (current.IsAttributeExist("sourceMapVirtualFileName"))
                {
                    text = current.GetAttribute("sourceMapVirtualFileName");
                    break;
                }
            }
            if (text == null)
                return;

            string directoryName = Path.GetDirectoryName(text);
            string str = directoryName + "\\LogicSystemCache";
            string logicSystemCacheDllPath = str + "\\" + directoryName.Replace('/', '_').Replace('\\', '_') + "_LogicSystem.dll";
            if (VirtualFile.Exists(logicSystemCacheDllPath))
            {
                Assembly assembly = LoadAssembly(logicSystemCacheDllPath);
                if (assembly != null)
                    EntitySystemWorld.Instance.Internal_SetLogicSystemScriptsAssembly(assembly);
            }
        }

        /// <summary>
        /// Saves a world to file.
        /// </summary>
        /// <param name="virtualFileName">The file name of virtual file system.</param>
        /// <param name="compressEntityUINs"></param>
        /// <returns><b>true</b> if the world has been saved; otherwise, <b>false</b>.</returns>
        public static bool WorldSave(string virtualFileName, bool compressEntityUINs)
        {
            virtualFileName = PathUtils.NormalizeSlashes(virtualFileName);
            if (World.Instance == null)
            {
                Log.Fatal("MapSystemWorld: WorldSave: World.Instance == null.");
            }
            Entities.Instance.DeleteEntitiesQueuedForDeletion();
            if (compressEntityUINs)
                Entities.Instance.CompressUINs();

            if (Map.Instance != null)
                Map.Instance.virtualFileName = virtualFileName;

            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(virtualFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(realPathByVirtual));
            TextBlock textBlock = new TextBlock();
            Entities.Instance.WriteEntityTreeToTextBlock(World.Instance, textBlock);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(realPathByVirtual))
                {
                    streamWriter.Write(textBlock.DumpToString());
                }
            }
            catch
            {
                Log.Error("Unable to save file \"{0}\".", realPathByVirtual);
                return false;
            }
 
            return true;
        }
    }
}
