namespace JxDesign.UI
{
    partial class EntityTypesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityTypesForm));
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.XTS = new System.Windows.Forms.ToolStrip();
            this.IL16_treeView = new System.Windows.Forms.ImageList(this.components);
            this.IL16 = new System.Windows.Forms.ImageList(this.components);
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageObjects = new System.Windows.Forms.TabPage();
            this.tabPage3dModel = new System.Windows.Forms.TabPage();
            this.treeView3dModel = new System.Windows.Forms.TreeView();
            this.treeViewObjects = new System.Windows.Forms.TreeView();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tableLayoutPanel1.SuspendLayout();
            this.XTS.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageObjects.SuspendLayout();
            this.tabPage3dModel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Timer1
            // 
            this.Timer1.Interval = 1000;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.XTS, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(390, 543);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // XTS
            // 
            this.XTS.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.XTS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.tsbRefresh,
            this.toolStripSeparator1});
            this.XTS.Location = new System.Drawing.Point(0, 0);
            this.XTS.Name = "XTS";
            this.XTS.Size = new System.Drawing.Size(390, 25);
            this.XTS.Stretch = true;
            this.XTS.TabIndex = 1;
            this.XTS.Text = "toolStrip1";
            // 
            // IL16_treeView
            // 
            this.IL16_treeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL16_treeView.ImageStream")));
            this.IL16_treeView.TransparentColor = System.Drawing.Color.Transparent;
            this.IL16_treeView.Images.SetKeyName(0, "folder");
            this.IL16_treeView.Images.SetKeyName(1, "file");
            this.IL16_treeView.Images.SetKeyName(2, "file_");
            // 
            // IL16
            // 
            this.IL16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL16.ImageStream")));
            this.IL16.TransparentColor = System.Drawing.Color.Transparent;
            this.IL16.Images.SetKeyName(0, "file");
            this.IL16.Images.SetKeyName(1, "folder");
            this.IL16.Images.SetKeyName(2, "refresh");
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRefresh.Image")));
            this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbRefresh.Text = "刷新";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl.Controls.Add(this.tabPageObjects);
            this.tabControl.Controls.Add(this.tabPage3dModel);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ItemSize = new System.Drawing.Size(54, 24);
            this.tabControl.Location = new System.Drawing.Point(3, 28);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(384, 512);
            this.tabControl.TabIndex = 2;
            // 
            // tabPageObjects
            // 
            this.tabPageObjects.Controls.Add(this.treeViewObjects);
            this.tabPageObjects.Location = new System.Drawing.Point(4, 4);
            this.tabPageObjects.Margin = new System.Windows.Forms.Padding(1);
            this.tabPageObjects.Name = "tabPageObjects";
            this.tabPageObjects.Size = new System.Drawing.Size(376, 480);
            this.tabPageObjects.TabIndex = 0;
            this.tabPageObjects.Text = "Objects";
            this.tabPageObjects.UseVisualStyleBackColor = true;
            // 
            // tabPage3dModel
            // 
            this.tabPage3dModel.Controls.Add(this.treeView3dModel);
            this.tabPage3dModel.Location = new System.Drawing.Point(4, 4);
            this.tabPage3dModel.Margin = new System.Windows.Forms.Padding(1);
            this.tabPage3dModel.Name = "tabPage3dModel";
            this.tabPage3dModel.Size = new System.Drawing.Size(376, 480);
            this.tabPage3dModel.TabIndex = 1;
            this.tabPage3dModel.Text = "3D Model";
            this.tabPage3dModel.UseVisualStyleBackColor = true;
            // 
            // treeView3dModel
            // 
            this.treeView3dModel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView3dModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView3dModel.Location = new System.Drawing.Point(0, 0);
            this.treeView3dModel.Name = "treeView3dModel";
            this.treeView3dModel.Size = new System.Drawing.Size(376, 480);
            this.treeView3dModel.TabIndex = 0;
            // 
            // treeViewObjects
            // 
            this.treeViewObjects.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewObjects.Location = new System.Drawing.Point(0, 0);
            this.treeViewObjects.Name = "treeViewObjects";
            this.treeViewObjects.Size = new System.Drawing.Size(376, 480);
            this.treeViewObjects.TabIndex = 0;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // EntityTypesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 543);
            this.Controls.Add(this.tableLayoutPanel1);
            this.HideOnClose = true;
            this.Name = "EntityTypesForm";
            this.Text = "类型管理";
            this.Load += new System.EventHandler(this.EntityTypesForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.XTS.ResumeLayout(false);
            this.XTS.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageObjects.ResumeLayout(false);
            this.tabPage3dModel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip XTS;
        private System.Windows.Forms.ImageList IL16_treeView;
        private System.Windows.Forms.ImageList IL16;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageObjects;
        private System.Windows.Forms.TreeView treeViewObjects;
        private System.Windows.Forms.TabPage tabPage3dModel;
        private System.Windows.Forms.TreeView treeView3dModel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}