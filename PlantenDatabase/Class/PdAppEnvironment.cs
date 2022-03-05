namespace PlantenDatabase
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    public class PdAppEnvironment : IDisposable
    {
        private readonly SafeHandle safeHandle = new SafeFileHandle(IntPtr.Zero, true); // Instantiate a SafeHandle instance.
        private bool disposed;  // Flag: Has Dispose already been called?

        /// <summary>
        /// Create a folder.
        /// </summary>
        /// <param name="folderName">The name of the new folder.</param>
        /// <param name="applicationDataFolder">if YES then the folder will be created in %appdata%, if NO then the folder will be created in de application directory.</param>
        /// <returns>True if succeeded.</returns>
        public static bool CreateFolder(string folderName, bool applicationDataFolder)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return false;
            }

            if (applicationDataFolder)
            {
                string pathString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folderName);
                if (!Directory.Exists(pathString))
                {
                    try
                    {
                        Directory.CreateDirectory(pathString);
                        return true;
                    }
                    catch (AccessViolationException)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                string appDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
                if (!Directory.Exists(appDir))
                {
                    try
                    {
                        Directory.CreateDirectory(appDir);
                        return true;
                    }
                    catch (AccessViolationException)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public static string GetApplicationPath() // Get the application path
        {
            try
            {
                string? appPath;
                appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                appPath += "\\";                                  // add \to the path
                return appPath.Replace("file:\\", string.Empty);  // Remove the text "file:\\" from the path
            }
            catch (ArgumentException aex)
            {
                throw new InvalidOperationException(aex.Message);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Ophalen locatie applicatie is mislukt.");
            }
        }

        public static string GetUserName()
        {
            try
            {
                return Environment.UserName;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Ophalen naam gebruiker is mislukt.");
            }
        }

        public static string GetMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Ophalen naam machine is mislukt.");
            }
        }

        public static string GetWindowsVersion(short type)
        {
            try
            {
                string? osVersion = string.Empty;

                switch (type)
                {
                    case 1:
                        {
                            osVersion = Environment.OSVersion.ToString();
                            break;
                        }

                    case 2:
                        {
                            osVersion = Convert.ToString(Environment.OSVersion.Version, CultureInfo.InvariantCulture);
                            break;
                        }

                    default:
                        {
                            osVersion = Convert.ToString(Environment.OSVersion.Version, CultureInfo.InvariantCulture);
                            break;
                        }
                }

                if (!string.IsNullOrEmpty(osVersion))
                {
                    return osVersion;
                }
                else
                {
                    return "-";
                }
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("Onverwachte fout opgetreden bij het bepalen van de Windowsversie (Argument Exception).");
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Onverwachte fout opgetreden bij het bepalen van de Windowsversie.");
            }
        }

        public static string GetProcessorCount()
        {
            try
            {
                return Convert.ToString(Environment.ProcessorCount, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Ophalen aantal processors is mislukt.");
            }
        }

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
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.safeHandle?.Dispose();

                    // Free other state (managed objects).
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                this.disposed = true;
            }
        }
    }
}
