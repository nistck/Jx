namespace JxDesign.UI
{
    partial class EntitiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntitiesForm));
            this.treeViewEntities = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.XTS = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCreateLayer = new System.Windows.Forms.ToolStripButton();
            this.tsmiDeleteLayer = new System.Windows.Forms.ToolStripButton();
            this.tsmiEditLayer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.IL16 = new System.Windows.Forms.ImageList(this.components);
            this.ILtreeView = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tsmiDelete = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.XTS.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewEntities
            // 
            this.treeViewEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewEntities.HideSelection = false;
            this.treeViewEntities.ItemHeight = 16;
            this.treeViewEntities.Location = new System.Drawing.Point(3, 28);
            this.treeViewEntities.Name = "treeViewEntities";
            this.treeViewEntities.Size = new System.Drawing.Size(338, 512);
            this.treeViewEntities.TabIndex = 0;
            this.treeViewEntities.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewEntities_AfterLabelEdit);
            this.treeViewEntities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewEntities_AfterSelect);
            this.treeViewEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewEntities_MouseDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeViewEntities, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.XTS, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(344, 543);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // XTS
            // 
            this.XTS.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.XTS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.tsbRefresh,
            this.toolStripSeparator2,
            this.tsmiCreateLayer,
            this.tsmiDeleteLayer,
            this.tsmiEditLayer,
            this.toolStripSeparator3,
            this.tsmiDelete});
            this.XTS.Location = new System.Drawing.Point(0, 0);
            this.XTS.Name = "XTS";
            this.XTS.Size = new System.Drawing.Size(344, 25);
            this.XTS.TabIndex = 1;
            this.XTS.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRefresh.Image")));
            this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbRefresh.Text = "刷新视图";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsmiCreateLayer
            // 
            this.tsmiCreateLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsmiCreateLayer.Image = ((System.Drawing.Image)(resources.GetObject("tsmiCreateLayer.Image")));
            this.tsmiCreateLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiCreateLayer.Name = "tsmiCreateLayer";
            this.tsmiCreateLayer.Size = new System.Drawing.Size(23, 22);
            this.tsmiCreateLayer.Text = "创建Layer";
            this.tsmiCreateLayer.Click += new System.EventHandler(this.tsmiCreateLayer_Click);
            // 
            // tsmiDeleteLayer
            // 
            this.tsmiDeleteLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsmiDeleteLayer.Image = ((System.Drawing.Image)(resources.GetObject("tsmiDeleteLayer.Image")));
            this.tsmiDeleteLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiDeleteLayer.Name = "tsmiDeleteLayer";
            this.tsmiDeleteLayer.Size = new System.Drawing.Size(23, 22);
            this.tsmiDeleteLayer.Text = "删除Layer";
            this.tsmiDeleteLayer.Click += new System.EventHandler(this.tsmiDeleteLayer_Click);
            // 
            // tsmiEditLayer
            // 
            this.tsmiEditLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsmiEditLayer.Image = ((System.Drawing.Image)(resources.GetObject("tsmiEditLayer.Image")));
            this.tsmiEditLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiEditLayer.Name = "tsmiEditLayer";
            this.tsmiEditLayer.Size = new System.Drawing.Size(23, 22);
            this.tsmiEditLayer.Text = "编辑Layer";
            this.tsmiEditLayer.Click += new System.EventHandler(this.tsmiEditLayer_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // IL16
            // 
            this.IL16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL16.ImageStream")));
            this.IL16.TransparentColor = System.Drawing.Color.Transparent;
            this.IL16.Images.SetKeyName(0, "refresh");
            this.IL16.Images.SetKeyName(1, "layer_delete");
            this.IL16.Images.SetKeyName(2, "layer_edit");
            this.IL16.Images.SetKeyName(3, "layer_new");
            this.IL16.Images.SetKeyName(4, "delete");
            // 
            // ILtreeView
            // 
            this.ILtreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ILtreeView.ImageStream")));
            this.ILtreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.ILtreeView.Images.SetKeyName(0, "folder");
            this.ILtreeView.Images.SetKeyName(1, "file");
            this.ILtreeView.Images.SetKeyName(2, "entity");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsmiDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsmiDelete.Image")));
            this.tsmiDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(23, 22);
            this.tsmiDelete.Text = "删除Entity";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // EntitiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 543);
            this.Controls.Add(this.tableLayoutPanel1);
            this.HideOnClose = true;
            this.Name = "EntitiesForm";
            this.Text = "实体管理";
            this.Load += new System.EventHandler(this.EntitiesForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.XTS.ResumeLayout(false);
            this.XTS.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewEntities;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip XTS;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ImageList IL16;
        private System.Windows.Forms.ImageList ILtreeView;
        private System.Windows.Forms.ToolStripButton tsmiCreateLayer;
        private System.Windows.Forms.ToolStripButton tsmiDeleteLayer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsmiEditLayer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripButton tsmiDelete;
    }
}