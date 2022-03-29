namespace PlantenDatabase
{
    using System;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Maintain the application database.
    /// </summary>
    public class PdApplicationDatabaseMaintain : PdSqliteDatabaseConnection
    {
        /// <summary>
        /// Compress the database.
        /// </summary>
        public void Compress()
        {
            // First make a copy
            if (this.CopyDatabaseFile())
            {
                this.DbConnection.Close();

                this.DbConnection.Open();
                SQLiteCommand command = new(this.DbConnection);
                command.Prepare();
                command.CommandText = "vacuum;";

                try
                {
                    command.ExecuteNonQuery();
                    PdLogging.WriteToLogInformation("De database is succesvol gecomprimeerd.");
                }
                catch (SQLiteException ex)
                {
                    PdLogging.WriteToLogError("Het comprimeren van de database is mislukt.");
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }
                }
                finally
                {
                    command.Dispose();
                    this.DbConnection.Close();
                }
            }
        }


        /// <summary>
        /// Reset all sequences in the appliction databae.
        /// </summary>
        public void ResetAllAutoIncrementFields()
        {
            this.DbConnection.Open();
            SQLiteCommand command = new(this.DbConnection);
            command.Prepare();
            command.CommandText = "delete from sqlite_sequence;";
            try
            {
                command.ExecuteNonQuery();
                PdLogging.WriteToLogInformation("Alle sequences in de applicatie database zijn gereset.");
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError("Het resetten van de sequences in de applicatie database is mislukt.");
                PdLogging.WriteToLogError("Melding :");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(ex.ToString());
                }
            }
            finally
            {
                command.Dispose();
                this.DbConnection.Close();
            }
        }

        /// <summary>
        /// Reset the sequence of one table.
        /// </summary>
        /// <param name="tableName">The table name of which the sequence will be reset.</param>
        /// </summary>
        public void ResetAutoIncrementFields(string tableName)
        {
            this.DbConnection.Open();
            SQLiteCommand command = new(this.DbConnection);
            command.Prepare();
            command.CommandText = "delete from sqlite_sequence WHERE name= @TABLE_NAME";
            try
            {
                command.Parameters.Add(new SQLiteParameter("@TABLE_NAME", tableName));
                command.ExecuteNonQuery();
                PdLogging.WriteToLogInformation(string.Format("De sequence van de tabel {0} is gereset.", tableName));
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError(string.Format("Het resetten van de sequences van de tabel {0} is mislukt.", tableName));
                PdLogging.WriteToLogError("Melding :");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(ex.ToString());
                }
            }
            finally
            {
                command.Dispose();
                this.DbConnection.Close();
            }
        }

        /// <summary>
        /// Check if the database is locked.
        /// </summary>
        /// <returns>True if the database is locked.</returns>
        public bool IsDatabaseLocked()
        {
            bool locked = true;
            this.DbConnection.Open();

            try
            {
                SQLiteCommand beginCommand = this.DbConnection.CreateCommand();
                beginCommand.CommandText = "BEGIN EXCLUSIVE"; // tries to acquire the lock

                // CommandTimeout is set to 0 to get error immediately if DB is locked. Otherwise it will wait for 30 sec by default.
                beginCommand.CommandTimeout = 0;
                beginCommand.ExecuteNonQuery();

                SQLiteCommand commitCommand = this.DbConnection.CreateCommand();
                commitCommand.CommandText = "COMMIT"; // releases the lock immediately
                commitCommand.ExecuteNonQuery();
                locked = false;
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError("Database is locked error.");
                PdLogging.WriteToLogError("Melding :");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(ex.ToString());
                }
            }
            finally
            {
                this.DbConnection.Close();
            }

            return locked;
        }

        private bool CopyDatabaseFile()
        {
            PdLogging.WriteToLogInformation("Maak eerst een kopie van de applicatie database voordat deze wordt gecomprimeerd.");
            bool result = false;

            if (string.IsNullOrEmpty(this.DatabaseFileName) || string.IsNullOrEmpty(Path.GetDirectoryName(this.DatabaseFileName)))
            {
                if (string.IsNullOrEmpty(this.DatabaseFileName))
                {
                    PdLogging.WriteToLogError("Database bestandsnaam ontbreekt.");
                }
                else if (string.IsNullOrEmpty(Path.GetDirectoryName(this.DatabaseFileName)))
                {
                    PdLogging.WriteToLogError("Pad naar Database bestand ontbreekt.");
                }

                return result;
            }

            PdLogging.WriteToLogInformation("Maak eerst een kopie van de applicatie database voordat deze wordt gecomprimeerd...");
            string fileToCopy = this.DatabaseFileName;
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);                
            string backUpPath = Path.Combine(Path.GetDirectoryName(this.DatabaseFileName), PdSettings.BackUpFolder);
            string newLocation = backUpPath + currentDate + "_" + PdSettings.SqlLiteDatabaseName;

            if (Directory.Exists(backUpPath))
            {
                if (File.Exists(fileToCopy))
                {
                    if (!File.Exists(newLocation))
                    {
                        File.Copy(fileToCopy, newLocation, false);  // Overwrite file = false
                        PdLogging.WriteToLogInformation("Het kopiëren van het bestand '" + PdSettings.SqlLiteDatabaseName + "' is gereed.");
                        result = true;
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Het bestand bestaat reeds. Wilt u het bestand overschrijven?", "Waarschuwing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            File.Copy(fileToCopy, newLocation, true);  // overwrite file = true
                            PdLogging.WriteToLogInformation("Het kopiëren van het bestand '" + PdSettings.SqlLiteDatabaseName + "' is gereed.");
                            result = true;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            PdLogging.WriteToLogInformation("Het kopiëren van het bestand '" + PdSettings.SqlLiteDatabaseName + "' is afgebroken.");
                            PdLogging.WriteToLogInformation("Het bestand komt reeds voor.");
                            result = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("De map '{0}' is niet aanwezig.", PdSettings.BackUpFolder), "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = false;
                }
            }

            return result;
        }
    }
}
