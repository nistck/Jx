namespace Jx
{
    partial class Form1
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
            this.wizard1 = new Jx.UI.Controls.Wizards.Wizard();
            this.wizardPage1 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage2 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage3 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage4 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage5 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage6 = new Jx.UI.Controls.Wizards.WizardPage();
            this.wizardPage7 = new Jx.UI.Controls.Wizards.WizardPage();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            this.wizard1.HelpVisible = true;
            this.wizard1.Location = new System.Drawing.Point(0, 0);
            this.wizard1.Name = "wizard1";
            this.wizard1.Pages.AddRange(new Jx.UI.Controls.Wizards.WizardPage[] {
            this.wizardPage1,
            this.wizardPage2,
            this.wizardPage3,
            this.wizardPage4,
            this.wizardPage5,
            this.wizardPage6,
            this.wizardPage7});
            this.wizard1.Size = new System.Drawing.Size(522, 315);
            this.wizard1.TabIndex = 0;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage1.Location = new System.Drawing.Point(3, 3);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.ShowGlyphPlaceHolder = true;
            this.wizardPage1.Size = new System.Drawing.Size(516, 269);
            this.wizardPage1.TabIndex = 11;
            // 
            // wizardPage2
            // 
            this.wizardPage2.Location = new System.Drawing.Point(3, 3);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.ShowGlyphPlaceHolder = true;
            this.wizardPage2.Size = new System.Drawing.Size(200, 100);
            this.wizardPage2.TabIndex = 11;
            // 
            // wizardPage3
            // 
            this.wizardPage3.Location = new System.Drawing.Point(3, 3);
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.ShowGlyphPlaceHolder = true;
            this.wizardPage3.Size = new System.Drawing.Size(200, 100);
            this.wizardPage3.TabIndex = 11;
            // 
            // wizardPage4
            // 
            this.wizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage4.Location = new System.Drawing.Point(3, 3);
            this.wizardPage4.Name = "wizardPage4";
            this.wizardPage4.ShowGlyphPlaceHolder = true;
            this.wizardPage4.Size = new System.Drawing.Size(516, 269);
            this.wizardPage4.TabIndex = 11;
            // 
            // wizardPage5
            // 
            this.wizardPage5.Location = new System.Drawing.Point(3, 3);
            this.wizardPage5.Name = "wizardPage5";
            this.wizardPage5.ShowGlyphPlaceHolder = true;
            this.wizardPage5.Size = new System.Drawing.Size(200, 100);
            this.wizardPage5.TabIndex = 11;
            // 
            // wizardPage6
            // 
            this.wizardPage6.Location = new System.Drawing.Point(3, 3);
            this.wizardPage6.Name = "wizardPage6";
            this.wizardPage6.ShowGlyphPlaceHolder = true;
            this.wizardPage6.Size = new System.Drawing.Size(200, 100);
            this.wizardPage6.TabIndex = 11;
            // 
            // wizardPage7
            // 
            this.wizardPage7.Location = new System.Drawing.Point(3, 3);
            this.wizardPage7.Name = "wizardPage7";
            this.wizardPage7.ShowGlyphPlaceHolder = true;
            this.wizardPage7.Size = new System.Drawing.Size(516, 267);
            this.wizardPage7.Style = Jx.UI.Controls.Wizards.WizardPageStyle.Finish;
            this.wizardPage7.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 315);
            this.Controls.Add(this.wizard1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private UI.Controls.Wizards.Wizard wizard1;
        private UI.Controls.Wizards.WizardPage wizardPage1;
        private UI.Controls.Wizards.WizardPage wizardPage2;
        private UI.Controls.Wizards.WizardPage wizardPage3;
        private UI.Controls.Wizards.WizardPage wizardPage4;
        private UI.Controls.Wizards.WizardPage wizardPage5;
        private UI.Controls.Wizards.WizardPage wizardPage6;
        private UI.Controls.Wizards.WizardPage wizardPage7;
    }
}