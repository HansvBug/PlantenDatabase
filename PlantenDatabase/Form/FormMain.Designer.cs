namespace PlantenDatabase
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StatusStripMain = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItemProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemProgramClose = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemMaintain = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemMaintainTables = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemMaintainCompress = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemOptionsConfigure = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStripMain.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStripMain
            // 
            this.StatusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabelMain});
            this.StatusStripMain.Location = new System.Drawing.Point(0, 428);
            this.StatusStripMain.Name = "StatusStripMain";
            this.StatusStripMain.Size = new System.Drawing.Size(800, 22);
            this.StatusStripMain.TabIndex = 0;
            this.StatusStripMain.Text = "statusStrip1";
            // 
            // ToolStripStatusLabelMain
            // 
            this.ToolStripStatusLabelMain.Name = "ToolStripStatusLabelMain";
            this.ToolStripStatusLabelMain.Size = new System.Drawing.Size(142, 17);
            this.ToolStripStatusLabelMain.Text = "ToolStripStatusLabelMain";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemProgram,
            this.ToolStripMenuItemMaintain,
            this.ToolStripMenuItemOptions});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuItemProgram
            // 
            this.ToolStripMenuItemProgram.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemProgramClose});
            this.ToolStripMenuItemProgram.Name = "ToolStripMenuItemProgram";
            this.ToolStripMenuItemProgram.Size = new System.Drawing.Size(82, 20);
            this.ToolStripMenuItemProgram.Text = "&Programma";
            // 
            // ToolStripMenuItemProgramClose
            // 
            this.ToolStripMenuItemProgramClose.Name = "ToolStripMenuItemProgramClose";
            this.ToolStripMenuItemProgramClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.ToolStripMenuItemProgramClose.Size = new System.Drawing.Size(163, 22);
            this.ToolStripMenuItemProgramClose.Text = "Afsluiten";
            this.ToolStripMenuItemProgramClose.Click += new System.EventHandler(this.ToolStripMenuItemProgramClose_Click);
            // 
            // ToolStripMenuItemMaintain
            // 
            this.ToolStripMenuItemMaintain.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemMaintainTables,
            this.ToolStripMenuItemMaintainCompress});
            this.ToolStripMenuItemMaintain.Name = "ToolStripMenuItemMaintain";
            this.ToolStripMenuItemMaintain.Size = new System.Drawing.Size(55, 20);
            this.ToolStripMenuItemMaintain.Text = "&Beheer";
            // 
            // ToolStripMenuItemMaintainTables
            // 
            this.ToolStripMenuItemMaintainTables.Name = "ToolStripMenuItemMaintainTables";
            this.ToolStripMenuItemMaintainTables.Size = new System.Drawing.Size(191, 22);
            this.ToolStripMenuItemMaintainTables.Text = "&Tabellen";
            this.ToolStripMenuItemMaintainTables.Click += new System.EventHandler(this.ToolStripMenuItemMaintainTables_Click);
            // 
            // ToolStripMenuItemMaintainCompress
            // 
            this.ToolStripMenuItemMaintainCompress.Name = "ToolStripMenuItemMaintainCompress";
            this.ToolStripMenuItemMaintainCompress.Size = new System.Drawing.Size(191, 22);
            this.ToolStripMenuItemMaintainCompress.Text = "Comprimeer &database";
            this.ToolStripMenuItemMaintainCompress.Click += new System.EventHandler(this.ToolStripMenuItemMaintainCompress_Click);
            // 
            // ToolStripMenuItemOptions
            // 
            this.ToolStripMenuItemOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemOptionsConfigure});
            this.ToolStripMenuItemOptions.Name = "ToolStripMenuItemOptions";
            this.ToolStripMenuItemOptions.Size = new System.Drawing.Size(53, 20);
            this.ToolStripMenuItemOptions.Text = "&Opties";
            // 
            // ToolStripMenuItemOptionsConfigure
            // 
            this.ToolStripMenuItemOptionsConfigure.Name = "ToolStripMenuItemOptionsConfigure";
            this.ToolStripMenuItemOptionsConfigure.Size = new System.Drawing.Size(108, 22);
            this.ToolStripMenuItemOptionsConfigure.Text = "&Opties";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StatusStripMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.StatusStripMain.ResumeLayout(false);
            this.StatusStripMain.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatusStrip StatusStripMain;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ToolStripMenuItemProgram;
        private ToolStripMenuItem ToolStripMenuItemProgramClose;
        private ToolStripMenuItem ToolStripMenuItemMaintain;
        private ToolStripMenuItem ToolStripMenuItemMaintainTables;
        private ToolStripMenuItem ToolStripMenuItemMaintainCompress;
        private ToolStripMenuItem ToolStripMenuItemOptions;
        private ToolStripMenuItem ToolStripMenuItemOptionsConfigure;
        private ToolStripStatusLabel ToolStripStatusLabelMain;
    }
}