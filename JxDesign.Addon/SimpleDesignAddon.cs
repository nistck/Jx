using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Jx;
using Jx.Editors;

namespace JxDesign.Addon
{
    public class SimpleDesignAddon : DesignerAddon
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
    }
}
