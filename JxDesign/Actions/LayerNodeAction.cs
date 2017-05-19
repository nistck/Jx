using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.Ext;
using Jx.MapSystem;

namespace JxDesign.Actions
{
    internal class LayerNodeAction : UndoSystem.Action
    {   
        public LayerNodeAction(TreeNode layerNode)
        {
            this.LayerNode = layerNode;
            if( layerNode != null )
            {
                this.LayerParentNode = layerNode.Parent;
                this.Layer = layerNode.Tag as Map.Layer;
                this.ParentLayer = Layer == null ? null : Layer.Parent;
            }
        }

        private TreeNode LayerNode { get; set; }
        private TreeNode LayerParentNode { get; set; }
        private Map.Layer Layer { get; set; }
        private Map.Layer ParentLayer { get; set; }

        protected override void Destroy()
        {
            LayerNode = null;
        }

        protected override void DoRedo()
        {
            MainForm.Instance.EntitiesForm.CreateLayerNode(Layer, LayerParentNode);
            if (ParentLayer != null)
                ParentLayer.Create(Layer);
        }

        protected override void DoUndo()
        {
            if( LayerNode != null )
            {
                Map.Layer layer = LayerNode.Tag as Map.Layer;
                LayerNode.Remove(); 
                layer.Remove();
            }
        }
    }
}
