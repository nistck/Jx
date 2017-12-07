using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.Engine;
using Jx.Engine.Game;
using Jx.Engine.System;
using Jx.Engine.Entity;
using Jx.Engine.Collections;

namespace JxECS
{
    public class GravitySystem : BaseSystem
    {
        public GravitySystem(EntityIncludeMatcher matcher) 
            : base(matcher)
        {
            this.R = new Random();
        } 

        private Random R { get; set; }

        protected override void OnUpdate(ITickEvent tickEvent) 
        { 
            int xdelta = R.Next(-10, 10);
            int ydelta = R.Next(-10, 10);
 
            foreach(var entity in this)
            {
                Position p = entity.GetComponent<Position>();
                p.X += xdelta;
                p.Y += ydelta;
            }
        }
    }
}
