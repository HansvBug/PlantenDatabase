namespace PlantenDatabase
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using Microsoft.Win32.SafeHandles;

    public class PdSettingsManager : IDisposable
    {
        private readonly SafeHandle safeHandle = new SafeFileHandle(IntPtr.Zero, true); // Instantiate a SafeHandle instance.
        private bool disposed; // Flag: Has Dispose already been called?

        /// <summary>
        /// Gets or sets a reference of the application settings.
        /// </summary>
        public AppSettingsMeta JsonObjSettings { get; set; }


        /// <summary>
        /// Create the application settings file.
        /// </summary>
        public static void CreateSettingsFile()
        {
            var getOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PdSettings.ApplicationName, PdSettings.SettingsFolder, PdSettings.ConfigFile);

            if (!File.Exists(settingsFile))
            {
                var appSettings = new AppSettingsMeta()
                {
                    AppParam = new List<AppParams>()
                    {
                        new AppParams()
                        {
                            ApplicationName = PdSettings.ApplicationName,
                            ApplicationVersion = PdSettings.ApplicationVersion,
                            ApplicationBuildDate = PdSettings.ApplicationBuildDate,
                            ActivateLogging = true,
                            AppendLogFile = true,
                            SettingsFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PdSettings.ApplicationName, PdSettings.SettingsFolder, PdSettings.ConfigFile),
                            LogFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PdSettings.ApplicationName, PdSettings.SettingsFolder),

                            // TODO; make it run un usb: DatabaseLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SettingsDefault.ApplicationName, SettingsDefault.DatabaseFolder),
                            DatabaseLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PdSettings.DatabaseFolder),  // Database will be stored next to de Program. Not in the roaming folder because everyone uses the same database
                            ResetAllAutoIncrementFields = true,
                        },
                    },
                };

                string jsonString;
                jsonString = JsonSerializer.Serialize(appSettings, getOptions);

                File.WriteAllText(settingsFile, jsonString);
            }
        }

        /// <summary>
        /// Allpication setttings meta class.
        /// </summary>
        public class AppSettingsMeta
        {
            /// <summary>
            /// Gets or sets a list with application settings.
            /// </summary>
            public List<AppParams>? AppParam { get; set; }
        }

        /// <summary>
        /// The application parameters/settings.
        /// </summary>
        public class AppParams
        {
            /// <summary>
            /// Gets or sets a value indicating whether logging will be activated.
            /// </summary>
            public bool ActivateLogging { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the log file will be appended.
            /// </summary>
            public bool AppendLogFile { get; set; }

            /// <summary>
            /// Gets or sets the application name.
            /// </summary>
            public string? ApplicationName { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            public string? ApplicationVersion { get; set; }

            /// <summary>
            /// Gets or sets the application build date.
            /// </summary>
            public string? ApplicationBuildDate { get; set; }

            /// <summary>
            /// Gets or sets the settings file location.
            /// </summary>
            public string? SettingsFileLocation { get; set; }

            /// <summary>
            /// Gets or sets log file location.
            /// </summary>
            public string? LogFileLocation { get; set; }

            /// <summary>
            /// Gets or sets application database file location.
            /// </summary>
            public string? DatabaseLocation { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether all Increment fields will be restted. = Sequences reset.
            /// </summary>
            public bool ResetAllAutoIncrementFields { get; set; }
        }

        #region Dispose

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Has Dispose already been called.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.safeHandle?.Dispose();

                // Free any other managed objects here.
            }

            this.disposed = true;
        }
        #endregion Dispose
    }
}
