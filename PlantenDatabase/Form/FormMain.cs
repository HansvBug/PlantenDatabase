namespace PlantenDatabase
{
    /// <summary>
    /// FormMain class.
    /// </summary>
    public partial class FormMain : Form
    {
        #region Create/start form

        /// <summary>
        /// Create/start FormMain.
        /// </summary>
        public FormMain()
        {
            this.InitializeComponent();

            this.Text = PdSettings.ApplicationName;
            this.SetStatusLabelMain = "Welkom.";
            CheckAppEnvironment();  // Checks if the application path exists. If not then it will be created.
            CreateSettingsFile();   // Create a settings file with default values. (If the file does not exists)
            
            // this.GetSettings();             // Get the settings
            // GetDebugMode();
            // this.StartLogging();
        }

        /// <summary>
        /// Sets the ToolStripStatusLabelMain text.
        /// </summary>
        public string SetStatusLabelMain
        {
            set { this.ToolStripStatusLabelMain.Text = value; }
        }

        private static void CheckAppEnvironment() // Checks if the application path exists. If not then it will be created.
        {
            using PdAppEnvironment checkPath = new();
            checkPath.CreateFolder(PdSettings.ApplicationName, true);
            checkPath.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.SettingsFolder, true);
            checkPath.CreateFolder(PdSettings.DatabaseFolder, false);
            checkPath.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.ImageFolder, true);
            checkPath.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.ExportFolder, true);
            checkPath.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.ImportFolder, true);
            checkPath.CreateFolder(PdSettings.DatabaseFolder + @"\" + PdSettings.BackUpFolder, false);
        }

        private static void CreateSettingsFile() // Create a settings file with default values. (If the file does not exists)
        {
            PdSettingsManager.CreateSettingsFile();
        }
        #endregion Create/start form

        private void FormMain_Load(object sender, EventArgs e)
        {
            // load
        }

        #region close load form
        private void ToolStripMenuItemProgramClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion close load form
    }
}