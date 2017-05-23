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
    }
}
