﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using WeifenLuo.WinFormsUI.Docking;

using Jx;
using Jx.Ext;
using Jx.UI;
using Jx.EntitySystem;
using Jx.Editors;
using Jx.FileSystem;

using JxDesign.Properties;

namespace JxDesign.UI
{
    public delegate void SelectedItemChangeDelegate(EntityTypesForm sender);

    public partial class EntityTypesForm : DockContent
    {
        public enum ItemTypes
        {
            Null,
            Entity,
            Mesh
        }

        private class TreeNodeComparer : IComparer
        {
            private TreeNode current;

            public TreeNodeComparer(TreeNode noSelectionNode)
            {
                this.current = noSelectionNode;
            }

            public int Compare(object x, object y)
            {
                TreeNode treeNode = (TreeNode)x;
                TreeNode treeNode2 = (TreeNode)y;
                if (treeNode == this.current)
                {
                    return -1;
                }
                if (treeNode2 == this.current)
                {
                    return 1;
                }
                if (treeNode.Nodes.Count != 0 && treeNode2.Nodes.Count == 0)
                {
                    return -1;
                }
                if (treeNode.Nodes.Count == 0 && treeNode2.Nodes.Count != 0)
                {
                    return 1;
                }
                return treeNode.Text.CompareTo(treeNode2.Text);
            }
        }

        private Dictionary<EntityType, bool> Blr = new Dictionary<EntityType, bool>();
        public event SelectedItemChangeDelegate SelectedItemChange;

        private bool objectTreeModified = false; 
        private TreeNode currentObjectsNode;

        private bool meshTreeModified = false;
        private TreeNode current3dModelNode;

        private bool Blq = false;
        private bool BlR = false;

        private ImageCache imageCache = null;

        public EntityTypesForm()
        {
            InitializeComponent();
        }


        [Browsable(false)]
        public Tuple<ItemTypes, object> SelectedItem
        {
            get
            {
                if (tabControl.SelectedTab == this.tabPageObjects)
                {
                    EntityType entityType = this.GetCurrentEntityType();
                    if (entityType != null)
                    {
                        return new Tuple<ItemTypes, object>(ItemTypes.Entity, entityType);
                    }
                }
                if (this.tabControl.SelectedTab == this.tabPage3dModel)
                {
                    TreeNode selectedNode = this.treeView3dModel.SelectedNode;
                    if (selectedNode != null && selectedNode.Tag != null && selectedNode.Tag is string)
                    {
                        return new Tuple<ItemTypes, object>(ItemTypes.Mesh, (string)selectedNode.Tag);
                    }
                }
                return new Tuple<ItemTypes, object>(ItemTypes.Null, null);
            }
        } 

        private void UpdateTreeViewObjects()
        {
            Blr.Clear();
            treeViewObjects.BeginUpdate();

            TreeNode selected = treeViewObjects.SelectedNode;
            string selectPath = null;
            if (selected != null)
                selectPath = selected.FullPath;
            treeViewObjects.Nodes.Clear();
            foreach (EntityType current in EntityTypes.Instance.Types)
            {
                if (current.CreatableInMapEditor /*&& !(current is DecorativeObjectType)*/ &&
                    (string.IsNullOrEmpty(current.FilePath) || VirtualFile.Exists(current.FilePath)))
                {
                    InsertEntityType(current);
                }
            }
            enumerateTreeNodes_objects();
            currentObjectsNode = null;
            if (TreeViewUtils.FindNodeByTag(this.treeViewObjects, null) == null)
            {
                currentObjectsNode = new TreeNode(ToolsLocalization.Translate("Various", "(no selection)"), 3, 3);
                treeViewObjects.Nodes.Add(currentObjectsNode);
                treeViewObjects.SelectedNode = currentObjectsNode;
            }
            treeViewObjects.TreeViewNodeSorter = new TreeNodeComparer(currentObjectsNode);
            treeViewObjects.Sort();
            treeViewObjects.EndUpdate();

            if (selectPath != null)
            {
                TreeNode treeNode = TreeViewUtils.FindNodeByText(treeViewObjects, selectPath);
                if (treeNode != null)
                {
                    List<TreeNode> nodes = new List<TreeNode>();
                    TreeNode node = treeNode;
                    while (node.Parent != null)
                    {
                        nodes.Add(node);
                        node = node.Parent;
                    }
                    for (int i = nodes.Count - 1; i >= 0; i--)
                        nodes[i].Expand();

                    treeViewObjects.SelectedNode = treeNode;
                }
            }
            if (treeViewObjects.Nodes.Count > 0)
            {
                TreeNode objectNode = treeViewObjects.Nodes[0];
                objectNode.EnsureVisible();
                objectNode.Expand();

                treeViewObjects.TopNode = objectNode;
            }
        }

