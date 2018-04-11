using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.BT
{
    [Name("BT环境")]
    public class BTContextType : EntityType
    {

    }

    [Name("BT环境")]
    public class BTContext : Entity
    {
        private BTContextType _type = null; 
        public new BTContextType Type { get { return _type; } }

        private BTTree tree;

        private BTDatabase db;

        private readonly Dictionary<string, BTResult> nodeResultDic = new Dictionary<string, BTResult>();
        private readonly List<WeakReference<BTNode>> travelNodes = new List<WeakReference<BTNode>>(); 

 
        /// <summary>
        /// 当前上下文，行为树本身
        /// </summary>
        public BTTree Tree
        {
            get { return tree; }
            private set { this.tree = value; }
        } 

        public BTDatabase Database
        {
            get {
                if (db == null)
                    db = new BTDatabase(); 
                return db;
            }
            set { this.db = value; }
        }

        public void Travel(BTNode node)
        {
            if (node == null)
                return; 

            travelNodes.Add(new WeakReference<BTNode>(node)); 
        }

        public List<BTNode> GetTravelNodes()
        {
            List<BTNode> result = new List<BTNode>();

            var q = travelNodes.Select(_x =>
            {
                BTNode _nd = null;
                _x.TryGetTarget(out _nd);
                return _nd;
            }).Where(_x => _x != null);
            result.AddRange(q); 

            return result; 
        }

        public virtual void OnSessionStart()
        {
            ClearNodeResults();
            travelNodes.Clear(); 
        }

        public virtual void OnSessionEnd()
        {

        }

        #region Session Node Result
        public void ClearNodeResults()
        {
            nodeResultDic.Clear(); 
        }

        public void SetNodeResult(BTNode node, BTResult result)
        {
            if (node == null || result == null)
                return;

            nodeResultDic[node.Id ?? ""] = result; 
        }

        public BTResult GetNodeResult(BTNode node)
        {
            if (node == null)
                return null;

            BTResult r = GetNodeResult(node.Id); 
            return r;
        }

        public BTResult GetNodeResult(string nodeId)
        {
            if (nodeId == null || !nodeResultDic.ContainsKey(nodeId))
                return null; 

            BTResult r = nodeResultDic[nodeId];
            return r;
        }
        #endregion
    }
}
