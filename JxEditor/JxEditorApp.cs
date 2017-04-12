using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.Editors;
using Jx.EntitySystem;
using JxEditor.Types;

namespace JxEditor
{
    class JxEditorApp : EngineApp
    {
        protected override bool OnCreate()
        {
            Log.Info(">> 初始化 EntitySystemWorld...");
            if (!EntitySystemWorld.Init(new EntitySystemWorld()))
            {
                Log.Info(">> EntitySystemWorld 初始化失败!");
                return false;
            }

            WorldType worldType = EntitySystemWorld.Instance.DefaultWorldType;
            if (!EntitySystemWorld.Instance.WorldCreate(WorldSimulationTypes.Editor, worldType))
            {
                Log.Info(">> 创建世界失败, WorldType: {0}", worldType);
                return false;
            }
            InitResourceTypeManager();

            return true;
        }

        private void InitResourceTypeManager()
        {
            ResourceTypeManager.Init();

            ResourceTypeManager.Instance.Register(new EntityTypeResourceType("EntityType", "Entity Type", new string[]
            {
                "type"
            }, DefaultResourceTypeImages.GetByName("Config_16")));

            ResourceTypeManager.Instance.Register(new ConfigurationResourceType("Configuration", "Configuration File", new string[]
            {
                "config"
            }, DefaultResourceTypeImages.GetByName("Config_16")));


        }
    }
}
