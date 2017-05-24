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
        public MapType()
        {
            base.CreatableInMapEditor = false;
        }

        [Browsable(false)]
        public new bool CreatableInMapEditor
        {
            get { return base.CreatableInMapEditor; }
            set { }
        }
    }

    public class Map : MapGeneralObject
    {
        public static string WorldFileName { get; internal set; }

        [TypeConverter(typeof(EditorLayerTypeConverter))]
        public class EditorLayer
        {
            private bool visible = true;
            private bool allowSelect = true;
            private bool allowEdit = true;
            private readonly List<EditorLayer> children = new List<EditorLayer>(); 

            public EditorLayer(EditorLayer parent)
            {
                this.Parent = parent; 
            }

            public EditorLayer(string name, EditorLayer parent)
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
                        
            public EditorLayer Parent { get; private set; }

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

            public List<EditorLayer> Children
            {
                get {
                    List<EditorLayer> result = new List<EditorLayer>();
                    result.AddRange(children);
                    return result;
                }
            }

            public List<EditorLayer> ChildrenDescent
            {
                get {
                    List<EditorLayer> result = new List<EditorLayer>();

                    foreach(EditorLayer child in children)
                    {
                        result.Add(child);
                        result.AddRange(child.ChildrenDescent);
                    }

                    return result; 
                }
            }

            public EditorLayer Create(string name = "New Layer_")
            {
                EditorLayer layer = new EditorLayer(name, this);
                children.Add(layer);
                return layer;
            }

            public void Create(EditorLayer layer)
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
                        if (entity.EditorLayer == null)
                            continue;
                        if( entity.EditorLayer.Path == p )
                            entity.EditorLayer = null;
                    }
                    Parent = null;
                }
                return result;
            }

            public EditorLayer Find(string p)
            {
                if (p == null)
                    return null;

                string[] ps = p.Split('\\');
                EditorLayer editorLayer = Instance.RootEditorLayer;
                for(int i = 1; i < ps.Length && editorLayer != null; i ++)
                {
                    string psi = ps[i];
                    EditorLayer layer = editorLayer.FindChild(psi);
                    if (layer == null)
                    {
                        editorLayer = null;
                        break;
                    }
                    editorLayer = layer;
                }
                return editorLayer;
            }

            public EditorLayer FindChild(string name)
            {
                if (name == null)
                    return null; 
                return children.Where(_c => _c.Name == name).FirstOrDefault();
            }

            public bool HasChild(string name, EditorLayer excludeLayer = null)
            {
                if (string.IsNullOrEmpty(name))
                    return false; 

                foreach(EditorLayer layer in children)
                {
                    if (excludeLayer != null && excludeLayer == layer)
                        continue; 
                    if (name == layer.Name)
                        return true;    
                }
                return false;
            }

            private bool RemoveChild(EditorLayer layerChild)
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

                foreach(EditorLayer child in children)
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
                    EditorLayer layer = new EditorLayer(this);
                    if (!layer.OnLoad(child))
                        return false;
                    children.Add(layer);
                }
                return true;
            }

            public override bool Equals(object obj)
            {
                EditorLayer layer = obj as EditorLayer;
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

        public class EditorLayerTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return false;
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null || Instance == null )
                    return null;

                string valueString = Convert.ToString(value);
                if (string.IsNullOrEmpty(valueString))
                    return Instance.RootEditorLayer;

                string[] dirs = valueString.Split(Path.AltDirectorySeparatorChar);
                EditorLayer layer = Instance.RootEditorLayer;

                for(int i = 1; i < dirs.Length; i ++)
                {
                    layer = layer.FindChild(dirs[i]);
                    if (layer == null)
                        return Instance.RootEditorLayer;
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
                    EditorLayer layer = value as EditorLayer; 
                    return layer == null? "" : layer.Path;
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
        private readonly EditorLayer rootEditorLayer = new EditorLayer("Root Layer", null);
        private EditorLayer layerSelected = null;

        public Map ()
        {
            if (instance != null)
                throw new Exception("地图实例已存在");

            instance = this; 
        }

        public EditorLayer RootEditorLayer
        {
            get { return rootEditorLayer; }
        }

        public EditorLayer LayerSelected
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
            if (editorLayersBlock != null && rootEditorLayer.OnLoad(editorLayersBlock))
            {
                if (Instance.RootEditorLayer != null)
                {
                    Instance.Children.OfType<MapObject>().Any(_mo =>
                    {
                        _mo.EditorLayer = Instance.RootEditorLayer.Find(_mo._editorLayerLast);
                        return false;
                    });
                }
            }

            return true;
        }

        protected override void OnSave(TextBlock block)
        {
            base.OnSave(block);

            TextBlock editorLayersBlock = block.AddChild("editorLayers");
            rootEditorLayer.OnSave(editorLayersBlock);
        }
    }
}
