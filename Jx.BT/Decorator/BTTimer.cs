using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    [BTProperty("定时", BTConstants.GROUP_DECORATOR)]
    public class BTTimer : BTDecorator
    {
        private long _timer = 0;

        /// <summary>
        /// 单位: 毫秒
        /// </summary>
        public float interval { get; set; }


        public BTTimer() : this(0.0f) { }

        public BTTimer(float interval, BTNode child = null) 
            : base(child)
        {
            this.interval = interval;
        }

        protected override BTResult OnTick(BTContext context)
        {
            if (_timer == 0)
                _timer = DateTime.Now.Ticks;

            long ts = (DateTime.Now.Ticks - _timer) / 10000;
            if (ts >= interval)
            {
                _timer = DateTime.Now.Ticks;
                BTResult result = m_Child.Tick_(context);
                return result;
            }
            else
            {
                return BTResult.Running;
            }
        }

        public override void Reset()
        {
            base.Reset();
            _timer = 0;
        }

        public override string ToString()
        {
            string text = string.Format("间隔{0}ms", interval);
            return text;
        }
    }
}
