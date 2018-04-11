using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    /// <summary>
    /// 顺序节点: (1) 子节点返回Success，继续执行下一个节点; (2) 所有节点返回Success，则返回Success； (3) 节点返回Running，则返回Running。
    /// </summary>
    [BTProperty("顺序", BTConstants.GROUP_COMPOSITE)]
    public class BTSequence : BTComposite
    { 
        private int _activeChildIndex;

        public int activeChildIndex { get { return _activeChildIndex; } }

        protected override BTResult OnTick(BTContext context)
        {
            if (_activeChildIndex == -1)
                _activeChildIndex = 0;

            for (; _activeChildIndex < m_Children.Count; _activeChildIndex++)
            {
                BTNode activeChild = m_Children[_activeChildIndex];

                BTResult result = activeChild.Tick_(context); 
                switch (result.Code)
                {
                    case BTResultCode.Running:
                        Running = true;
                        return BTResult.Running;
                    case BTResultCode.Success:
                        activeChild.Reset();
                        continue;
                    case BTResultCode.Failed:
                        activeChild.Reset();
                        _activeChildIndex = -1;
                        Running = false;
                        return BTResult.Failed;
                }
            }

            _activeChildIndex = -1;
            Running = false;
            return BTResult.Success;
        }

        public override void Reset()
        {
            base.Reset();
            _activeChildIndex = -1; 
        }
    }
}