        private void CreateTreeNodeByDirectory(
            TreeView treeView, ResourceType resourceType, string path, TreeNode parentNode)
        {
            string[] directories = VirtualDirectory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                string dir = directories[i];
                string dirName = Path.GetFileName(dir);
                TreeNode node = new TreeNode(dirName, 0, 0);
                node.Name = node.Text;
                if (parentNode != null)
                {
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    treeView.Nodes.Add(node);
                }
                CreateTreeNodeByDirectory(treeView, resourceType, dir, node);
                if (node.Nodes.Count == 0)
                {
                    node.Remove();
                }
            }
            string searchPattern;
            if (resourceType != null && resourceType.Extensions.Length == 1)
            {
                searchPattern = "*." + resourceType.Extensions[0];
            }
            else
            {
                searchPattern = "*";
            }
            string[] files = VirtualDirectory.GetFiles(path, searchPattern);
            int j = 0;
            while (j < files.Length)
            {
                string file = files[j];
                if (resourceType == null || resourceType.Extensions.Length == 1)
                {
                    CreateTreeNodeByFile(treeView, resourceType, parentNode, file);
                    j++;
                    continue;
                }
                string fileExt = System.IO.Path.GetExtension(file);
                if (fileExt.Length > 0)
                    fileExt = fileExt.Substring(1);

                string[] extensions = resourceType.Extensions;
                for (int k = 0; k < extensions.Length; k++)
                {
                    string strB = extensions[k];
                    if (string.Compare(fileExt, strB, true) == 0)
                    {
                        CreateTreeNodeByFile(treeView, resourceType, parentNode, file);
                        break;
                    }
                }
                j++;
            }
        }

        private void CreateTreeNodeByFile(TreeView treeView, ResourceType resourceType, TreeNode parentNode, string text)
        {
            string fileName = Path.GetFileName(text);
            TreeNode node = new TreeNode(fileName, 2, 2);
            node.Name = node.Text;
            node.Tag = text;
            if (parentNode != null)
            {
                parentNode.Nodes.Add(node);
                return;
            }
            treeView.Nodes.Add(node);
        }

        private void UpdateTreeView3dModel()
        {
            treeView3dModel.BeginUpdate();
            treeView3dModel.Nodes.Clear();
            CreateTreeNodeByDirectory(treeView3dModel, ResourceTypeManager.Instance.GetByExtension("mesh"), "", null);
            current3dModelNode = null;
            if (TreeViewUtils.FindNodeByTag(treeView3dModel, null) == null)
            {
                current3dModelNode = new TreeNode(ToolsLocalization.Translate("Various", "(no selection)"), 3, 3);
                treeView3dModel.Nodes.Add(current3dModelNode);
                treeView3dModel.SelectedNode = current3dModelNode;
            }
            treeView3dModel.TreeViewNodeSorter = new TreeNodeComparer(current3dModelNode);
            treeView3dModel.Sort();
            treeView3dModel.EndUpdate();
        }

        public void UpdateTrees()
        {
            UpdateTreeViewObjects();
            UpdateTreeView3dModel();
        }

