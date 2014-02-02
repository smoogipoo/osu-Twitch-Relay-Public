namespace osu_Twitch_Relay
{
    partial class mainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainFrm));
            this.oNameTB = new System.Windows.Forms.TextBox();
            this.AuthBtn = new System.Windows.Forms.Button();
            this.tNameTB = new System.Windows.Forms.TextBox();
            this.tOAuthTB = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.forumLink = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // oNameTB
            // 
            this.oNameTB.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.oNameTB.Location = new System.Drawing.Point(12, 12);
            this.oNameTB.Name = "oNameTB";
            this.oNameTB.Size = new System.Drawing.Size(173, 22);
            this.oNameTB.TabIndex = 0;
            this.oNameTB.Text = "osu! Username";
            this.oNameTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // AuthBtn
            // 
            this.AuthBtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AuthBtn.Location = new System.Drawing.Point(12, 88);
            this.AuthBtn.Name = "AuthBtn";
            this.AuthBtn.Size = new System.Drawing.Size(103, 33);
            this.AuthBtn.TabIndex = 3;
            this.AuthBtn.Text = "Connect";
            this.AuthBtn.UseVisualStyleBackColor = true;
            this.AuthBtn.Click += new System.EventHandler(this.Button1_Click);
            // 
            // tNameTB
            // 
            this.tNameTB.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tNameTB.Location = new System.Drawing.Point(12, 38);
            this.tNameTB.Name = "tNameTB";
            this.tNameTB.Size = new System.Drawing.Size(173, 22);
            this.tNameTB.TabIndex = 1;
            this.tNameTB.Text = "Twitch.tv Username";
            this.tNameTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tOAuthTB
            // 
            this.tOAuthTB.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tOAuthTB.Location = new System.Drawing.Point(12, 64);
            this.tOAuthTB.Name = "tOAuthTB";
            this.tOAuthTB.Size = new System.Drawing.Size(173, 22);
            this.tOAuthTB.TabIndex = 2;
            this.tOAuthTB.Text = "Twitch.tv OAuth Token";
            this.tOAuthTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::osu_Twitch_Relay.Properties.Resources._16;
            this.pictureBox2.Location = new System.Drawing.Point(189, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseClick);
            this.pictureBox2.MouseEnter += new System.EventHandler(this.pictureBox2_MouseEnter);
            this.pictureBox2.MouseLeave += new System.EventHandler(this.pictureBox2_MouseLeave);
            // 
            // forumLink
            // 
            this.forumLink.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.forumLink.Location = new System.Drawing.Point(122, 90);
            this.forumLink.Name = "forumLink";
            this.forumLink.Size = new System.Drawing.Size(84, 26);
            this.forumLink.TabIndex = 8;
            this.forumLink.TabStop = true;
            this.forumLink.Text = "osu! Twitch Relay Forum Thread";
            this.forumLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.forumLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forumLink_LinkClicked);
            // 
            // mainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 125);
            this.Controls.Add(this.forumLink);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.tOAuthTB);
            this.Controls.Add(this.AuthBtn);
            this.Controls.Add(this.tNameTB);
            this.Controls.Add(this.oNameTB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mainFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "osu! Twitch Relay";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.LocationChanged += new System.EventHandler(this.Form1_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.TextBox oNameTB;
        private System.Windows.Forms.Button AuthBtn;
        private System.Windows.Forms.TextBox tNameTB;
        private System.Windows.Forms.TextBox tOAuthTB;

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.LinkLabel forumLink;
    }
}