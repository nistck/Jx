﻿using System;
using System.IO; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.Ext;
using Jx.FileSystem;

namespace Jx.Editors
{
    public partial class ChooseResourceForm : Form
    {
        private class NodeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode treeNode = (TreeNode)x;
                TreeNode treeNode2 = (TreeNode)y;
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


        private ResourceType resourceType;
        private bool allowChooseNull;
        private Predicate<string> shouldAddDelegate;
        private bool supportRelativePath;
        private TreeNode rootNode;
        private TreeNode nullValueNode;

        private static string currentHelperDirectoryName;
        [Config("ChooseResourceForm", "allowRelativePath")]
        public static bool allowRelativePath = true; 


        public static string CurrentHelperDirectoryName
        {
            get { return currentHelperDirectoryName; }
            set { currentHelperDirectoryName = value; }
        }

        public ChooseResourceForm(
            ResourceType resourceType, bool allowChooseNull, Predicate<string> shouldAddDelegate, string currentPath, bool supportRelativePath)
        {
            InitializeComponent();

            this.nullValueNode = null;
            this.resourceType = resourceType;
            this.allowChooseNull = allowChooseNull;
            this.shouldAddDelegate = shouldAddDelegate;
            this.supportRelativePath = supportRelativePath;
            this.checkBoxAllowRelativePath.Enabled = (supportRelativePath && CurrentHelperDirectoryName != null);
            if (this.checkBoxAllowRelativePath.Enabled)
            {
                this.checkBoxAllowRelativePath.Checked = ChooseResourceForm.allowRelativePath;
            }
            string currentPath2 = RelativePathUtils.ConvertToFullPath(ChooseResourceForm.CurrentHelperDirectoryName, currentPath);
            this.UpdateData(currentPath2);
        }

        public string FilePath
        {
            get {
                string filePath = GetNodePath(treeView.SelectedNode);
                return filePath;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }


        private void UpdateData(string currentPath)
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();
            while (this.imageListTreeView.Images.Count > 2)
            {
                this.imageListTreeView.Images.RemoveAt(2);
            }
            foreach (ResourceType current in ResourceTypeManager.Instance.Types)
            {
                Image icon = current.Icon;
                if (icon != null)
                {
                    int count = this.imageListTreeView.Images.Count;
                    this.imageListTreeView.Images.Add(icon);
                    icon.Tag = count;
                }
            }
            this.rootNode = new TreeNode("Data", 0, 0);
            this.treeView.Nodes.Add(rootNode);
            this.UpdateDataDirectory("", rootNode);
            this.treeView.TreeViewNodeSorter = new NodeComparer();
            this.treeView.Sort();
            this.rootNode.Expand();
            if (this.rootNode.Nodes.Count == 1)
            {
                this.rootNode.Nodes[0].Expand();
            }
            bool flag = false;
            if (!string.IsNullOrEmpty(currentPath))
            {
                TreeNode nodeByPath = this.GetNodeByPath(currentPath);
                if (nodeByPath != null)
                {
                    TreeViewUtil.ExpandTo(nodeByPath);
                    this.treeView.SelectedNode = nodeByPath;
                    flag = true;
                }
            }
            if (!flag && string.IsNullOrEmpty(currentPath) && !string.IsNullOrEmpty(currentHelperDirectoryName))
            {
                TreeNode nodeByPath2 = GetNodeByPath(currentHelperDirectoryName);
                if (nodeByPath2 != null)
                {
                    TreeViewUtil.ExpandTo(nodeByPath2);
                    treeView.SelectedNode = nodeByPath2;
                    flag = true;
                }
            }
            if (this.allowChooseNull)
            {
                this.nullValueNode = new TreeNode(ToolsLocalization.Translate("ChooseResourceForm", "(Null)"), 1, 1);
                this.nullValueNode.Name = nullValueNode.Text;
                this.treeView.Nodes.Add(nullValueNode);
                if (string.IsNullOrEmpty(currentPath) && !flag)
                {
                    this.treeView.SelectedNode = nullValueNode;
                    flag = true;
                }
            }
            if (!flag && treeView.Nodes.Count != 0)
            {
                this.treeView.SelectedNode = treeView.Nodes[0];
            }
            this.treeView.EndUpdate();
        }

