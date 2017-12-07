using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Engine
{
    public interface IUpdatable
    { 
        void Update(ITickEvent tickEvent);
    }
}
