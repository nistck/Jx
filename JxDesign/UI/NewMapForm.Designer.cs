namespace JxDesign.UI
{
    partial class NewMapForm
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
            this.wizardNewMap = new Jx.UI.Controls.Wizards.Wizard();
            this.wizardPage1 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage2 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage3 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage4 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage5 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardNewMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardNewMap
            // 
            this.wizardNewMap.Controls.Add(this.wizardPage2);
            this.wizardNewMap.Controls.Add(this.wizardPage3);
            this.wizardNewMap.Controls.Add(this.wizardPage4);
            this.wizardNewMap.Controls.Add(this.wizardPage5);
            this.wizardNewMap.Location = new System.Drawing.Point(0, 0);
            this.wizardNewMap.Name = "wizardNewMap";
            this.wizardNewMap.Pages.AddRange(new Jx.UI.Controls.Wizards.WizardPage[] {
            this.wizardPage1,
            this.wizardPage2,
            this.wizardPage3,
            this.wizardPage4,
            this.wizardPage5});
            this.wizardNewMap.Size = new System.Drawing.Size(502, 349);
            this.wizardNewMap.TabIndex = 0;
            this.wizardNewMap.WelcomeImage = global::JxDesign.Properties.Resources.help_10;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Description = "48645646546";
            this.wizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage1.Location = new System.Drawing.Point(3, 3);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.ShowGlyphPlaceHolder = true;
            this.wizardPage1.Size = new System.Drawing.Size(496, 293);
            this.wizardPage1.Style = Jx.UI.Controls.Wizards.WizardPageStyle.Welcome;
            this.wizardPage1.TabIndex = 12;
            // 
            // wizardPage2
            // 
            this.wizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage2.Location = new System.Drawing.Point(0, 0);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.ShowGlyphPlaceHolder = true;
            this.wizardPage2.Size = new System.Drawing.Size(598, 420);
            this.wizardPage2.TabIndex = 12;
            // 
            // wizardPage3
            // 
            this.wizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage3.Location = new System.Drawing.Point(0, 0);
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.ShowGlyphPlaceHolder = true;
            this.wizardPage3.Size = new System.Drawing.Size(598, 420);
            this.wizardPage3.TabIndex = 12;
            // 
            // wizardPage4
            // 
            this.wizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage4.Location = new System.Drawing.Point(0, 0);
            this.wizardPage4.Name = "wizardPage4";
            this.wizardPage4.ShowGlyphPlaceHolder = true;
            this.wizardPage4.Size = new System.Drawing.Size(598, 420);
            this.wizardPage4.TabIndex = 12;
            // 
            // wizardPage5
            // 
            this.wizardPage5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage5.Location = new System.Drawing.Point(0, 0);
            this.wizardPage5.Name = "wizardPage5";
            this.wizardPage5.ShowGlyphPlaceHolder = true;
            this.wizardPage5.Size = new System.Drawing.Size(502, 349);
            this.wizardPage5.Style = Jx.UI.Controls.Wizards.WizardPageStyle.Finish;
            this.wizardPage5.TabIndex = 12;
            // 
            // NewMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 349);
            this.Controls.Add(this.wizardNewMap);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewMapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "创建地图";
            this.wizardNewMap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Jx.UI.Controls.Wizards.Wizard wizardNewMap;
        private Jx.UI.Controls.Wizards.WizardPage wizardPage1;
        private Jx.UI.Controls.Wizards.WizardPage wizardPage2;
        private Jx.UI.Controls.Wizards.WizardPage wizardPage3;
        private Jx.UI.Controls.Wizards.WizardPage wizardPage4;
        private Jx.UI.Controls.Wizards.WizardPage wizardPage5;
    }
}