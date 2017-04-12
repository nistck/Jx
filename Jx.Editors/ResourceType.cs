using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms; 

namespace Jx.Editors
{
    public class ResourceType
    {
        private string name;
        private string displayName;
        private string[] extensions;
        private Image icon;

        public string Name
        {
            get
            {
                return this.name;
            }
        }
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }
        public string[] Extensions
        {
            get
            {
                return this.extensions;
            }
        }
        public Image Icon
        {
            get
            {
                return this.icon;
            }
        }
        public virtual Type ResourceObjectEditorType
        {
            get
            {
                return null;
            }
        }
        public virtual bool AllowNewResource
        {
            get
            {
                return false;
            }
        }
        public ResourceType(string name, string displayName, string[] extensions, Image icon)
        {
            this.name = name;
            this.displayName = displayName;
            this.extensions = extensions;
            this.icon = icon;
        }
        public override string ToString()
        {
            return this.name;
        }
        protected virtual void OnNewResource(string directory)
        {
        }
        public void DoNewResource(string directory)
        {
            this.OnNewResource(directory);
        }
        protected virtual bool OnLoadResource(string path)
        {
            return true;
        }
        public bool DoLoadResource(string path)
        {
            return this.OnLoadResource(path);
        }
        protected virtual bool OnUnloadResource(string path)
        {
            return true;
        }
        public bool DoUnloadResource(string path)
        {
            return this.OnUnloadResource(path);
        }
        public virtual bool OnResourceRenamed(string path, string oldPath)
        {
            return true;
        }
        public virtual bool IsSpecialRenameResourceMode()
        {
            return false;
        }
        protected virtual string OnUserRenameResource(string path)
        {
            return null;
        }
        public string DoUserRenameResource(string path)
        {
            return this.OnUserRenameResource(path);
        }
        protected virtual bool OnOutsideAddResource(string path)
        {
            return true;
        }
        public bool DoOutsideAddResource(string path)
        {
            return this.OnOutsideAddResource(path);
        }
        protected virtual void OnResourcesTreeContextMenu(string path, ContextMenuStrip menu)
        {
        }
        public void DoResourcesTreeContextMenu(string path, ContextMenuStrip menu)
        {
            this.OnResourcesTreeContextMenu(path, menu);
        }
    }
}
