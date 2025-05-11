namespace Large_Address_Aware__DotNETFramework_
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.filePathBox = new System.Windows.Forms.TextBox();
            this.patchButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.backupCheckbox = new System.Windows.Forms.CheckBox();
            this.restoreButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // filePathBox
            // 
            this.filePathBox.AllowDrop = true;
            this.filePathBox.Location = new System.Drawing.Point(10, 377);
            this.filePathBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.filePathBox.Name = "filePathBox";
            this.filePathBox.Size = new System.Drawing.Size(681, 41);
            this.filePathBox.TabIndex = 0;
            this.filePathBox.TextChanged += new System.EventHandler(this.filePathBox_TextChanged);
            this.filePathBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.filePathBox_DragDrop);
            // 
            // patchButton
            // 
            this.patchButton.Location = new System.Drawing.Point(318, 443);
            this.patchButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.patchButton.Name = "patchButton";
            this.patchButton.Size = new System.Drawing.Size(172, 77);
            this.patchButton.TabIndex = 2;
            this.patchButton.Text = "Patch";
            this.patchButton.UseVisualStyleBackColor = true;
            this.patchButton.Click += new System.EventHandler(this.patchButton_Click);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(141, 443);
            this.openButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(172, 77);
            this.openButton.TabIndex = 3;
            this.openButton.Text = "Open File";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // backupCheckbox
            // 
            this.backupCheckbox.AutoSize = true;
            this.backupCheckbox.Location = new System.Drawing.Point(496, 492);
            this.backupCheckbox.Name = "backupCheckbox";
            this.backupCheckbox.Size = new System.Drawing.Size(234, 38);
            this.backupCheckbox.TabIndex = 4;
            this.backupCheckbox.Text = "Create Backup?";
            this.backupCheckbox.UseVisualStyleBackColor = true;
            // 
            // restoreButton
            // 
            this.restoreButton.Location = new System.Drawing.Point(496, 424);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(174, 62);
            this.restoreButton.TabIndex = 5;
            this.restoreButton.Text = "Restore From Backup";
            this.restoreButton.UseVisualStyleBackColor = true;
            this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(701, 548);
            this.Controls.Add(this.restoreButton);
            this.Controls.Add(this.backupCheckbox);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.patchButton);
            this.Controls.Add(this.filePathBox);
            this.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "Large Address Aware";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filePathBox;
        private System.Windows.Forms.Button patchButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.CheckBox backupCheckbox;
        private System.Windows.Forms.Button restoreButton;
    }
}

