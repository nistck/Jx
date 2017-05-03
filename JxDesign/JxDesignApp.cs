using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx;
using Jx.Editors;
using Jx.EntitySystem;
using Jx.Ext;
using Jx.FileSystem; 
using Jx.UI.Editors;

using JxDesign.UI;

namespace JxDesign
{
    internal class JxDesignApp : EngineApp
    {
        private static JxDesignApp instance;

        public new static JxDesignApp Instance
        {
            get { return instance; }
        }
 
        protected override bool OnCreate()
        {
            if (!base.OnCreate())
                return false; 

            LongOperationNotifier.Setup();

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

            DesignerInterface.Init(new DesignerInterfaceImpl()); 
            if (MainForm.Instance != null)
            {
                /*
                if (MainForm.Instance.ResourcesForm != null)
                {
                    MainForm.Instance.ResourcesForm.ResourceChange += new ResourcesForm.ResourceChangeDelegate(this.OnResourceChange);
                    MainForm.Instance.ResourcesForm.IsResourceEditModeActive += new ResourcesForm.IsResourceEditModeActiveDelegate(this.OnIsResourceEditMode);
                    MainForm.Instance.ResourcesForm.ResourceBeginEditMode += new ResourcesForm.ResourceBeginEditModeDelegate(this.OnResourceBeginEditMode);
                    MainForm.Instance.ResourcesForm.ResourceRename += new ResourcesForm.ResourceRenameDelegate(this.OnResourceRename);
                }
                //*/
                if( MainForm.Instance.PropertiesForm != null)
                {
                    MainForm.Instance.PropertiesForm.ReadOnly = true;
                }
            }
            ResourceUtils.OnUITypeEditorEditValue += ResourceUtils_OnUITypeEditorEditValue;

            AddonManager.PreInit();
            InitResourceTypeManager();

            UndoSystem.Init(64); 

            instance = this;
            return true;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            LongOperationNotifier.Shutdown();
        }

        private void ResourceUtils_OnUITypeEditorEditValue(ResourceUtils.ResourceUITypeEditorEditValueEventHandler e)
        {
            ResourceType byName = ResourceTypeManager.Instance.GetByName(e.ResourceTypeName);
            if (byName == null)
            {
                Log.Fatal("Resource type is not defined \"{0}\"", e.ResourceTypeName);
                return;
            }
   
            ChooseResourceForm chooseResourceForm = new ChooseResourceForm(byName, true, e.ShouldAddDelegate, e.ResourceName, e.SupportRelativePath);
            if (chooseResourceForm.ShowDialog() == DialogResult.OK)
            {
                e.ResourceName = chooseResourceForm.FilePath;
                e.Modified = true;
            } 
        } 

        private void InitResourceTypeManager()
        {
            ResourceTypeManager.Init();

            ResourceTypeManager.Instance.Register(new ResourceType("EntityType", "Entity Type", new string[]
            {
                "type"
            }, DefaultResourceTypeImages.GetByName("Config_16")));

            ResourceTypeManager.Instance.Register(new ResourceType("Configuration", "Configuration File", new string[]
            {
                "config"
            }, DefaultResourceTypeImages.GetByName("Config_16")));


        }
    }
}
