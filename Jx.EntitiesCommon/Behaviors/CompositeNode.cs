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
                set { 
                    /*
                    if( value is CompositeNodeType )
                    {
                        CompositeNodeType cnt = value as CompositeNodeType;

                        List<CompositeNodeType> L = new List<CompositeNodeType>(); 
                        var q = cnt.Children.Select(_nx => _nx.NodeType).OfType<CompositeNodeType>().ToList();
                        L.AddRange(q);
                        while( L.Count > 0 )
                        {
                            CompositeNodeType nt = L[0];
                            L.RemoveAt(0);
                            
                            // 当前值 的 孩子
                            List<CompositeNodeType> Lc = nt.Children.Select(_nx => _nx.NodeType).OfType<CompositeNodeType>().ToList();
                            if (Lc.Contains(cnt))
                                throw new Exception("循环引用");
                            L.AddRange(Lc);
                        }
                    }
                    //*/
                    this.nodeType = value;
                }
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
         
    }

    public abstract class CompositeNode : DeciderNode
    {
        private CompositeNodeType _type = null;
        public new CompositeNodeType Type { get { return _type; } }
    }
}
