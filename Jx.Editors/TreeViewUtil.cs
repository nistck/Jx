using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Jx.Ext
{
    public delegate bool EnumerateNodesDelegate(TreeNode node);
    public static class TreeViewUtil
    {        
        private const int GWL_STYLE = -16;
        private const int WS_VSCROLL = 2097152;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static void ExpandTo(TreeNode node)
        {
            for (TreeNode treeNode = node; treeNode != null; treeNode = treeNode.Parent)
            {
                treeNode.Expand();
            }
        }

        public static TreeNode GetNodeByFullPath(TreeView treeView, string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) 
                return null;
   
            string[] array = fullPath.Split("\\/".ToCharArray());
            TreeNode treeNode = null; 
            for (int i = 0; i < array.Length; i++)
            {
                string key = array[i];
                if (treeNode == null) 
                    treeNode = treeView.Nodes[key];
                else
                    treeNode = treeNode.Nodes[key];

                if (treeNode == null)
                    return null;
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
 
        public static TreeNode FindNodeByTag(TreeNode node, object tag)
        {
            if (tag == null || node == null)
                return null;

            List<TreeNode> nodes = new List<TreeNode>();
            nodes.Add(node);

            while (nodes.Count > 0)
            {
                TreeNode tn = nodes[0];
                nodes.RemoveAt(0);

                if (tn.Tag == tag)
                    return tn;

                TreeNode[] tns = new TreeNode[tn.Nodes.Count];
                tn.Nodes.CopyTo(tns, 0);
                nodes.AddRange(tns.ToList());
            }
            return null;
        }

        public static TreeNode FindNodeByTag(TreeView treeView, object tag)
        {
            foreach (TreeNode parentNode in treeView.Nodes)
            {
                TreeNode treeNode = FindNodeByTag(parentNode, tag);
                if (treeNode != null)
                    return treeNode;
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

        public static bool EnumerateNodes(TreeNode node, EnumerateNodesDelegate callback)
        {
            if (node == null || callback == null)
                return false; 

            if (!callback(node))
                return false;
            
            foreach (TreeNode node2 in node.Nodes)
            {
                if (!EnumerateNodes(node2, callback))
                    return false;
            }
            return true;
        }
        public static bool EnumerateNodes(TreeView treeView, EnumerateNodesDelegate callback)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                if (!EnumerateNodes(node, callback))
                    return false;
            }
            return true;
        }
        public static bool IsVScrollVisible(TreeView treeView)
        {
            int windowLong = GetWindowLong(treeView.Handle, -16);
            return (windowLong & 2097152) != 0;
        }
    }

}
