using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jx.Ext
{
    public static class ResourceUtils
    {
        public class ResourceUITypeEditorEditValueEventHandler
        {
            private string eQ;
            private string eq;
            private Predicate<string> eR;
            private bool er;
            private bool eS;
            public string ResourceTypeName
            {
                get
                {
                    return this.eQ;
                }
                set
                {
                    this.eQ = value;
                }
            }
            public string ResourceName
            {
                get
                {
                    return this.eq;
                }
                set
                {
                    this.eq = value;
                }
            }
            public Predicate<string> ShouldAddDelegate
            {
                get
                {
                    return this.eR;
                }
                set
                {
                    this.eR = value;
                }
            }
            public bool SupportRelativePath
            {
                get
                {
                    return this.er;
                }
                set
                {
                    this.er = value;
                }
            }
            public bool Modified
            {
                get
                {
                    return this.eS;
                }
                set
                {
                    this.eS = value;
                }
            }
            public ResourceUITypeEditorEditValueEventHandler(string resourceTypeName, string resourceName, Predicate<string> shouldAddDelegate, bool supportRelativePath)
            {
                this.ResourceTypeName = resourceTypeName;
                this.eq = resourceName;
                this.eR = shouldAddDelegate;
                this.er = supportRelativePath;
            }
        }
        public delegate void OnUITypeEditorEditValueDelegate(ResourceUtils.ResourceUITypeEditorEditValueEventHandler e);
        private static ResourceUtils.OnUITypeEditorEditValueDelegate EP;
        public static event ResourceUtils.OnUITypeEditorEditValueDelegate OnUITypeEditorEditValue
        {
            add
            {
                ResourceUtils.OnUITypeEditorEditValueDelegate onUITypeEditorEditValueDelegate = ResourceUtils.EP;
                ResourceUtils.OnUITypeEditorEditValueDelegate onUITypeEditorEditValueDelegate2;
                do
                {
                    onUITypeEditorEditValueDelegate2 = onUITypeEditorEditValueDelegate;
                    ResourceUtils.OnUITypeEditorEditValueDelegate value2 = (ResourceUtils.OnUITypeEditorEditValueDelegate)Delegate.Combine(onUITypeEditorEditValueDelegate2, value);
                    onUITypeEditorEditValueDelegate = Interlocked.CompareExchange<ResourceUtils.OnUITypeEditorEditValueDelegate>(ref ResourceUtils.EP, value2, onUITypeEditorEditValueDelegate2);
                }
                while (onUITypeEditorEditValueDelegate != onUITypeEditorEditValueDelegate2);
            }
            remove
            {
                ResourceUtils.OnUITypeEditorEditValueDelegate onUITypeEditorEditValueDelegate = ResourceUtils.EP;
                ResourceUtils.OnUITypeEditorEditValueDelegate onUITypeEditorEditValueDelegate2;
                do
                {
                    onUITypeEditorEditValueDelegate2 = onUITypeEditorEditValueDelegate;
                    ResourceUtils.OnUITypeEditorEditValueDelegate value2 = (ResourceUtils.OnUITypeEditorEditValueDelegate)Delegate.Remove(onUITypeEditorEditValueDelegate2, value);
                    onUITypeEditorEditValueDelegate = Interlocked.CompareExchange<ResourceUtils.OnUITypeEditorEditValueDelegate>(ref ResourceUtils.EP, value2, onUITypeEditorEditValueDelegate2);
                }
                while (onUITypeEditorEditValueDelegate != onUITypeEditorEditValueDelegate2);
            }
        }
        public static bool DoUITypeEditorEditValueDelegate(string resourceTypeName, ref string resourceName, Predicate<string> shouldAddDelegate, bool supportRelativePath)
        {
            if (ResourceUtils.EP == null)
            {
                return false;
            }
            ResourceUtils.ResourceUITypeEditorEditValueEventHandler resourceUITypeEditorEditValueEventHandler = new ResourceUtils.ResourceUITypeEditorEditValueEventHandler(resourceTypeName, resourceName, shouldAddDelegate, supportRelativePath);
            ResourceUtils.EP(resourceUITypeEditorEditValueEventHandler);
            if (resourceUITypeEditorEditValueEventHandler.Modified)
            {
                resourceName = resourceUITypeEditorEditValueEventHandler.ResourceName;
                return true;
            }
            return false;
        }
    }
}
