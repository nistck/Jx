using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;
using Jx.FileSystem;
using Jx.MapSystem;

namespace Jx.EntitiesCommon
{
    [JxName("简单Bean")]
    public class SimpleBeanType : MapObjectType
    {
        [FieldSerialize]
        private int id;
        [FieldSerialize]
        private string scope;
        [FieldSerialize]
        private uint clock;

        [JxName("Id")]
        [DefaultValue(1981)]
        public int Id
        {
            get { return id; }
            set { this.id = value; }
        }

        [JxName("作用域")]
        [DefaultValue("property")]
        public string Scope
        {
            get { return scope; }
            set { this.scope = value; }
        }

        [Description("时间间隔, 单位: 秒")]
        [JxName("时间间隔")]
        public uint Clock
        {
            get { return clock == 0? 1000 : clock; }
            set { this.clock = value; }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
#if DEBUG_ENTITY
            Log.Info(">> Type {0} Loaded: id = {1}, scope = {2}", this, Id, Scope);
#endif
        }

    }

    [JxName("简单Bean")]
    public class SimpleBean : MapObject
    {
        public enum MethodType
        {
            Get, 
            Post
        }

        private SimpleBeanType _type = null; 
        public new SimpleBeanType Type { get { return _type; } }

        [FieldSerialize]
        private int beanId;
        [FieldSerialize]
        private MethodType method = MethodType.Get;

        private Clock clock = null;

        protected override bool OnLoad(TextBlock block)
        { 
            return base.OnLoad(block);
        }

        [JxName("Bean Id")]
        public int BeanId
        {
            get { return beanId; }
            set { beanId = value; }
        }

        public MethodType Method
        {
            get { return method; }
            set { this.method = value; }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            BeanId = Type.Id;
        }

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);

            clock = new Clock(Type.Clock, this);
            clock.Alarm += Clock_Alarm;
            SubscribeToTickEvent();
        }

        protected override void OnDestroy()
        {
            if (clock != null)
                clock.Alarm -= Clock_Alarm;

            base.OnDestroy();
        }

        private void Clock_Alarm(object state, Clock clock)
        {
            Log.Debug(">> OnTick: {0}, {1}, {2}", EngineApp.Instance.Time, this.Name, this.UIN);
        }

        protected override void OnTick()
        {
            base.OnTick();
            clock.Tick(TickDelta);            
        }
    }
}
