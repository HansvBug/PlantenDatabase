namespace PlantenDatabase
{
    using Microsoft.Win32.SafeHandles;
    using System.Data;
    using System.Runtime.InteropServices;

    /// <summary>
    /// CSV data import.
    /// </summary>
    public class PdImportCsv : IDisposable
    {
        private readonly SafeHandle safeHandle = new SafeFileHandle(IntPtr.Zero, true); // Instantiate a SafeHandle instance.
        private bool disposed;      // Flag: Has Dispose already been called?
        private DataTable dt;
        private string tableName { get; set; }
        private string[]? csvData;  // Holds the imported CSV data.
        private dynamic? JsonObjSettings { get; set; }

        #region constructor
        public PdImportCsv(string tableName, DataTable dt)
        {
            this.tableName = tableName;
            this.dt = dt;
            csvData = null;
            LoadSettings();
        }

        private void LoadSettings()
        {
            using PdSettingsManager set = new();
            set.LoadSettings();
            this.JsonObjSettings = set.JsonObjSettings;
        }
        #endregion constructor

        /// <summary>
        /// Start the import of CSV data.
        /// </summary>
        public bool ImportCsv()
        {
            // Open the file and put the data in string array csvData.
            if (this.OpenCsvFile())
            {
                if (this.DataImport())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool OpenCsvFile()
        {
            using OpenFileDialog openFileDialog = new();
             openFileDialog.InitialDirectory = this.JsonObjSettings.FormConfigure[0].FolderImportLocation;            

            openFileDialog.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Open een csv bestand.";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.CSVToArray(openFileDialog.FileName);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CSVToArray(string file)
        {
            if (File.Exists(file))
            {
                using StreamReader sr = new(file);
                string strResult = sr.ReadToEnd();

                this.csvData = strResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
        }

        private bool DataImport()
        {
            int maxImportRecords = 1000000;

            // Import max 1.000.000 records.
            if (this.csvData != null && this.csvData.Length > 0 && this.csvData.Length <= maxImportRecords)
            {
                string[] newRecordData = Array.Empty<string>();
                int errorCounter = 0;
                int importedCounter = 0;
                string importTable = string.Empty;

                switch (this.tableName)
                {
                    case "PD_DOMEIN":
                        try
                        {
                            int firstCol, secondCol;
                            newRecordData = this.csvData[0].Split(";");
                            if (newRecordData[0] == "NAAM" && newRecordData[1] == "NED_NAAM")
                            {
                                firstCol = 0; // NAME
                                secondCol = 1;  // LOCAL_NAME

                                for (var i = 1; i <= this.csvData.Length - 1; i++)
                                {
                                    newRecordData = this.csvData[i].Split(";");

                                    // Check the length of the data
                                    if (newRecordData[firstCol].Length <= 100 && newRecordData[secondCol].Length <= 100)
                                    {
                                        DataRow newRow = this.dt.NewRow();
                                        newRow["NAAM"] = newRecordData[firstCol];
                                        newRow["NED_NAAM"] = newRecordData[secondCol];

                                        newRow["DATUM_AANGEMAAKT"] = DateTime.Now;
                                        newRow["GUID"] = Guid.NewGuid().ToString();
                                        newRow["AANGEMAAKT_DOOR"] = Environment.UserName;

                                        this.dt.Rows.Add(newRow);
                                        importedCounter++;
                                        importTable = "PD_DOMEIN";
                                    }
                                    else
                                    {
                                        errorCounter++;
                                        PdLogging.WriteToLogWarning(string.Format("Domein waarde niet geïmporteerd; Naam = {0} , ned_naam = {1}", newRecordData[firstCol], newRecordData[secondCol]));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("De header in de CSV moet 'NAAM, NEDNAAM' zijn. Pas dit aan en probeer opnieuw.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }


                            if (errorCounter > 0)
                            {
                                MessageBox.Show("Niet alle records zijn geïmporteerd. Zie logbestand voor de geweigerde records.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            break;
                        }
                        catch (IndexOutOfRangeException ioex)
                        {
                            PdLogging.WriteToLogError("Fout bij het importeren van een domein lijst. (Domein).");
                            PdLogging.WriteToLogError("melding:");
                            PdLogging.WriteToLogError(ioex.Message);
                            if (PdDebugMode.DebugMode)
                            {
                                PdLogging.WriteToLogError(ioex.ToString());
                            }
                        }

                        break;
                    case "PD_GESLACHT":
                        try
                        {
                            int firstCol, secondCol;
                            newRecordData = this.csvData[0].Split(";");
                            if (newRecordData[0] == "NAAM" && newRecordData[1] == "FAMILIE_ID")
                            {
                                firstCol = 0;  // NAME
                                secondCol = 1; // FAMILY_ID

                                for (var i = 1; i <= this.csvData.Length - 1; i++)
                                {
                                    newRecordData = this.csvData[i].Split(";");

                                    // Check the length of the data
                                    if (newRecordData[firstCol].Length <= 100)
                                    {
                                        DataRow newRow = this.dt.NewRow();
                                        newRow["NAAM"] = newRecordData[firstCol];
                                        if (!string.IsNullOrEmpty(newRecordData[secondCol]))
                                        {
                                            newRow["FAMILIE_ID"] = newRecordData[secondCol];
                                        }

                                        newRow["DATUM_AANGEMAAKT"] = DateTime.Now;
                                        newRow["GUID"] = Guid.NewGuid().ToString();
                                        newRow["AANGEMAAKT_DOOR"] = Environment.UserName;

                                        this.dt.Rows.Add(newRow);
                                        importedCounter++;
                                        importTable = "PD_GESLACHT";
                                    }
                                    else
                                    {
                                        errorCounter++;
                                        PdLogging.WriteToLogWarning(string.Format("Domein waarde niet geïmporteerd; Naam = {0} , Famlilie_id = {1}", newRecordData[firstCol], newRecordData[secondCol]));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("De header in de CSV moet 'NAAM, FAMLILIE_ID' zijn. Pas dit aan en probeer opnieuw.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }


                            if (errorCounter > 0)
                            {
                                MessageBox.Show("Niet alle records zijn geïmporteerd. Zie logbestand voor de geweigerde records.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            break;
                        }
                        catch (IndexOutOfRangeException ioex)
                        {
                            PdLogging.WriteToLogError("Fout bij het importeren van een domein lijst. (Geslacht.)");
                            PdLogging.WriteToLogError("melding:");
                            PdLogging.WriteToLogError(ioex.Message);
                            if (PdDebugMode.DebugMode)
                            {
                                PdLogging.WriteToLogError(ioex.ToString());
                            }
                        }

                        break;
                    case "PD_GRONDSOORT":
                        try
                        {
                            int firstCol;
                            newRecordData = this.csvData[0].Split(";");
                            if (newRecordData[0] == "NAAM")
                            {
                                firstCol = 0;  // NAME

                                for (var i = 1; i <= this.csvData.Length - 1; i++)
                                {
                                    newRecordData = this.csvData[i].Split(";");

                                    // Check the length of the data
                                    if (newRecordData[firstCol].Length <= 100)
                                    {
                                        DataRow newRow = this.dt.NewRow();
                                        newRow["NAAM"] = newRecordData[firstCol];
                                        newRow["DATUM_AANGEMAAKT"] = DateTime.Now;
                                        newRow["GUID"] = Guid.NewGuid().ToString();
                                        newRow["AANGEMAAKT_DOOR"] = Environment.UserName;
                                        this.dt.Rows.Add(newRow);
                                        importedCounter++;
                                        importTable = "PD_GRONDSOORT";
                                    }
                                    else
                                    {
                                        errorCounter++;
                                        PdLogging.WriteToLogWarning(string.Format("Domein waarde niet geïmporteerd; Naam = {0}.", newRecordData[firstCol]));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("De header in de CSV moet 'NAAM' zijn. Pas dit aan en probeer opnieuw.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }


                            if (errorCounter > 0)
                            {
                                MessageBox.Show("Niet alle records zijn geïmporteerd. Zie logbestand voor de geweigerde records.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            break;
                        }
                        catch (IndexOutOfRangeException ioex)
                        {
                            PdLogging.WriteToLogError("Fout bij het importeren van een domein lijst. (Grondsoort).");
                            PdLogging.WriteToLogError("melding:");
                            PdLogging.WriteToLogError(ioex.Message);
                            if (PdDebugMode.DebugMode)
                            {
                                PdLogging.WriteToLogError(ioex.ToString());
                            }
                        }

                        break;
                    case "PD_FAMILIE":
                        try
                        {
                            int firstCol, secondCol, thirdCol;
                            newRecordData = this.csvData[0].Split(";");
                            if (newRecordData[0] == "NAAM" && newRecordData[1] == "NED_NAAM")
                            {
                                firstCol = 0;   // NAAM
                                secondCol = 1;  // NED_NAAM
                                thirdCol = 2;   // ORDE_ID

                                for (var i = 1; i <= this.csvData.Length - 1; i++)
                                {
                                    newRecordData = this.csvData[i].Split(";");

                                    // Check the length of the data
                                    if (newRecordData[firstCol].Length <= 100 && newRecordData[secondCol].Length <= 100)
                                    {
                                        DataRow newRow = this.dt.NewRow();
                                        newRow["NAAM"] = newRecordData[firstCol];
                                        newRow["NED_NAAM"] = newRecordData[secondCol];

                                        if (!string.IsNullOrEmpty(newRecordData[thirdCol]))
                                        {
                                            newRow["ORDE_ID"] = newRecordData[thirdCol];
                                        }
                                        newRow["DATUM_AANGEMAAKT"] = DateTime.Now;
                                        newRow["GUID"] = Guid.NewGuid().ToString();
                                        newRow["AANGEMAAKT_DOOR"] = Environment.UserName;

                                        this.dt.Rows.Add(newRow);
                                        importedCounter++;
                                        importTable = "PD_FAMILIE";
                                    }
                                    else
                                    {
                                        errorCounter++;
                                        PdLogging.WriteToLogWarning(string.Format("Domein waarde niet geïmporteerd; Naam = {0} , Ned_naam = {1}", newRecordData[firstCol], newRecordData[secondCol]));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("De header in de CSV moet 'NAAM, NED_NAAM, ORDE_ID' zijn. Pas dit aan en probeer opnieuw.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }


                            if (errorCounter > 0)
                            {
                                MessageBox.Show("Niet alle records zijn geïmporteerd. Zie logbestand voor de geweigerde records.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            break;
                        }
                        catch (IndexOutOfRangeException ioex)
                        {
                            PdLogging.WriteToLogError("Fout bij het importeren van een domein lijst.");
                            PdLogging.WriteToLogError("melding:");
                            PdLogging.WriteToLogError(ioex.Message);
                            if (PdDebugMode.DebugMode)
                            {
                                PdLogging.WriteToLogError(ioex.ToString());
                            }
                        }

                        break;
                    case "PD_ORDE":
                        try
                        {
                            int firstCol;
                            newRecordData = this.csvData[0].Split(";");
                            if (newRecordData[0] == "NAAM")
                            {
                                firstCol = 0;  // NAAM

                                for (var i = 1; i <= this.csvData.Length - 1; i++)
                                {
                                    newRecordData = this.csvData[i].Split(";");

                                    // Check the length of the data
                                    if (newRecordData[firstCol].Length <= 100)
                                    {
                                        DataRow newRow = this.dt.NewRow();
                                        newRow["NAAM"] = newRecordData[firstCol];
                                        newRow["DATUM_AANGEMAAKT"] = DateTime.Now;
                                        newRow["GUID"] = Guid.NewGuid().ToString();
                                        newRow["AANGEMAAKT_DOOR"] = Environment.UserName;
                                        this.dt.Rows.Add(newRow);
                                        importedCounter++;
                                        importTable = "PD_GRONDSOORT";
                                    }
                                    else
                                    {
                                        errorCounter++;
                                        PdLogging.WriteToLogWarning(string.Format("Domein waarde niet geïmporteerd; Naam = {0}.", newRecordData[firstCol]));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("De header in de CSV moet 'NAAM' zijn. Pas dit aan en probeer opnieuw.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }


                            if (errorCounter > 0)
                            {
                                MessageBox.Show("Niet alle records zijn geïmporteerd. Zie logbestand voor de geweigerde records.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch (IndexOutOfRangeException ioex)
                        {
                            PdLogging.WriteToLogError("Fout bij het importeren van een domein lijst. (Orde).");
                            PdLogging.WriteToLogError("melding:");
                            PdLogging.WriteToLogError(ioex.Message);
                            if (PdDebugMode.DebugMode)
                            {
                                PdLogging.WriteToLogError(ioex.ToString());
                            }
                        }

                        break;
                            default:
                        break;
                }

                if (importedCounter > 0)
                {
                    PdLogging.WriteToLogInformation(string.Format("{0} records geïmporteerd in tabel: {1}.",importedCounter.ToString(), importTable));
                }
                return true;
            }
            else
            {
                if (this.csvData != null && this.csvData.Length > maxImportRecords)
                {
                    MessageBox.Show(string.Format("Het bestand bevat te veel records. U kunt maximaal {0} tegelijk inlezen.", maxImportRecords), "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (this.csvData == null)
                {
                    PdLogging.WriteToLogError("Onverwachte fout bij het imporeren van een csv bestand. Oorzaak: this.csvData == null");
                    MessageBox.Show("Onverwachte fout bij het importeren van het bestand. Conroleer het log bestand.", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
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
