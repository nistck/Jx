namespace JxRes.UI
{
    partial class ResourcesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourcesForm));
            this.ResourcesView = new System.Windows.Forms.TreeView();
            this.IL16 = new System.Windows.Forms.ImageList(this.components);
            this.aTP = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ResourcesView
            // 
            this.ResourcesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResourcesView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.ResourcesView.ImageIndex = 0;
            this.ResourcesView.ImageList = this.IL16;
            this.ResourcesView.Location = new System.Drawing.Point(0, 0);
            this.ResourcesView.Name = "ResourcesView";
            this.ResourcesView.SelectedImageIndex = 0;
            this.ResourcesView.Size = new System.Drawing.Size(263, 387);
            this.ResourcesView.TabIndex = 0;
            this.ResourcesView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnBeforeNodeLabelEdit);
            this.ResourcesView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnAfterNodeLabelEdit);
            this.ResourcesView.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.OnDrawTreeNode);
            this.ResourcesView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnTreeBeforeSelect);
            this.ResourcesView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeAfterSelect);
            this.ResourcesView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeNodeMouseClick);
            this.ResourcesView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnTreeKeyDown);
            this.ResourcesView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnTreeKeyUp);
            this.ResourcesView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnTreeMouseClick);
            this.ResourcesView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnTreeMouseUp);
            // 
            // IL16
            // 
            this.IL16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL16.ImageStream")));
            this.IL16.TransparentColor = System.Drawing.Color.Transparent;
            this.IL16.Images.SetKeyName(0, "folder_16.png");
            this.IL16.Images.SetKeyName(1, "file_16.png");
            // 
            // aTP
            // 
            this.aTP.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.aTP.ImageSize = new System.Drawing.Size(16, 16);
            this.aTP.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // ResourcesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 387);
            this.Controls.Add(this.ResourcesView);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.HideOnClose = true;
            this.Name = "ResourcesForm";
            this.Text = "资源管理器";
            this.Load += new System.EventHandler(this.ResourcesForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView ResourcesView;
        private System.Windows.Forms.ImageList aTP;
        private System.Windows.Forms.ImageList IL16;
        private System.Windows.Forms.Timer timer1;
    }
}