        public void ClearSelection()
        {
            if (this.treeViewObjects.SelectedNode != null && this.treeViewObjects.SelectedNode.Tag != null)
            {
                this.treeViewObjects.BeginUpdate();
                if (this.treeViewObjects.SelectedNode.Parent != null)
                {
                    this.treeViewObjects.SelectedNode = this.treeViewObjects.SelectedNode.Parent;
                }
                else if (this.currentObjectsNode != null)
                {
                    this.treeViewObjects.SelectedNode = this.currentObjectsNode;
                }
                else
                {
                    this.treeViewObjects.SelectedNode = TreeViewUtils.FindNodeByTag(this.treeViewObjects, null);
                }
                this.treeViewObjects.EndUpdate();
            }
            if (this.treeView3dModel.SelectedNode != null && this.treeView3dModel.SelectedNode.Tag != null)
            {
                this.treeView3dModel.BeginUpdate();
                if (this.treeView3dModel.SelectedNode.Parent != null)
                {
                    this.treeView3dModel.SelectedNode = this.treeView3dModel.SelectedNode.Parent;
                }
                else if (this.current3dModelNode != null)
                {
                    this.treeView3dModel.SelectedNode = this.current3dModelNode;
                }
                else
                {
                    this.treeView3dModel.SelectedNode = TreeViewUtils.FindNodeByTag(this.treeView3dModel, null);
                }
                this.treeView3dModel.EndUpdate();
            }
            OnSelectedItemChange();
        }
        private void TreeView_Objects_OnAfterSelect(object obj, TreeViewEventArgs treeViewEventArgs)
        {
            if (tabControl.SelectedTab == tabPageObjects)
                OnSelectedItemChange();
        }

        private void OnSelectedItemChange()
        {
            if (this.SelectedItemChange == null)
                return;
            SelectedItemChange(this);
        }

        private void TreeView_3dModel_OnAfterSelect(object obj, TreeViewEventArgs treeViewEventArgs)
        {
            if (this.tabControl.SelectedTab == this.tabPage3dModel)
                OnSelectedItemChange();
        }
 
        private void LocalizationTranslate()
        {
            base.TabText = ToolsLocalization.Translate("EntityTypesForm", base.TabText);
            foreach (ToolStripItem toolStripItem in this.XTS.Items)
            {
                toolStripItem.ToolTipText = ToolsLocalization.Translate("EntityTypesForm", toolStripItem.ToolTipText);
            }
            foreach (TabPage tabPage in this.tabControl.TabPages)
            {
                tabPage.Text = ToolsLocalization.Translate("EntityTypesForm", tabPage.Text);
            }
        } 

        private void TreeView_Objects_OnKeyDown(object obj, KeyEventArgs keyEventArgs)
        {
            if ((keyEventArgs.KeyCode == Keys.Return || keyEventArgs.KeyCode == Keys.Space) && this.treeViewObjects.SelectedNode != null && this.treeViewObjects.SelectedNode.Nodes.Count > 0)
            {
                if (this.treeViewObjects.SelectedNode.IsExpanded)
                {
                    this.treeViewObjects.SelectedNode.Collapse();
                }
                else
                {
                    this.treeViewObjects.SelectedNode.Expand();
                }
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
                return;
            }
            if (keyEventArgs.KeyCode == Keys.Return || keyEventArgs.KeyCode == Keys.Escape)
            {
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
            }
        }

        private void TreeView_3dModel_OnKeyDown(object obj, KeyEventArgs keyEventArgs)
        {
            if ((keyEventArgs.KeyCode == Keys.Return || keyEventArgs.KeyCode == Keys.Space) && this.treeView3dModel.SelectedNode != null && this.treeView3dModel.SelectedNode.Nodes.Count > 0)
            {
                if (this.treeView3dModel.SelectedNode.IsExpanded)
                {
                    this.treeView3dModel.SelectedNode.Collapse();
                }
                else
                {
                    this.treeView3dModel.SelectedNode.Expand();
                }
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
                return;
            }
            if (keyEventArgs.KeyCode == Keys.Return || keyEventArgs.KeyCode == Keys.Escape)
            {
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
            }
        }

        private void TabControl_OnSelectedIndexChanged(object obj, System.EventArgs eventArgs)
        {
            OnSelectedItemChange();
        }

        public void NeedUpdateObjectsTree()
        {
            this.objectTreeModified = true;
        }

        public void NeedUpdateMeshesTree()
        {
            this.meshTreeModified = true;
        }

