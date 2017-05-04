using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Reflection;
using System.Drawing;

using Jx;
using Jx.Editors;
using Jx.EntitySystem;
using Jx.FileSystem;

namespace JxRes.UI
{
    public class AddonManager
    {
        private static AddonManager instance;
        private List<ResourceEditorAddon> resourceEditorAddons = new List<ResourceEditorAddon>();
        private Dictionary<ToolStripMenuItem, ResourceEditorAddon> resourceEditorAddonMenuDic = new Dictionary<ToolStripMenuItem, ResourceEditorAddon>();

        public static AddonManager Instance
        {
            get
            {
                return AddonManager.instance;
            }
        }
        public IList<ResourceEditorAddon> Addons
        {
            get
            {
                return this.resourceEditorAddons.AsReadOnly();
            }
        }

        public static bool PreInit()
        {
            if (AddonManager.instance != null)
            {
                Log.Fatal("AddonManager: Init: The currentEditor is already initialized.");
                return false;
            }
            AddonManager.instance = new AddonManager();
            if (!AddonManager.instance.PreInitInternal())
            {
                AddonManager.Shutdown();
                return false;
            }
            return true;
        }

        public static void Shutdown()
        {
            AddonManager.instance = null;
        }

        public bool PreInitInternal()
        {
            ResourceEditorAddon.Internal_InitApplicationData(MainForm.Instance, MainForm.Instance.MainMenu, MainForm.Instance.ToolStripGeneral, MainForm.Instance.DockPanel);
            List<Assembly> list = new List<Assembly>();

            ComponentManager.ComponentInfo[] componentsByType = ComponentManager.Instance.GetComponentsByType(ComponentManager.ComponentTypeFlags.ResourceEditorAddon, true);
            for (int i = 0; i < componentsByType.Length; i++)
            {
                ComponentManager.ComponentInfo componentInfo = componentsByType[i];
                ComponentManager.ComponentInfo.PathInfo[] allEntryPointsForThisPlatform = componentInfo.GetAllEntryPointsForThisPlatform();
                for (int j = 0; j < allEntryPointsForThisPlatform.Length; j++)
                {
                    ComponentManager.ComponentInfo.PathInfo pathInfo = allEntryPointsForThisPlatform[j];
                    Assembly item = AssemblyUtils.LoadAssemblyByRealFileName(pathInfo.Path, false);
                    if (item != null && !list.Contains(item))
                        list.Add(item);
                }
            }


            foreach (Assembly current in list)
            {
                Type[] types = current.GetTypes();
                for (int k = 0; k < types.Length; k++)
                {
                    Type type = types[k];
                    if (typeof(ResourceEditorAddon).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                        ResourceEditorAddon item2 = (ResourceEditorAddon)constructor.Invoke(null);
                        resourceEditorAddons.Add(item2);
                    }
                }
            }
            return true;
        }

        public DockContent CreateDockingWindowAtLoading(string windowTypeName)
        {
            foreach (ResourceEditorAddon current in this.resourceEditorAddons)
            {
                DockContent dockContent = current.OnCreateDockingWindowAtLoading(windowTypeName);
                if (dockContent != null)
                {
                    return dockContent;
                }
            }
            return null;
        }

        public bool PostInit()
        {
            foreach (ResourceEditorAddon current in this.resourceEditorAddons)
            {
                string text;
                Image image;
                if (!current.OnInit(out text, out image))
                    return false;
                
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(text, image, AddonsMainMenu_Click);
                MainForm.Instance.AddonsToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
                this.resourceEditorAddonMenuDic.Add(toolStripMenuItem, current);
            }
            return true;
        }

        private void AddonsMainMenu_Click(object obj, EventArgs eventArgs)
        {
            ToolStripMenuItem key = (ToolStripMenuItem)obj;
            ResourceEditorAddon resourceEditorAddon = this.resourceEditorAddonMenuDic[key];
            resourceEditorAddon.OnMainMenuItemClick();
        }
    }

}
