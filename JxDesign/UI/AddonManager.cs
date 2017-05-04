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

namespace JxDesign.UI
{
    public class AddonManager
    {
        private static AddonManager instance;
        private List<DesignerAddon> resourceEditorAddons = new List<DesignerAddon>();
        private Dictionary<ToolStripMenuItem, DesignerAddon> designerAddonMenuDic = new Dictionary<ToolStripMenuItem, DesignerAddon>();

        public static AddonManager Instance
        {
            get
            {
                return instance;
            }
        }
        public IList<DesignerAddon> Addons
        {
            get
            {
                return this.resourceEditorAddons.AsReadOnly();
            }
        }

        public static bool PreInit()
        {
            if (instance != null)
            {
                Log.Fatal("AddonManager: Init: The currentEditor is already initialized.");
                return false;
            }
            instance = new AddonManager();
            if (!instance.PreInitInternal())
            {
                Shutdown();
                return false;
            }
            return true;
        }

        public static void Shutdown()
        {
            instance = null;
        }

        public bool PreInitInternal()
        {
            DesignerAddon.Internal_InitApplicationData(MainForm.Instance, MainForm.Instance.MainMenu, MainForm.Instance.ToolStripGeneral, MainForm.Instance.DockPanel);
            List<Assembly> list = new List<Assembly>();

            ComponentManager.ComponentInfo[] componentsByType = ComponentManager.Instance.GetComponentsByType(ComponentManager.ComponentTypeFlags.DesignerAddon, true);
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
                    if (typeof(DesignerAddon).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                        DesignerAddon item2 = (DesignerAddon)constructor.Invoke(null);
                        resourceEditorAddons.Add(item2);
                    }
                }
            }
            return true;
        }

        public DockContent CreateDockingWindowAtLoading(string windowTypeName)
        {
            foreach (DesignerAddon current in this.resourceEditorAddons)
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
            foreach (DesignerAddon current in this.resourceEditorAddons)
            {
                string text;
                Image image;
                if (!current.OnInit(out text, out image))
                    return false;
                
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(text, image, AddonsMainMenu_Click);
                MainForm.Instance.AddonsToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
                this.designerAddonMenuDic.Add(toolStripMenuItem, current);
            }
            return true;
        }

        private void AddonsMainMenu_Click(object obj, EventArgs eventArgs)
        {
            ToolStripMenuItem key = (ToolStripMenuItem)obj;
            DesignerAddon designerAddon = this.designerAddonMenuDic[key];
            designerAddon.OnMainMenuItemClick();
        }
    }

}
