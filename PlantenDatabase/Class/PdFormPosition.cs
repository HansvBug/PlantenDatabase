namespace PlantenDatabase
{
    using Microsoft.Win32.SafeHandles;
    using System.Runtime.InteropServices;

    public class PdFormPosition : IDisposable
    {
        private readonly SafeHandle safeHandle = new SafeFileHandle(IntPtr.Zero, true); // Instantiate a SafeHandle instance.
        private bool disposed;  // Flag: Has Dispose already been called?

        private readonly FormMain? MainForm;
        private readonly FormTableMaintenance? TableMaintenanceForm;
        private dynamic? JsonObjSettings { get; set; }


        public PdFormPosition(FormMain MainForm)
        {
            this.MainForm = MainForm;
            this.JsonObjSettings = MainForm.JsonObjSettings;
        }

        public PdFormPosition(FormTableMaintenance TableMaintenanceForm)
        {
            this.TableMaintenanceForm = TableMaintenanceForm;
            this.JsonObjSettings = TableMaintenanceForm.JsonObjSettings;
        }

        #region Helper
        private static bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion Helper

        #region FormMain
        public void LoadMainFormPosition()
        {
            if (PdDebugMode.DebugMode)
            {
                PdLogging.WriteToLogInformation("Ophalen scherm positie hoofdscherm.");
            }

            // default
            this.MainForm.WindowState = FormWindowState.Normal;
            this.MainForm.StartPosition = FormStartPosition.WindowsDefaultBounds;

            if (this.JsonObjSettings != null && this.JsonObjSettings.FormMain != null)
            {
                Rectangle FrmRect = new()
                {
                    X = this.JsonObjSettings.FormMain[0].FrmX,
                    Y = this.JsonObjSettings.FormMain[0].FrmY,
                    Width = this.JsonObjSettings.FormMain[0].FrmWidth,
                    Height = this.JsonObjSettings.FormMain[0].FrmHeight,
                };

                // check if the saved bounds are nonzero and visible on any screen
                if (FrmRect != Rectangle.Empty && IsVisibleOnAnyScreen(FrmRect))
                {   // first set the bounds
                    this.MainForm.StartPosition = FormStartPosition.Manual;
                    this.MainForm.DesktopBounds = FrmRect;

                    // afterwards set the window state to the saved value (which could be Maximized)
                    this.MainForm.WindowState = this.JsonObjSettings.FormMain[0].FrmWindowState;
                }
                else
                {
                    // this resets the upper left corner of the window to windows standards
                    this.MainForm.StartPosition = FormStartPosition.WindowsDefaultLocation;

                    // we can still apply the saved size
                    if (FrmRect != Rectangle.Empty)
                    {
                        this.MainForm.Size = FrmRect.Size;
                    }
                }
            }
        }

        public void SaveMainFormPosition()
        {
            if (PdDebugMode.DebugMode)
            {
                PdLogging.WriteToLogInformation("Opslaan scherm positie hoofdscherm.");
            }

            string SettingsFile = this.JsonObjSettings.AppParam[0].SettingsFileLocation;

            if (File.Exists(SettingsFile))
            {
                if (this.MainForm.WindowState == FormWindowState.Normal)
                {
                    this.JsonObjSettings.FormMain[0].FrmWindowState = FormWindowState.Normal;

                    if (this.MainForm.Location.X >= 0)
                    {
                        this.JsonObjSettings.FormMain[0].FrmX = this.MainForm.Location.X;
                    }
                    else
                    {
                        this.JsonObjSettings.FormMain[0].FrmX = 0;
                    }

                    if (this.MainForm.Location.Y >= 0)
                    {
                        this.JsonObjSettings.FormMain[0].FrmY = this.MainForm.Location.Y;
                    }
                    else
                    {
                        this.JsonObjSettings.FormMain[0].FrmY = 0;
                    }

                    this.JsonObjSettings.FormMain[0].FrmHeight = this.MainForm.Height;
                    this.JsonObjSettings.FormMain[0].FrmWidth = this.MainForm.Width;
                }
                else
                {
                    this.JsonObjSettings.FormMain[0].FrmWindowState = this.MainForm.WindowState;
                }
            }
        }

        #endregion FormMain

        public void LoadMaintainTablesFormPosition()
        {
            if (PdDebugMode.DebugMode)
            {
                PdLogging.WriteToLogInformation("Ophalen scherm positie scherm onderhoud tabellen.");
            }

            this.TableMaintenanceForm.WindowState = FormWindowState.Normal;
            this.TableMaintenanceForm.StartPosition = FormStartPosition.WindowsDefaultBounds;

            if (this.JsonObjSettings != null && this.JsonObjSettings.FormTableMaintenance != null)
            {
                Rectangle FrmRect = new()
                {
                    X = this.JsonObjSettings.FormTableMaintenance[0].FrmX,
                    Y = this.JsonObjSettings.FormTableMaintenance[0].FrmY,
                    Width = this.JsonObjSettings.FormTableMaintenance[0].FrmWidth,
                    Height = this.JsonObjSettings.FormTableMaintenance[0].FrmHeight,
                };

                if (FrmRect != Rectangle.Empty && IsVisibleOnAnyScreen(FrmRect))
                {
                    this.TableMaintenanceForm.StartPosition = FormStartPosition.Manual;
                    this.TableMaintenanceForm.DesktopBounds = FrmRect;

                    this.TableMaintenanceForm.WindowState = this.JsonObjSettings.FormTableMaintenance[0].FrmWindowState;
                }
                else
                {
                    this.TableMaintenanceForm.StartPosition = FormStartPosition.WindowsDefaultLocation;

                    if (FrmRect != Rectangle.Empty)
                    {
                        this.TableMaintenanceForm.Size = FrmRect.Size;
                    }
                }
            }
        }

        public void SaveMaintainTablesFormPosition()
        {
            if (PdDebugMode.DebugMode)
            {
                PdLogging.WriteToLogInformation("Opslaan scherm positie scherm onderhoud tabellen.");
            }

            string SettingsFile = this.JsonObjSettings.AppParam[0].SettingsFileLocation;

            if (File.Exists(SettingsFile))
            {
                if (this.TableMaintenanceForm.WindowState == FormWindowState.Normal)
                {
                    this.JsonObjSettings.FormTableMaintenance[0].FrmWindowState = FormWindowState.Normal;

                    if (this.TableMaintenanceForm.Location.X >= 0)
                    {
                        this.JsonObjSettings.FormTableMaintenance[0].FrmX = this.TableMaintenanceForm.Location.X;
                    }
                    else
                    {
                        this.JsonObjSettings.FormTableMaintenance[0].FrmX = 0;
                    }

                    if (this.TableMaintenanceForm.Location.Y >= 0)
                    {
                        this.JsonObjSettings.FormTableMaintenance[0].FrmY = this.TableMaintenanceForm.Location.Y;
                    }
                    else
                    {
                        this.JsonObjSettings.FormTableMaintenance[0].FrmY = 0;
                    }

                    this.JsonObjSettings.FormTableMaintenance[0].FrmHeight = this.TableMaintenanceForm.Height;
                    this.JsonObjSettings.FormTableMaintenance[0].FrmWidth = this.TableMaintenanceForm.Width;
                }
                else
                {
                    this.JsonObjSettings.FormTableMaintenance[0].FrmWindowState = this.TableMaintenanceForm.WindowState;
                }
            }
        }

        #region dispose
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
        #endregion dispose
    }
}
