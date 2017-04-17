using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;
using Jx.FileSystem;

namespace Jx.EntitiesCommon
{
    [JxName("简单Bean")]
    public class SimpleBeanType : EntityType
    {
        [FieldSerialize]
        private int id;
        [FieldSerialize]
        private string scope; 

        [JxName("Id")]
        public int Id
        {
            get { return id; }
            set { this.id = value; }
        }

        [JxName("作用域")]
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

    public class SimpleBean : Entity
    {
        private SimpleBeanType _type = null; 
        public new SimpleBeanType Type { get { return _type; } }

        protected override bool OnLoad(TextBlock block)
        { 
            return base.OnLoad(block);
        }

        
    }
}
