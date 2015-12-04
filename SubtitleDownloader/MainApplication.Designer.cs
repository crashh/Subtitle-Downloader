namespace SubtitleDownloader
{
    partial class MainApplication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainApplication));
            this.currentPathLabel = new System.Windows.Forms.Label();
            this.listViewDirContents = new System.Windows.Forms.ListView();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnModify = new System.Windows.Forms.Button();
            this.errorLabel = new System.Windows.Forms.Label();
            this.textBoxProgress = new System.Windows.Forms.TextBox();
            this.btnOpenBrowser = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // currentPathLabel
            // 
            this.currentPathLabel.AutoSize = true;
            this.currentPathLabel.BackColor = System.Drawing.Color.Transparent;
            this.currentPathLabel.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentPathLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.currentPathLabel.Location = new System.Drawing.Point(6, 586);
            this.currentPathLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.currentPathLabel.Name = "currentPathLabel";
            this.currentPathLabel.Size = new System.Drawing.Size(77, 13);
            this.currentPathLabel.TabIndex = 1;
            this.currentPathLabel.Text = "Current path: ";
            // 
            // listViewDirContents
            // 
            this.listViewDirContents.BackColor = System.Drawing.Color.White;
            this.listViewDirContents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewDirContents.Font = new System.Drawing.Font("Rockwell", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewDirContents.FullRowSelect = true;
            this.listViewDirContents.Location = new System.Drawing.Point(252, 0);
            this.listViewDirContents.Margin = new System.Windows.Forms.Padding(0);
            this.listViewDirContents.MultiSelect = false;
            this.listViewDirContents.Name = "listViewDirContents";
            this.listViewDirContents.Size = new System.Drawing.Size(800, 625);
            this.listViewDirContents.TabIndex = 4;
            this.listViewDirContents.UseCompatibleStateImageBehavior = false;
            this.listViewDirContents.View = System.Windows.Forms.View.Details;
            this.listViewDirContents.SelectedIndexChanged += new System.EventHandler(this.listViewDirContents_SelectedIndexChanged_1);
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.Color.SkyBlue;
            this.btnDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDownload.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDownload.Enabled = false;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownload.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDownload.Image = ((System.Drawing.Image)(resources.GetObject("btnDownload.Image")));
            this.btnDownload.Location = new System.Drawing.Point(0, 461);
            this.btnDownload.Margin = new System.Windows.Forms.Padding(0);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(252, 123);
            this.btnDownload.TabIndex = 5;
            this.btnDownload.Text = "Search";
            this.btnDownload.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDownload.UseCompatibleTextRendering = true;
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnModify
            // 
            this.btnModify.BackColor = System.Drawing.Color.SkyBlue;
            this.btnModify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModify.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModify.Image = ((System.Drawing.Image)(resources.GetObject("btnModify.Image")));
            this.btnModify.Location = new System.Drawing.Point(126, 359);
            this.btnModify.Margin = new System.Windows.Forms.Padding(0);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(126, 50);
            this.btnModify.TabIndex = 6;
            this.btnModify.Text = "Modify Entry";
            this.btnModify.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnModify.UseCompatibleTextRendering = true;
            this.btnModify.UseVisualStyleBackColor = false;
            this.btnModify.Visible = false;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.BackColor = System.Drawing.Color.Transparent;
            this.errorLabel.ForeColor = System.Drawing.Color.Red;
            this.errorLabel.Location = new System.Drawing.Point(6, 602);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(61, 13);
            this.errorLabel.TabIndex = 7;
            this.errorLabel.Text = "*error label*";
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.textBoxProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxProgress.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBoxProgress.Font = new System.Drawing.Font("Rockwell", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxProgress.ForeColor = System.Drawing.Color.ForestGreen;
            this.textBoxProgress.Location = new System.Drawing.Point(1052, 0);
            this.textBoxProgress.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxProgress.Multiline = true;
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.Size = new System.Drawing.Size(132, 625);
            this.textBoxProgress.TabIndex = 8;
            // 
            // btnOpenBrowser
            // 
            this.btnOpenBrowser.BackColor = System.Drawing.Color.LightPink;
            this.btnOpenBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenBrowser.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenBrowser.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenBrowser.Image")));
            this.btnOpenBrowser.Location = new System.Drawing.Point(0, 304);
            this.btnOpenBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpenBrowser.Name = "btnOpenBrowser";
            this.btnOpenBrowser.Size = new System.Drawing.Size(252, 42);
            this.btnOpenBrowser.TabIndex = 9;
            this.btnOpenBrowser.Text = "Do it manually";
            this.btnOpenBrowser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnOpenBrowser.UseCompatibleTextRendering = true;
            this.btnOpenBrowser.UseVisualStyleBackColor = false;
            this.btnOpenBrowser.Visible = false;
            this.btnOpenBrowser.Click += new System.EventHandler(this.btnOpenBrowser_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = System.Drawing.Color.SkyBlue;
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFolder.Image")));
            this.btnOpenFolder.Location = new System.Drawing.Point(0, 359);
            this.btnOpenFolder.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(126, 50);
            this.btnOpenFolder.TabIndex = 10;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnOpenFolder.UseCompatibleTextRendering = true;
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Visible = false;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(69, 21);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(41, 21);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.infoToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1184, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MainApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1184, 624);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.btnOpenBrowser);
            this.Controls.Add(this.textBoxProgress);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.btnModify);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.listViewDirContents);
            this.Controls.Add(this.currentPathLabel);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "MainApplication";
            this.Text = "SubtitleDownloader";
            this.Load += new System.EventHandler(this.MainApplication_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label currentPathLabel;
        private System.Windows.Forms.ListView listViewDirContents;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.TextBox textBoxProgress;
        private System.Windows.Forms.Button btnOpenBrowser;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}

