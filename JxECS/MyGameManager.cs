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
    public class MyGameManager : BaseGameManager
    {
        MyGameManager()
        {
            
        } 


        public static MyGameManager New()
        { 
            MyGameManager gm = new MyGameManager();               
            return gm;
        }
    }
}
