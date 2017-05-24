namespace JxRes.UI
{
    partial class ContentForm
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
            Jx.Drawing.Base.Pointer pointer2 = new Jx.Drawing.Base.Pointer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.XSS = new System.Windows.Forms.StatusStrip();
            this.XTS = new System.Windows.Forms.ToolStrip();
            this.drawingPanel = new Jx.Drawing.Base.DrawingPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(699, 492);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.XSS, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.XTS, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.drawingPanel, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(693, 486);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // XSS
            // 
            this.XSS.Location = new System.Drawing.Point(0, 464);
            this.XSS.Name = "XSS";
            this.XSS.Size = new System.Drawing.Size(693, 22);
            this.XSS.TabIndex = 0;
            this.XSS.Text = "statusStrip1";
            // 
            // XTS
            // 
            this.XTS.Location = new System.Drawing.Point(0, 0);
            this.XTS.Name = "XTS";
            this.XTS.Size = new System.Drawing.Size(693, 25);
            this.XTS.TabIndex = 1;
            this.XTS.Text = "toolStrip1";
            // 
            // drawingPanel
            // 
            this.drawingPanel.ActiveCursor = System.Windows.Forms.Cursors.Default;
            pointer2.MouseDownPoint = new System.Drawing.Point(0, 0);
            pointer2.MouseUpPoint = new System.Drawing.Point(0, 0);
            this.drawingPanel.ActiveTool = pointer2;
            this.drawingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawingPanel.EnableWheelZoom = true;
            this.drawingPanel.Location = new System.Drawing.Point(3, 28);
            this.drawingPanel.Name = "drawingPanel";
            this.drawingPanel.Size = new System.Drawing.Size(687, 433);
            this.drawingPanel.TabIndex = 2;
            this.drawingPanel.Zoom = 1F;
            // 
            // ContentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 492);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.HideOnClose = true;
            this.Name = "ContentForm";
            this.Text = "MainViewForm";
            this.Load += new System.EventHandler(this.MainViewForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.StatusStrip XSS;
        private System.Windows.Forms.ToolStrip XTS;
        private Jx.Drawing.Base.DrawingPanel drawingPanel;
    }
}