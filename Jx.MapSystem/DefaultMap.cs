using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx.MapSystem
{
    [ManualTypeCreate]
    public sealed class DefaultMapType : MapType
    {
    }

    public sealed class DefaultMap : Map
    {
        private DefaultMapType _type = null;
        public new DefaultMapType Type { get { return _type; } }

        [FieldSerialize]
        [DefaultValue("-")]
        private string dateCreated;
 
        protected override void OnCreate()
        {
            base.OnCreate(); 
            this.dateCreated = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }
    }
}
