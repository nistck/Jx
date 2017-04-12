using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Jx.Editors
{
    public class ResourceTypeManager
    {
        private static ResourceTypeManager instance;
        private List<ResourceType> types = new List<ResourceType>();
        private Dictionary<string, ResourceType> typesNameDictionary = new Dictionary<string, ResourceType>();
        private Dictionary<string, ResourceType> typesExtensionDictionary = new Dictionary<string, ResourceType>();

        public static ResourceTypeManager Instance
        {
            get
            {
                return ResourceTypeManager.instance;
            }
        }

        public ReadOnlyCollection<ResourceType> Types
        {
            get
            {
                return this.types.AsReadOnly();
            }
        }

        public static void Init()
        {
            Trace.Assert(ResourceTypeManager.instance == null);
            ResourceTypeManager.instance = new ResourceTypeManager();
            ResourceTypeManager.instance.InitInternal();
        }

        public static void Shutdown()
        {
            if (ResourceTypeManager.instance != null)
            {
                ResourceTypeManager.instance.ShutdownInternal();
                ResourceTypeManager.instance = null;
            }
        }

        private void InitInternal()
        {
        }

        private void ShutdownInternal()
        {
        }

        public void Register(ResourceType type)
        {
            ResourceType resourceType;
            if (this.typesNameDictionary.TryGetValue(type.Name, out resourceType))
            {
                Log.Fatal("Resource type manager: Name \"{0}\" already registered \"{1}\"", type.Name, type.ToString());
                return;
            }
            string[] extensions = type.Extensions;
            for (int i = 0; i < extensions.Length; i++)
            {
                string text = extensions[i];
                if (this.typesExtensionDictionary.TryGetValue(text.ToLower(), out resourceType))
                {
                    Log.Fatal("Resource type manager: Extension \"{0}\" already registered \"{1}\"", text, type.ToString());
                    return;
                }
            }
            this.types.Add(type);
            this.typesNameDictionary.Add(type.Name, type);
            string[] extensions2 = type.Extensions;
            for (int j = 0; j < extensions2.Length; j++)
            {
                string text2 = extensions2[j];
                this.typesExtensionDictionary.Add(text2.ToLower(), type);
            }
        }

        public ResourceType GetByName(string name)
        {
            ResourceType result;
            if (!this.typesNameDictionary.TryGetValue(name, out result))
            {
                return null;
            }
            return result;
        }

        public ResourceType GetByExtension(string extension)
        {
            ResourceType result;
            if (!this.typesExtensionDictionary.TryGetValue(extension.ToLower(), out result))
            {
                return null;
            }
            return result;
        }
    }
}
