using System;
using System.IO;
using System.Linq;
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
using Jx.UI;
using Jx.Editors;
using Jx.FileSystem;
using Jx.EntitySystem;
using JxRes.Editors;

namespace JxRes.UI
{
    public partial class ResourcesForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public static readonly string DATAOBJECT_KEY = "Preferred DropEffect";
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

        [Config("JxRes", "hideDirectoriesAndFiles")]
        public static List<string> hideDirectoriesAndFiles = null;
        [Config("JxRes", "sortBy")]
        public static SortByItems sortBy = SortByItems.Name;
        [Config("JxRes", "sortByAscending")]
        public static bool sortByAscending = true;

        private ImageCache imageCache16;
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
            this.UpdateMultiSelectionList(ResourcesRoot);
            this.ResourcesView.Nodes.Add(this.ResourcesRoot);
            this.CreateTreeNodeForPath(ResourcesRoot, "");
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

        private void CreateTreeNodeForPath(MyTreeNode parent, string path)
        {
            string[] directories = VirtualDirectory.GetDirectories(path); 
            for (int i = 0; i < directories.Length; i++)
            {
                string d = directories[i];
                CreateTreeNodeForDirectory(parent, d);
            }

            string[] files = VirtualDirectory.GetFiles(path); 
            for (int j = 0; j < files.Length; j++)
            {
                string f = files[j];
                CreateTreeNodeForFile(parent, f);
            }
        }

        private bool IsHideResource(string name)
        {
            string nameLower = name.ToLower();
            return hideDirectoriesAndFiles.Exists((string hideItem) => nameLower.Contains(hideItem.ToLower()));
        }

        private void CreateTreeNodeForDirectory(MyTreeNode parent, string p)
        {
            string fileName = Path.GetFileName(p);
            MyTreeNode node = new MyTreeNode(fileName, VirtualDirectory.IsInArchive(p), this.IsHideResource(fileName) || parent.HideNode);
            node.Name = node.Text;
            parent.Nodes.Add(node);
            this.UpdateMultiSelectionList(node);
            this.CreateTreeNodeForPath(node, p);
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
            CreateTreeNodeForDirectory(parentNode, p);
        }

