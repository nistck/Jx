﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Jx;
using Jx.Ext;
using Jx.UI;
using Jx.MapSystem;
using Jx.EntitySystem;
using JxDesign.Actions;
using Jx.Editors;


namespace JxDesign.UI
{ 
    public partial class EntitiesForm : DockContent
    {
        private ImageCache imageCache = null;
        private ImageCache tvImageCache = null;

        private TreeNode rootNode = null;

        private TreeNode rootLayerNode = null;
        private readonly Dictionary<string, TreeNode> layerPathDic = new Dictionary<string, TreeNode>();

        private TreeNode currentMouseNode = null;
        public event TreeNodeSelectChangedHandler NodeSelectChanged;

        public EntitiesForm()
        {
            InitializeComponent();
        }

        private void EntitiesForm_Load(object sender, EventArgs e)
        {
            imageCache = new ImageCache(IL16);
            tvImageCache = new ImageCache(ILtreeView);

            tsbRefresh.Image = imageCache["refresh"];
            tsmiCreateLayer.Image = imageCache["layer_new"];
            tsmiDeleteLayer.Image = imageCache["layer_delete"];
            tsmiEditLayer.Image = imageCache["layer_edit"];
            tsmiDelete.Image = imageCache["delete"];

            treeViewEntities.ImageList = ILtreeView; 
        }

        public void UpdateData()
        {
            treeViewEntities.Nodes.Clear();
            layerPathDic.Clear();
            rootNode = null;
            rootLayerNode = null;

            // Root is Map
            if (Map.Instance == null)
                return;

            rootNode = CreateEntityNode(Map.Instance, null, false);

            BuildLayerNode(); 
            LoadEntities();

            if( rootLayerNode != null )
                rootLayerNode.Expand();
            SetNodeSelected(rootLayerNode);
        }

        private void LoadEntities()
        {
            List<Entity> entities = new List<Entity>();
            if (Map.Instance == null)
                return;

            entities.AddRange(Map.Instance.Children);
            while( entities.Count > 0 )
            {
                Entity entity = entities[0];
                entities.RemoveAt(0);

                Map.EditorLayer editorLayer = null;
                if( entity is MapObject )
                {
                    MapObject mapObject = entity as MapObject;
                    editorLayer = mapObject.EditorLayer;
                }

                TreeNode layerNode = TreeViewUtil.FindNodeByTag(rootLayerNode, editorLayer);
                layerNode = layerNode ?? rootNode;
                CreateEntityNode(entity, layerNode, false);
            }
        } 

        public void UpdateMapObjectLayer(MapObject target)
        {
            if (target == null)
                return;

            TreeNode objectNode = TreeViewUtil.FindNodeByTag(rootNode, target);
            if (objectNode == null)
                return;

            TreeNode layerNode = target.EditorLayer == null? rootNode : TreeViewUtil.FindNodeByTag(rootNode, target.EditorLayer);
            if ( layerNode != null)
            {
                objectNode.Remove();
                layerNode.Nodes.Add(objectNode);
                TreeViewUtil.ExpandTo(objectNode);
            }
        } 

        internal void OnNodeSelectChanged(TreeNode nodeNew)
        {
            if (EntityWorld.Instance != null)
            {
                if (currentMouseNode != null)
                {
                    Entity currentEntity = currentMouseNode.Tag as Entity;
                    EntityWorld.Instance.SetEntitySelected(currentEntity, false);
                }

                if (nodeNew != null)
                {
                    Entity newEntity = nodeNew.Tag as Entity;
                    EntityWorld.Instance.SetEntitySelected(newEntity, true);
                }
            }
 
            if (currentMouseNode == null && nodeNew == null)
                return;

            TreeNode nodeOld = currentMouseNode;
            currentMouseNode = nodeNew;

            bool b1 = nodeOld != null && nodeNew == null;
            bool b2 = nodeOld == null && nodeNew != null;
            bool b3 = nodeOld != null && nodeNew != null && !nodeOld.Equals(nodeNew); 

            bool changed = b1 || b2 || b3;
            if (NodeSelectChanged != null)
                NodeSelectChanged(nodeNew, nodeOld, changed);
        }

        private void SetNodeSelected(TreeNode node)
        {
            treeViewEntities.SelectedNode = node;
            OnNodeSelectChanged(node);
        }

