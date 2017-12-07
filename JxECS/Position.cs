using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.Engine.Component;

namespace JxECS
{
    public class Position : BaseComponent
    {
        public int X { get; set; }
        public int Y { get; set; }
 
        public override void Reset()
        {
            X = 0;
            Y = 0;
        }
    }
}
