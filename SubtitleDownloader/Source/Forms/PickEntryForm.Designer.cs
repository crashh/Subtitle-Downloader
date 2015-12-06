namespace SubtitleDownloader
{
    partial class PickEntryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PickEntryForm));
            this.listViewEntryListing = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.labelHelper = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewEntryListing
            // 
            this.listViewEntryListing.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.listViewEntryListing.Font = new System.Drawing.Font("Rockwell", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewEntryListing.Location = new System.Drawing.Point(12, 67);
            this.listViewEntryListing.Name = "listViewEntryListing";
            this.listViewEntryListing.Size = new System.Drawing.Size(351, 263);
            this.listViewEntryListing.TabIndex = 0;
            this.listViewEntryListing.UseCompatibleStateImageBehavior = false;
            this.listViewEntryListing.SelectedIndexChanged += new System.EventHandler(this.listViewEntryListing_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.SkyBlue;
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(12, 336);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(351, 47);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelHelper
            // 
            this.labelHelper.AutoSize = true;
            this.labelHelper.BackColor = System.Drawing.Color.Transparent;
            this.labelHelper.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHelper.ForeColor = System.Drawing.Color.White;
            this.labelHelper.Location = new System.Drawing.Point(12, 44);
            this.labelHelper.Name = "labelHelper";
            this.labelHelper.Size = new System.Drawing.Size(267, 20);
            this.labelHelper.TabIndex = 2;
            this.labelHelper.Text = "Select the most likely search result...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 24);
            this.label5.TabIndex = 12;
            this.label5.Text = "Select Entry";
            // 
            // PickEntryForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(375, 389);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelHelper);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listViewEntryListing);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PickEntryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PickEntry";
            this.Load += new System.EventHandler(this.PickEntry_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PickEntryForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewEntryListing;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelHelper;
        private System.Windows.Forms.Label label5;
    }
}