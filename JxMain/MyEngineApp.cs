using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.EntitySystem;

namespace Jx
{
    internal class MyEngineApp : EngineApp
    {
        protected override bool OnCreate()
        {
            Log.Info(">> 初始化 EntitySystemWorld...");
            if( !EntitySystemWorld.Init(new EntitySystemWorld()) )
            {
                Log.Info(">> EntitySystemWorld 初始化失败!");
                return false;
            } 

            return true;
        }
    }
}
