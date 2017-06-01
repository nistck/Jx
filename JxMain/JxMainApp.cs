using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Jx.EntitySystem;
using Jx;

namespace JxMain
{
    internal class JxMainApp : JxEngineApp
    {
        public JxMainApp(int loopInterval)
            : base(loopInterval)
        {
        }

        protected override bool OnCreate()
        {
            Log.Info(">> 初始化 EntitySystemWorld...");
            if( !EntitySystemWorld.Init(new EntitySystemWorld()) )
            {
                Log.Info(">> EntitySystemWorld 初始化失败!");
                return false;
            }

            /*
            WorldType worldType = EntitySystemWorld.Instance.DefaultWorldType;
            if ( !EntitySystemWorld.Instance.WorldCreate(WorldSimulationTypes.Editor, worldType) )
            {
                Log.Info(">> 创建世界失败, WorldType: {0}", worldType);
                return false;
            }
            //*/

            EntitySystemWorld.Instance.Simulation = true;
            return true;
        }
    }
}
