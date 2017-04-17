using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using Jx;
using Jx.Editors;

namespace JxRes.Addon
{
    public class SimpleResAddon : ResourceEditorAddon
    {
        public override bool OnInit(out string mainMenuItemText, out Image mainMenuItemIcon)
        {
            mainMenuItemText = "SimpleAddon";
            mainMenuItemIcon = null;
            return true;
        }

        public override void OnMainMenuItemClick()
        {
            Log.Info(">> SimpleAddon Main Menu Click!");
        }

        public override void OnShowContextMenuOfResourcesTree(ContextMenuStrip menu)
        {
        }

        public override void OnShowOptionsDialog(/*OptionsDialog*/object dialog)
        {
        }
    }
}
