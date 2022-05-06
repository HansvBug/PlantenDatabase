namespace PlantenDatabase
{
    partial class FormConfigure
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
            this.LabelDefaultImportLocation = new System.Windows.Forms.Label();
            this.TextBoxDefaultImportLocation = new System.Windows.Forms.TextBox();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelDefaultImportLocation
            // 
            this.LabelDefaultImportLocation.AutoSize = true;
            this.LabelDefaultImportLocation.Location = new System.Drawing.Point(12, 9);
            this.LabelDefaultImportLocation.Name = "LabelDefaultImportLocation";
            this.LabelDefaultImportLocation.Size = new System.Drawing.Size(84, 15);
            this.LabelDefaultImportLocation.TabIndex = 0;
            this.LabelDefaultImportLocation.Text = "Import locatie:";
            // 
            // TextBoxDefaultImportLocation
            // 
            this.TextBoxDefaultImportLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxDefaultImportLocation.Location = new System.Drawing.Point(102, 6);
            this.TextBoxDefaultImportLocation.Name = "TextBoxDefaultImportLocation";
            this.TextBoxDefaultImportLocation.Size = new System.Drawing.Size(637, 23);
            this.TextBoxDefaultImportLocation.TabIndex = 1;
            this.TextBoxDefaultImportLocation.Leave += new System.EventHandler(this.TextBoxDefaultImportLocation_Leave);
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClose.Location = new System.Drawing.Point(712, 417);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 2;
            this.ButtonClose.Text = "Sluiten";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // FormConfigure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 452);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.TextBoxDefaultImportLocation);
            this.Controls.Add(this.LabelDefaultImportLocation);
            this.Name = "FormConfigure";
            this.Text = "FormConfigure";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigure_FormClosing);
            this.Load += new System.EventHandler(this.FormConfigure_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label LabelDefaultImportLocation;
        private TextBox TextBoxDefaultImportLocation;
        private Button ButtonClose;
    }
}