        private void CreateTreeNodeForFile(MyTreeNode parent, string path)
        {
            string fileName = Path.GetFileName(path);
            TreeNode treeNode = new MyTreeNode(fileName, VirtualFile.IsInArchive(path), this.IsHideResource(fileName) || parent.HideNode);
            treeNode.Name = treeNode.Text;
            treeNode.Tag = fileName;
            parent.Nodes.Add(treeNode);
            UpdateTreeNodeIcon(treeNode);
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
            this.CreateTreeNodeForFile(myTreeNode, fileName);
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
                    this.DeleteSelectedResources();
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
            if (((keyEventArgs.Control && keyEventArgs.KeyCode == Keys.V) || (keyEventArgs.Shift && keyEventArgs.KeyCode == Keys.Insert)) && this.IsClipboardHasData())
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
                this.ShowTreeNodeContextMenu(new Point(0, 0));
            }
        }

        private void OnTreeNodeMouseClick(object obj, TreeNodeMouseClickEventArgs treeNodeMouseClickEventArgs)
        {
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
                    string format = this.LocalizationTranslate("File \"{0}\" cannot be moved or copied because it's inside archive.");
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
            dataObject.SetData(DATAOBJECT_KEY, memoryStream);
            Clipboard.SetDataObject(dataObject);
        }

        private string A()
        {
            MyTreeNode treeNode = null;
            foreach (MyTreeNode current in multiSelectedList)
            {
                MyTreeNode px = (current.Tag == null) ? current : ((MyTreeNode)current.Parent);
                if (treeNode == null)
                {
                    treeNode = px;
                }
                else if (treeNode != px)
                {
                    treeNode = null;
                    break;
                }
            }
            if (treeNode != null)
                return GetNodePath(treeNode);

            return null;
        }

        private bool IsClipboardHasData()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            MemoryStream memoryStream = dataObject.GetData(DATAOBJECT_KEY) as MemoryStream;
            return memoryStream != null;
        }

        private void Copy(string sourcePath, string destPath)
        {
            string[] directories = Directory.GetDirectories(sourcePath);
            Directory.CreateDirectory(destPath); 
            for (int i = 0; i < directories.Length; i++)
            {
                string sourceSubDirectory = directories[i];
                Copy(sourceSubDirectory, Path.Combine(destPath, Path.GetFileName(sourceSubDirectory)));
            }
            string[] files = Directory.GetFiles(sourcePath);
            for (int j = 0; j < files.Length; j++)
            {
                string filePath = files[j];
                File.Copy(filePath, Path.Combine(destPath, Path.GetFileName(filePath)));
            }
        }

        private void b(string virtualPath)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            MemoryStream memoryStream = dataObject.GetData(DATAOBJECT_KEY) as MemoryStream;
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
                        this.Copy(text, text2);
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

        private void ShowEditModeActiveContextMenu(Point position)
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem value = new ToolStripMenuItem(LocalizationTranslate("结束编辑"), imageCache16["closed"], MenuCloseEditing_Click);
            contextMenuStrip.Items.Add(value);
            contextMenuStrip.Show(ResourcesView, position);
        }

        private void MenuNewResource_Click(object obj, EventArgs eventArgs)
        {
            Tuple<string, ResourceType> pair = (Tuple<string, ResourceType>)((ToolStripMenuItem)obj).Tag;
            this.CreateResourceType(pair.Item1, pair.Item2);
        }

        private void ShowTreeNodeContextMenu(Point point)
        { 
            if (IsResourceEditModeActive != null)
            {
                ActiveEventArgs activeEventArgs = new ActiveEventArgs();
                IsResourceEditModeActive(activeEventArgs);
                if (activeEventArgs.Active)
                {
                    this.ShowEditModeActiveContextMenu(point);
                    return;
                }
            }
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //contextMenuStrip.Font = MainForm.GetFont(MainForm.fontContextMenu, contextMenuStrip.Font);
            TreeNode node = this.ResourcesView.SelectedNode;
            if (node == null)
                return;
            
            string nodePath = GetNodePath(node);
            if (node.Tag == null)
            {
                ToolStripMenuItem menuNew = new ToolStripMenuItem(LocalizationTranslate("创建.."), imageCache16["add"]);
                ToolStripMenuItem menuNewFolder = new ToolStripMenuItem(LocalizationTranslate("文件夹"), imageCache16["folder"], 
                    delegate (object s, EventArgs e2)
                    {
                        this.CreateFolder(nodePath);
                    });
                menuNew.DropDownItems.Add(menuNewFolder);
                menuNew.DropDownItems.Add(new ToolStripSeparator());

                foreach (ResourceType resourceType in ResourceTypeManager.Instance.Types)
                {
                    if (resourceType.AllowNewResource)
                    {
                        string text = LocalizationTranslate(resourceType.DisplayName);
                        ToolStripMenuItem menuNewResourceType = new ToolStripMenuItem(text, resourceType.Icon, MenuNewResource_Click);
                        menuNewResourceType.Tag = new Tuple<string, ResourceType>(nodePath, resourceType);
                        menuNew.DropDownItems.Add(menuNewResourceType);
                    }
                }
                contextMenuStrip.Items.Add(menuNew);
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                ToolStripMenuItem menuOpenFolder = new ToolStripMenuItem(this.LocalizationTranslate("打开所在目录"), imageCache16["folder_open"], 
                    delegate (object s, EventArgs e2)
                    {
                        string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(nodePath);
                        Shell32Api.ShellExecuteEx(null, realPathByVirtual);
                    });
                contextMenuStrip.Items.Add(menuOpenFolder);
            }
            if (node.Tag != null)
            {   // is File
                string fileExt = Path.GetExtension(nodePath);
                if (!string.IsNullOrEmpty(fileExt))
                {
                    fileExt = fileExt.Substring(1);
                    ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(fileExt);
                    if (byExtension != null)
                    {
                        ToolStripMenuItem menuEditResource = new ToolStripMenuItem("编 辑(&E)", imageCache16["edit"], MenuEditResource_Click);
                        contextMenuStrip.Items.Add(menuEditResource);
                    }
                }
                ToolStripMenuItem menuOtherApp = new ToolStripMenuItem(
                    LocalizationTranslate("使用外部程序打开"), imageCache16["applications_other"], 
                    delegate (object s, EventArgs e2)
                    {
                        string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(nodePath);
                        Shell32Api.ShellExecuteEx(null, realPathByVirtual);
                    });
                contextMenuStrip.Items.Add(menuOtherApp);
            }
            if (node.Tag is string)
            {   // is File 
                string fileExt = Path.GetExtension(nodePath);
                if (!string.IsNullOrEmpty(fileExt))
                {
                    fileExt = fileExt.Substring(1);
                    ResourceType byExt = ResourceTypeManager.Instance.GetByExtension(fileExt);
                    if (byExt != null)
                    {
                        byExt.DoResourcesTreeContextMenu(nodePath, contextMenuStrip);
                    }
                }
            }
            if (this.multiSelectedList.Count != 0)
            {
                bool flag = false;
                foreach (MyTreeNode nd in multiSelectedList)
                {
                    if (nd.Parent == null)
                        flag = true;
                }

                contextMenuStrip.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem menuCut = new ToolStripMenuItem(LocalizationTranslate("剪 切(&T)"), imageCache16["cut"], MenuCut_Click);
                menuCut.Enabled = !flag;
                contextMenuStrip.Items.Add(menuCut);

                ToolStripMenuItem menuCopy = new ToolStripMenuItem(LocalizationTranslate("复 制(&C)"), imageCache16["copy"], MenuCopy_Click);
                contextMenuStrip.Items.Add(menuCopy);
                if (node.Tag == null)
                {   // is Folder
                    string directoryToPaste = null;
                    if (this.IsClipboardHasData())
                    {
                        directoryToPaste = this.A();
                    }
                    ToolStripMenuItem menuPaste = new ToolStripMenuItem(LocalizationTranslate("粘 贴(&P)"), imageCache16["paste"], 
                        delegate (object s, EventArgs e2)
                        {
                            this.b(directoryToPaste);
                        });
                    menuPaste.Enabled = (directoryToPaste != null);
                    contextMenuStrip.Items.Add(menuPaste);
                }
                contextMenuStrip.Items.Add(new ToolStripSeparator());


                ToolStripMenuItem menuDelete = new ToolStripMenuItem(LocalizationTranslate("删 除(&D)"), imageCache16["delete"], MenuDelete_Click);
                menuDelete.Enabled = !flag;
                contextMenuStrip.Items.Add(menuDelete);

                ToolStripMenuItem menuRename = new ToolStripMenuItem(LocalizationTranslate("重命名(&N)"), imageCache16["rename"], delegate (object s, EventArgs e2)
                {
                    if (node != this.ResourcesView.SelectedNode)
                        return;
                    
                    if (!node.IsEditing)
                        node.BeginEdit();
                });
                menuRename.Enabled = (this.multiSelectedList.Count == 1 && !flag);
                contextMenuStrip.Items.Add(menuRename);
            }
            contextMenuStrip.Items.Add(new ToolStripSeparator());

            CreateSortingMenu(contextMenuStrip);
            ToolStripMenuItem menuRefresh = new ToolStripMenuItem(LocalizationTranslate("刷 新(&R)"), imageCache16["refresh"], delegate (object s, EventArgs e2)
            {
                RefreshTreeNode((MyTreeNode)node);
            });
            contextMenuStrip.Items.Add(menuRefresh);
            contextMenuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem menuProperties = new ToolStripMenuItem(LocalizationTranslate("属 性(&P)"), imageCache16["properties"], delegate (object s, EventArgs e2)
            {
                string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(nodePath);
                Shell32Api.ShellExecuteEx("properties", realPathByVirtual);
            });
            contextMenuStrip.Items.Add(menuProperties);
            
            foreach (ResourceEditorAddon current3 in AddonManager.Instance.Addons)
            {
                current3.OnShowContextMenuOfResourcesTree(contextMenuStrip);
            }

            contextMenuStrip.Show(this.ResourcesView, point);
        }

        private void ShowTreeContextMenu(Point point)
        {
            if (IsResourceEditModeActive != null)
            {
                ActiveEventArgs activeEventArgs = new ActiveEventArgs();
                this.IsResourceEditModeActive(activeEventArgs);
                if (activeEventArgs.Active)
                {
                    this.ShowEditModeActiveContextMenu(point);
                    return;
                }
            }
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //contextMenuStrip.Font = MainForm.GetFont(MainForm.fontContextMenu, contextMenuStrip.Font);
            CreateSortingMenu(contextMenuStrip);
            contextMenuStrip.Show(ResourcesView, point);
        }

        private void CreateSortingMenu(ContextMenuStrip contextMenuStrip)
        {
            ToolStripMenuItem menuSortBy = new ToolStripMenuItem(LocalizationTranslate("排序方式"), SortByIcon);

            ToolStripMenuItem menuSortByName = new ToolStripMenuItem(LocalizationTranslate("名称"), imageCache16["sort_by_name"], new EventHandler(SortTreeNodeByName));
            menuSortByName.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Name);
            menuSortBy.DropDownItems.Add(menuSortByName);

            ToolStripMenuItem menuSortByDate = new ToolStripMenuItem(this.LocalizationTranslate("修改日期"), imageCache16["sort_by_date"], new EventHandler(SortTreeNodeByDate));
            menuSortByDate.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Date);
            menuSortBy.DropDownItems.Add(menuSortByDate);

            ToolStripMenuItem menuSortByType = new ToolStripMenuItem(this.LocalizationTranslate("类型"), imageCache16["sort_by_type"], new EventHandler(SortTreeNodeByType));
            menuSortByType.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Type);
            menuSortBy.DropDownItems.Add(menuSortByType);

            ToolStripMenuItem menuSortBySize = new ToolStripMenuItem(this.LocalizationTranslate("大小"), imageCache16["sort_by_size"], new EventHandler(SortTreeNodeBySize));
            menuSortBySize.Checked = (ResourcesForm.sortBy == ResourcesForm.SortByItems.Size);
            menuSortBy.DropDownItems.Add(menuSortBySize);

            menuSortBy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem menuSortingMethod = new ToolStripMenuItem(this.LocalizationTranslate("升序"), SortByIcon, new EventHandler(ChangeTreeNodeSortingMethod));
            menuSortingMethod.Checked = ResourcesForm.sortByAscending;
            menuSortBy.DropDownItems.Add(menuSortingMethod);
            contextMenuStrip.Items.Add(menuSortBy);
        }

        private Image SortByIcon
        {
            get { return ResourcesForm.sortByAscending ? imageCache16["sort_ascend"] : imageCache16["sort_descend"]; }
        }

        private void OnTreeMouseClick(object obj, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                TreeNode nodeAt = this.ResourcesView.GetNodeAt(mouseEventArgs.Location);
                if (nodeAt != null)
                    ResourcesView.SelectedNode = nodeAt;
                
                ShowTreeNodeContextMenu(mouseEventArgs.Location);
            }
        }
        private void OnTreeMouseUp(object obj, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right && this.ResourcesView.GetNodeAt(mouseEventArgs.Location) == null)
            {
                ShowTreeContextMenu(mouseEventArgs.Location);
            }
        }

        private void CreateResourceType(string directory, ResourceType resourceType)
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

        private List<MyTreeNode> A(List<MyTreeNode> list)
        {
            List<MyTreeNode> list2 = new List<MyTreeNode>(list.Count);
            foreach (MyTreeNode current in list)
            {
                bool flag = false;
                for (MyTreeNode myTreeNode = (MyTreeNode)current.Parent; myTreeNode != null; myTreeNode = (MyTreeNode)myTreeNode.Parent)
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
 
        private void DeleteSelectedResources()
        {
            if (this.multiSelectedList.Count == 0)
            {
                return;
            }
            if (this.IsResourceEditModeActive != null)
            {
                ActiveEventArgs activeEventArgs = new ActiveEventArgs();
                this.IsResourceEditModeActive(activeEventArgs);
                if (activeEventArgs.Active)
                {
                    Log.Warning(LocalizationTranslate("Need to leave the editing mode first."));
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
                string format = LocalizationTranslate("确定删除 \"{0}\"?");
                string arg = ResourcesForm.GetNodePath((TreeNode)this.multiSelectedList[0]);
                text = string.Format(format, arg);
            }
            else
            {
                string format2 = LocalizationTranslate("确定删除 {0} 个项目?");
                text = string.Format(format2, this.multiSelectedList.Count);
            }
            string caption = ToolsLocalization.Translate("Various", "JxRes");
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

            MyTreeNode myTreeNode = (MyTreeNode)TreeViewUtils.GetNeedSelectNodeAfterRemoveNode(this.ResourcesView.SelectedNode);
            List<MyTreeNode> list = this.A(this.multiSelectedList);
            foreach (MyTreeNode current in list)
            {
                string nodePath = GetNodePath(current);
                if (current.Tag != null)
                {
                    if (!this.c(nodePath))
                    {
                        return;
                    }
                }
                else if (!this.C(nodePath))
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

        private void CreateFolder(string parentDirectory)
        {
            MyTreeNode parentNode = FindNodeByPath(parentDirectory);
            Trace.Assert(parentNode != null);

            List<string> currentFolders = parentNode.Nodes.OfType<MyTreeNode>().Select(_node => _node.Name).ToList();

            int folderIndex = 1;
            string folderName;
            while (true)
            {
                folderName = string.Format("新文件夹 ({0})", folderIndex);
                if (currentFolders.Contains(folderName))
                {
                    folderIndex++;
                    continue;
                }
                break;
            }

            string path = Path.Combine(VirtualFileSystem.GetRealPathByVirtual(parentDirectory), folderName);
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                return;
            }
            TreeNode newNode = new MyTreeNode(folderName, VirtualDirectory.IsInArchive(parentDirectory), this.IsHideResource(folderName) || parentNode.HideNode);
            newNode.Name = newNode.Text;
            this.UpdateTreeNodeIcon(newNode);
            parentNode.Nodes.Add(newNode);
            ResourcesView.SelectedNode = newNode;
            newNode.BeginEdit();
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
            this.CreateFolder(text);
        }

        public void BeginNewResourceCreation(ResourceType type)
        {
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            if (selectedNode == null)
                return;
   
            string p = GetNodePath(selectedNode);
            if (selectedNode.Tag != null)
                p = Path.GetDirectoryName(p);
            
            this.CreateResourceType(p, type);
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

        private bool A(MyTreeNode myTreeNode)
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
                UpdateMultiSelectionList(evtNode);
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

        private void UpdateMultiSelectionList(MyTreeNode node)
        {
            List<MyTreeNode> list = new List<MyTreeNode>(multiSelectedList);
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
                this.UpdateMultiSelectionList((MyTreeNode)null);
            }

            ResourceObjectEditor resourceObjectEditor = JxResApp.Instance.ResourceObjectEditor;
            if (resourceObjectEditor != null)
            {
                if (resourceObjectEditor.EditModeActive)
                {
                    this.UpdateMultiSelectionList((MyTreeNode)null);
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
            string virtualPathByRealNew = VirtualFileSystem.GetVirtualPathByReal(realPath);
            string virtualPathByRealOld = VirtualFileSystem.GetVirtualPathByReal(oldRealPath);

            string extNew = Path.GetExtension(virtualPathByRealNew);
            string extOld = Path.GetExtension(virtualPathByRealOld);
                        
            if (!string.IsNullOrEmpty(extOld) && !string.IsNullOrEmpty(extNew) && string.Compare(extOld, extNew, true) == 0)
            {
                extNew = extNew.Substring(1);
                ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(extNew);
                if (byExtension != null)
                    byExtension.OnResourceRenamed(virtualPathByRealNew, virtualPathByRealOld);
            }
            else
            {
                if (!string.IsNullOrEmpty(extOld))
                {
                    extOld = extOld.Substring(1);
                    ResourceType byExtensionOld = ResourceTypeManager.Instance.GetByExtension(extOld);
                    if (byExtensionOld != null)
                        byExtensionOld.DoUnloadResource(virtualPathByRealOld);
                }
                if (!string.IsNullOrEmpty(extNew))
                {
                    extNew = extNew.Substring(1);
                    ResourceType byExtensionNew = ResourceTypeManager.Instance.GetByExtension(extNew);
                    if (byExtensionNew != null)
                        byExtensionNew.DoLoadResource(virtualPathByRealNew);
                }
            }
            TreeNode treeNodeOld = this.FindNodeByPath(virtualPathByRealOld);
            if (treeNodeOld != null)
            {
                string fileName = Path.GetFileName(realPath);
                treeNodeOld.Text = fileName;
                treeNodeOld.Name = fileName;
                UpdateTreeNodeIcon(treeNodeOld);
            }
            if (!VirtualFile.Exists(virtualPathByRealNew) && VirtualDirectory.Exists(virtualPathByRealNew))
            {
                OnDirectoryRenamed(virtualPathByRealNew, virtualPathByRealOld);
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

        private void OnDirectoryRenamed(string virtualPathByRealNew, string virtualPathByRealOld)
        {
            string[] files = Directory.GetFiles(VirtualFileSystem.GetRealPathByVirtual(virtualPathByRealNew), "*.type", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string fileNameNoExt = file.Substring(5);
                TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(fileNameNoExt);
                if (textBlock != null && textBlock.Children.Count == 1)
                {
                    string typeName = textBlock.Children[0].Data;
                    EntityType byName = EntityTypes.Instance.GetByName(typeName);
                    if (byName != null)
                    {
                        EntityTypes.Instance.DestroyType(byName);
                        EntityType entityType = EntityTypes.Instance.LoadTypeFromFile(fileNameNoExt);
                        if (entityType == null)
                        {
                            Log.Fatal("ResourceForm: OnDirectoryRenamed: EntityTypes.LoadType failed.");
                            return;
                        }
                        EntityTypes.Instance.Editor_ChangeAllReferencesToType(byName, entityType);
                        if (JxResApp.Instance.ResourceObjectEditor != null)
                        {
                            ResourceObjectEditor resourceObjectEditor = JxResApp.Instance.ResourceObjectEditor;
                            if (!resourceObjectEditor.EditModeActive && string.Compare(resourceObjectEditor.FileName, fileNameNoExt, true) == 0)
                            {
                                SelectNodeByPath(virtualPathByRealNew);
                                SelectNodeByPath(fileNameNoExt);
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
                return false;

            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            if (selectedNode != null && selectedNode.Tag != null)
            {
                if (this.ResourceBeginEditMode != null)
                {
                    ResourceBeginEditMode(new EventArgs());
                }
                return true;
            }
            return false;
        }

        private void SortTreeNodes()
        {
            nodesSizeDic.Clear();
            nodesLastWtDic.Clear();
            this.ResourcesView.BeginUpdate();
            TreeNode selectedNode = this.ResourcesView.SelectedNode;
            this.ResourcesView.Sort();
            this.ResourcesView.SelectedNode = selectedNode;
            this.ResourcesView.EndUpdate();
            nodesSizeDic.Clear();
            nodesLastWtDic.Clear();
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

        private string LocalizationTranslate(string text)
        {
            return ToolsLocalization.Translate("JxRes", text);
        }

        private void Form_Load(object obj, EventArgs eventArgs)
        {
            base.TabText = LocalizationTranslate(base.TabText);
        }

        private void RefreshTreeNode(MyTreeNode treeNode)
        {
            nodesSizeDic.Clear();
            nodesLastWtDic.Clear();
            this.ResourcesView.BeginUpdate();
            treeNode.Nodes.Clear();
            this.CreateTreeNodeForPath(treeNode, ResourcesForm.GetNodePath((TreeNode)treeNode));
            treeNode.Expand();
            this.ResourcesView.SelectedNode = treeNode;
            this.ResourcesView.EndUpdate();
            nodesSizeDic.Clear();
            nodesLastWtDic.Clear();
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

        private static void MenuCloseEditing_Click(object obj, EventArgs eventArgs)
        {
            //*
            ResourceObjectEditor resourceObjectEditor = JxResApp.Instance.ResourceObjectEditor;
            if (resourceObjectEditor != null && resourceObjectEditor.EditModeActive)
            {
                resourceObjectEditor.EndEditMode();
            }
            //*/
        }
        private void MenuEditResource_Click(object obj, EventArgs eventArgs)
        {
            this.TryBeginEditMode();
        }
        private void MenuCut_Click(object obj, EventArgs eventArgs)
        {
            this.A(true);
        }

        private void MenuCopy_Click(object obj, EventArgs eventArgs)
        {
            this.A(false);
        }
        private void MenuDelete_Click(object obj, EventArgs eventArgs)
        {
            this.DeleteSelectedResources();
        }
        private void SortTreeNodeByName(object obj, EventArgs eventArgs)
        {
            if (sortBy != SortByItems.Name)
            {
                sortBy = SortByItems.Name;
                SortTreeNodes();
            }
        }
        private void SortTreeNodeByDate(object obj, EventArgs eventArgs)
        {
            if (sortBy != SortByItems.Date)
            {
                sortBy = SortByItems.Date;
                SortTreeNodes();
            }
        }
        private void SortTreeNodeByType(object obj, EventArgs eventArgs)
        {
            if (sortBy != SortByItems.Type)
            {
                sortBy = SortByItems.Type;
                SortTreeNodes();
            }
        }

        private void SortTreeNodeBySize(object obj, EventArgs eventArgs)
        {
            if (sortBy != SortByItems.Size)
            {
                sortBy = SortByItems.Size;
                SortTreeNodes();
            }
        }

        private void ChangeTreeNodeSortingMethod(object obj, EventArgs eventArgs)
        {
            sortByAscending = !sortByAscending;

            ToolStripMenuItem mi = obj as ToolStripMenuItem;
            mi.Image = SortByIcon;
            mi.Text = sortByAscending ? "升序" : "降序";
            
            SortTreeNodes();
        }

        private void ResourcesForm_Load(object sender, EventArgs e)
        {
            imageCache16 = new ImageCache(ILCache16);
        }
    }
}
