using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.FileSystem;

namespace Jx.MapSystem
{
    [ManualTypeCreate]
    public class MapType : MapGeneralObjectType
    {
 
    }

    public class Map : MapGeneralObject
    {
        public static string WorldFileName { get; internal set; }

        public class Layer
        {
            private Layer parent; 
            private readonly List<Layer> children = new List<Layer>(); 

            public Layer(Layer parent)
            {
                this.parent = parent; 
            }
            public string Name { get; set; }
            public bool Visible { get; set; }
            public bool AllowSelect { get; set; }
            public List<Layer> Children
            {
                get {
                    List<Layer> result = new List<Layer>();
                    result.AddRange(children);
                    return result;
                }
            }

            public void Remove()
            {
                if (parent == null)
                    return; 

            }


        }

        public class EditorData
        {
            public void ClearDeletedEntities()
            {

            }
        }

        private static Map instance = null; 

        public static Map Instance
        {
            get { return instance; }
        }

        private MapType _type = null; 
        public new MapType Type { get { return _type; } }

        internal string virtualFileName;

        private EditorData editorData = new EditorData();

        public Map ()
        {
            if (instance != null)
                throw new Exception("地图实例已存在");

            instance = this; 
        }

        /// <summary>
        /// 地图文件虚拟路径
        /// </summary>
        public string VirtualFileName
        {
            get { return this.virtualFileName; }
        }

        /// <summary>
        /// 地图全路径
        /// </summary>
        public string FileName
        {
            get {
                if (!string.IsNullOrEmpty(virtualFileName))
                    return VirtualFileSystem.GetRealPathByVirtual(virtualFileName);
                return ""; 
            }
        }

        public EditorData GetDataForEditor()
        {
            return editorData;
        }
    }
}
