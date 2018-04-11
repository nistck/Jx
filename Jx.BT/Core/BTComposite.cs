using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public abstract class BTComposite : BTNode
    {
        protected List<BTNode> m_Children = new List<BTNode>();

        public BTComposite()
            : base()
        { 
        }
 
        public void AddChildren(params BTNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; ++i)
            {
                AddChild(nodes[i]);
            }
        }


        public void AddChild(BTNode node)
        {
            if (node != null && !m_Children.Contains(node))
            {
                node.Parent = this;
                m_Children.Add(node);
            }
        }


        public void InsertChild(int index, BTNode child)
        {
            if (child != null && m_Children.Contains(child))
            {
                child.Parent = this; 
                m_Children.Insert(index, child);
            }
        }


        public void RemoveChild(BTNode child)
        {
            if (child != null)
            {
                child.Parent = null; 
                m_Children.Remove(child);
            }
        }


        public void RemoveChild(int index)
        {
            if (index >= 0 && index < m_Children.Count)
            {
                m_Children[index].Parent = null; 
                m_Children.RemoveAt(index);
            }
        }


        public void RemoveAllChildren()
        {
            m_Children.Any(_x => {
                _x.Parent = null; 
                return false; 
            });
            m_Children.Clear();
        }


        public List<BTNode> Children
        {
            get {
                return m_Children;
            }
        } 

        public BTNode GetChildAt(int i)
        {
            return m_Children[i];
        }


        public int GetIndex(BTNode node)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                if (m_Children[i] == node)
                    return i;
            }
            return -1;
        }


        public void SortChildren(System.Comparison<BTNode> comparison)
        {
            m_Children.Sort(comparison);
        }
    }
}
