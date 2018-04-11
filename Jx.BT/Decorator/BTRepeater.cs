using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    [BTProperty("重复", BTConstants.GROUP_DECORATOR)]
    public class BTRepeater : BTDecorator
    {

        /// <summary>
		/// 重复次数。0 -> 无限
		/// </summary>
		public int count { get; set; }

        /// <summary>
        /// Should end the repetition if the child return failure.
        /// </summary>
        public bool endOnFailure { get; set; }

        private int currentCount;
        public int CurrentCount
        {
            get { return currentCount; }
        }

        public BTRepeater(int count = 0, bool endOnFailure = true, BTNode child = null)
            : base(child)
        {
            this.count = count < 0 ? 0 : count;
            this.endOnFailure = endOnFailure;
        }

        public BTRepeater(int count = 0, BTNode child = null)
            : this(count, true, child)
        { 
        }

        public BTRepeater(BTNode child = null)
            : this(0, child)
        {
        }

        public BTRepeater() : this(null) { }
        
        protected override BTResult OnTick(BTContext context)
        {
            if (count == 0)
            {
                BTResult result = m_Child.Tick_(context);

                if (endOnFailure && result == BTResult.Failed)
                {
                    return BTResult.Failed;
                }

                Running = true;
                return BTResult.Running;
            }
            else if (currentCount < count)
            {
                BTResult result = m_Child.Tick_(context);
                currentCount++;

                if (currentCount >= count)
                {
                    if (result == BTResult.Running)
                    {
                        result = BTResult.Success;
                    }
                    return result;
                }

                if (endOnFailure && result == BTResult.Failed)
                {
                    return BTResult.Failed;
                }
                Running = true;
                return BTResult.Running;
            }

            return BTResult.Failed;
        }

        public override void Reset()
        {
            base.Reset(); 
        }

        public override string ToString()
        {
            string text = string.Format("重复{0}{1}, 当前: {2}",
                count == 0 ? "无限" : "" + count, endOnFailure ? ", 遇异常退出" : "", CurrentCount);
            return text;
        }
    }
}