        private void Timer1_Tick(object obj, System.EventArgs eventArgs)
        {
            if (this.Blq)
            {
                tabControl.Visible = true;
                treeView3dModel.Select();
                treeView3dModel.Focus();
                tabControl.Select();
                treeViewObjects.Select();
                treeViewObjects.Focus();
            }
            if (objectTreeModified)
            {
                UpdateTreeViewObjects();
                objectTreeModified = false;
            }
            if (meshTreeModified)
            {
                UpdateTreeView3dModel();
                meshTreeModified = false;
            }
            bool flag = TreeViewUtils.IsVScrollVisible(this.treeViewObjects);
            if (this.Blq)
            {
                this.BlR = flag;
            }
            if (flag != this.BlR)
            {
                this.BlR = flag;
                this.treeViewObjects.Invalidate(true);
            } 
            this.Blq = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((long)msg.Msg == 256L)
            {
                Keys keys = keyData & Keys.KeyCode;
                Keys modifiers = keyData & ~keys;
                if (MainForm.Instance.ToolsProcessKeyDownHotKeys(keys, modifiers, false))
                {
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private Rectangle GetHelpButtonBounds(TreeNode treeNode)
        {
            Point location = new Point(this.treeViewObjects.ClientRectangle.Width - 25, treeNode.Bounds.Top);
            return new Rectangle(location, new Size(16, 16));
        }

        private void TreeView_Objects_OnDrawTreeNode(object obj, DrawTreeNodeEventArgs drawTreeNodeEventArgs)
        {
            Font font = drawTreeNodeEventArgs.Node.NodeFont;
            if (font == null)
                font = drawTreeNodeEventArgs.Node.TreeView.Font;

            TreeNode node = drawTreeNodeEventArgs.Node;
            Rectangle bounds = drawTreeNodeEventArgs.Bounds;

            bool nodeSelected = (drawTreeNodeEventArgs.State & TreeNodeStates.Selected) != (TreeNodeStates)0;
            if (nodeSelected)
                drawTreeNodeEventArgs.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);

            Brush brush = nodeSelected ? SystemBrushes.HighlightText : SystemBrushes.WindowText; 
            bounds.Size = new Size((int)drawTreeNodeEventArgs.Graphics.MeasureString(node.Text, font).Width + 2, bounds.Size.Height);
 
            if (node.Bounds.Height != 0)
            {
                drawTreeNodeEventArgs.Graphics.DrawString(node.Text, font, brush, bounds);
            }

            EntityType entityType = node.Tag as EntityType;
            if (entityType != null)
            {
                bool flag2;
                if (!this.Blr.TryGetValue(entityType, out flag2))
                {
                    string helpButtonLink = GetHelpButtonLink(entityType);
                    flag2 = !string.IsNullOrEmpty(helpButtonLink);
                    this.Blr.Add(entityType, flag2);
                }
                if (flag2)
                {
                    int x = this.GetHelpButtonBounds(node).Left - 4;
                    Rectangle rect = new Rectangle(new Point(x, node.Bounds.Top), node.Bounds.Size);
                    drawTreeNodeEventArgs.Graphics.FillRectangle(SystemBrushes.Window, rect); 
                    drawTreeNodeEventArgs.Graphics.DrawImage(Resources.help_10, this.GetHelpButtonBounds(node).Location);
                }
            }
        }

        private string GetHelpButtonLink(EntityType entityType)
        {
            return "entityType";
        }

        private void TreeView_Objects_OnMouseDown(object obj, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                TreeNode nodeAt = this.treeViewObjects.GetNodeAt(mouseEventArgs.Location);
                if (nodeAt != null && this.GetHelpButtonBounds(nodeAt).Contains(mouseEventArgs.Location))
                {
                    EntityType entityType = nodeAt.Tag as EntityType;
                    if (entityType != null)
                    {
                        string helpButtonLink = GetHelpButtonLink(entityType);
                        if (!string.IsNullOrEmpty(helpButtonLink))
                        {
                            Log.Info(">> Click on {0} Help Button Link", entityType);
                        }
                    }
                }
            }
        }
        private void InsertEntityType(EntityType entityType)
        {
            string entityDirectory = "";
            if (!string.IsNullOrEmpty(entityType.FilePath))
            {
                entityDirectory = Path.GetDirectoryName(entityType.FilePath);
            }
            if (entityDirectory != "")
            {
                TreeNode treeNode = Find3dModelByPath(entityDirectory);
                if (treeNode == null)
                    treeNode = FindObjectByPath(entityDirectory);

                TreeNode treeNode2 = new TreeNode(entityType.FullName, 1, 1);
                treeNode.Nodes.Add(treeNode2);
                treeNode2.Tag = entityType;
                return;
            }

            TreeNode treeNode3 = new TreeNode(entityType.FullName, 1, 1);
            treeViewObjects.Nodes.Add(treeNode3);
            treeNode3.Tag = entityType;
        }

        private TreeNode FindNodeByPath(TreeNodeCollection treeNodeCollection, string b)
        {
            foreach (TreeNode treeNode in treeNodeCollection)
            {
                if (treeNode.Tag == null && treeNode.Text == b)
                {
                    return treeNode;
                }
            }
            return null;
        }
        private TreeNode Find3dModelByPath(string text)
        {
            string[] PathNameList = text.Split("\\/".ToCharArray());
            Trace.Assert(PathNameList.Length != 0, "listPath.Length != 0");
            TreeNode treeNode = null;
            for (int i = 0; i < PathNameList.Length; i++)
            {
                string PathName = PathNameList[i];
                if (treeNode == null)
                {
                    treeNode = FindNodeByPath(treeViewObjects.Nodes, PathName);
                }
                else
                {
                    treeNode = FindNodeByPath(treeNode.Nodes, PathName);
                }
                if (treeNode == null)
                    return null;
            }
            return treeNode;
        }

        private TreeNode FindObjectByPath(string text)
        {
            string[] array = text.Split("\\/".ToCharArray());
            Trace.Assert(array.Length != 0, "listPath.Length != 0");
            TreeNode treeNode = null;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text2 = array2[i];
                TreeNode treeNode2;
                if (treeNode == null)
                {
                    treeNode2 = FindNodeByPath(this.treeViewObjects.Nodes, text2);
                }
                else
                {
                    treeNode2 = FindNodeByPath(treeNode.Nodes, text2);
                }
                if (treeNode2 == null)
                {
                    treeNode2 = new TreeNode(text2, 0, 0);
                    if (treeNode == null)
                    {
                        this.treeViewObjects.Nodes.Add(treeNode2);
                    }
                    else
                    {
                        treeNode.Nodes.Add(treeNode2);
                    }
                }
                treeNode = treeNode2;
            }
            return treeNode;
        }

