using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    /// <summary>
    /// 随机节点
    /// </summary>
    [BTProperty("随机", BTConstants.GROUP_COMPOSITE)]
    public class BTRandom : BTComposite
    {
        private int lastSelectedIndex = -1; 

        public BTRandom()
        {
        }

        public int LastSelectedIndex
        {
            get { return lastSelectedIndex; }
        }

        protected override BTResult OnTick(BTContext context)
        {
            if (m_Children == null || m_Children.Count == 0)
                return BTResult.Failed;

            int index = SelectChild();
            BTResult result = m_Children[index].Tick_(context);
            //m_Children[index].Reset();
            lastSelectedIndex = index; 
            return result;
        }

        protected virtual int SelectChild()
        {
            int index = BTUtility.RandomInt(0, m_Children.Count - 1);
            return index; 
        }
    }
}
