using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public delegate void AlarmHandler(object state, Clock clock);

    /// <summary>
    /// 闹钟
    /// </summary>
    public class Clock
    {
        public event AlarmHandler Alarm;

        private uint alarmCount = 0;
        private float tick = 0;

        public Clock(uint timeout, object state = null)
        {
            this.Timeout = timeout;
            this.State = state;
        }
        /// <summary>
        /// 超时间隔， 单位: 毫秒
        /// </summary>
        public uint Timeout { get; private set; }
        /// <summary>
        /// 状态
        /// </summary>
        public object State { get; private set; }
        /// <summary>
        /// 提醒次数
        /// </summary>
        public uint AlarmCount
        {
            get { return alarmCount; }
        }
        /// <summary>
        /// 禁止闹钟提醒
        /// </summary>
        public void Disable()
        {
            this.Timeout = 0;
        }
        /// <summary>
        /// 闹钟Tick
        /// </summary>
        /// <param name="delta"></param>
        public void Tick(float delta)
        {
            tick += delta;
            if( Timeout > 0 && tick >= Timeout )
            {
                tick = 0;
                alarmCount++;
                OnAlarm();
            }
        }

        private void OnAlarm()
        {
            if (Alarm == null)
                return;
            try
            {
                Alarm(State, this);
            }
            catch { }
        }
    }
}