        public TreeNode CreateEntityNode(Entity entity, TreeNode parent = null, bool selected = true)
        {
            if (entity == null)
                return null;

            int index = tvImageCache.Index("entity");
            TreeNode node = new TreeNode(entity.Name ?? "-", index, index);
            node.Tag = entity;
            if (parent != null)
                parent.Nodes.Add(node);
            else
                treeViewEntities.Nodes.Add(node);

            if(parent != null && entity is MapObject && parent.Tag is Map.EditorLayer )
            {
                ((MapObject)entity).EditorLayer = parent.Tag as Map.EditorLayer;
            }
               

            if (selected)
                SetNodeSelected(node);
            return node;
        }

        public TreeNode CreateLayerNode(Map.EditorLayer layer, TreeNode parent = null, bool selected = true)
        {
            if (layer == null)
                return null;

            string layerName = string.IsNullOrEmpty(layer.Name) ? "New Layer_" : layer.Name;
            TreeNode layerNode = new TreeNode(layerName, 0, 0);
            layerNode.Tag = layer;
            if (parent != null)
                parent.Nodes.Add(layerNode);
            else
                treeViewEntities.Nodes.Add(layerNode);
            if (selected)
                treeViewEntities.SelectedNode = layerNode;
            return layerNode;
        }

        private void BuildLayerNode(Map.EditorLayer layer = null, TreeNode parent = null)
        {
            if (Map.Instance == null)
                return;
 
            layer = layer ?? Map.Instance.RootEditorLayer;
            if (layer == null)
                return; 
            parent = parent ?? rootNode; 

            TreeNode node = CreateLayerNode(layer, parent);
            if (layer == Map.Instance.RootEditorLayer)
                rootLayerNode = node;
            if (node == null)
                return;

            foreach(Map.EditorLayer layerChild in layer.Children)
                BuildLayerNode(layerChild, node);
        } 
        
        private Map.EditorLayer LayerSelected
        {
            get {
                if (Map.Instance == null)
                    return null;
                return Map.Instance.LayerSelected;
            }
            set {
                if (Map.Instance == null)
                    return;

                if( Map.Instance.LayerSelected != value )
                    Map.Instance.LayerSelected = value;
            }
        }
 
        private T CurrentNode<T>()
        {
            if (treeViewEntities.SelectedNode == null || treeViewEntities.SelectedNode.Tag == null)
                return default(T);

            object tag = treeViewEntities.SelectedNode.Tag;
            if (typeof(T).IsAssignableFrom(tag.GetType()))
                return (T)tag;

            return default(T);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tsmiCreateLayer.Enabled = Map.Instance != null;
            tsmiDeleteLayer.Enabled = (LayerSelected != null) && (Map.Instance != null && Map.Instance.RootEditorLayer != LayerSelected);
            tsmiEditLayer.Enabled = IsLayerSelected && Map.Instance != null;

            Entity entity = CurrentNode<Entity>();
            tsmiDelete.Enabled = entity != null && Map.Instance != entity && Map.Instance != null;
        }

        /// <summary>
        /// 如果当前选中的TreeNode是由Map.Layer创建的， 返回当前选中的TreeNode, 否则返回NULL
        /// </summary>
        private TreeNode CurrentLayerNode
        {
            get
            {
                if (treeViewEntities.SelectedNode == null)
                    return null;
                Map.EditorLayer layer = treeViewEntities.SelectedNode.Tag as Map.EditorLayer;
                LayerSelected = layer;
                return layer == null ? null : treeViewEntities.SelectedNode;
            }
        }

        /// <summary>
        /// 当前是否选中了Map.Layer
        /// </summary>
        private bool IsLayerSelected
        {
            get { return CurrentLayerNode != null; }
        }

        private void treeViewEntities_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        /// <summary>
        /// 获得当前选中节点所在的Map.Layer， 如果当前选中节点本身是一个Map.Layer，则返回当前选中节点
        /// </summary>
        /// <returns></returns>
        public TreeNode GetCurrentNodeLayer()
        {
            if (treeViewEntities.SelectedNode == null)
                return null;

            TreeNode node = treeViewEntities.SelectedNode;
            while(node != null )
            {
                Map.EditorLayer layer = node.Tag as Map.EditorLayer;
                if (layer != null)
                    return node;
                node = node.Parent;
            }
            return rootLayerNode;
        }

