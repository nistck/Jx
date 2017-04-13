using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using Jx;
using Jx.Editors;
using Jx.FileSystem;
using Jx.EntitySystem;
using JxRes.Editors;

namespace JxRes.UI
{
    public partial class ResourcesForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public enum SortByItems
        {
            Name,
            Date,
            Type,
            Size
        }
        public class MyTreeNode : TreeNode
        {
            private bool multiSelected;
            private bool inArchive;
            private bool hideNode;
            private Color foreColor;

            public bool IsArchive
            {
                get
                {
                    return this.inArchive;
                }
            }
            public bool HideNode
            {
                get
                {
                    return this.hideNode;
                }
                set
                {
                    if (this.hideNode == value)
                    {
                        return;
                    }
                    this.hideNode = value;
                    this.UpdateForeColor();
                }
            }
            public bool MultiSelected
            {
                get
                {
                    return this.multiSelected;
                }
            }
            public MyTreeNode(string text, bool inArchive, bool hideNode) 
                : base(text)
            {
                this.inArchive = inArchive;
                this.hideNode = hideNode;
                this.foreColor = base.ForeColor;
                this.UpdateForeColor();
            }

            public void SetMultiSelected(bool select, bool mustRepaint)
            {
                if (this.multiSelected == select)
                    return;

                this.multiSelected = select;
                if (this.multiSelected)
                {
                    if (!MainForm.Instance.ResourcesForm.multiSelectedList.Contains(this))
                        MainForm.Instance.ResourcesForm.multiSelectedList.Add(this);
                }
                else
                {
                    MainForm.Instance.ResourcesForm.multiSelectedList.Remove(this);
                }

                if (mustRepaint && base.TreeView != null)
                {
                    base.TreeView.Invalidate(new Rectangle(0, base.Bounds.Top, 10000, base.Bounds.Size.Height), false);
                }
            }

            private void UpdateForeColor()
            {
                if (this.IsArchive || this.HideNode)
                {
                    base.ForeColor = SystemColors.GrayText;
                    return;
                }
                base.ForeColor = this.foreColor;
            }
        }

        private class TreeNodeComparer : IComparer
        {
            private DateTime GetNodeLastWriteTime(TreeNode tn)
            {
                DateTime dt;
                if (!nodesLastWtDic.TryGetValue(tn, out dt))
                {
                    string virtualPath = ResourcesForm.GetNodePath(tn);
                    string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(virtualPath);
                    try
                    {
                        if (tn.Tag != null)
                            dt = File.GetLastWriteTime(realPathByVirtual);
                        else
                            dt = Directory.GetLastWriteTime(realPathByVirtual);
                    }
                    catch
                    {
                        dt = default(DateTime);
                    }
                    nodesLastWtDic.Add(tn, dt);
                }
                return dt;
            }

            private long GetNodeSize(TreeNode tn)
            {
                long result;
                if (!nodesSizeDic.TryGetValue(tn, out result))
                {
                    string p = ResourcesForm.GetNodePath(tn);
                    string rp = VirtualFileSystem.GetRealPathByVirtual(p);
                    try
                    {
                        using (FileStream fileStream = File.OpenRead(rp))
                        {
                            result = fileStream.Length;
                        }
                    }
                    catch
                    {
                        result = 0L;
                    }
                    nodesSizeDic.Add(tn, result);
                }
                return result;
            }

            public int Compare(object x, object y)
            {
                TreeNode tn1 = (TreeNode)x;
                TreeNode tn2 = (TreeNode)y;
                if (!sortByAscending)
                {
                    TreeNode tn = tn1;
                    tn1 = tn2;
                    tn2 = tn;
                }

                if (tn1.Tag == null && tn2.Tag != null)
                    return -1;

                if (tn1.Tag != null && tn2.Tag == null)
                    return 1;

                switch (sortBy)
                {
                    case SortByItems.Name:
                        return tn1.Text.CompareTo(tn2.Text);
                    case SortByItems.Date:
                        {
                            DateTime dt1 = GetNodeLastWriteTime(tn1); 
                            DateTime dt2 = GetNodeLastWriteTime(tn2); 

                            if (dt1 < dt2)
                                return -1;
                            if (dt1 > dt2)
                                return 1;
                            return 0;
                        }
                    case SortByItems.Type:
                        if (tn1.Tag != null)
                        {
                            string extension = Path.GetExtension(tn1.Text);
                            string extension2 = Path.GetExtension(tn2.Text);
                            return extension.CompareTo(extension2);
                        }
                        return tn1.Text.CompareTo(tn2.Text);
                    case SortByItems.Size:
                        {
                            if (tn1.Tag == null)
                            {
                                return tn1.Text.CompareTo(tn2.Text);
                            }
                            long num = GetNodeSize(tn1); 
                            long num2 = GetNodeSize(tn2); 

                            if (num > num2)
                                return -1;
                            if (num < num2)
                                return 1;
                            return 0;
                        }
                    default:
                        return 0;
                }
            }
        }

        public delegate void ResourceChangeDelegate(string fileName, CancelEventArgs e);
        public delegate void ResourceRenameDelegate(string path, ref bool renameOverrided, ref string newFileName);
        public class ActiveEventArgs : EventArgs
        {
            public bool Active { get; set; }
        }

        public delegate void IsResourceEditModeActiveDelegate(ActiveEventArgs e);
        public delegate void ResourceBeginEditModeDelegate(EventArgs e);

        [Config("ResourcesForm", "hideDirectoriesAndFiles")]
        public static List<string> hideDirectoriesAndFiles = null;
        [Config("ResourcesForm", "sortBy")]
        public static SortByItems sortBy = SortByItems.Name;
        [Config("ResourcesForm", "sortByAscending")]
        public static bool sortByAscending = true;
        private MyTreeNode ResourcesRoot;
        private List<MyTreeNode> multiSelectedList = new List<MyTreeNode>();
        private MyTreeNode aTg;
        private FileSystemWatcher fileSystemWatcher;
        private List<FileSystemEventArgs> fileSystemEvents = new List<FileSystemEventArgs>();
        private static object fileSystemSyncLock = new object();

        /// <summary>
        /// 大小
        /// </summary>
        private static Dictionary<TreeNode, long> nodesSizeDic = new Dictionary<TreeNode, long>();
        /// <summary>
        /// 上次写入时间
        /// </summary>
        private static Dictionary<TreeNode, DateTime> nodesLastWtDic = new Dictionary<TreeNode, DateTime>();
 
        private bool firstRun = true;
        private bool aTL;
         
        private static EventHandler aTQ;

        public event ResourcesForm.ResourceChangeDelegate ResourceChange;
        public event ResourcesForm.ResourceRenameDelegate ResourceRename;
        public event ResourcesForm.IsResourceEditModeActiveDelegate IsResourceEditModeActive;
        public event ResourcesForm.ResourceBeginEditModeDelegate ResourceBeginEditMode;
        
        public ResourcesForm()
        {
            InitializeComponent();

            if (ResourcesForm.hideDirectoriesAndFiles == null)
            {
                ResourcesForm.hideDirectoriesAndFiles = new List<string>();
                ResourcesForm.hideDirectoriesAndFiles.Add("cvs");
                ResourcesForm.hideDirectoriesAndFiles.Add(".svn");
            } 
            this.fileSystemWatcher = new FileSystemWatcher(VirtualFileSystem.ResourceDirectoryPath);
            this.fileSystemWatcher.IncludeSubdirectories = true;
            this.fileSystemWatcher.Created += new FileSystemEventHandler(this.OnFileSystemAction);
            this.fileSystemWatcher.Deleted += new FileSystemEventHandler(this.OnFileSystemAction);
            this.fileSystemWatcher.Renamed += new RenamedEventHandler(this.OnFileSystemAction);
            this.fileSystemWatcher.Changed += new FileSystemEventHandler(this.OnFileSystemAction);
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.ResourcesView.Visible = false;
        }

