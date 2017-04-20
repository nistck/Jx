using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx.FileSystem;

namespace Jx.Ext
{
    public static class ToolsLocalization
    {
        public class GroupItem
        {
            internal string eM;
            internal Dictionary<string, string> em = new Dictionary<string, string>();
            public string Name
            {
                get
                {
                    return this.eM;
                }
            }
            public ICollection<KeyValuePair<string, string>> Values
            {
                get
                {
                    return new ReadOnlyICollection<KeyValuePair<string, string>>(this.em);
                }
            }
            internal GroupItem(string text)
            {
                this.eM = text;
            }
        }
        private static bool Ek;
        private static string EL;
        private static bool El;
        private static Dictionary<string, ToolsLocalization.GroupItem> EM;
        public static bool IsInitialized
        {
            get
            {
                return ToolsLocalization.Ek;
            }
        }
        public static bool NewKeysWasAdded
        {
            get
            {
                return ToolsLocalization.El;
            }
        }
        public static ICollection<ToolsLocalization.GroupItem> Groups
        {
            get
            {
                return new ReadOnlyICollection<ToolsLocalization.GroupItem>(ToolsLocalization.EM.Values);
            }
        }
        public static void Init(string fileName)
        {
            if (ToolsLocalization.Ek)
            {
                Log.Fatal("ToolsLocalization: Init: Already initialized.");
            }
            if (VirtualFile.Exists(fileName))
            {
                ToolsLocalization.Ek = true;
                ToolsLocalization.EL = fileName;
                ToolsLocalization.EM = new Dictionary<string, ToolsLocalization.GroupItem>();
                ToolsLocalization.El = false;
                ToolsLocalization.A();
            }
        }
        private static void A()
        {
            TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(ToolsLocalization.EL);
            if (textBlock != null)
            {
                TextBlock textBlock2 = textBlock.FindChild("groups");
                if (textBlock2 != null)
                {
                    foreach (TextBlock current in textBlock2.Children)
                    {
                        if (!(current.Name != "group"))
                        {
                            string data = current.Data;
                            ToolsLocalization.GroupItem groupItem = new ToolsLocalization.GroupItem(data);
                            ToolsLocalization.EM.Add(data, groupItem);
                            foreach (TextBlock.Attribute current2 in current.Attributes)
                            {
                                groupItem.em.Add(current2.Name, current2.Value);
                            }
                        }
                    }
                }
            }
        }
        public static void Save()
        {
            if (!ToolsLocalization.IsInitialized)
            {
                return;
            }
            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(ToolsLocalization.EL);
            TextBlock textBlock = new TextBlock();
            TextBlock textBlock2 = textBlock.AddChild("groups");
            foreach (ToolsLocalization.GroupItem current in ToolsLocalization.EM.Values)
            {
                TextBlock textBlock3 = textBlock2.AddChild("group", current.eM);
                foreach (KeyValuePair<string, string> current2 in current.em)
                {
                    textBlock3.SetAttribute(current2.Key, current2.Value);
                }
            }
            using (StreamWriter streamWriter = new StreamWriter(realPathByVirtual))
            {
                streamWriter.Write(textBlock.DumpToString());
            }
        }
        private static ToolsLocalization.GroupItem A(string text)
        {
            ToolsLocalization.GroupItem groupItem;
            if (ToolsLocalization.EM.TryGetValue(text, out groupItem))
            {
                return groupItem;
            }
            groupItem = new ToolsLocalization.GroupItem(text);
            ToolsLocalization.EM.Add(text, groupItem);
            return groupItem;
        }
        public static string Translate(string groupName, string text)
        {
            if (ToolsLocalization.IsInitialized && !string.IsNullOrEmpty(text))
            {
                ToolsLocalization.GroupItem groupItem = ToolsLocalization.A(groupName);
                if (groupItem != null)
                {
                    string text2;
                    if (groupItem.em.TryGetValue(text, out text2))
                    {
                        if (!string.IsNullOrEmpty(text2))
                        {
                            return text2;
                        }
                    }
                    else
                    {
                        ToolsLocalization.El = true;
                        groupItem.em.Add(text, "");
                    }
                }
            }
            return text;
        }
    }
}
