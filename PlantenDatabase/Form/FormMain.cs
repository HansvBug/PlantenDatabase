namespace PlantenDatabase
{
    using System.Globalization;

    /// <summary>
    /// FormMain class.
    /// </summary>
    public partial class FormMain : Form
    {
        /// <summary>
        /// Create/start FormMain.
        /// </summary>
        public FormMain()
        {
            this.InitializeComponent();

            this.Text = PdSettings.ApplicationName;
            this.SetStatusLabelMain = "Welkom.";
            CheckAppEnvironment();  // Check if the application path exists. If not then it will be created.
            CreateSettingsFile();   // Create a settings file with default values. (If the file does not exists).
            this.GetSettings();     // Get the settings.
            GetDebugMode();         // DebugMode is a static class.
            this.StartLogging();
            this.ApplicationAccess = "Start Application";
        }

        private string ApplicationAccess { get; set; }

        /// <summary>
        /// Gets or sets the application settings. Holds the user and application setttings.
        /// </summary>
        public dynamic? JsonObjSettings { get; set; }

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
            PdAppEnvironment.CreateFolder(PdSettings.ApplicationName, true);
            PdAppEnvironment.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.SettingsFolder, true);
            PdAppEnvironment.CreateFolder(PdSettings.DatabaseFolder, false);
            PdAppEnvironment.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.ImageFolder, true);
            PdAppEnvironment.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.ExportFolder, true);
            PdAppEnvironment.CreateFolder(PdSettings.ApplicationName + @"\" + PdSettings.ImportFolder, true);
            PdAppEnvironment.CreateFolder(PdSettings.DatabaseFolder + @"\" + PdSettings.BackUpFolder, false);
        }

        private static void CreateSettingsFile() // Create a settings file with default values. (If the file does not exists)
        {
            PdSettingsManager.CreateSettingsFile();
        }

        private static void GetDebugMode()
        {
            using PdProcessArguments getArg = new();
            foreach (string arg in getArg.CmdLineArg)
            {
                string argument = Convert.ToString(arg, CultureInfo.InvariantCulture);

                if (argument == getArg.ArgDebug)
                {
                    PdDebugMode.DebugMode = true;
                }
            }
        }

        private void GetSettings()
        {
            try
            {
                using PdSettingsManager setMan = new();
                setMan.LoadSettings();

                if (setMan.JsonObjSettings != null && setMan.JsonObjSettings.AppParam != null)
                {
                    this.JsonObjSettings = setMan.JsonObjSettings;
                }
                else
                {
                    MessageBox.Show("Het settingsbestand is niet gevonden.", "Waarschuwing.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (AccessViolationException aex)
            {
                // Logging is not available here
                MessageBox.Show(
                    "Fout bij het laden van de instellingen. " + Environment.NewLine +
                    "De default instellingen worden ingeladen.",
                    "Waarschuwing.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                MessageBox.Show(
                    string.Format("Fout: {0}", aex.Message) + Environment.NewLine + Environment.NewLine +
                    string.Format("Fout: {0}", aex.ToString()),
                    "Fout.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void StartLogging()
        {
            PdLogging.NameLogFile = PdSettings.LogFileName;
            PdLogging.LogFolder = this.JsonObjSettings.AppParam[0].LogFileLocation;
            PdLogging.AppendLogFile = this.JsonObjSettings.AppParam[0].AppendLogFile;
            PdLogging.ActivateLogging = this.JsonObjSettings.AppParam[0].ActivateLogging;

            PdLogging.ApplicationName = PdSettings.ApplicationName;
            PdLogging.ApplicationVersion = PdSettings.ApplicationVersion;
            PdLogging.ApplicationBuildDate = PdSettings.ApplicationBuildDate;
            PdLogging.Parent = this;

            if (PdDebugMode.DebugMode)
            {
                PdLogging.DebugMode = true;
            }

            if (!PdLogging.StartLogging())
            {
                PdLogging.WriteToFile = false;  // Stop the logging
                PdLogging.ActivateLogging = false;
                this.JsonObjSettings.AppParam[0].ActivateLogging = false;
                this.JsonObjSettings.AppParam[0].AppendLogFile = false;
            }
            else
            {
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(string.Empty);
                    PdLogging.WriteToLogDebug("DebugMode = ON.");
                    PdLogging.WriteToLogDebug(string.Empty);
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.CreateDatabase();   // Create the application database file with tables.

            if (this.CheckDatabaseFileExists())
            {
                // if the database is not created correct then this.ApplicationAccess = "Minimal" else this.ApplicationAccess = "Start Application"
                if (this.ApplicationAccess == "Start Application")
                {
                    this.ApplicationAccess = "Full";
                }
            }
            else
            {
                this.ApplicationAccess = "Minimal";
            }

            this.EnableFunctions();
        }

        private void EnableFunctions()
        {
            switch (this.ApplicationAccess)
            {
                case "Full":
                    this.ToolStripMenuItemMaintainTables.Enabled = true;
                    this.ToolStripMenuItemMaintainCompress.Enabled = true;
                    this.ToolStripMenuItemOptionsConfigure.Enabled = true;
                    break;
                case "Minimal":
                    this.ToolStripMenuItemMaintainTables.Enabled = false;
                    this.ToolStripMenuItemMaintainCompress.Enabled = false;
                    this.ToolStripMenuItemOptionsConfigure.Enabled = false;
                    break;
                default:
                    this.ToolStripMenuItemMaintainTables.Enabled = false;
                    this.ToolStripMenuItemMaintainCompress.Enabled = false;
                    this.ToolStripMenuItemOptionsConfigure.Enabled = false;
                    break;
            }
        }

        private bool CheckDatabaseFileExists()
        {
            string dbLocation = this.JsonObjSettings.AppParam[0].DatabaseLocation;
            string databaseFileName = Path.Combine(dbLocation, PdSettings.SqlLiteDatabaseName);

            if (File.Exists(databaseFileName))
            {
                return true;
            }
            else
            {
                PdLogging.WriteToLogWarning("De database met de planten gegevens is niet gevonden. De meeste functionaliteit wordt uitgeschakeld.");
                MessageBox.Show(
                    "De database met de planten gegevens is niet gevonden." + Environment.NewLine +
                    "De meeste functionaliteit wordt uitgeschakeld.",
                    "Informatie.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }
        }

        private void ToolStripMenuItemProgramClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveSettings();
            PdLogging.StopLogging();
        }

        private void SaveSettings()
        {
            PdSettingsManager.SaveSettings(this.JsonObjSettings);
        }

        private void CreateDatabase()
        {
            using PdProcessArguments getArg = new();
            foreach (string arg in getArg.CmdLineArg)
            {
                string argument = Convert.ToString(arg, CultureInfo.InvariantCulture);

                if (argument == getArg.ArgIntall)
                {
                    PdApplicationDatabaseCreate createAppDb = new();
                    if (createAppDb.CreateDatabase())
                    {
                        this.ApplicationAccess = "Full";
                    }
                    else
                    {
                        this.ApplicationAccess = "Minimal";
                    }
                }
            }
        }
    }
}