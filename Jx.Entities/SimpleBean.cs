using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.Entities
{
    public class SimpleBeanType : EntityType
    {
        [FieldSerialize]
        private int id;
        [FieldSerialize]
        private string scope; 

        public int Id
        {
            get { return id; }
            set { this.id = value; }
        }

        public string Scope
        {
            get { return scope; }
            set { this.scope = value; }
        }
    }

    public class SimpleBean : Entity
    {
        private SimpleBeanType _type = null; 
        public new SimpleBeanType Type { get { return _type; } }
    }
}
