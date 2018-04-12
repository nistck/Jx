using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.Ext;
using Jx.EntitySystem;
using Jx.Editors.Properties;
 
namespace Jx.Editors
{
    public partial class ChooseEntityForm : Form
    {
        private class AT : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode treeNode = (TreeNode)x;
                TreeNode treeNode2 = (TreeNode)y;
                if (treeNode.Nodes.Count != 0 && treeNode2.Nodes.Count == 0)
                {
                    return -1;
                }
                if (treeNode.Nodes.Count == 0 && treeNode2.Nodes.Count != 0)
                {
                    return 1;
                }
                return treeNode.Text.CompareTo(treeNode2.Text);
            }
        }

        private static readonly Size ZERO = new Size(0, 0);         
 
        private Entity BJE;
        private System.Type BJe;
        private bool BJF;
        private TreeNode BJf;

        [Config("ChooseEntityForm", "lastWindowPosition")]
        public static Point lastWindowPosition;
        [Config("ChooseEntityForm", "lastWindowSize")]
        public static Size lastWindowSize;

        public ChooseEntityForm(Entity ownerEntity, System.Type entityClassType, bool allowChooseNull, Entity currentEntity)
        {
            InitializeComponent();

            //EngineApp.Instance.Config.RegisterClassParameters(typeof(ChooseResourceForm));

            this.BJf = null;
            this.BJE = ownerEntity;
            this.BJe = entityClassType;
            this.BJF = allowChooseNull;
            this.A(currentEntity);
        }


        private void A(Entity entity)
        {
            this.BJb.BeginUpdate();
            this.BJb.Nodes.Clear();
            TreeNode treeNode = null;
            foreach (Entity current in Entities.Instance.EntitiesCollection)
            {
                if (!current.Editor_IsExcludedFromWorld() && (!(this.BJe != null) || this.BJe.IsAssignableFrom(current.GetType())))
                {
                    bool flag = !string.IsNullOrEmpty(current.Name);
                    if (this.BJE != null)
                    {
                        LogicEntityClass logicEntityClass = current as LogicEntityClass;
                        if (logicEntityClass != null && logicEntityClass.EntityClassInfo != null && logicEntityClass.EntityClassInfo.EntityClassType.IsAssignableFrom(this.BJE.GetType()))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        TreeNode treeNode2 = new TreeNode(current.ToString(), 2, 2);
                        treeNode2.Name = treeNode2.Text;
                        treeNode2.Tag = current;
                        this.BJb.Nodes.Add(treeNode2);
                        if (current == entity)
                        {
                            treeNode = treeNode2;
                        }
                    }
                }
            }
            this.BJb.TreeViewNodeSorter = new AT();
            this.BJb.Sort();
            if (treeNode != null)
            {
                //TreeViewUtils.ExpandAllPathToNode(treeNode);
                this.BJb.SelectedNode = treeNode;
            }
            if (this.BJF)
            {
                this.BJf = new TreeNode(ToolsLocalization.Translate("ChooseEntityForm", "(Null)"), 1, 1);
                this.BJf.Name = this.BJf.Text;
                this.BJb.Nodes.Add(this.BJf);
                if (entity == null)
                {
                    this.BJb.SelectedNode = this.BJf;
                }
            }
            this.BJb.EndUpdate();
        }


        private void treeNodeMouseDoubleClick(object obj, TreeNodeMouseClickEventArgs treeNodeMouseClickEventArgs)
        {
            if (treeNodeMouseClickEventArgs.Node != this.BJb.SelectedNode)
            {
                return;
            }
            if (this.buttonConfirm.Enabled)
            {
                base.DialogResult = DialogResult.OK;
                base.Close();
            }
        }
        private void keyDown(object obj, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Return && this.buttonConfirm.Enabled)
            {
                base.DialogResult = DialogResult.OK;
                base.Close();
            }
        }
        private void afterSelect(object obj, TreeViewEventArgs treeViewEventArgs)
        {
            if (treeViewEventArgs.Node == null)
            {
                this.buttonConfirm.Enabled = false;
                return;
            }
            if (this.BJF && treeViewEventArgs.Node == this.BJf)
            {
                this.buttonConfirm.Enabled = true;
                return;
            }
            this.buttonConfirm.Enabled = (treeViewEventArgs.Node.Tag != null);
        }

        private void ChooseEntityForm_Load(object sender, EventArgs e)
        {
            initImageList();
            this.BJb.ImageList = this.BJD;

            if (lastWindowSize != ZERO)
            {
                base.Location = new Point(lastWindowPosition.X, lastWindowPosition.Y);
                base.Size = new Size(lastWindowSize.Width, lastWindowSize.Height);
                base.StartPosition = FormStartPosition.Manual;
            }

            this.BJb.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.treeNodeMouseDoubleClick);
            this.BJb.AfterSelect += new TreeViewEventHandler(this.afterSelect);
            this.BJb.KeyDown += new KeyEventHandler(this.keyDown);
        }

        private void initImageList()
        {
            this.BJD.Images.Add("Folder_16.png", Resources.folder_16);
            this.BJD.Images.Add("File_16.png", Resources.file_16);
            this.BJD.Images.Add("Entity_16.png", Resources.entity_16);
        }

        public Entity Entity
        {
            get
            { 
                if (this.BJF && this.BJb.SelectedNode == this.BJf)
                {
                    return null;
                }
                return (Entity)this.BJb.SelectedNode.Tag;
            }
        }

        private void ChooseEntityForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lastWindowPosition = new Point(Location.X, Location.Y);
            lastWindowSize = new Size(Size.Width, Size.Height);
        }
    }
}
