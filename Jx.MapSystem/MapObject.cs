using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jx.FileSystem;

namespace Jx.MapSystem
{
    public class MapObjectType : MapGeneralObjectType
    {
    }

    public class MapObject : MapGeneralObject
    {
        private MapObjectType _type = null; 
        public new MapObjectType Type { get { return _type; } }

        internal string _editorLayerLast;
        private Map.EditorLayer editorLayer;

        [Browsable(true)]
        [LogicSystemBrowsable(false)]
        [Editor("Jx.EntitiesCommon.Editors.EditorLayerEditor, Jx.EntitiesCommon.Editors", typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Map.EditorLayer EditorLayer
        {
            get { return editorLayer; }
            set { this.editorLayer = value; }
        }

        protected override bool OnLoad(TextBlock block)
        {
            if (!base.OnLoad(block))
                return false;

            if (block.IsAttributeExist("editorLayer"))
            {
                _editorLayerLast = block.GetAttribute("editorLayer");
                if (Map.Instance != null && Map.Instance.RootEditorLayer != null)
                    EditorLayer = Map.Instance.RootEditorLayer.Find(_editorLayerLast);
            }

            return true;
        }

        protected override void OnSave(TextBlock block)
        {
            if( EditorLayer != null )
                block.SetAttribute("editorLayer", EditorLayer.Path);

            base.OnSave(block);
        }
    }
}
