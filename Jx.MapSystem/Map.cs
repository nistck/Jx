using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;
using Jx.FileSystem;
using System.Globalization;

namespace Jx.MapSystem
{
    [ManualTypeCreate]
    public class MapType : MapGeneralObjectType
    {
 
    }

    public class Map : MapGeneralObject
    {
        public static string WorldFileName { get; internal set; }

        [TypeConverter(typeof(LayerTypeConverter))]
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

            public int Indent
            {
                get {
                    if (Parent != null)
                        return Parent.Indent + 1;
                    return 0;
                }
            }

            public string Path
            {
                get {
                    if( Parent != null )
                    {
                        string r0 = string.Format(@"{0}{1}{2}", Parent.Path, System.IO.Path.DirectorySeparatorChar, Name);
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

            public List<Layer> ChildrenDescent
            {
                get {
                    List<Layer> result = new List<Layer>();

                    foreach(Layer child in children)
                    {
                        result.Add(child);
                        result.AddRange(child.ChildrenDescent);
                    }

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
                    var q = Entities.Instance.EntitiesCollection.OfType<MapObject>();
                    foreach(MapObject entity in q)
                    {
                        if( entity.EditorLayer == p )
                            entity.EditorLayer = null;
                    }
                    Parent = null;
                }
                return result;
            }

            public Layer FindChild(string name)
            {
                if (name == null)
                    return null; 
                return children.Where(_c => _c.Name == name).FirstOrDefault();
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
                    children.Add(layer);
                }
                return true;
            }

            public override bool Equals(object obj)
            {
                Layer layer = obj as Layer;
                if (layer == null || layer.Path == null || Path == null)
                    return false;

                return layer.Path == Path;
            }

            public override int GetHashCode()
            {
                return Path == null ? 0 : Path.GetHashCode(); 
            }

            public override string ToString()
            {
                return Path;
            }
        }

        public class LayerTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true; 
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null || Instance == null )
                    return null;

                string valueString = Convert.ToString(value);
                if (string.IsNullOrEmpty(valueString))
                    return Instance.RootLayer;

                string[] dirs = valueString.Split(Path.AltDirectorySeparatorChar);
                Layer layer = Instance.RootLayer;

                for(int i = 1; i < dirs.Length; i ++)
                {
                    layer = layer.FindChild(dirs[i]);
                    if (layer == null)
                        return Instance.RootLayer;
                }
                return layer;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true; 
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if( destinationType == typeof(string) )
                {
                    Layer layer = value as Layer;
                    layer = layer ?? Instance.RootLayer;
                    return layer.Path;
                }

                return base.ConvertTo(context, culture, value, destinationType);
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
            if (editorLayersBlock != null && !rootLayer.OnLoad(editorLayersBlock))
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