        public bool WatchFileSystem
        {
            get
            {
                return this.fileSystemWatcher.EnableRaisingEvents;
            }
            set
            {
                this.fileSystemWatcher.EnableRaisingEvents = value;
            }
        }

        public void OnClose()
        {
            if (this.fileSystemWatcher != null)
            {
                this.fileSystemWatcher.Dispose();
                this.fileSystemWatcher = null;
            }
        }

        private void OnFileSystemAction(object obj, FileSystemEventArgs item)
        {
            lock (fileSystemSyncLock)
            {
                fileSystemEvents.Add(item);
#if DEBUG_RES
                Log.Info("File System Event: {0}, {1}", item.ChangeType, item.FullPath);
#endif
            }
        }

        public void UpdateView() 
        {
            ResourcesView.BeginUpdate();
            ResourcesView.Nodes.Clear();
            while (IL16.Images.Count > 2)
                IL16.Images.RemoveAt(2);
            
            foreach (ResourceType current in ResourceTypeManager.Instance.Types)
            {
                Image icon = current.Icon;
                if (icon != null)
                {
                    int count = IL16.Images.Count;
                    IL16.Images.Add(icon);
                    icon.Tag = count;
                }
            }

            this.ResourcesRoot = new MyTreeNode("Data", false, false);
            this.a(ResourcesRoot);
            this.ResourcesView.Nodes.Add(this.ResourcesRoot);
            this.A(ResourcesRoot, "");
            nodesSizeDic.Clear();
            nodesLastWtDic.Clear();
            this.ResourcesView.TreeViewNodeSorter = new TreeNodeComparer();
            this.ResourcesView.Sort();
            nodesSizeDic.Clear();
            nodesLastWtDic.Clear();
            this.ResourcesRoot.Expand();
            this.ResourcesView.EndUpdate();
        }

        private MyTreeNode FindNodeByPath(string p)
        {
            if (string.IsNullOrEmpty(p))
                return ResourcesRoot;
            
            string[] r = p.Split("\\/".ToCharArray());
            TreeNode treeNode = ResourcesRoot;
            
            for (int i = 0; i < r.Length; i++)
            {
                string key = r[i];
                treeNode = treeNode.Nodes[key];
                if (treeNode == null)
                {
                    return null;
                }
            }
            return (MyTreeNode)treeNode;
        }

        private static string GetNodePath(TreeNode treeNode)
        {
            string text = "";
            for (TreeNode tn = treeNode; tn != null; tn = tn.Parent)
            {
                if (tn.Parent != null)
                {
                    if (text != "")
                        text = tn.Text + "\\" + text;
                    else
                        text = tn.Text;
                }
            }
            return text;
        }

        private void A(MyTreeNode parent, string path)
        {
            string[] directories = VirtualDirectory.GetDirectories(path); 
            for (int i = 0; i < directories.Length; i++)
            {
                string d = directories[i];
                this.a(parent, d);
            }

            string[] files = VirtualDirectory.GetFiles(path); 
            for (int j = 0; j < files.Length; j++)
            {
                string f = files[j];
                this.B(parent, f);
            }
        }

        private bool IsHideResource(string name)
        {
            string nameLower = name.ToLower();
            return hideDirectoriesAndFiles.Exists((string hideItem) => nameLower.Contains(hideItem.ToLower()));
        }

        private void a(MyTreeNode parent, string p)
        {
            string fileName = Path.GetFileName(p);
            MyTreeNode node = new MyTreeNode(fileName, VirtualDirectory.IsInArchive(p), this.IsHideResource(fileName) || parent.HideNode);
            node.Name = node.Text;
            parent.Nodes.Add(node);
            this.a(node);
            this.A(node, p);
        }

        private void UpdateResource(string p)
        {
            if (FindNodeByPath(p) != null)
                return;

            string directoryName = Path.GetDirectoryName(p);
            MyTreeNode parentNode = FindNodeByPath(directoryName);
            if (parentNode == null)
            {
                Log.Warning("ResourcesForm: UpdateAddDirectory: parentNode == null.");
                return;
            }
            this.a(parentNode, p);
        }

        private void B(MyTreeNode parent, string path)
        {
            string fileName = Path.GetFileName(path);
            TreeNode treeNode = new MyTreeNode(fileName, VirtualFile.IsInArchive(path), this.IsHideResource(fileName) || parent.HideNode);
            treeNode.Name = treeNode.Text;
            treeNode.Tag = fileName;
            parent.Nodes.Add(treeNode);
            this.UpdateTreeNodeIcon(treeNode);
        }

        public void UpdateAddResource(string fileName)
        {
            if (this.FindNodeByPath(fileName) != null)
            {
                return;
            }
            string directoryName = Path.GetDirectoryName(fileName);
            ResourcesForm.MyTreeNode myTreeNode = this.FindNodeByPath(directoryName);
            if (myTreeNode == null)
            {
                Log.Warning("ResourcesForm: UpdateAddResource: parentNode == null.");
                return;
            }
            this.B(myTreeNode, fileName);
        }

