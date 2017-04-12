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

            WorldType worldType = EntitySystemWorld.Instance.DefaultWorldType;
            if ( !EntitySystemWorld.Instance.WorldCreate(WorldSimulationTypes.Editor, worldType) )
            {
                Log.Info(">> 创建世界失败, WorldType: {0}", worldType);
                return false;
            }

            return true;
        }
    }
}
