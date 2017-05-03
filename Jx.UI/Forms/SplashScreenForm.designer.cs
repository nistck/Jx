namespace Jx.UI
{
    partial class SplashScreenForm
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
            this.PB = new System.Windows.Forms.ProgressBar();
            this.labelMessage = new System.Windows.Forms.Label();
            this.SplashView = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.SplashView)).BeginInit();
            this.SuspendLayout();
            // 
            // PB
            // 
            this.PB.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PB.Location = new System.Drawing.Point(0, 394);
            this.PB.MarqueeAnimationSpeed = 50;
            this.PB.Name = "PB";
            this.PB.Size = new System.Drawing.Size(502, 11);
            this.PB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.PB.TabIndex = 0;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.Green;
            this.labelMessage.Location = new System.Drawing.Point(1, 360);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(0, 14);
            this.labelMessage.TabIndex = 1;
            // 
            // SplashView
            // 
            this.SplashView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SplashView.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.SplashView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplashView.Location = new System.Drawing.Point(0, 0);
            this.SplashView.Name = "SplashView";
            this.SplashView.Size = new System.Drawing.Size(502, 394);
            this.SplashView.TabIndex = 2;
            this.SplashView.TabStop = false;
            // 
            // SplashScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Azure;
            this.ClientSize = new System.Drawing.Size(502, 405);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.SplashView);
            this.Controls.Add(this.PB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreenForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashForm";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SplashForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.SplashView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar PB;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.PictureBox SplashView;
    }
}