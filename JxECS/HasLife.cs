using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.Engine;
using Jx.Engine.Component;

namespace JxECS
{
    public class HasLife : BaseComponent, IUpdatable
    {
        public int Health { get; set; }

        public override IComponent Clone()
        {
            return MemberwiseClone() as HasLife;
        }

        public override void Reset()
        {
            Health = 0;
        }

        public void Update(ITickEvent tickEvent)
        {
            Health++;
            if (Health > 100)
                Health = 100;
        }
    }
}
