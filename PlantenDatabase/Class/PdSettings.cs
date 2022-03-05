namespace PlantenDatabase
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Settings class.
    /// </summary>
    public static class PdSettings
    {
        /// <summary>
        /// System menu year.
        /// </summary>
        public const string SystemMenu = "2022";  // For the systemmenu line

        /// <summary>
        /// The name of the config file.
        /// </summary>
        public const string ConfigFile = "PlantenDatabase.cfg";

        /// <summary>
        /// The name of the log file.
        /// </summary>
        public const string LogFileName = "PlantenDatabase.log";

        /// <summary>
        /// Copyright.
        /// </summary>
        public const string Copyright = "2022";  // Started in 2022

        /// <summary>
        /// The name of the application database.
        /// </summary>
        public const string SqlLiteDatabaseName = "PlantenDatabase.db";

        /// <summary>
        /// Database version number.
        /// </summary>
        public const int DatabaseVersion = 1;  // Start with 1

        /// <summary>
        /// Settings folder name.
        /// </summary>
        public const string SettingsFolder = "Settings\\";

        /// <summary>
        /// Database folder name.
        /// </summary>
        public const string DatabaseFolder = "Database\\";

        /// <summary>
        /// Import folder name.
        /// </summary>
        public const string ImportFolder = "Import\\";

        /// <summary>
        /// Export folder.
        /// </summary>
        public const string ExportFolder = "Export\\";

        /// <summary>
        /// Image folder.
        /// </summary>
        public const string ImageFolder = "Image\\";

        /// <summary>
        /// Back-up folder.
        /// </summary>
        public const string BackUpFolder = "Backup\\";

        /// <summary>
        /// The name of the application.
        /// </summary>
        public const string ApplicationName = "Plantendatabase";  // Folder

        /// <summary>
        /// Main Form Text.
        /// </summary>
        public const string ApplicationNameShow = "Planten database";  // Show in form

        /// <summary>
        /// Application version.
        /// </summary>
        public const string ApplicationVersion = "0.0.0.1";

        /// <summary>
        /// gets the application build date.
        /// </summary>
        public static string ApplicationBuildDate
        {
            get { return DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture); }
        }
    }
}
