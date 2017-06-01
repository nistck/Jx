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
        private static readonly List<WeakReference<Clock>> clocksKnown = new List<WeakReference<Clock>>(); 

        internal static void Tick() 
        { 
            lock(clocksKnown)
            {
                for(int i = clocksKnown.Count - 1; i >= 0; i --)
                {
                    WeakReference<Clock> r = clocksKnown[i];

                    Clock item = null;
                    if (r.TryGetTarget(out item))
                    {
                        item._Tick();
                    }
                    else
                        clocksKnown.RemoveAt(i);
                }
            }
        }

        public static Clock New(uint ticks, object state = null)
        {
            Clock c = new Clock(ticks, state);
            return c;
        }
 
        public event AlarmHandler Alarm;

        private uint alarmCount = 0;
        private float tick = 0;

        Clock(uint ticks, object state = null)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Ticks = ticks;
            this.State = state;

            lock (clocksKnown)
            {
                clocksKnown.Add(new WeakReference<Clock>(this));
            }
        }

        ~Clock()
        {
            lock (clocksKnown)
            {
                for (int i = clocksKnown.Count - 1; i >= 0; i--)
                {
                    WeakReference<Clock> r = clocksKnown[i];

                    Clock item = null;
                    if (r.TryGetTarget(out item))
                    {
                        if (item == this)
                            clocksKnown.RemoveAt(i);
                    }
                    else
                        clocksKnown.RemoveAt(i);
                }
            }
        }

        public string Id { get; private set; }

        /// <summary>
        /// 超时间隔， 单位: 次数
        /// </summary>
        public uint Ticks { get; private set; }
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
            this.Ticks = 0;
        }

        /// <summary>
        /// 闹钟Tick
        /// </summary> 
        private void _Tick( )
        {
            tick++;
            if( Ticks > 0 && tick >= Ticks )
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

        public override bool Equals(object obj)
        {
            Clock c = obj as Clock;
            if (c == null)
                return false;
            return c.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