        private void tsmiCreateLayer_Click(object sender, EventArgs e)
        {
            TreeNode layerNode = GetCurrentNodeLayer();
            if (layerNode == null)
            {
                Log.Info("找不到当前选中节点所在的Layer");
                return;
            }
            Map.EditorLayer nodeLayer = layerNode.Tag as Map.EditorLayer;
            List<string> names = nodeLayer.Children.Select(_layer => _layer.Name).ToList();
            int index = 0;
            string nameNew = "New Layer"; 
            do
            {
                nameNew = string.Format("New Layer_{0}", index++);
                if (!names.Contains(nameNew))
                    break;
            } while (true);

            Map.EditorLayer layerNew = nodeLayer.Create(nameNew);
            TreeNode nodeNew = CreateLayerNode(layerNew, layerNode);
            if (nodeNew != null)
            {
                treeViewEntities.SelectedNode = nodeNew;
                nodeNew.EnsureVisible();
                treeViewEntities.LabelEdit = true;
                nodeNew.BeginEdit();

                /*
                LayerNodeAction layerNodeAction = new LayerNodeAction(nodeNew);
                UndoSystem.Instance.CommitAction(layerNodeAction);
                //*/
            }
            MapWorld.Instance.Modified = true;
        }



        private void tsmiDeleteLayer_Click(object sender, EventArgs e)
        {
            TreeNode node = CurrentLayerNode;
            if( node == null )
            {
                tsmiDeleteLayer.Enabled = false;
                return; 
            }
            List<TreeNode> valueNodes = new List<TreeNode>();
            TreeViewUtil.EnumerateNodes(node, (_node) => {
                if (_node.Tag is Entity)
                    valueNodes.Add(_node);
                return true;
            }); 
            if( valueNodes.Count > 0 )
            {
                string infoMessage = string.Format("该Layer下有{0}个节点, 删除该Layer会将这些节点放在根节点下。是否继续？", valueNodes.Count);
                if (MessageBox.Show(infoMessage, "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                    return;
            }

            Map.EditorLayer layer = node.Tag as Map.EditorLayer;
            if (Map.Instance != null && layer == Map.Instance.RootEditorLayer)
            {
                tsmiDeleteLayer.Enabled = false;
                return;
            }

            if (layer.Remove())
            {
                node.Tag = null;                
                if (node.Parent != null)
                    treeViewEntities.SelectedNode = node.Parent;
                node.Remove();

                UpdateData();

                MapWorld.Instance.Modified = true;
            }
        }

        private void tsmiEditLayer_Click(object sender, EventArgs e)
        {
            TreeNode node = CurrentLayerNode;
            if (node == null)
            {
                tsmiEditLayer.Enabled = false;
                return;
            }
            treeViewEntities.LabelEdit = true;
            node.BeginEdit();
        }

        private void treeViewEntities_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            treeViewEntities.LabelEdit = false;
            if (e.Label == null)
                return;

            Map.EditorLayer layer = e.Node.Tag as Map.EditorLayer;
            if (layer == null)
            {
                e.CancelEdit = true;
                e.Node.EndEdit(false);
                return;
            }

            if (e.Label.Length > 0)
            {
                if (e.Label.IndexOf("\\") > -1)
                {
                    string infoMessage = string.Format("不允许包含字符【\\】");
                    e.CancelEdit = true;
                    MessageBox.Show(infoMessage, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Node.EndEdit(true);
                }
                else
                {
                    if (layer.Parent != null && layer.Parent.HasChild(e.Label, layer))
                    {
                        e.CancelEdit = true;
                        string infoMessage = string.Format("Layer 【{0}】 已存在", e.Label);
                        MessageBox.Show(infoMessage, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Node.EndEdit(true);
                    }
                    else
                    {
                        e.Node.EndEdit(false);
                        MapWorld.Instance.Modified = true;
                    }
                }
            }
            else
            {
                e.CancelEdit = true;
                e.Node.BeginEdit();
            }
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void treeViewEntities_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = treeViewEntities.GetNodeAt(new Point(e.X, e.Y));
            SetNodeSelected(node);
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            Entity entity = CurrentNode<Entity>(); 
            if( entity == null || entity == Map.Instance )
            {
                tsmiDelete.Enabled = false;
                return; 
            }
            TreeNode node = treeViewEntities.SelectedNode;
            if (node == null)
                return;


            List<Entity> rList = Entities.Instance.GetReferenceTo(entity);
            string referenceInfo = string.Format("删除回影响到{0}个引用。", rList.Count);
            if (rList.Count == 0)
                referenceInfo = "";

            string infoMessage = string.Format("确定要删除【{0}】吗？ \n\n{1}", entity, referenceInfo);
            if (MessageBox.Show(infoMessage, "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            rList.Any(_entity => {

                return false;
            });

            entity.SetForDeletion(false);
            if (node.Parent != null)
                treeViewEntities.SelectedNode = node.Parent;
            node.Remove();

            MapWorld.Instance.Modified = true;
        }
    }
}