        private void UpdateDataDirectory(string path, TreeNode parentNode)
        {
            string[] directories = VirtualDirectory.GetDirectories(path); 
            for (int i = 0; i < directories.Length; i++)
            {
                string path2 = directories[i];
                string fileName = Path.GetFileName(path2);
                TreeNode treeNode = new TreeNode(fileName, 0, 0);
                treeNode.Name = treeNode.Text;
                parentNode.Nodes.Add(treeNode);
                UpdateDataDirectory(path2, treeNode);
                if (treeNode.Nodes.Count == 0)
                    treeNode.Remove();
            }

            string searchPattern;
            if (resourceType != null && resourceType.Extensions.Length == 1)
                searchPattern = "*." + resourceType.Extensions[0];
            else
                searchPattern = "*";
            
            string[] files = VirtualDirectory.GetFiles(path, searchPattern); 
            for (int j = 0; j < files.Length; j++)
            {
                string file = files[j];
                if (shouldAddDelegate == null || shouldAddDelegate(file))
                {
                    if (resourceType != null && resourceType.Extensions.Length != 1)
                    {
                        string ext = Path.GetExtension(file);
                        if (ext.Length > 0)
                            ext = ext.Substring(1);

                        bool resourceFound = resourceType.Extensions.Where(_x => _x != null).Select(_s => _s.ToLower()).Where(_s => _s == ext).Count() > 0;
                        if (!resourceFound)
                            continue;
                    }
                    UpdateAddResource(parentNode, file);
                }
            }
        }

        private void UpdateAddResource(TreeNode parentNode, string fileName)
        {
            string fileName2 = Path.GetFileName(fileName);
            int num = 1;
            string text = Path.GetExtension(fileName2);
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Substring(1);
                ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text);
                if (byExtension != null && byExtension.Icon != null)
                {
                    num = (int)byExtension.Icon.Tag;
                }
            }
            TreeNode treeNode = new TreeNode(fileName2, num, num);
            treeNode.Name = treeNode.Text;
            treeNode.Tag = fileName;
            parentNode.Nodes.Add(treeNode);
        }

        private TreeNode GetNodeByPath(string path)
        {
            if (path == null || path == "")
            {
                return this.rootNode;
            }
            string[] array = path.Split("\\/".ToCharArray());
            TreeNode treeNode = this.rootNode;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string key = array2[i];
                treeNode = treeNode.Nodes[key];
                if (treeNode == null)
                {
                    return null;
                }
            }
            return treeNode;
        }

        private string GetNodePath(TreeNode node)
        {
            string text = "";
            for (TreeNode treeNode = node; treeNode != null; treeNode = treeNode.Parent)
            {
                if (treeNode.Parent != null)
                {
                    if (text != "")
                    {
                        text = treeNode.Text + "\\" + text;
                    }
                    else
                    {
                        text = treeNode.Text;
                    }
                }
            }
            return text;
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != this.treeView.SelectedNode)
            {
                return;
            }
            if (this.buttonConfirm.Enabled)
            {
                base.DialogResult = DialogResult.OK;
                base.Close();
            }
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && this.buttonConfirm.Enabled)
            {
                base.DialogResult = DialogResult.OK;
                base.Close();
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                this.buttonConfirm.Enabled = false;
                return;
            }
            if (this.allowChooseNull && e.Node == this.nullValueNode)
            {
                this.buttonConfirm.Enabled = true;
                return;
            }
            this.buttonConfirm.Enabled = (e.Node.Tag != null);
        }

        private void ChooseResourceForm_Load(object sender, EventArgs e)
        {
 
        }
 
        private string Translate(string text)
        {
            return ToolsLocalization.Translate("ChooseResourceForm", text);
        }
 
        private void checkBoxAllowRelativePath_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAllowRelativePath.Enabled)
            {
                allowRelativePath = this.checkBoxAllowRelativePath.Checked;
            }
        }

        public void UpdateFonts(string fontForm, string fontTreeControl)
        {
            if (!string.IsNullOrEmpty(fontForm) && fontForm[0] != '(')
            {
                try
                {
                    FontConverter fontConverter = new FontConverter();
                    this.Font = (Font)fontConverter.ConvertFromString(fontForm);
                }
                catch
                {
                }
            }
            if (!string.IsNullOrEmpty(fontTreeControl) && fontTreeControl[0] != '(')
            {
                try
                {
                    FontConverter fontConverter2 = new FontConverter();
                    this.treeView.Font = (Font)fontConverter2.ConvertFromString(fontTreeControl);
                }
                catch
                {
                }
            }
        }
    }
}
