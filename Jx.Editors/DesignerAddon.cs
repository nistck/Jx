using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Jx.Editors
{
    public abstract class DesignerAddon
    {
        public class ApplicationDataClass
        {
            internal Form mainForm;
            internal MenuStrip mainMenu;
            internal ToolStrip toolbar;
            internal DockPanel dockPanel;
            public Form MainForm
            {
                get
                {
                    return this.mainForm;
                }
            }
            public MenuStrip MainMenu
            {
                get
                {
                    return this.mainMenu;
                }
            }
            public ToolStrip Toolbar
            {
                get
                {
                    return this.toolbar;
                }
            }
            public DockPanel DockPanel
            {
                get
                {
                    return this.dockPanel;
                }
            }
        }
        private static ApplicationDataClass applicationData = new ApplicationDataClass();
        public static ApplicationDataClass ApplicationData
        {
            get
            {
                return applicationData;
            }
        }
        public static void Internal_InitApplicationData(Form mainForm, MenuStrip mainMenu, ToolStrip toolbar, DockPanel dockPanel)
        {
            applicationData.mainForm = mainForm;
            applicationData.mainMenu = mainMenu;
            applicationData.toolbar = toolbar;
            applicationData.dockPanel = dockPanel;
        }
        public virtual DockContent OnCreateDockingWindowAtLoading(string windowTypeName)
        {
            return null;
        }
        public abstract bool OnInit(out string mainMenuItemText, out Image mainMenuItemIcon);
        public virtual void OnMainMenuItemClick()
        {
        }
        public virtual void OnShowContextMenuOfWorkingArea(ContextMenuStrip menu)
        {
        }
        public virtual void OnShowContextMenuOfLayer(ContextMenuStrip menu)
        {
        }
        /*
        public virtual void OnShowOptionsDialog(OptionsDialog dialog)
        {
        }
        public virtual void OnUpdateCameraSettings(Camera camera)
        {
        }
        public virtual void OnRender(Camera camera)
        {
        }
        public virtual void OnRenderScreenUI(GuiRenderer renderer)
        {
        }
        //*/
    }
}
