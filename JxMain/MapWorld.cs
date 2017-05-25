using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

using Jx;
using Jx.FileSystem;
using Jx.EntitySystem;
using Jx.MapSystem;
using Jx.Ext;
using Jx.Editors;
 

namespace JxMain
{
    public class MapWorld
    {
        public const string MAP_FILTER = "Map files (*.map)|*.map|All files (*.*)|*.*";

        private static MapWorld instance = null; 
        private static List<string> recentlyLoadedMap = new List<string>(); 
 
        public static MapWorld Instance
        {
            get {
                if( instance == null )
                {
                    instance = new MapWorld(); 
                }
                return instance;
            }
        }

        public static bool MapLoaded
        {
            get { return Map.Instance != null && !string.IsNullOrEmpty(Map.Instance.VirtualFileName); }
        }

        private static void RecordRecentlyLoadedMap(string p)
        {
            if (p == null)
                return;
            if (recentlyLoadedMap.Contains(p))
                recentlyLoadedMap.Remove(p);

            recentlyLoadedMap.Add(p);

#if _MAP_WORLD_
            XLog.debug("MapWorld.RecordRecentlyLoadedMap: {0}", p); 
#endif
        }

        private static void UpdateRecentlyLoadedMapIntoMenu()
        {
#if _MAP_WORLD_
            XLog.debug("MapWorld.UpdateRecentlyLoadedMapIntoMenu.."); 
#endif
        }

        public static List<string> RecentlyLoadedMapNames
        {
            get
            {
                List<string> result = new List<string>();
                result.AddRange(recentlyLoadedMap);
                return result;
            }
        }

        //-///////////////////////////////////////////////////////////////////
 
        public string MapTypeName { get; private set; } 
           
        private void DestroyWorld()
        {
            MapSystemWorld.MapDestroy();
            EntitySystemWorld.Instance.WorldDestroy();
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="p">虚拟路径</param>
        /// <returns></returns>
        public bool MapLoad(string p)
        {
            if (!ResetWorld())
                return false;
             
            bool result = MapSystemWorld.MapLoad(p);
            return result;
        }

        public bool MapDestroy()
        {
            if (Map.Instance == null)
                return false;

            MapSystemWorld.MapDestroy();
            DestroyWorld();
            return true;
        }

        private bool ResetWorld()
        {
            DestroyWorld();

            WorldType defaultWorldType = EntitySystemWorld.Instance.DefaultWorldType;
            if (!EntitySystemWorld.Instance.WorldCreate(WorldSimulationTypes.Single, defaultWorldType))
            {
                Log.Fatal("EntitySystemWorld.Instance.WorldCreate failed.");
                return false;
            }
            return true;
        }
            

        private MapWorld() { }
    }
}
