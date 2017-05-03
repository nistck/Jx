namespace Jx.UI.Forms
{
    partial class PropertiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesForm));
            this.SplitView = new System.Windows.Forms.SplitContainer();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jxPropertyGrid = new Jx.UI.Controls.PGEx.JxPropertyGrid();
            this.IL16 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SplitView)).BeginInit();
            this.SplitView.Panel1.SuspendLayout();
            this.SplitView.SuspendLayout();
            this.contextMenuStripPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitView
            // 
            this.SplitView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitView.Location = new System.Drawing.Point(0, 0);
            this.SplitView.Name = "SplitView";
            this.SplitView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitView.Panel1
            // 
            this.SplitView.Panel1.Controls.Add(this.jxPropertyGrid);
            this.SplitView.Panel2MinSize = 12;
            this.SplitView.Size = new System.Drawing.Size(284, 428);
            this.SplitView.SplitterDistance = 330;
            this.SplitView.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 20;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // contextMenuStripPropertyGrid
            // 
            this.contextMenuStripPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem});
            this.contextMenuStripPropertyGrid.Name = "contextMenuStrip1";
            this.contextMenuStripPropertyGrid.Size = new System.Drawing.Size(121, 26);
            this.contextMenuStripPropertyGrid.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripPropertyGrid_Opening);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.resetToolStripMenuItem.Text = "重 置(&R)";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // jxPropertyGrid
            // 
            this.jxPropertyGrid.ContextMenuStrip = this.contextMenuStripPropertyGrid;
            // 
            // 
            // 
            this.jxPropertyGrid.DocCommentDescription.AutoEllipsis = true;
            this.jxPropertyGrid.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.jxPropertyGrid.DocCommentDescription.Location = new System.Drawing.Point(3, 19);
            this.jxPropertyGrid.DocCommentDescription.Name = "";
            this.jxPropertyGrid.DocCommentDescription.Size = new System.Drawing.Size(278, 36);
            this.jxPropertyGrid.DocCommentDescription.TabIndex = 1;
            this.jxPropertyGrid.DocCommentImage = null;
            // 
            // 
            // 
            this.jxPropertyGrid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.jxPropertyGrid.DocCommentTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.jxPropertyGrid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.jxPropertyGrid.DocCommentTitle.Name = "";
            this.jxPropertyGrid.DocCommentTitle.Size = new System.Drawing.Size(278, 16);
            this.jxPropertyGrid.DocCommentTitle.TabIndex = 0;
            this.jxPropertyGrid.DocCommentTitle.UseMnemonic = false;
            this.jxPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jxPropertyGrid.LabelRatio = 6.1304347826086953D;
            this.jxPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.jxPropertyGrid.Name = "jxPropertyGrid";
            this.jxPropertyGrid.ReadOnly = true;
            this.jxPropertyGrid.Size = new System.Drawing.Size(284, 330);
            this.jxPropertyGrid.TabIndex = 0;
            // 
            // 
            // 
            this.jxPropertyGrid.ToolStrip.AccessibleName = "工具栏";
            this.jxPropertyGrid.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.jxPropertyGrid.ToolStrip.AllowMerge = false;
            this.jxPropertyGrid.ToolStrip.AutoSize = false;
            this.jxPropertyGrid.ToolStrip.CanOverflow = false;
            this.jxPropertyGrid.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.jxPropertyGrid.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.jxPropertyGrid.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.jxPropertyGrid.ToolStrip.Name = "";
            this.jxPropertyGrid.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.jxPropertyGrid.ToolStrip.Size = new System.Drawing.Size(284, 25);
            this.jxPropertyGrid.ToolStrip.TabIndex = 1;
            this.jxPropertyGrid.ToolStrip.TabStop = true;
            this.jxPropertyGrid.ToolStrip.Text = "PropertyGridToolBar";
            this.jxPropertyGrid.GridItemDoubleClick += new System.EventHandler(this.propertyGrid1_GridItemDoubleClick);
            this.jxPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.jxPropertyGrid_PropertyValueChanged);
            this.jxPropertyGrid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
            // 
            // IL16
            // 
            this.IL16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL16.ImageStream")));
            this.IL16.TransparentColor = System.Drawing.Color.Transparent;
            this.IL16.Images.SetKeyName(0, "reset");
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 428);
            this.Controls.Add(this.SplitView);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.HideOnClose = true;
            this.Name = "PropertiesForm";
            this.Text = "属性编辑器";
            this.Load += new System.EventHandler(this.PropertiesForm_Load);
            this.SplitView.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitView)).EndInit();
            this.SplitView.ResumeLayout(false);
            this.contextMenuStripPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitView;
        private Controls.PGEx.JxPropertyGrid jxPropertyGrid;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ImageList IL16;
    }
}