using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    /// <summary>
    /// 并行节点
    /// </summary>
    [BTProperty("并行", BTConstants.GROUP_COMPOSITE)]
    public class BTParallel : BTComposite
    {
        private bool m_failOnAny;
        private bool m_succeedOnAny;
        private bool m_failOnTie;

        public BTParallel()
            : base()
        {
            m_failOnAny = true;
            m_succeedOnAny = false;
            m_failOnTie = true;
        }

        /// <summary>
        /// 任一失败
        /// </summary>
        public bool FailOnAny
        {
            get { return m_failOnAny; }
            set { m_failOnAny = value; }
        }

        /// <summary>
        /// 任一成功
        /// </summary>
        public bool SucceedOnAny
        {
            get { return m_succeedOnAny; }
            set { m_succeedOnAny = value; }
        }

        /// <summary>
        /// 是否是优先失败?
        /// </summary>
        public bool FailOnTie
        {
            get { return m_failOnTie; }
            set { m_failOnTie = value; }
        }

        protected override BTResult OnTick(BTContext context)
        {
            if (m_Children == null || m_Children.Count == 0)
                return BTResult.Success;

            BTResult status = BTResult.Success;
            int numberOfFailures = 0;
            int numberOfSuccesses = 0;
            int numberOfRunning = 0;

            for (int i = 0; i < m_Children.Count; i++)
            {
                BTResult childStatus = m_Children[i].Tick_(context);
 
                if (childStatus == BTResult.Success)
                    numberOfSuccesses++;
                else if (childStatus == BTResult.Failed)
                    numberOfFailures++;
                else if (childStatus == BTResult.Running)
                    numberOfRunning++;  
            }


            if ((m_failOnAny && numberOfFailures > 0) || (m_succeedOnAny && numberOfSuccesses > 0))
            {
                if (m_failOnTie)
                {
                    if (m_failOnAny && numberOfFailures > 0)
                        status = BTResult.Failed;
                    else if (m_succeedOnAny && numberOfSuccesses > 0)
                        status = BTResult.Success;
                }
                else
                {
                    if (m_succeedOnAny && numberOfSuccesses > 0)
                        status = BTResult.Success;
                    else if (m_failOnAny && numberOfFailures > 0)
                        status = BTResult.Failed;
                }
            }
            else
            {
                if (numberOfSuccesses == m_Children.Count)
                    status = BTResult.Success;
                else if (numberOfFailures == m_Children.Count)
                    status = BTResult.Failed;
                else
                    status = BTResult.Running;
            }

            return status;
        }
    }
}
