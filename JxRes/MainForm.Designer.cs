namespace JxRes
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.dddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResourcesForm = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropertiesForm = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiContentForm = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOthers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConsoleForm = new System.Windows.Forms.ToolStripMenuItem();
            this.addonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.UIStatus_TIMER = new System.Windows.Forms.Timer(this.components);
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vsToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(this.components);
            this.vS2015LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();
            this.vS2015BlueTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            this.vS2015DarkTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            this.timerEntitySystemWorld = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 593);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(963, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dddToolStripMenuItem,
            this.tsmiView,
            this.addonsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(963, 25);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip1";
            // 
            // dddToolStripMenuItem
            // 
            this.dddToolStripMenuItem.Name = "dddToolStripMenuItem";
            this.dddToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.dddToolStripMenuItem.Text = "ddd";
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResourcesForm,
            this.tsmiPropertiesForm,
            this.tsmiContentForm,
            this.tsmiOthers});
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.Size = new System.Drawing.Size(64, 21);
            this.tsmiView.Text = "视 图(&V)";
            // 
            // tsmiResourcesForm
            // 
            this.tsmiResourcesForm.Name = "tsmiResourcesForm";
            this.tsmiResourcesForm.Size = new System.Drawing.Size(136, 22);
            this.tsmiResourcesForm.Text = "资源管理器";
            this.tsmiResourcesForm.Click += new System.EventHandler(this.tsmiResourcesForm_Click);
            // 
            // tsmiPropertiesForm
            // 
            this.tsmiPropertiesForm.Name = "tsmiPropertiesForm";
            this.tsmiPropertiesForm.Size = new System.Drawing.Size(136, 22);
            this.tsmiPropertiesForm.Text = "属性编辑器";
            this.tsmiPropertiesForm.Click += new System.EventHandler(this.tsmiPropertiesForm_Click);
            // 
            // tsmiContentForm
            // 
            this.tsmiContentForm.Name = "tsmiContentForm";
            this.tsmiContentForm.Size = new System.Drawing.Size(136, 22);
            this.tsmiContentForm.Text = "内容编辑器";
            this.tsmiContentForm.Click += new System.EventHandler(this.tsmiContentForm_Click);
            // 
            // tsmiOthers
            // 
            this.tsmiOthers.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiConsoleForm});
            this.tsmiOthers.Name = "tsmiOthers";
            this.tsmiOthers.Size = new System.Drawing.Size(136, 22);
            this.tsmiOthers.Text = "其 他";
            // 
            // tsmiConsoleForm
            // 
            this.tsmiConsoleForm.Name = "tsmiConsoleForm";
            this.tsmiConsoleForm.Size = new System.Drawing.Size(124, 22);
            this.tsmiConsoleForm.Text = "信息输出";
            this.tsmiConsoleForm.Click += new System.EventHandler(this.tsmiConsoleForm_Click);
            // 
            // addonsToolStripMenuItem
            // 
            this.addonsToolStripMenuItem.Name = "addonsToolStripMenuItem";
            this.addonsToolStripMenuItem.Size = new System.Drawing.Size(63, 21);
            this.addonsToolStripMenuItem.Text = "扩 展(&E)";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(963, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // UIStatus_TIMER
            // 
            this.UIStatus_TIMER.Enabled = true;
            this.UIStatus_TIMER.Tick += new System.EventHandler(this.UIStatus_TIMER_Tick);
            // 
            // dockPanel
            // 
            this.dockPanel.BackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 50);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.RightToLeftLayout = true;
            this.dockPanel.ShowDocumentIcon = true;
            this.dockPanel.Size = new System.Drawing.Size(963, 543);
            this.dockPanel.TabIndex = 1;
            // 
            // vsToolStripExtender1
            // 
            this.vsToolStripExtender1.DefaultRenderer = null;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 615);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JxRes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem dddToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender vsToolStripExtender1;
        private WeifenLuo.WinFormsUI.Docking.VS2015LightTheme vS2015LightTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme vS2015BlueTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme vS2015DarkTheme1;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private System.Windows.Forms.ToolStripMenuItem tsmiResourcesForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropertiesForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiContentForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiOthers;
        private System.Windows.Forms.ToolStripMenuItem tsmiConsoleForm;
        private System.Windows.Forms.Timer UIStatus_TIMER;
        private System.Windows.Forms.ToolStripMenuItem addonsToolStripMenuItem;
        private System.Windows.Forms.Timer timerEntitySystemWorld;
    }
}

