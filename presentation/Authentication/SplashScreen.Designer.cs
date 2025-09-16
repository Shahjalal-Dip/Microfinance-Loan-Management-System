namespace Microfinance_Loan_Management_System.Presentation.Authentication
{
    partial class SplashScreen
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
            this.BrandNameLable = new System.Windows.Forms.Label();
            this.parcentageLable = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.versionLable = new System.Windows.Forms.Label();
            this.companyLable = new System.Windows.Forms.Label();
            this.statusLable = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BrandNameLable
            // 
            this.BrandNameLable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BrandNameLable.AutoSize = true;
            this.BrandNameLable.Font = new System.Drawing.Font("Algerian", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrandNameLable.ForeColor = System.Drawing.Color.Tomato;
            this.BrandNameLable.Location = new System.Drawing.Point(51, 74);
            this.BrandNameLable.Name = "BrandNameLable";
            this.BrandNameLable.Size = new System.Drawing.Size(373, 41);
            this.BrandNameLable.TabIndex = 0;
            this.BrandNameLable.Text = "Application Name";
            this.BrandNameLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // parcentageLable
            // 
            this.parcentageLable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.parcentageLable.AutoSize = true;
            this.parcentageLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parcentageLable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.parcentageLable.Location = new System.Drawing.Point(53, 261);
            this.parcentageLable.Name = "parcentageLable";
            this.parcentageLable.Size = new System.Drawing.Size(157, 25);
            this.parcentageLable.TabIndex = 1;
            this.parcentageLable.Text = "parcentage...%";
            this.parcentageLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.parcentageLable.Click += new System.EventHandler(this.parcentageLable_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.progressBar1.Location = new System.Drawing.Point(33, 303);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(838, 23);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(425, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 61);
            this.label1.TabIndex = 3;
            this.label1.Text = "💰";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // versionLable
            // 
            this.versionLable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.versionLable.AutoSize = true;
            this.versionLable.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLable.ForeColor = System.Drawing.Color.Aqua;
            this.versionLable.Location = new System.Drawing.Point(418, 138);
            this.versionLable.Name = "versionLable";
            this.versionLable.Size = new System.Drawing.Size(60, 20);
            this.versionLable.TabIndex = 4;
            this.versionLable.Text = "version";
            this.versionLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // companyLable
            // 
            this.companyLable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.companyLable.AutoSize = true;
            this.companyLable.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.companyLable.ForeColor = System.Drawing.Color.MediumSpringGreen;
            this.companyLable.Location = new System.Drawing.Point(323, 171);
            this.companyLable.Name = "companyLable";
            this.companyLable.Size = new System.Drawing.Size(124, 21);
            this.companyLable.TabIndex = 5;
            this.companyLable.Text = "company name";
            this.companyLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusLable
            // 
            this.statusLable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.statusLable.AutoSize = true;
            this.statusLable.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLable.ForeColor = System.Drawing.Color.Aqua;
            this.statusLable.Location = new System.Drawing.Point(645, 269);
            this.statusLable.Name = "statusLable";
            this.statusLable.Size = new System.Drawing.Size(108, 17);
            this.statusLable.TabIndex = 6;
            this.statusLable.Text = "Status update";
            this.statusLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(908, 371);
            this.Controls.Add(this.statusLable);
            this.Controls.Add(this.companyLable);
            this.Controls.Add(this.versionLable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.parcentageLable);
            this.Controls.Add(this.BrandNameLable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.Load += new System.EventHandler(this.SplashScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label BrandNameLable;
        private System.Windows.Forms.Label parcentageLable;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label versionLable;
        private System.Windows.Forms.Label companyLable;
        private System.Windows.Forms.Label statusLable;
    }
}