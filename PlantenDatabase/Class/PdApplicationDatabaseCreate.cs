namespace PlantenDatabase
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    public class PdApplicationDatabaseCreate : PdSqliteDatabaseConnection
    {
        private int latestDbVersion;

        public PdApplicationDatabaseCreate()
        {
            this.Error = false;
            this.latestDbVersion = PdSettings.DatabaseVersion;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an Error has occurred.
        /// </summary>
        private bool Error { get; set; }

        private readonly string creTblSettingsMeta = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.SETTINGS_META) +
                               "GUID               VARCHAR(50)          ," +
                               "KEY                VARCHAR(50)  UNIQUE  ," +
                               "VALUE              VARCHAR(255))";

        private readonly string createTblDomain = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_DOMEIN) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "WET_NAAM           VARCHAR(100)    ," +
                                "NED_NAAM           VARCHAR(100)    ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))    ";

        private readonly string createTblDomainIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_DOMEIN_ID_IDX, PdTableName.PD_DOMEIN);

        private readonly string createTblKingdom = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_RIJK) +
                                "ID             INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID           VARCHAR(50)         ," +
                                "CODE           VARCHAR(10)         ," +
                                "WET_NAAM       VARCHAR(100)        ," +
                                "DOMEIN_ID      INTEGER             ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100)    ," +
                                string.Format("FOREIGN KEY (DOMEIN_ID) REFERENCES {0}(ID) ", PdTableName.PD_DOMEIN) +
                                "ON UPDATE RESTRICT " +
                                "ON DELETE RESTRICT )";

        private readonly string createTblKingdomIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_RIJK_ID_IDX, PdTableName.PD_RIJK);

        private readonly string createTblDivision = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_STAM) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "WET_NAAM           VARCHAR(100)    ," +
                                "RIJK_ID            INTEGER         ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100)    ," +
                                string.Format("FOREIGN KEY (RIJK_ID) REFERENCES {0}(ID) ", PdTableName.PD_RIJK) +
                                "ON UPDATE RESTRICT " +
                                "ON DELETE RESTRICT )";

        private readonly string createTblDivisionIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_STAM_ID_IDX, PdTableName.PD_STAM);

        private readonly string createTblClass = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_KLASSE) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "WET_NAAM           VARCHAR(100)    ," +
                                "STAM_ID            INTEGER         ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100)    ," +
                                string.Format("FOREIGN KEY (STAM_ID) REFERENCES {0}(ID) ", PdTableName.PD_STAM) +
                                "ON UPDATE RESTRICT " +
                                "ON DELETE RESTRICT )";

        private readonly string createTblClassIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_KLASSE_ID_IDX, PdTableName.PD_KLASSE);

        private readonly string createTblOrder = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_ORDE) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "WET_NAAM           VARCHAR(100)    ," +
                                "KLASSE_ID          INTEGER         ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100)    ," +
                                string.Format("FOREIGN KEY (KLASSE_ID) REFERENCES {0}(ID) ", PdTableName.PD_KLASSE) +
                                "ON UPDATE RESTRICT " +
                                "ON DELETE RESTRICT )";

        private readonly string createTblOrderIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_ORDE_ID_IDX, PdTableName.PD_ORDE);

        private readonly string createTblFamily = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_FAMILIE) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "WET_NAME           VARCHAR(100)    ," +
                                "NED_NAME           VARCHAR(100)    ," +
                                "ORDE_ID            INTEGER         ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100)    ," +
                                string.Format("FOREIGN KEY (ORDE_ID) REFERENCES {0}(ID) ", PdTableName.PD_ORDE) +
                                "ON UPDATE RESTRICT " +
                                "ON DELETE RESTRICT )";

        private readonly string createTblFamilyIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_FAMILIE_ID_IDX, PdTableName.PD_FAMILIE);

        private readonly string createTblGenus = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_GESLACHT) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "WET_NAAM           VARCHAR(100)    ," +
                                "FAMILIE_ID         INTEGER         ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100)    ," +
                                string.Format("FOREIGN KEY (FAMILIE_ID) REFERENCES {0}(ID) ", PdTableName.PD_FAMILIE) +
                                "ON UPDATE RESTRICT " +
                                 "ON DELETE RESTRICT )";

        private readonly string createTblLocation = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_STANDPLAATS) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "NAAM               VARCHAR(100)    ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))    ";

        private readonly string createTblSoilType = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_GRONDSOORT) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "NAAM               VARCHAR(100)    ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))    ";

        private readonly string createTblColor = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_KLEUR) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "NAAM               VARCHAR(100)    ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))    ";

        private readonly string createTblCategory = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_CATEGORIE) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "NAAM               VARCHAR(100)    ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))    ";

        private readonly string createTblShape = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_VORM) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "GUID               VARCHAR(50)     ," +
                                "CODE               VARCHAR(10)     ," +
                                "NAAM               VARCHAR(100)    ," +
                                "DATUM_AANGEMAAKT   DATE            ," +
                                "DATE_GEWIJZIGD     DATE            ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)    ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))    ";

        private readonly string createTblGenusIndex = string.Format("CREATE UNIQUE INDEX IF NOT EXISTS {0} ON {1}(ID)", PdTableName.PD_GESLACHT_ID_IDX, PdTableName.PD_GESLACHT);

        private readonly string createPlantData = string.Format("CREATE TABLE IF NOT EXISTS {0} (", PdTableName.PD_PLANTENDATA) +
                                "ID                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ," +
                                "CODE               VARCHAR(10)         ," +
                                "GESLACHT_ID        INTEGER NOT NULL    ," +
                                "SOORT              VARCHAR(100)        ," +
                                "VARIETEIT          VARCHAR(100)        ," +
                                "CULTIVAR           VARCHAR(100)        ," +
                                "NED_NAAM_1         VARCHAR(100)        ," +
                                "NED_NAAM_2         VARCHAR(100)        ," +
                                "NED_NAAM_3         VARCHAR(100)        ," +
                                "HOOGTE_M           INTEGER             ," +
                                "BETEKENIS          VARCHAR(500)        ," +
                                "HOOGTE_BEREIK      VARCHAR(25)         ," +
                                "STANDPLAATS_ID     INTEGER             ," +
                                "GRONDSOORT_ID      INTEGER             ," +
                                "BODEMVOCHTIGHEID   VARCHAR(50)         ," +
                                "BLOEIPERIODE       VARCHAR(50)         ," +
                                "BLOEMKLEUR_ID      INTEGER             ," +
                                "BLOEIWIJZE         VARCHAR(50)         ," +
                                "BLOEMVORM_ID       VARCHAR(50)         ," +
                                "CATEGORIE_ID       INTEGER             ," +
                                "VORM_ID            INTEGER             ," +
                                "BLADSTAND          VARCHAR(50)         ," +
                                "BLADKLEUR          VARCHAR(50)         ," +
                                "BLADRAND           VARCHAR(50)         ," +
                                "HERFSTKLEUR        VARCHAR(50)         ," +
                                "VRUCHTEN           VARCHAR(50)         ," +
                                "STEKELS_DOORNS     BOOL                ," +
                                "WINTERHARDHEID     VARCHAR(50)         ," +
                                "WINDBESTENDIGHEID  VARCHAR(50)         ," +
                                "WINTERGROEN        BOOL                ," +
                                "VERHARDING         VARCHAR(50)         ," +
                                "TOEPASSING         VARCHAR(255)        ," +
                                "OORSPRONG          VARCHAR(200)        ," +
                                "TOELICHTING        VARCHAR(1500)       ," +
                                "EIGEN_TUIN         BOOL                ," +
                                "DATUM_AANGEMAAKT   DATE                ," +
                                "DATE_GEWIJZIGD     DATE                ," +
                                "AANGEMAAKT_DOOR    VARCHAR(100)        ," +
                                "GEMUTEERD_DOOR     VARCHAR(100))        ";

        private void ErrorMessage()
        {
            if (!this.Error)
            {
                MessageBox.Show("De database is aangemaakt.", "Informatie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Het database bestand of 1 van de tabellen is niet aangemaakt." + Environment.NewLine +
                    Environment.NewLine +
                    "Controleer het log bestand.",
                    "Fout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void CreDatabaseFile()
        {
            Cursor.Current = Cursors.WaitCursor;
            if (!string.IsNullOrEmpty(this.DatabaseFileName))
            {
                try
                {
                    // Only with a first install. (Unless a user removed the database file)
                    if (!File.Exists(this.DatabaseFileName))
                    {
                        SQLiteConnection.CreateFile(this.DatabaseFileName); // The creation of a new empty database file

                        PdLogging.WriteToLogInformation("De database '" + this.DatabaseFileName + "' is aangemaakt.");
                    }
                    else
                    {
                        PdLogging.WriteToLogInformation("Het database bestand is aanwezig, er is géén nieuw leeg database bestand aangemaakt.");
                    }
                }
                catch (IOException ex)
                {
                    this.Error = true;

                    PdLogging.WriteToLogError(string.Format("De applicatie database is niet aangemaakt. Een IOException heeft opgetreden. ({0}).", this.DatabaseFileName));
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }

                    MessageBox.Show("Onverwachte fout bij het aanmaken van een leeg database bestand. (IOException)", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    this.Error = true;

                    PdLogging.WriteToLogError("De applicatie database is niet aangemaakt. (" + this.DatabaseFileName + ").");
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }

                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("Onverwachte fout bij het aanmaken van een leeg database bestand.", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                PdLogging.WriteToLogError("De SQlite database is niet aangemaakt omdat er geen locatie of database naam is opgegeven.");
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Bestandlocatie ontbreekt.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default;
        }

        private void EnableFK()
        {
            string fk = "PRAGMA foreign_keys = ON";
            SQLiteCommand command = new(fk, this.DbConnection);

            try
            {
                command.ExecuteNonQuery();
                PdLogging.WriteToLogInformation("Foreign_keys aangezet. (Versie " + PdSettings.DatabaseVersion.ToString() + ").");
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError("Aanmaken foreign keys optie is mislukt. (Versie " + PdSettings.DatabaseVersion.ToString() + ").");
                PdLogging.WriteToLogError("Melding :");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(ex.ToString());
                }

                this.Error = true;
            }
            finally
            {
                command.Dispose();
            }
        }

        private bool GetTableName(string tblName)
        {
            string selectSql = string.Format("select NAME from sqlite_master where type = 'table' and NAME = '{0}' order by NAME", tblName);

            SQLiteCommand command = new(selectSql, this.DbConnection);
            try
            {
                SQLiteDataReader dr = command.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    // Returns only one row... so no while reader read
                    if (dr.GetValue(0).ToString() == tblName)
                    {
                        dr.Close();
                        return true;
                    }
                }

                return false;
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError(string.Format("Opvragen tablenaam is mislukt. Betreft tabelnaam: {0}", PdTableName.SETTINGS_META));
                PdLogging.WriteToLogError("Melding :");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(ex.ToString());
                }

                return false;
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Get the database version.
        /// </summary>
        /// <returns>The database version number.</returns>
        public int SelectMeta() // Made public so you can check the version on every application start
        {
            int sqlLiteMetaVersion = 0;

            // First check if the table exists. (The first time when de database file is created, the table does noet exists).
            if (this.GetTableName(PdTableName.SETTINGS_META))
            {
                string selectSql = string.Format("SELECT VALUE FROM {0} WHERE KEY = 'VERSION'", PdTableName.SETTINGS_META);

                PdLogging.WriteToLogInformation("Controle op versie van de applicatie database.");

                SQLiteCommand command = new(selectSql, this.DbConnection);
                try
                {
                    SQLiteDataReader dr = command.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        sqlLiteMetaVersion = int.Parse(dr[0].ToString(), CultureInfo.InvariantCulture);
                    }

                    dr.Close();
                }
                catch (SQLiteException ex)
                {
                    PdLogging.WriteToLogError("Opvragen meta versie is misukt. (Versie " + Convert.ToString(this.latestDbVersion, CultureInfo.InvariantCulture) + ").");
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }

                    this.Error = true;
                }
                finally
                {
                    command.Dispose();
                    this.DbConnection.Close();
                }
            }

            return sqlLiteMetaVersion;
        }

        private void InsertMeta(string version)
        {
            if (!this.Error)
            {
                string insertSQL = string.Format("INSERT INTO {0} (GUID, KEY, VALUE) VALUES(@GUID, @KEY, @VERSION)", PdTableName.SETTINGS_META);

                SQLiteCommand command = new(insertSQL, this.DbConnection);
                try
                {
                    command.Parameters.Add(new SQLiteParameter("@GUID", Guid.NewGuid().ToString()));
                    command.Parameters.Add(new SQLiteParameter("@KEY", "VERSION"));
                    command.Parameters.Add(new SQLiteParameter("@VERSION", version));

                    command.ExecuteNonQuery();
                    PdLogging.WriteToLogInformation(string.Format("De tabel {0} is gewijzigd. (Versie ", PdTableName.SETTINGS_META) + version + ").");
                }
                catch (SQLiteException ex)
                {
                    PdLogging.WriteToLogError(string.Format("Het invoeren van het database versienummer in de tabel {0} is misukt. (Versie " + Convert.ToString(this.latestDbVersion, CultureInfo.InvariantCulture) + ").", PdTableName.SETTINGS_META));
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }

                    this.Error = true;
                }
                finally
                {
                    command.Dispose();
                    this.DbConnection.Close();
                }
            }
            else
            {
                PdLogging.WriteToLogError(string.Format("Het invoeren van het database versienummer in de tabel {0} is mislukt.", PdTableName.SETTINGS_META));
            }
        }

        private void UpdateMeta(string version)
        {
            using var tr = this.DbConnection.BeginTransaction();
            string insertSQL = string.Format("UPDATE {0} SET VALUE  = @VERSION WHERE KEY = @KEY", PdTableName.SETTINGS_META);

            SQLiteCommand command = new(insertSQL, this.DbConnection);
            try
            {
                command.Parameters.Add(new SQLiteParameter("@VERSION", version));
                command.Parameters.Add(new SQLiteParameter("@KEY", "VERSION"));

                command.ExecuteNonQuery();
                PdLogging.WriteToLogInformation(string.Format("De tabel {0} is gewijzigd. (Versie " + version + ").", PdTableName.SETTINGS_META));
                command.Dispose();
                tr.Commit();
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError(string.Format("het wijzigen van de versie in tabel {0} is misukt. (Versie " + version + ").", PdTableName.SETTINGS_META));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug(ex.ToString());
                }

                command.Dispose();
                tr.Rollback();
            }
        }

        private void CreateTable(string sqlCreateString, string tableName, string version)
        {
            if (!this.Error)
            {
                SQLiteCommand command = new(sqlCreateString, this.DbConnection);
                try
                {
                    command.ExecuteNonQuery();
                    PdLogging.WriteToLogInformation(string.Format("De tabel {0} is aangemaakt. (Versie {1}).", tableName, version));
                }
                catch (SQLiteException ex)
                {
                    PdLogging.WriteToLogError(string.Format("Aanmaken van de tabel {0} is misukt. (Versie {1}).", tableName, version));
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }

                    this.Error = true;
                }
                finally
                {
                    command.Dispose();
                }
            }
            else
            {
                PdLogging.WriteToLogError(string.Format("Het aanmaken van de tabel {0} is niet uitgevoerd.", tableName));
            }
        }

        private void CreateIndex(string sqlCreIndexString, string indexName, string version)
        {
            if (!this.Error)
            {
                SQLiteCommand command = new(sqlCreIndexString, this.DbConnection);
                try
                {
                    command.ExecuteNonQuery();
                    PdLogging.WriteToLogInformation(string.Format("De index {0} is aangemaakt. (Versie {1}).", indexName, version));
                }
                catch (SQLiteException ex)
                {
                    PdLogging.WriteToLogError(string.Format("Aanmaken van de index {0} is misukt. (Versie {1}).", indexName, version));
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogError(ex.Message);
                    if (PdDebugMode.DebugMode)
                    {
                        PdLogging.WriteToLogDebug(ex.ToString());
                    }

                    this.Error = true;
                }
                finally
                {
                    command.Dispose();
                }
            }
            else
            {
                PdLogging.WriteToLogError(string.Format("Het aanmaken van de index {0} is niet uitgevoerd.", indexName));
            }
        }

        /// <summary>
        /// Create the database file and the tables.
        /// </summary>
        public bool CreateDatabase()
        {
            this.DbConnection.Open();
            if (this.latestDbVersion >= 1 && this.SelectMeta() == 0)
            {
                this.CreDatabaseFile();

                this.EnableFK();

                string version = "1";
                this.CreateTable(this.creTblSettingsMeta, PdTableName.SETTINGS_META, version);

                this.CreateTable(this.createTblDomain, PdTableName.PD_DOMEIN, version);
                this.CreateTable(this.createTblKingdom, PdTableName.PD_RIJK, version);
                this.CreateTable(this.createTblDivision, PdTableName.PD_STAM, version);
                this.CreateTable(this.createTblClass, PdTableName.PD_KLASSE, version);
                this.CreateTable(this.createTblOrder, PdTableName.PD_ORDE, version);
                this.CreateTable(this.createTblFamily, PdTableName.PD_FAMILIE, version);
                this.CreateTable(this.createTblGenus, PdTableName.PD_GESLACHT, version);
                this.CreateTable(this.createTblLocation, PdTableName.PD_STANDPLAATS, version);
                this.CreateTable(this.createTblSoilType, PdTableName.PD_GRONDSOORT, version);
                this.CreateTable(this.createTblColor, PdTableName.PD_KLEUR, version);
                this.CreateTable(this.createTblCategory, PdTableName.PD_CATEGORIE, version);
                this.CreateTable(this.createTblShape, PdTableName.PD_VORM, version);
                this.CreateTable(this.createPlantData, PdTableName.PD_PLANTENDATA, version);

                this.CreateIndex(this.createTblDomainIndex, PdTableName.PD_DOMEIN_ID_IDX, version);
                this.CreateIndex(this.createTblKingdomIndex, PdTableName.PD_RIJK_ID_IDX, version);
                this.CreateIndex(this.createTblDivisionIndex, PdTableName.PD_STAM_ID_IDX, version);
                this.CreateIndex(this.createTblClassIndex, PdTableName.PD_KLASSE_ID_IDX, version);
                this.CreateIndex(this.createTblOrderIndex, PdTableName.PD_ORDE_ID_IDX, version);
                this.CreateIndex(this.createTblFamilyIndex, PdTableName.PD_FAMILIE_ID_IDX, version);
                this.CreateIndex(this.createTblGenusIndex, PdTableName.PD_GESLACHT_ID_IDX, version);

                this.InsertMeta(version);  // Set the version 1
            }

            this.DbConnection.Close();
            this.DbConnection.Dispose();

            this.ErrorMessage();
            if (!this.Error)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