        private void enumerateTreeNodes_objects()
        {
            TreeViewUtils.EnumerateAllNodesDelegate enumerateAllNodesDelegate =
                new TreeViewUtils.EnumerateAllNodesDelegate(enumerateAllNodes); ;
            while (!TreeViewUtils.EnumerateAllNodes(treeViewObjects, enumerateAllNodesDelegate)) ;
        }

        private EntityType GetCurrentEntityType()
        {
            TreeNode selectedNode = this.treeViewObjects.SelectedNode;
            if (selectedNode == null)
            {
                return null;
            }
            return selectedNode.Tag as EntityType;
        }
 
        private bool enumerateAllNodes(TreeNode treeNode)
        {
            if (treeNode.Nodes.Count == 1 && treeNode.Nodes[0].Tag != null)
            {
                TreeNode treeNode2 = treeNode.Nodes[0];
                TreeNode treeNode3 = new TreeNode(treeNode2.Text, treeNode2.ImageIndex, treeNode2.SelectedImageIndex);
                treeNode3.Tag = treeNode2.Tag;
                if (treeNode.Parent == null)
                {
                    this.treeViewObjects.Nodes.Add(treeNode3);
                }
                else
                {
                    treeNode.Parent.Nodes.Add(treeNode3);
                }
                treeNode.Remove();
                return false;
            }
            return true;
        }

        private void EntityTypesForm_Load(object sender, EventArgs e)
        {
            imageCache = new ImageCache(IL16);
            tsbRefresh.Image = imageCache["refresh"];

            this.treeViewObjects.ImageList = IL16_treeView;
            this.treeView3dModel.ImageList = IL16_treeView;


            this.treeViewObjects.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeViewObjects.DrawNode += TreeView_Objects_OnDrawTreeNode;
            this.treeViewObjects.MouseDown += TreeView_Objects_OnMouseDown;
            this.treeViewObjects.KeyDown += TreeView_Objects_OnKeyDown;
            this.treeViewObjects.AfterSelect += TreeView_Objects_OnAfterSelect;

            //this.treeView3dModel.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView3dModel.KeyDown += TreeView_3dModel_OnKeyDown;
            this.treeView3dModel.AfterSelect += TreeView_3dModel_OnAfterSelect;

            UpdateTrees();

            LocalizationTranslate();
            Timer1.Start();
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            UpdateTrees();
        }
    }
}