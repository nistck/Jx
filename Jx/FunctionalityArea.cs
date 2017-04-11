using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public class FunctionalityArea
    {

        protected virtual void OnTick(float delta)
        {
        }

        protected virtual void OnDestroy()
        {
        }

        public void DoTick(float delta)
        {
            this.OnTick(delta);
        }

        public void DoDestroy()
        {
            this.OnDestroy();
        }
    }
}
