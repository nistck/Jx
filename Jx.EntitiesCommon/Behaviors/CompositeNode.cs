using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing.Design;

using Jx;
using Jx.EntitySystem;
using Jx.FileSystem;

namespace Jx.EntitiesCommon.Behaviors
{
    public abstract class CompositeNodeType : DeciderNodeType
    { 
        public class ChildNodeItem
        {
            [FieldSerialize]
            private BehaviorTreeNodeType nodeType;

            [JxName("节点类型")]
            public BehaviorTreeNodeType NodeType
            {
                get { return nodeType; }
                set { this.nodeType = value; }
            }

            public override bool Equals(object obj)
            {
                ChildNodeItem cni = obj as ChildNodeItem;
                if (cni == null || cni.NodeType == null || NodeType == null)
                    return false;

                return cni.NodeType.Equals(NodeType);
            }

            public override int GetHashCode()
            {
                return NodeType == null ? 0 : NodeType.GetHashCode();
            }

            public override string ToString()
            {
                if (NodeType == null)
                    return "<未选择>";

                string text = string.Format("类型: {0}", NodeType);
                return text;
            }
        }

        [FieldSerialize]
        private List<ChildNodeItem> children = new List<ChildNodeItem>();

        [JxName("子集合")] 
        [TypeConverter(typeof(CollectionTypeConverter))]
        [Editor("Jx.EntitiesCommon.Editors.Behaviors.CompositeNodeType_ChildNodeItemCollectionEditor, Jx.EntitiesCommon.Editors", typeof(UITypeEditor))]
        public List<ChildNodeItem> Children
        {
            get {
                return children;
            }
        }

        protected bool CheckCycle(ChildNodeItem cni, ref List<CompositeNodeType> L)
        {
            CompositeNodeType cnt = cni.NodeType as CompositeNodeType;
            if (cnt == null)
                return false;

            if (L.Contains(cnt) )
                return true;

            foreach (ChildNodeItem c in cnt.Children)
            {
                bool bx = CheckCycle(c, ref L);
                if (bx)
                {
                    L.Add(c.NodeType as CompositeNodeType);
                    return true;
                }
            }
            return false;
        }

        public void CheckCycle()
        {
            List<CompositeNodeType> L = new List<CompositeNodeType>();
            L.Add(this);

            for (int i = children.Count - 1; i >= 0; i--)
            {
                ChildNodeItem cni = children[i];

                if (cni == null || cni.NodeType == null || CheckCycle(cni, ref L))
                {
                    L.Add(cni.NodeType as CompositeNodeType);
                    children.RemoveAt(i);
                    continue;
                }
            }
        } 
    }

    public abstract class CompositeNode : DeciderNode
    {
        private CompositeNodeType _type = null;
        public new CompositeNodeType Type { get { return _type; } }
    }
}
