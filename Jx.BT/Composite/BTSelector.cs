using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    /// <summary> 
    /// 选择节点: (1) 子节点返回Failed，继续执行下一个节点； (2) 所有节点返回Failed，则返回Failed; (3) 子节点返回Running，则返回Running。
    /// </summary>
    [BTProperty("选择", BTConstants.GROUP_COMPOSITE)]
    public class BTSelector : BTComposite
    {
        private int _activeChildIndex = -1;
        private int _previousSuccessChildIndex = -1;
         
        public int activeChildIndex { get { return _activeChildIndex; } }

        protected override BTResult OnTick(BTContext context)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                BTNode child = m_Children[i];

                BTResult result = child.Tick_(context); 
                switch (result.Code)
                {
                    case BTResultCode.Running:
                        if (_activeChildIndex != i && _activeChildIndex != -1)
                        {
                            m_Children[_activeChildIndex].Reset();
                        }
                        _activeChildIndex = i;
                        _previousSuccessChildIndex = -1;
                        Running = true;
                        return BTResult.Running;

                    case BTResultCode.Success:
                        if (_activeChildIndex != i && _activeChildIndex != -1)
                        {
                            m_Children[_activeChildIndex].Reset();
                        }
                        child.Reset();
                        _activeChildIndex = -1;
                        _previousSuccessChildIndex = i;
                        Running = false;
                        return BTResult.Success;

                    case BTResultCode.Failed:
                        child.Reset();
                        continue;
                }
            }

            _activeChildIndex = -1;
            _previousSuccessChildIndex = -1;
            Running = false;
            return BTResult.Failed;
        }

        public override void Reset()
        {
            base.Reset();

            _activeChildIndex = -1;
            _previousSuccessChildIndex = -1;
        }
    }
}
