using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;
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
            private bool visible = true;
            private bool allowSelect = true;
            private bool allowEdit = true;
            private readonly List<Layer> children = new List<Layer>(); 

            public Layer(Layer parent)
            {
                this.Parent = parent; 
            }

            public Layer(string name, Layer parent)
            {
                this.Name = name;
                this.Parent = parent;
            }

            public string Name { get; set; } 
            public bool Visible
            {
                get { return this.visible; }
                set { this.visible = value; }
            }
            public bool AllowSelect
            {
                get { return this.allowSelect; }
                set { this.allowSelect = value; }
            }
            public bool AllowEdit
            {
                get { return this.allowEdit; }
                set { this.allowEdit = value; }
            }
                        
            public Layer Parent { get; private set; }

            public string Path
            {
                get {
                    if( Parent != null )
                    {
                        string r0 = string.Format(@"{0}\{1}", Parent.Path, Name);
                        return r0;
                    }
                    return Name;
                }
            }

            public List<Layer> Children
            {
                get {
                    List<Layer> result = new List<Layer>();
                    result.AddRange(children);
                    return result;
                }
            }

            public Layer Create(string name = "New Layer_")
            {
                Layer layer = new Layer(name, this);
                children.Add(layer);
                return layer;
            }

            public void Create(Layer layer)
            {
                if (layer == null)
                    return;
                children.Add(layer);
            }

            public bool Remove()
            {
                if (Parent == null)
                    return false;

                string p = Path;
                bool result = Parent.RemoveChild(this);
                if( result )
                {
                    foreach(Entity entity in Entities.Instance.EntitiesCollection)
                    {
                        if( entity.EditorLayer == p )
                            entity.EditorLayer = null;
                    }
                    Parent = null;
                }
                return result;
            }

            public bool HasChild(string name, Layer excludeLayer = null)
            {
                if (string.IsNullOrEmpty(name))
                    return false; 

                foreach(Layer layer in children)
                {
                    if (excludeLayer != null && excludeLayer == layer)
                        continue; 
                    if (name == layer.Name)
                        return true;    
                }
                return false;
            }

            private bool RemoveChild(Layer layerChild)
            {
                if (layerChild == null || !children.Contains(layerChild))
                    return false;

                bool result = children.Remove(layerChild);
                return result;
            }

            internal void OnSave(TextBlock block)
            {
                if (block == null)
                    return;

                block.SetAttribute("name", this.Name);
                if (!Visible)
                    block.SetAttribute("visible", Visible.ToString());
                if (!AllowSelect)
                    block.SetAttribute("allowSelect", AllowSelect.ToString());
                if (!AllowEdit)
                    block.SetAttribute("allowEdit", AllowEdit.ToString());
                foreach(Layer child in children)
                {
                    TextBlock childBlock = block.AddChild("layer");
                    child.OnSave(childBlock);
                }
            }

            internal bool OnLoad(TextBlock block)
            {
                if (block == null)
                    return false;

                if (block.IsAttributeExist("name"))
                    this.Name = block.GetAttribute("name");
                if (block.IsAttributeExist("visible"))
                    this.Visible = bool.Parse(block.GetAttribute("visible"));
                if (block.IsAttributeExist("allowSelect"))
                    this.AllowSelect = bool.Parse(block.GetAttribute("allowSelect"));
                if (block.IsAttributeExist("allowEdit"))
                    this.AllowEdit = bool.Parse(block.GetAttribute("allowEdit"));
                foreach(TextBlock child in block.Children)
                {
                    Layer layer = new Layer(this);
                    if (!layer.OnLoad(child))
                        return false; 
                }
                return true;
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
        private readonly Layer rootLayer = new Layer("Root", null);
        private Layer layerSelected = null;

        public Map ()
        {
            if (instance != null)
                throw new Exception("地图实例已存在");

            instance = this; 
        }

        public Layer RootLayer
        {
            get { return rootLayer; }
        }

        public Layer LayerSelected
        {
            get { return layerSelected; }
            set {
                layerSelected = value;
            }
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        protected override bool OnLoad(TextBlock block)
        {
            if (!base.OnLoad(block))
                return false;

            TextBlock editorLayersBlock = block.FindChild("editorLayers");
            if (editorLayersBlock != null && !rootLayer.OnLoad(block))
                return false;

            return true;
        }

        protected override void OnSave(TextBlock block)
        {
            base.OnSave(block);

            TextBlock editorLayersBlock = block.AddChild("editorLayers");
            rootLayer.OnSave(editorLayersBlock);
        }
    }
}
