using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Jx.Editors
{
    public static class TreeViewUtils
    {
        public delegate bool EnumerateAllNodesDelegate(TreeNode node);
        private const int GWL_STYLE = -16;
        private const int WS_VSCROLL = 2097152;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        public static void ExpandAllPathToNode(TreeNode node)
        {
            for (TreeNode treeNode = node; treeNode != null; treeNode = treeNode.Parent)
            {
                treeNode.Expand();
            }
        }
        public static TreeNode GetNodeByFullPath(TreeView treeView, string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string[] array = fullPath.Split("\\/".ToCharArray());
            TreeNode treeNode = null;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string key = array2[i];
                if (treeNode == null)
                {
                    treeNode = treeView.Nodes[key];
                }
                else
                {
                    treeNode = treeNode.Nodes[key];
                }
                if (treeNode == null)
                {
                    return null;
                }
            }
            return treeNode;
        }
        public static TreeNode FindNodeByText(TreeView treeView, string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string[] array = fullPath.Split("\\/".ToCharArray());
            TreeNode treeNode = null;
            for (int i = 0; i < array.Length; i++)
            {
                string b = array[i];
                TreeNodeCollection treeNodeCollection = (treeNode != null) ? treeNode.Nodes : treeView.Nodes;
                treeNode = null;
                foreach (TreeNode treeNode2 in treeNodeCollection)
                {
                    if (treeNode2.Text == b)
                    {
                        treeNode = treeNode2;
                        break;
                    }
                }
                if (treeNode == null)
                {
                    return null;
                }
            }
            return treeNode;
        }
        private static TreeNode FindNodeByTagRecursive(TreeNode parentNode, object tag)
        {
            if (parentNode.Tag == tag)
            {
                return parentNode;
            }
            foreach (TreeNode parentNode2 in parentNode.Nodes)
            {
                TreeNode treeNode = TreeViewUtils.FindNodeByTagRecursive(parentNode2, tag);
                if (treeNode != null)
                {
                    return treeNode;
                }
            }
            return null;
        }
        public static TreeNode FindNodeByTag(TreeView treeView, object tag)
        {
            foreach (TreeNode parentNode in treeView.Nodes)
            {
                TreeNode treeNode = TreeViewUtils.FindNodeByTagRecursive(parentNode, tag);
                if (treeNode != null)
                {
                    return treeNode;
                }
            }
            return null;
        }
        public static TreeNode GetNeedSelectNodeAfterRemoveNode(TreeNode node)
        {
            TreeNode parent = node.Parent;
            int num = 0;
            while (num < parent.Nodes.Count && parent.Nodes[num] != node)
            {
                num++;
            }
            if (num + 1 < parent.Nodes.Count)
            {
                return parent.Nodes[num + 1];
            }
            if (num - 1 >= 0)
            {
                return parent.Nodes[num - 1];
            }
            return parent;
        }
        private static bool EnumerateAllNodes(TreeNode node, TreeViewUtils.EnumerateAllNodesDelegate callback)
        {
            if (!callback(node))
            {
                return false;
            }
            foreach (TreeNode node2 in node.Nodes)
            {
                if (!TreeViewUtils.EnumerateAllNodes(node2, callback))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool EnumerateAllNodes(TreeView treeView, TreeViewUtils.EnumerateAllNodesDelegate callback)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                if (!TreeViewUtils.EnumerateAllNodes(node, callback))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsVScrollVisible(TreeView treeView)
        {
            int windowLong = TreeViewUtils.GetWindowLong(treeView.Handle, -16);
            return (windowLong & 2097152) != 0;
        }
    }

}