        private void UpdateTreeNodeIcon(TreeNode treeNode)
        {
            int iconIndex = 0;
            if (treeNode.Tag != null)
            {
                iconIndex = 1;
                string text = Path.GetExtension(treeNode.Text);
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Substring(1);
                    ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text);
                    if (byExtension != null && byExtension.Icon != null)
                        iconIndex = (int)byExtension.Icon.Tag;
                }
            }
            treeNode.ImageIndex = iconIndex;
            treeNode.SelectedImageIndex = iconIndex;
        }

        private void OnTreeKeyDown(object obj, KeyEventArgs keyEventArgs)
        {
            if ((keyEventArgs.KeyCode == Keys.Return || keyEventArgs.KeyCode == Keys.Space) && ResourcesView.SelectedNode != null && this.ResourcesView.SelectedNode.Nodes.Count > 0)
            {
                if (this.ResourcesView.SelectedNode.IsExpanded)
                {
                    this.ResourcesView.SelectedNode.Collapse();
                }
                else
                {
                    this.ResourcesView.SelectedNode.Expand();
                }
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
                return;
            }
            if ((keyEventArgs.KeyCode == Keys.Return || keyEventArgs.KeyCode == Keys.Space) && this.TryBeginEditMode())
            {
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
                return;
            }
            if (keyEventArgs.KeyCode == Keys.Delete)
            {
                bool flag = false;
                foreach (ResourcesForm.MyTreeNode current in this.multiSelectedList)
                {
                    if (current.Parent == null)
                    {
                        flag = true;
                    }
                }
                if (this.multiSelectedList.Count != 0 && !flag)
                {
                    this.B();
                    keyEventArgs.Handled = true;
                    keyEventArgs.SuppressKeyPress = true;
                    return;
                }
            }
            if (keyEventArgs.Control && keyEventArgs.KeyCode == Keys.X)
            {
                this.A(true);
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
                return;
            }
            if (keyEventArgs.Control && (keyEventArgs.KeyCode == Keys.C || keyEventArgs.KeyCode == Keys.Insert))
            {
                this.A(false);
                keyEventArgs.Handled = true;
                keyEventArgs.SuppressKeyPress = true;
                return;
            }
            if (((keyEventArgs.Control && keyEventArgs.KeyCode == Keys.V) || (keyEventArgs.Shift && keyEventArgs.KeyCode == Keys.Insert)) && this.a())
            {
                string text = this.A();
                if (text != null)
                {
                    this.b(text);
                    keyEventArgs.Handled = true;
                    keyEventArgs.SuppressKeyPress = true;
                }
            }
        }

        private void OnTreeKeyUp(object obj, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Apps && this.ResourcesView.SelectedNode != null)
            {
                this.a(new Point(0, 0));
            }
        }

        private void OnTreeNodeMouseClick(object obj, TreeNodeMouseClickEventArgs treeNodeMouseClickEventArgs)
        {
#if DEBUG_RES
            Log.Info(">> TreeNode MouseClick");
#endif
            if (treeNodeMouseClickEventArgs.Node != this.ResourcesView.SelectedNode)
            {
                return;
            }
            this.TryBeginEditMode();
        }

        private void A(bool flag)
        {
            if (this.multiSelectedList.Count == 0)
            {
                return;
            }
            foreach (ResourcesForm.MyTreeNode current in this.multiSelectedList)
            {
                if (current.Parent == null)
                {
                    return;
                }
            }
            List<string> list = new List<string>();
            List<ResourcesForm.MyTreeNode> list2 = this.A(this.multiSelectedList);
            foreach (ResourcesForm.MyTreeNode current2 in list2)
            {
                string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(ResourcesForm.GetNodePath((TreeNode)current2));
                list.Add(realPathByVirtual);
            }
            foreach (string current3 in list)
            {
                string virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(current3);
                if (VirtualFile.IsInArchive(virtualPathByReal))
                {
                    string format = this.d("File \"{0}\" cannot be moved or copied because it's inside archive.");
                    Log.Warning(format, virtualPathByReal);
                    return;
                }
            }
            IDataObject dataObject = new DataObject(DataFormats.FileDrop, list.ToArray());
            MemoryStream memoryStream = new MemoryStream();
            Stream arg_127_0 = memoryStream;
            byte[] array = new byte[4];
            array[0] = (flag ? (byte)2 : (byte)5);
            arg_127_0.Write(array, 0, 4);
            memoryStream.SetLength(4L);
            dataObject.SetData("Preferred DropEffect", memoryStream);
            Clipboard.SetDataObject(dataObject);
        }

        private string A()
        {
            ResourcesForm.MyTreeNode myTreeNode = null;
            foreach (ResourcesForm.MyTreeNode current in this.multiSelectedList)
            {
                ResourcesForm.MyTreeNode myTreeNode2 = (current.Tag == null) ? current : ((ResourcesForm.MyTreeNode)current.Parent);
                if (myTreeNode == null)
                {
                    myTreeNode = myTreeNode2;
                }
                else if (myTreeNode != myTreeNode2)
                {
                    myTreeNode = null;
                    break;
                }
            }
            if (myTreeNode != null)
            {
                return ResourcesForm.GetNodePath((TreeNode)myTreeNode);
            }
            return null;
        }

        private bool a()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            MemoryStream memoryStream = dataObject.GetData("Preferred DropEffect") as MemoryStream;
            return memoryStream != null;
        }
        private void A(string path, string text)
        {
            string[] directories = Directory.GetDirectories(path);
            Directory.CreateDirectory(text);
            string[] array = directories;
            for (int i = 0; i < array.Length; i++)
            {
                string text2 = array[i];
                this.A(text2, Path.Combine(text, Path.GetFileName(text2)));
            }
            string[] files = Directory.GetFiles(path);
            for (int j = 0; j < files.Length; j++)
            {
                string text3 = files[j];
                File.Copy(text3, Path.Combine(text, Path.GetFileName(text3)));
            }
        }
        private void b(string virtualPath)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            MemoryStream memoryStream = dataObject.GetData("Preferred DropEffect") as MemoryStream;
            if (memoryStream == null)
            {
                return;
            }
            byte[] array = new byte[memoryStream.Length];
            memoryStream.Read(array, 0, array.Length);
            bool flag = array[0] == 2;
            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(virtualPath);
            string[] array2 = (string[])dataObject.GetData(DataFormats.FileDrop);
            string[] array3 = array2;
            for (int i = 0; i < array3.Length; i++)
            {
                string text = array3[i];
                try
                {
                    string text2 = Path.Combine(realPathByVirtual, Path.GetFileName(text));
                    if (File.Exists(text))
                    {
                        if (string.Compare(realPathByVirtual, Path.GetDirectoryName(text), true) == 0)
                        {
                            int num = 1;
                            string text3;
                            while (true)
                            {
                                text3 = realPathByVirtual + "//";
                                text3 += Path.GetFileNameWithoutExtension(text);
                                if (num != 1)
                                {
                                    text3 += num.ToString();
                                }
                                if (Path.GetExtension(text) != null)
                                {
                                    text3 += Path.GetExtension(text);
                                }
                                if (!File.Exists(text3))
                                {
                                    break;
                                }
                                num++;
                            }
                            text2 = text3;
                        }
                        if (flag)
                        {
                            File.Move(text, text2);
                        }
                        else
                        {
                            File.Copy(text, text2);
                        }
                    }
                    else if (Directory.Exists(text))
                    {
                        int num2 = 1;
                        string text4;
                        while (true)
                        {
                            text4 = Path.GetDirectoryName(text2) + "\\";
                            text4 += Path.GetFileName(text2);
                            if (num2 != 1)
                            {
                                text4 += num2.ToString();
                            }
                            if (!Directory.Exists(text4))
                            {
                                break;
                            }
                            num2++;
                        }
                        text2 = text4;
                        this.A(text, text2);
                        if (flag)
                        {
                            Directory.Delete(text, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex.Message);
                    break;
                }
            }
        }
        private void ShowMenu(Point position)
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //contextMenuStrip.Font = MainForm.GetFont(MainForm.fontContextMenu, contextMenuStrip.Font);
            string text = this.d("Close Editing");
            Image icon = this.aTP.Images["EditStop_16.png"];
            if (ResourcesForm.aTQ == null)
                ResourcesForm.aTQ = new EventHandler(ResourcesForm.OnMenu_CloseEditing);

            ToolStripMenuItem value = new ToolStripMenuItem(text, icon, ResourcesForm.aTQ);
            contextMenuStrip.Items.Add(value);
            contextMenuStrip.Show(this.ResourcesView, position);
        }

        private void A(object obj, EventArgs eventArgs)
        {
            Tuple<string, ResourceType> pair = (Tuple<string, ResourceType>)((ToolStripMenuItem)obj).Tag;
            this.A(pair.Item1, pair.Item2);
        }

        private void a(Point point)
        {
            EventHandler eventHandler = null;
            EventHandler eventHandler2 = null;
            EventHandler eventHandler3 = null;
            EventHandler eventHandler4 = null;
            if (this.IsResourceEditModeActive != null)
            {
                ResourcesForm.ActiveEventArgs activeEventArgs = new ResourcesForm.ActiveEventArgs();
                this.IsResourceEditModeActive(activeEventArgs);
                if (activeEventArgs.Active)
                {
                    this.ShowMenu(point);
                    return;
                }
            }
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //contextMenuStrip.Font = MainForm.GetFont(MainForm.fontContextMenu, contextMenuStrip.Font);
            TreeNode node = this.ResourcesView.SelectedNode;
            if (node == null)
            {
                return;
            }
            string nodePath = ResourcesForm.GetNodePath(node);
            ToolStripMenuItem toolStripMenuItem2;
            if (node.Tag == null)
            {
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(this.d("New"), this.aTP.Images["New_16.png"]);
                toolStripMenuItem2 = new ToolStripMenuItem(this.d("Folder"), this.aTP.Images["Folder_16.png"], delegate (object s, EventArgs e2)
                {
                    this.D(nodePath);
                });
                toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
                toolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                foreach (ResourceType current in ResourceTypeManager.Instance.Types)
                {
                    if (current.AllowNewResource)
                    {
                        string text = this.d(current.DisplayName);
                        toolStripMenuItem2 = new ToolStripMenuItem(text, current.Icon, new EventHandler(this.A));
                        toolStripMenuItem2.Tag = new Tuple<string, ResourceType>(nodePath, current);
                        toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
                    }
                }
                contextMenuStrip.Items.Add(toolStripMenuItem);
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                toolStripMenuItem2 = new ToolStripMenuItem(this.d("Open Folder in Explorer"), null, delegate (object s, EventArgs e2)
                {
                    string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(nodePath);
                    Shell32Api.ShellExecuteEx(null, realPathByVirtual);
                });
                contextMenuStrip.Items.Add(toolStripMenuItem2);
            }
            if (node.Tag != null)
            {
                string text2 = Path.GetExtension(nodePath);
                if (!string.IsNullOrEmpty(text2))
                {
                    text2 = text2.Substring(1);
                    ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text2);
                    if (byExtension != null)
                    {
                        string arg_288_0 = this.d("编 辑");
                        Image arg_288_1 = this.aTP.Images["Edit_16.png"];
                        if (eventHandler == null)
                        {
                            eventHandler = new EventHandler(this.C);
                        }
                        toolStripMenuItem2 = new ToolStripMenuItem(arg_288_0, arg_288_1, eventHandler);
                        contextMenuStrip.Items.Add(toolStripMenuItem2);
                    }
                }
                toolStripMenuItem2 = new ToolStripMenuItem(this.d("Open in External Program"), null, delegate (object s, EventArgs e2)
                {
                    string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(nodePath);
                    Shell32Api.ShellExecuteEx(null, realPathByVirtual);
                });
                contextMenuStrip.Items.Add(toolStripMenuItem2);
            }
            if (node.Tag is string)
            {
                string path = ResourcesForm.GetNodePath(node);
                string text3 = Path.GetExtension(path);
                if (!string.IsNullOrEmpty(text3))
                {
                    text3 = text3.Substring(1);
                    ResourceType byExtension2 = ResourceTypeManager.Instance.GetByExtension(text3);
                    if (byExtension2 != null)
                    {
                        byExtension2.DoResourcesTreeContextMenu(path, contextMenuStrip);
                    }
                }
            }
            if (this.multiSelectedList.Count != 0)
            {
                bool flag = false;
                foreach (ResourcesForm.MyTreeNode current2 in this.multiSelectedList)
                {
                    if (current2.Parent == null)
                    {
                        flag = true;
                    }
                }
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                string arg_3BD_0 = this.d("Cut");
                Image arg_3BD_1 = this.aTP.Images["Cut_16.png"];
                if (eventHandler2 == null)
                {
                    eventHandler2 = new EventHandler(this.c);
                }
                toolStripMenuItem2 = new ToolStripMenuItem(arg_3BD_0, arg_3BD_1, eventHandler2);
                toolStripMenuItem2.Enabled = !flag;
                contextMenuStrip.Items.Add(toolStripMenuItem2);
                string arg_40F_0 = this.d("Copy");
                Image arg_40F_1 = this.aTP.Images["Copy_16.png"];
                if (eventHandler3 == null)
                {
                    eventHandler3 = new EventHandler(this.D);
                }
                toolStripMenuItem2 = new ToolStripMenuItem(arg_40F_0, arg_40F_1, eventHandler3);
                contextMenuStrip.Items.Add(toolStripMenuItem2);
                if (node.Tag == null)
                {
                    string directoryToPaste = null;
                    if (this.a())
                    {
                        directoryToPaste = this.A();
                    }
                    toolStripMenuItem2 = new ToolStripMenuItem(this.d("Paste"), this.aTP.Images["Paste_16.png"], delegate (object s, EventArgs e2)
                    {
                        this.b(directoryToPaste);
                    });
                    toolStripMenuItem2.Enabled = (directoryToPaste != null);
                    contextMenuStrip.Items.Add(toolStripMenuItem2);
                }
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                string arg_503_0 = this.d("Delete");
                Image arg_503_1 = this.aTP.Images["Delete_16.png"];
                if (eventHandler4 == null)
                {
                    eventHandler4 = new EventHandler(this.d);
                }
                toolStripMenuItem2 = new ToolStripMenuItem(arg_503_0, arg_503_1, eventHandler4);
                toolStripMenuItem2.Enabled = !flag;
                contextMenuStrip.Items.Add(toolStripMenuItem2);
                toolStripMenuItem2 = new ToolStripMenuItem(this.d("Rename"), null, delegate (object s, EventArgs e2)
                {
                    if (node != this.ResourcesView.SelectedNode)
                    {
                        return;
                    }
                    if (!node.IsEditing)
                    {
                        node.BeginEdit();
                    }
                });
                toolStripMenuItem2.Enabled = (this.multiSelectedList.Count == 1 && !flag);
                contextMenuStrip.Items.Add(toolStripMenuItem2);
            }
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            this.A(contextMenuStrip);
            toolStripMenuItem2 = new ToolStripMenuItem(this.d("Refresh"), this.aTP.Images["Refresh_16.png"], delegate (object s, EventArgs e2)
            {
                this.b((ResourcesForm.MyTreeNode)node);
            });
            contextMenuStrip.Items.Add(toolStripMenuItem2);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            toolStripMenuItem2 = new ToolStripMenuItem(this.d("Properties"), null, delegate (object s, EventArgs e2)
            {
                string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(nodePath);
                Shell32Api.ShellExecuteEx("properties", realPathByVirtual);
            });
            contextMenuStrip.Items.Add(toolStripMenuItem2);
            /*
            foreach (ResourceEditorAddon current3 in AddonManager.Instance.Addons)
            {
                current3.OnShowContextMenuOfResourcesTree(contextMenuStrip);
            }
            //*/
            contextMenuStrip.Show(this.ResourcesView, point);
        }
        private void B(Point point)
        {
            if (this.IsResourceEditModeActive != null)
            {
                ResourcesForm.ActiveEventArgs activeEventArgs = new ResourcesForm.ActiveEventArgs();
                this.IsResourceEditModeActive(activeEventArgs);
                if (activeEventArgs.Active)
                {
                    this.ShowMenu(point);
                    return;
                }
            }
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //contextMenuStrip.Font = MainForm.GetFont(MainForm.fontContextMenu, contextMenuStrip.Font);
            this.A(contextMenuStrip);
            contextMenuStrip.Show(this.ResourcesView, point);
        }
        private void A(ContextMenuStrip contextMenuStrip)
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(this.d("Sort by"), null);
            ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(this.d("Name"), null, new EventHandler(this.E));
            toolStripMenuItem2.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Name);
            toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
            toolStripMenuItem2 = new ToolStripMenuItem(this.d("Date modified"), null, new EventHandler(this.e));
            toolStripMenuItem2.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Date);
            toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
            toolStripMenuItem2 = new ToolStripMenuItem(this.d("Type"), null, new EventHandler(this.F));
            toolStripMenuItem2.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Type);
            toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
            toolStripMenuItem2 = new ToolStripMenuItem(this.d("Size"), null, new EventHandler(this.f));
            toolStripMenuItem2.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Size);
            toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
            toolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            toolStripMenuItem2 = new ToolStripMenuItem(this.d("Ascending"), null, new EventHandler(this.G));
            toolStripMenuItem2.Checked = ResourcesForm.sortByAscending;
            toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
            contextMenuStrip.Items.Add(toolStripMenuItem);
        }
        private void OnTreeMouseClick(object obj, MouseEventArgs mouseEventArgs)
        {
#if DEBUG_RES
            Log.Info(">> Tree MouseClick");
#endif
            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                TreeNode nodeAt = this.ResourcesView.GetNodeAt(mouseEventArgs.Location);
                if (nodeAt != null)
                {
                    this.ResourcesView.SelectedNode = nodeAt;
                }
                this.a(mouseEventArgs.Location);
            }
        }
        private void OnTreeMouseUp(object obj, MouseEventArgs mouseEventArgs)
        {
#if DEBUG_RES
            Log.Info(">> Tree MouseUp");
#endif
            if (mouseEventArgs.Button == MouseButtons.Right && this.ResourcesView.GetNodeAt(mouseEventArgs.Location) == null)
            {
                this.B(mouseEventArgs.Location);
            }
        }
        private void A(string directory, ResourceType resourceType)
        {
            bool flag = false;
            try
            {
                if (this.WatchFileSystem)
                {
                    this.WatchFileSystem = false;
                    flag = true;
                }
                resourceType.DoNewResource(directory);
            }
            finally
            {
                if (flag)
                {
                    this.WatchFileSystem = true;
                }
            }
        }
        private List<ResourcesForm.MyTreeNode> A(List<ResourcesForm.MyTreeNode> list)
        {
            List<ResourcesForm.MyTreeNode> list2 = new List<ResourcesForm.MyTreeNode>(list.Count);
            foreach (ResourcesForm.MyTreeNode current in list)
            {
                bool flag = false;
                for (ResourcesForm.MyTreeNode myTreeNode = (ResourcesForm.MyTreeNode)current.Parent; myTreeNode != null; myTreeNode = (ResourcesForm.MyTreeNode)myTreeNode.Parent)
                {
                    if (list.Contains(myTreeNode))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list2.Add(current);
                }
            }
            return list2;
        }
        private void B()
        {
            if (this.multiSelectedList.Count == 0)
            {
                return;
            }
            if (this.IsResourceEditModeActive != null)
            {
                ResourcesForm.ActiveEventArgs activeEventArgs = new ResourcesForm.ActiveEventArgs();
                this.IsResourceEditModeActive(activeEventArgs);
                if (activeEventArgs.Active)
                {
                    Log.Warning(ToolsLocalization.Translate("ResourcesForm", "Need to leave the editing mode first."));
                    return;
                }
            }
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            if (this.ResourceChange != null)
            {
                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                this.ResourceChange(null, cancelEventArgs);
                Trace.Assert(!cancelEventArgs.Cancel);
            }
            string text;
            if (this.multiSelectedList.Count == 1)
            {
                string format = ToolsLocalization.Translate("ResourcesForm", "Are you sure you want to delete \"{0}\"?");
                string arg = ResourcesForm.GetNodePath((TreeNode)this.multiSelectedList[0]);
                text = string.Format(format, arg);
            }
            else
            {
                string format2 = ToolsLocalization.Translate("ResourcesForm", "Are you sure you want to delete these {0} items?");
                text = string.Format(format2, this.multiSelectedList.Count);
            }
            string caption = ToolsLocalization.Translate("Various", "Resource Editor");
            DialogResult dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
            {
                if (this.ResourceChange != null)
                {
                    string fileName = ResourcesForm.GetNodePath(selectedNode);
                    CancelEventArgs cancelEventArgs2 = new CancelEventArgs();
                    this.ResourceChange(fileName, cancelEventArgs2);
                    Trace.Assert(!cancelEventArgs2.Cancel);
                }
                return;
            }
            ResourcesForm.MyTreeNode myTreeNode = (ResourcesForm.MyTreeNode)TreeViewUtils.GetNeedSelectNodeAfterRemoveNode(this.ResourcesView.SelectedNode);
            List<ResourcesForm.MyTreeNode> list = this.A(this.multiSelectedList);
            foreach (ResourcesForm.MyTreeNode current in list)
            {
                string text2 = ResourcesForm.GetNodePath((TreeNode)current);
                if (current.Tag != null)
                {
                    if (!this.c(text2))
                    {
                        return;
                    }
                }
                else if (!this.C(text2))
                {
                    return;
                }
            }
            if (myTreeNode.TreeView != null)
            {
                this.ResourcesView.SelectedNode = myTreeNode;
            }
        }
        private bool C(string text)
        {
            TreeNode treeNode = this.FindNodeByPath(text);
            while (treeNode.Nodes.Count != 0)
            {
                TreeNode treeNode2 = treeNode.Nodes[0];
                string text2 = ResourcesForm.GetNodePath(treeNode2);
                if (treeNode2.Tag != null)
                {
                    if (!this.c(text2))
                    {
                        return false;
                    }
                }
                else if (!this.C(text2))
                {
                    return false;
                }
            }
            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(text);
            try
            {
                Directory.Delete(realPathByVirtual);
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                return false;
            }
            treeNode.Parent.Nodes.Remove(treeNode);
            this.B((ResourcesForm.MyTreeNode)treeNode);
            return true;
        }
        private bool c(string text)
        {
            string text2 = Path.GetExtension(text);
            if (!string.IsNullOrEmpty(text2))
            {
                text2 = text2.Substring(1);
            }
            ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text2);
            if (this.ResourceChange != null)
            {
                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                this.ResourceChange(null, cancelEventArgs);
                Trace.Assert(!cancelEventArgs.Cancel);
            }
            bool flag = false;
            if (byExtension != null)
            {
                if (!byExtension.DoUnloadResource(text))
                {
                    goto IL_79;
                }
            }
            try
            {
                File.Delete(VirtualFileSystem.GetRealPathByVirtual(text));
                flag = true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
            }
            IL_79:
            if (!flag)
            {
                if (this.ResourceChange != null)
                {
                    CancelEventArgs cancelEventArgs2 = new CancelEventArgs();
                    this.ResourceChange(text, cancelEventArgs2);
                    Trace.Assert(!cancelEventArgs2.Cancel);
                }
                return false;
            }
            TreeNode treeNode = this.FindNodeByPath(text);
            Trace.Assert(treeNode != null);
            treeNode.Parent.Nodes.Remove(treeNode);
            this.B((ResourcesForm.MyTreeNode)treeNode);
            return true;
        }
        private void D(string text)
        {
            ResourcesForm.MyTreeNode myTreeNode = this.FindNodeByPath(text);
            Trace.Assert(myTreeNode != null);
            int num = 1;
            string text2;
            while (true)
            {
                text2 = "New Folder";
                if (num != 1)
                {
                    text2 = text2 + " (" + num.ToString() + ")";
                }
                bool flag = false;
                foreach (TreeNode treeNode in myTreeNode.Nodes)
                {
                    if (string.Compare(treeNode.Name, text2, true) == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    break;
                }
                num++;
            }
            string path = Path.Combine(VirtualFileSystem.GetRealPathByVirtual(text), text2);
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                return;
            }
            TreeNode treeNode2 = new ResourcesForm.MyTreeNode(text2, VirtualDirectory.IsInArchive(text), this.IsHideResource(text2) || myTreeNode.HideNode);
            treeNode2.Name = treeNode2.Text;
            this.UpdateTreeNodeIcon(treeNode2);
            myTreeNode.Nodes.Add(treeNode2);
            this.ResourcesView.SelectedNode = treeNode2;
            treeNode2.BeginEdit();
        }
        public void BeginNewDirectoryCreation()
        {
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            if (selectedNode == null)
            {
                return;
            }
            string text = ResourcesForm.GetNodePath(selectedNode);
            if (selectedNode.Tag != null)
            {
                text = Path.GetDirectoryName(text);
            }
            this.D(text);
        }
        public void BeginNewResourceCreation(ResourceType type)
        {
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            if (selectedNode == null)
            {
                return;
            }
            string text = ResourcesForm.GetNodePath(selectedNode);
            if (selectedNode.Tag != null)
            {
                text = Path.GetDirectoryName(text);
            }
            this.A(text, type);
        }
        public void SelectNodeByPath(string path)
        {
            TreeNode treeNode = this.FindNodeByPath(path);
            TreeViewUtils.ExpandAllPathToNode(treeNode);
            Trace.Assert(treeNode != null);
            this.ResourcesView.SelectedNode = treeNode;
        }
        private void OnBeforeNodeLabelEdit(object obj, NodeLabelEditEventArgs nodeLabelEditEventArgs)
        {
            if (nodeLabelEditEventArgs.Node == this.ResourcesRoot)
            {
                nodeLabelEditEventArgs.CancelEdit = true;
                return;
            }
            if (nodeLabelEditEventArgs.Node.Tag != null)
            {
                if (this.IsResourceEditModeActive != null)
                {
                    ResourcesForm.ActiveEventArgs activeEventArgs = new ResourcesForm.ActiveEventArgs();
                    this.IsResourceEditModeActive(activeEventArgs);
                    if (activeEventArgs.Active)
                    {
                        nodeLabelEditEventArgs.CancelEdit = true;
                        return;
                    }
                }
                if (this.ResourceChange != null)
                {
                    CancelEventArgs cancelEventArgs = new CancelEventArgs();
                    this.ResourceChange(null, cancelEventArgs);
                    Trace.Assert(!cancelEventArgs.Cancel);
                }
                if (this.ResourceRename != null)
                {
                    bool flag = false;
                    string text = "";
                    this.ResourceRename(ResourcesForm.GetNodePath(nodeLabelEditEventArgs.Node), ref flag, ref text);
                    if (flag)
                    {
                        nodeLabelEditEventArgs.CancelEdit = true;
                        if (!string.IsNullOrEmpty(text))
                        {
                            nodeLabelEditEventArgs.Node.Text = Path.GetFileName(text);
                            nodeLabelEditEventArgs.Node.Name = nodeLabelEditEventArgs.Node.Text;
                        }
                        if (this.ResourceChange != null)
                        {
                            CancelEventArgs cancelEventArgs2 = new CancelEventArgs();
                            this.ResourceChange(ResourcesForm.GetNodePath(nodeLabelEditEventArgs.Node), cancelEventArgs2);
                            Trace.Assert(!cancelEventArgs2.Cancel);
                        }
                    }
                }
            }
            this.aTL = true;
        }
        private void OnAfterNodeLabelEdit(object obj, NodeLabelEditEventArgs nodeLabelEditEventArgs)
        {
            this.aTL = false;
            if (string.IsNullOrEmpty(nodeLabelEditEventArgs.Label))
            {
                nodeLabelEditEventArgs.CancelEdit = true;
                if (nodeLabelEditEventArgs.Node.Tag != null && this.ResourceChange != null)
                {
                    CancelEventArgs cancelEventArgs = new CancelEventArgs();
                    this.ResourceChange(ResourcesForm.GetNodePath(nodeLabelEditEventArgs.Node), cancelEventArgs);
                    Trace.Assert(!cancelEventArgs.Cancel);
                }
                return;
            }
            string text = ResourcesForm.GetNodePath(nodeLabelEditEventArgs.Node);
            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(text);
            string text2 = Path.Combine(VirtualFileSystem.GetRealPathByVirtual(Path.GetDirectoryName(text)), nodeLabelEditEventArgs.Label);
            bool flag = false;
            try
            {
                if (nodeLabelEditEventArgs.Node.Tag == null)
                {
                    Directory.Move(realPathByVirtual, text2);
                }
                else
                {
                    File.Move(realPathByVirtual, text2);
                }
                flag = true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                nodeLabelEditEventArgs.CancelEdit = true;
            }
            if (flag)
            {
                nodeLabelEditEventArgs.Node.Text = nodeLabelEditEventArgs.Label;
                nodeLabelEditEventArgs.Node.Name = nodeLabelEditEventArgs.Label;
                this.UpdateTreeNodeIcon(nodeLabelEditEventArgs.Node);
            }
            if (nodeLabelEditEventArgs.Node.Tag != null && this.ResourceChange != null)
            {
                CancelEventArgs cancelEventArgs2 = new CancelEventArgs();
                this.ResourceChange(ResourcesForm.GetNodePath(nodeLabelEditEventArgs.Node), cancelEventArgs2);
                Trace.Assert(!cancelEventArgs2.Cancel);
            }
        }
        private bool A(ResourcesForm.MyTreeNode myTreeNode)
        {
            if (((Control.MouseButtons & MouseButtons.Left) != MouseButtons.None || (Control.MouseButtons & MouseButtons.Right) != MouseButtons.None) && myTreeNode.Bounds.Height != 0)
            {
                Rectangle bounds = myTreeNode.Bounds;
                bounds.Location = new Point(bounds.Location.X - 17, bounds.Location.Y);
                bounds.Size = new Size(bounds.Size.Width + 17, bounds.Size.Height + 2);
                if (bounds.Contains(base.PointToClient(Cursor.Position)))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDrawTreeNode(object obj, DrawTreeNodeEventArgs drawTreeNodeEventArgs)
        {
            Font font = drawTreeNodeEventArgs.Node.NodeFont;
            if (font == null)
                font = drawTreeNodeEventArgs.Node.TreeView.Font;
            
            MyTreeNode node = (MyTreeNode)drawTreeNodeEventArgs.Node;
            bool flag = (drawTreeNodeEventArgs.State == TreeNodeStates.Selected) || node.MultiSelected;
            if (flag)
                drawTreeNodeEventArgs.Graphics.FillRectangle(SystemBrushes.Highlight, drawTreeNodeEventArgs.Bounds);

            Brush brush = flag ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
            if (node.IsArchive || node.HideNode)
                brush = SystemBrushes.GrayText;

            Rectangle bounds = drawTreeNodeEventArgs.Bounds;
            bounds.Size = new Size((int)drawTreeNodeEventArgs.Graphics.MeasureString(node.Text, font).Width + 2, bounds.Size.Height);
            if (node.Bounds.Height != 0)
            {
                drawTreeNodeEventArgs.Graphics.DrawString(node.Text, font, brush, bounds);
            }
        }

        private void OnTreeBeforeSelect(object obj, TreeViewCancelEventArgs treeViewCancelEventArgs)
        {
            if (this.ResourceChange == null) 
                return;
            
            if (_PropertyGridUtils.EditValueDropDownControlCreated)
            {
                treeViewCancelEventArgs.Cancel = true;
                return;
            }

            string fileName = null;
            MyTreeNode evtNode = (MyTreeNode)treeViewCancelEventArgs.Node;
            if (evtNode != null && evtNode.Tag != null)
                fileName = ResourcesForm.GetNodePath((TreeNode)evtNode);

            CancelEventArgs cancelEventArgs = new CancelEventArgs();
            ResourceChange(fileName, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                treeViewCancelEventArgs.Cancel = true;
                return;
            }

            MyTreeNode selected = (MyTreeNode)ResourcesView.SelectedNode;
            if ((Control.ModifierKeys & Keys.Control) == Keys.None)
            {
                this.a(evtNode);
            }

            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                MyTreeNode myTreeNode3;
                if (this.aTg != null)
                {
                    myTreeNode3 = this.aTg;
                }
                else
                {
                    myTreeNode3 = selected;
                }
                ResourcesForm.MyTreeNode myTreeNode4 = evtNode;
                if (myTreeNode3 != null && myTreeNode4 != null && myTreeNode3 != myTreeNode4 && myTreeNode3.Parent != null && myTreeNode3.Parent == myTreeNode4.Parent)
                {
                    List<MyTreeNode> list = new List<MyTreeNode>();
                    bool flag = false;
                    foreach (MyTreeNode myTreeNode5 in myTreeNode3.Parent.Nodes)
                    {
                        if (flag)
                        {
                            list.Add(myTreeNode5);
                        }
                        if (myTreeNode5 == myTreeNode3 || myTreeNode5 == myTreeNode4)
                        {
                            if (flag)
                            {
                                break;
                            }
                            flag = true;
                            list.Add(myTreeNode5);
                        }
                    }
                    foreach (MyTreeNode current in list)
                    {
                        if (current != evtNode)
                        {
                            current.SetMultiSelected(true, true);
                        }
                    }
                }
                if (this.aTg == null)
                {
                    this.aTg = selected;
                    return;
                }
            }
            else
            {
                this.aTg = null;
            }
        }

        private void OnTreeAfterSelect(object obj, TreeViewEventArgs treeViewEventArgs)
        {
            MyTreeNode node = (MyTreeNode)treeViewEventArgs.Node;
            if (node != null)
            {
                node.SetMultiSelected(true, false);

                if (MainForm.Instance != null)
                {
                    string nodePath = GetNodePath(node);
                    MainForm.Instance.UpdateLastSelectedResourcePath(nodePath); 
                }
            }
        }

        private void a(MyTreeNode node)
        {
            List<MyTreeNode> list = new List<MyTreeNode>(this.multiSelectedList);
            foreach (MyTreeNode item in list)
            {
                if (item != node)
                {
                    item.SetMultiSelected(false, true);
                }
            }
        }

        private void Timer_Tick(object obj, EventArgs eventArgs)
        {
            if (JxResApp.Instance == null)
                return;

            if (this.firstRun)
            {
                ResourcesView.Visible = true;
                ResourcesView.Select();
            }

            if (ResourcesView.SelectedNode == null)
            {
                this.a((MyTreeNode)null);
            }

            ResourceObjectEditor resourceObjectEditor = JxResApp.Instance.ResourceObjectEditor;
            if (resourceObjectEditor != null)
            {
                if (resourceObjectEditor.EditModeActive)
                {
                    this.a((MyTreeNode)null);
                }
                else
                {
                    ResourcesForm.MyTreeNode myTreeNode = (ResourcesForm.MyTreeNode)this.ResourcesView.SelectedNode;
                    if (myTreeNode != null)
                    {
                        myTreeNode.SetMultiSelected(true, true);
                    }
                }
            }
            HandleFileSystemEvents();
            this.firstRun = false;
        }

        public void DoCreatedEvent(string realPath)
        {
            string virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(realPath);
            if (Directory.Exists(realPath))
            {   // 文件夹
                string[] dr = virtualPathByReal.Split(new char[]
                {
                    '/',
                    '\\'
                }, StringSplitOptions.RemoveEmptyEntries);
                string fullPath = ""; 
                for (int i = 0; i < dr.Length; i++)
                {
                    string p1 = dr[i];
                    fullPath = Path.Combine(fullPath, p1);
                    UpdateResource(fullPath);
                }
                return;
            }

            string[] fr = Path.GetDirectoryName(virtualPathByReal).Split(new char[]
            {
                '/',
                '\\'
            }, StringSplitOptions.RemoveEmptyEntries);
            string text2 = "";
            for (int j = 0; j < fr.Length; j++)
            {
                string p1 = fr[j];
                text2 = Path.Combine(text2, p1);
                UpdateResource(text2);
            }
            this.UpdateAddResource(virtualPathByReal);
            string text3 = Path.GetExtension(virtualPathByReal);
            if (!string.IsNullOrEmpty(text3))
            {
                text3 = text3.Substring(1);
                ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text3);
                if (byExtension != null)
                {
                    bool flag = false;
                    try
                    {
                        if (this.WatchFileSystem)
                        {
                            this.WatchFileSystem = false;
                            flag = true;
                        }
                        if (byExtension.DoOutsideAddResource(virtualPathByReal))
                        {
                            byExtension.DoLoadResource(virtualPathByReal);
                        }
                    }
                    finally
                    {
                        if (flag)
                        {
                            this.WatchFileSystem = true;
                        }
                    }
                }
            }
        }
        public void DoDeletedEvent(string realPath)
        {
            string virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(realPath);
            TreeNode treeNode = this.FindNodeByPath(virtualPathByReal);
            if (treeNode != null)
            {
                if (treeNode.Tag != null)
                {
                    string text = Path.GetExtension(virtualPathByReal);
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = text.Substring(1);
                        ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text);
                        if (byExtension != null)
                        {
                            byExtension.DoUnloadResource(virtualPathByReal);
                        }
                    }
                }
                treeNode.Remove();
                this.B((ResourcesForm.MyTreeNode)treeNode);
            }
        }
        public void DoRenamedEvent(string realPath, string oldRealPath)
        {
            string virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(realPath);
            string virtualPathByReal2 = VirtualFileSystem.GetVirtualPathByReal(oldRealPath);
            string text = Path.GetExtension(virtualPathByReal2);
            string text2 = Path.GetExtension(virtualPathByReal);
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && string.Compare(text, text2, true) == 0)
            {
                text2 = text2.Substring(1);
                ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text2);
                if (byExtension != null)
                {
                    byExtension.OnResourceRenamed(virtualPathByReal, virtualPathByReal2);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Substring(1);
                    ResourceType byExtension2 = ResourceTypeManager.Instance.GetByExtension(text);
                    if (byExtension2 != null)
                    {
                        byExtension2.DoUnloadResource(virtualPathByReal2);
                    }
                }
                if (!string.IsNullOrEmpty(text2))
                {
                    text2 = text2.Substring(1);
                    ResourceType byExtension3 = ResourceTypeManager.Instance.GetByExtension(text2);
                    if (byExtension3 != null)
                    {
                        byExtension3.DoLoadResource(virtualPathByReal);
                    }
                }
            }
            TreeNode treeNode = this.FindNodeByPath(virtualPathByReal2);
            if (treeNode != null)
            {
                string fileName = Path.GetFileName(realPath);
                treeNode.Text = fileName;
                treeNode.Name = fileName;
                this.UpdateTreeNodeIcon(treeNode);
            }
            if (!VirtualFile.Exists(virtualPathByReal) && VirtualDirectory.Exists(virtualPathByReal))
            {
                this.a(virtualPathByReal, virtualPathByReal2);
            }
        }
        public void DoChangedEvent(string realPath)
        {
            string virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(realPath);
            string text = Path.GetExtension(virtualPathByReal);
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Substring(1);
                ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text);
                if (byExtension != null)
                {
                    byExtension.DoUnloadResource(virtualPathByReal);
                    byExtension.DoLoadResource(virtualPathByReal);
                }
            }
        }

        private void HandleFileSystemEvents()
        {
            List<FileSystemEventArgs> list;
            lock (fileSystemSyncLock)
            {
                list = new List<FileSystemEventArgs>(fileSystemEvents.Count);
                foreach (FileSystemEventArgs evt in fileSystemEvents)
                {
                    if (evt.ChangeType == WatcherChangeTypes.Changed)
                    {
                        int num = list.FindIndex((FileSystemEventArgs evt2) => string.Compare(evt2.FullPath, evt.FullPath, true) == 0);
                        if (num != -1)
                        {
                            continue;
                        }
                    }
                    list.Add(evt);
                }
                fileSystemEvents.Clear();
            }

            foreach (FileSystemEventArgs e in list)
            {
                WatcherChangeTypes changeType = e.ChangeType;
                switch (changeType)
                {
                    case WatcherChangeTypes.Created:
                        DoCreatedEvent(e.FullPath);
                        break;
                    case WatcherChangeTypes.Deleted:
                        DoDeletedEvent(e.FullPath);
                        break;
                    case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted:
                        break;
                    case WatcherChangeTypes.Changed:
                        DoChangedEvent(e.FullPath);
                        break;
                    default:
                        if (changeType == WatcherChangeTypes.Renamed)
                        {
                            RenamedEventArgs renamedEventArgs = (RenamedEventArgs)e;
                            DoRenamedEvent(e.FullPath, renamedEventArgs.OldFullPath);
                        }
                        break;
                }
            }
        }

        private void a(string text, string text2)
        {
            string[] files = Directory.GetFiles(VirtualFileSystem.GetRealPathByVirtual(text), "*.type", SearchOption.AllDirectories);
            string[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                string text3 = array[i];
                string text4 = text3.Substring(5);
                TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(text4);
                if (textBlock != null && textBlock.Children.Count == 1)
                {
                    string data = textBlock.Children[0].Data;
                    EntityType byName = EntityTypes.Instance.GetByName(data);
                    if (byName != null)
                    {
                        EntityTypes.Instance.DestroyType(byName);
                        EntityType entityType = EntityTypes.Instance.LoadTypeFromFile(text4);
                        if (entityType == null)
                        {
                            Log.Fatal("ResourceForm: OnDirectoryRenamed: EntityTypes.LoadType failed.");
                        }
                        EntityTypes.Instance.Editor_ChangeAllReferencesToType(byName, entityType);
                        if (JxResApp.Instance.ResourceObjectEditor != null)
                        {
                            ResourceObjectEditor resourceObjectEditor = JxResApp.Instance.ResourceObjectEditor;
                            if (!resourceObjectEditor.EditModeActive && string.Compare(resourceObjectEditor.FileName, text4, true) == 0)
                            {
                                this.SelectNodeByPath(text);
                                this.SelectNodeByPath(text4);
                            }
                        }
                    }
                }
            }
        }

        public bool DoSelectPath(string path)
        {
            TreeNode treeNode = this.FindNodeByPath(path);
            if (treeNode == null)
            {
                return false;
            }
            this.ResourcesView.SelectedNode = treeNode;
            return true;
        }

        public bool TryBeginEditMode()
        {
            ResourceObjectEditor resourceObjectEditor = JxResApp.Instance.ResourceObjectEditor;
            if (resourceObjectEditor != null && resourceObjectEditor.EditModeActive)
            {
                return false;
            }
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            if (selectedNode != null && selectedNode.Tag != null)
            {
                if (this.ResourceBeginEditMode != null)
                {
                    this.ResourceBeginEditMode(new EventArgs());
                }
                return true;
            }
            return false;
        }

        private void C()
        {
            ResourcesForm.nodesSizeDic.Clear();
            ResourcesForm.nodesLastWtDic.Clear();
            this.ResourcesView.BeginUpdate();
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            this.ResourcesView.Sort();
            this.ResourcesView.SelectedNode = selectedNode;
            this.ResourcesView.EndUpdate();
            ResourcesForm.nodesSizeDic.Clear();
            ResourcesForm.nodesLastWtDic.Clear();
        }

        private void A(ResourcesForm.MyTreeNode myTreeNode, bool flag)
        {
            myTreeNode.HideNode = (this.IsHideResource(myTreeNode.Text) || flag);
            foreach (TreeNode treeNode in myTreeNode.Nodes)
            {
                this.A((ResourcesForm.MyTreeNode)treeNode, myTreeNode.HideNode);
            }
        }
        public void UpdateHideNodes()
        {
            foreach (TreeNode treeNode in this.ResourcesView.Nodes)
            {
                this.A((ResourcesForm.MyTreeNode)treeNode, false);
            }
            this.ResourcesView.Invalidate();
        }
        private void B(ResourcesForm.MyTreeNode myTreeNode)
        {
            if (this.aTg == myTreeNode)
            {
                this.aTg = null;
            }
            this.multiSelectedList.Remove(myTreeNode);
        }
        private string d(string text)
        {
            return ToolsLocalization.Translate("ResourcesForm", text);
        }
        private void Form_Load(object obj, EventArgs eventArgs)
        {
            base.TabText = this.d(base.TabText);
        }
        private void b(ResourcesForm.MyTreeNode myTreeNode)
        {
            ResourcesForm.nodesSizeDic.Clear();
            ResourcesForm.nodesLastWtDic.Clear();
            this.ResourcesView.BeginUpdate();
            myTreeNode.Nodes.Clear();
            this.A(myTreeNode, ResourcesForm.GetNodePath((TreeNode)myTreeNode));
            myTreeNode.Expand();
            this.ResourcesView.SelectedNode = myTreeNode;
            this.ResourcesView.EndUpdate();
            ResourcesForm.nodesSizeDic.Clear();
            ResourcesForm.nodesLastWtDic.Clear();
        }
        public void UpdateFonts()
        {
            /*
            if (this.aTj == null)
            {
                this.aTj = this.aTO2.Font;
            }
            if (this.aTK != MainForm.fontTreeControl)
            {
                this.aTO2.Font = MainForm.GetFont(MainForm.fontTreeControl, this.aTj);
                this.aTK = MainForm.fontTreeControl;
            }
            //*/
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((long)msg.Msg == 256L && !this.aTL)
            {
                Keys keys = keyData & Keys.KeyCode;
                Keys modifiers = keyData & ~keys;
                /*
                if (MainForm.Instance.ToolsProcessKeyDownHotKeys(keys, modifiers, false))
                {
                    return true;
                }
                //*/
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void TreeViewSelect()
        {
            this.ResourcesView.Select();
        }

        private static void OnMenu_CloseEditing(object obj, EventArgs eventArgs)
        {
            /*
            ResourceObjectEditor resourceObjectEditor = ResourceEditorEngineApp.Instance.ResourceObjectEditor;
            if (resourceObjectEditor != null && resourceObjectEditor.EditModeActive)
            {
                resourceObjectEditor.EndEditMode();
            }
            //*/
        }
        private void C(object obj, EventArgs eventArgs)
        {
            this.TryBeginEditMode();
        }
        private void c(object obj, EventArgs eventArgs)
        {
            this.A(true);
        }
        private void D(object obj, EventArgs eventArgs)
        {
            this.A(false);
        }
        private void d(object obj, EventArgs eventArgs)
        {
            this.B();
        }
        private void E(object obj, EventArgs eventArgs)
        {
            if (ResourcesForm.sortBy != ResourcesForm.SortByItems.Name)
            {
                ResourcesForm.sortBy = ResourcesForm.SortByItems.Name;
                this.C();
            }
        }
        private void e(object obj, EventArgs eventArgs)
        {
            if (ResourcesForm.sortBy != ResourcesForm.SortByItems.Date)
            {
                ResourcesForm.sortBy = ResourcesForm.SortByItems.Date;
                this.C();
            }
        }
        private void F(object obj, EventArgs eventArgs)
        {
            if (ResourcesForm.sortBy != ResourcesForm.SortByItems.Type)
            {
                ResourcesForm.sortBy = ResourcesForm.SortByItems.Type;
                this.C();
            }
        }
        private void f(object obj, EventArgs eventArgs)
        {
            if (ResourcesForm.sortBy != ResourcesForm.SortByItems.Size)
            {
                ResourcesForm.sortBy = ResourcesForm.SortByItems.Size;
                this.C();
            }
        }
        private void G(object obj, EventArgs eventArgs)
        {
            ResourcesForm.sortByAscending = !ResourcesForm.sortByAscending;
            this.C();
        }

        private void ResourcesForm_Load(object sender, EventArgs e)
        {

        }
    }
}
