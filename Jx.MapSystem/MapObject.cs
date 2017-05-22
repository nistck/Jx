using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.MapSystem
{
    public class MapObjectType : MapGeneralObjectType
    {
    }

    public class MapObject : MapGeneralObject
    {
        private MapObjectType _type = null; 
        public new MapObjectType Type { get { return _type; } }

        [FieldSerialize]
        private string editorLayer;

        [Browsable(true)]
        [LogicSystemBrowsable(false)]
        [Editor("Jx.EntitiesCommon.Editors.EditorLayerEditor, Jx.EntitiesCommon.Editors", typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public string EditorLayer
        {
            get { return editorLayer; }
            set { this.editorLayer = value; }
        }

    }
}
