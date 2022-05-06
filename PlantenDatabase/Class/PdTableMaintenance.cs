namespace PlantenDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.Windows.Forms;

    public class PdTableMaintenance : PdSqliteDatabaseConnection
    {
        private readonly DataGridView dgv = new();
        private PdDatabaseTable dbTable = new();
        private PdDatabaseTables dbTables = new();

        /// <summary>
        /// Default treeview rootnode name.
        /// </summary>
        public const string TrvRootNodeName = "Tabellen";

        private BindingSource bndSource = new();

        /// <summary>
        /// Get or set the bindingsource.
        /// </summary>
        public BindingSource BndSource
        {
            get { return this.bndSource; }
            set { this.bndSource = value; }
        }

        private DataTable dt = new();

        /// <summary>
        /// Gets or sets the selected data table.
        /// </summary>
        public DataTable Dt
        {
            get { return this.dt; }
            set { if (value != null)
                {
                    this.dt = value;
                }
            }
        }

        private SQLiteDataAdapter da = new();

        /// <summary>
        /// Gets or sets the DataAdapeter used for updating the selected table.
        /// </summary>
        public SQLiteDataAdapter Da
        {
            get { return this.da; }
            set { this.da = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdTableMaintenance"/> class.
        /// </summary>
        /// <param name="dgv">Datagrid view on the form.</param>
        /// <param name="bndSource">The bindingsource between datagridview and the data.</param>
        public PdTableMaintenance(DataGridView dgv, BindingSource bndSource)
        {
            this.dgv = dgv;
            this.BndSource = bndSource; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdTableMaintenance"/> class.
        /// Default constructor.
        /// </summary>
        public PdTableMaintenance()
        {
            // Default constructor
        }        

        /// <summary>
        /// Get all table names.
        /// </summary>
        public void GetAllTableNames()
        {
            string selectSql = string.Format("SELECT name FROM sqlite_master WHERE type = 'table' and name not like 'sqlite_%' and name not in ('{0}', '{1}')", PdTableName.SETTINGS_META, PdTableName.PD_PLANTENDATA);

            try
            {
                PdLogging.WriteToLogInformation("Ophalen tabelnamen.");
                int counter = 1;
                this.DbConnection.Open();
                using SQLiteCommand command = new(selectSql, this.DbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                using (DataTable dt = new())
                {
                    dt.Load(reader);
                    foreach (DataRow row in dt.Rows)
                    {
                        this.dbTable = new PdDatabaseTable
                        {
                            TableName = row["Name"].ToString(),
                            Text = row["Name"].ToString(), // Used in the treeview as node.text.
                            ID = counter,  // Just testing, not used yet.
                        }; // New object holding the table name
                        this.dbTables.Items.Add(this.dbTable);

                        counter++;
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError("Fout bij het ophalen van alle tabelnamen.");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        /// <summary>
        /// Load the tablenames into the treeview.
        /// </summary>
        /// <param name="trv">The treeview on the form.</param>
        public void LoadTreeviewTableNames(TreeView trv)
        {
            PdLogging.WriteToLogInformation("De tabelnamen worden in de treeview gezet. (Tabellen beheer).");
            trv.BeginUpdate();
            trv.Nodes.Clear();
            TreeNode rootNode = trv.Nodes.Add(PdSettings.ApplicationName, TrvRootNodeName); // Settings.ApplicationName);    //Set the rootnode

            foreach (PdDatabaseTable allDbTbl in this.dbTables.Items)
            {
                TreeNode tn = rootNode.Nodes.Add(allDbTbl.TableName, allDbTbl.Text);
                tn.Tag = allDbTbl;
            }

            trv.Sort();
            trv.ExpandAll();
            trv.EndUpdate();
        }

        /// <summary>
        /// Select all records ftom the selected table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public void SelectFromTable(string tableName)
        {
            string selectSql = "select * from " + tableName;
            try
            {
                // Create a new data adapter based on the specified query.
                this.DbConnection.Open();
                this.Da = new SQLiteDataAdapter(selectSql, this.DbConnection);

                SQLiteCommandBuilder commandBuilder = new(this.Da);

                // Populate a new data table and bind it to the BindingSource.
                this.Dt = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture,
                    TableName = tableName,
                };
                this.Da.Fill(this.Dt);

                this.BndSource.DataSource = this.Dt;
                this.dgv.DataSource = this.BndSource;

                string relIdColumnName = string.Empty;  // xxxx_ID

                Dictionary<DataGridViewComboBoxColumn, string> cmbAndColumnName = new();

                if (tableName == PdTableName.PD_FAMILIE)
                {
                    cmbAndColumnName.Add(this.CreateDgvComboBox(string.Format("SELECT * FROM {0}", PdTableName.PD_ORDE), "ORDE_ID"), "ORDE_ID");
                }
                else if (tableName == PdTableName.PD_GESLACHT)
                {
                    cmbAndColumnName.Add(this.CreateDgvComboBox(string.Format("SELECT * FROM {0}", PdTableName.PD_FAMILIE), "FAMILIE_ID"), "FAMILIE_ID");
                }
                else if (tableName == PdTableName.PD_ORDE)
                {
                    cmbAndColumnName.Add(this.CreateDgvComboBox(string.Format("SELECT * FROM {0}", PdTableName.PD_KLASSE), "KLASSE_ID"), "KLASSE_ID");
                }
                else if (tableName == PdTableName.PD_RIJK)
                {
                    cmbAndColumnName.Add(this.CreateDgvComboBox(string.Format("SELECT * FROM {0}", PdTableName.PD_DOMEIN), "DOMEIN_ID"), "DOMEIN_ID");
                }
                else if (tableName == PdTableName.PD_STAM)
                {
                    cmbAndColumnName.Add(this.CreateDgvComboBox(string.Format("SELECT * FROM {0}", PdTableName.PD_RIJK), "RIJK_ID"), "RIJK_ID");
                }
                else if (tableName == PdTableName.PD_KLASSE)
                {
                    cmbAndColumnName.Add(this.CreateDgvComboBox(string.Format("SELECT * FROM {0}", PdTableName.PD_STAM), "STAM_ID"), "STAM_ID");
                }

                // add combobox
                int index = 0;
                foreach (KeyValuePair<DataGridViewComboBoxColumn, string> pair in cmbAndColumnName)
                {
                    foreach (DataColumn column in this.Dt.Columns)
                    {
                        if (!string.IsNullOrEmpty(pair.Value))
                        {
                            if (column.ColumnName.Equals(pair.Value))
                            {
                                index = column.Ordinal;

                                this.dgv.Columns[pair.Value].Visible = false;  // Do not show the column ID
                                this.dgv.Columns.Insert(index, pair.Key);
                            }
                        }
                    }
                }

                PdLogging.WriteToLogInformation("De tabel '" + tableName + "' is opgehaald.");
            }
            catch (SQLiteException ex)
            {
                PdLogging.WriteToLogError("Fout bij het ophalen van alle gegevens van de tabel : " + tableName);
                PdLogging.WriteToLogError(ex.Message);
                PdLogging.WriteToLogError(ex.ToString());
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        private DataGridViewComboBoxColumn CreateDgvComboBox(string query, string columnName)
        {
            var ordeAdapter = new SQLiteDataAdapter(query, this.DbConnection);
            DataTable ordeTable = new();

            ordeAdapter.Fill(ordeTable);

            DataGridViewComboBoxColumn newComboBoxColumn = new();

            newComboBoxColumn.Name = columnName;
            newComboBoxColumn.HeaderText = columnName.Replace("_ID", string.Empty);

            newComboBoxColumn.DataSource = ordeTable;
            newComboBoxColumn.DataPropertyName = columnName;
            newComboBoxColumn.DisplayMember = "NAAM";
            newComboBoxColumn.ValueMember = "ID";

            newComboBoxColumn.AutoComplete = true;
            newComboBoxColumn.MaxDropDownItems = 20;

            newComboBoxColumn.FlatStyle = FlatStyle.Flat;
            newComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;  // You see only a combobox when editing

            // Create extra row with a null value so you don't have to insert a ID value in xxxx_ID
            ordeTable.Columns["ID"].AllowDBNull = true;

            DataRow newRow = ordeTable.NewRow();
            newRow["NAAM"] = " ";
            newRow["ID"] = DBNull.Value;
            ordeTable.Rows.Add(newRow);

            return newComboBoxColumn;
        }

        /// <summary>
        /// Return the table field names, types and lengths.
        /// </summary>
        /// <param name="tableName">The selected table.</param>
        /// <returns>a Dictionary witht fieldnames and types.</returns>
        public Dictionary<string, string> GetFieldnamesAndtypes(string tableName)
        {
            string selectSql = "PRAGMA table_info(" + tableName + ")";

            var temp = new Dictionary<string, string>();
            try
            {
                PdLogging.WriteToLogInformation("Ophalen tabel gegevens.");
                this.DbConnection.Open();
                using SQLiteCommand command = new(selectSql, this.DbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                using (DataTable dt = new())
                {                   
                    dt.Load(reader);
                    foreach (DataRow row in dt.Rows)
                    {
                        temp.Add(row.ItemArray[1].ToString(), row.ItemArray[2].ToString());
                    }
                }

                reader.Close();
                return temp;
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError("Fout bij het ophalen van de tabel gegevens.");
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van de tabel gegevens.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return temp;
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        /// <summary>
        /// Check for relations between tables and update if needed.
        /// </summary>
        public void CheckForUpdatedRelations()
        {
            this.UpdClassDevision();
            this.UpdDivisionKingom();
            this.UpdFamilyOrder();
            this.UpdGenusFamily();
            this.UpdKingdomDomain();
            this.UpdOrderClass();
        }

        private void UpdClassDevision()
        {
            string updateSql = string.Format("update {0} set STAM_ID = null where STAM_ID not in (select id from {1})", PdTableName.PD_KLASSE, PdTableName.PD_STAM);

            try
            {
                this.DbConnection.Open();
                using var tr = this.DbConnection.BeginTransaction();
                SQLiteCommand command = new(updateSql, this.DbConnection);
                command.ExecuteNonQuery();
                command.Dispose();
                tr.Commit();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError(string.Format("Fout bij het update van STAM_ID in de tabel {0}.", PdTableName.PD_KLASSE));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        private void UpdDivisionKingom()
        {
            string updateSql = string.Format("update {0} set RIJK_ID = null where RIJK_ID not in (select id from {1})", PdTableName.PD_STAM, PdTableName.PD_RIJK);
            try
            {
                this.DbConnection.Open();
                using var tr = this.DbConnection.BeginTransaction();
                SQLiteCommand command = new(updateSql, this.DbConnection);
                command.ExecuteNonQuery();
                command.Dispose();
                tr.Commit();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError(string.Format("Fout bij het update van RIJK_ID in de tabel {0}.", PdTableName.PD_STAM));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        private void UpdFamilyOrder()
        {
            string updateSql = string.Format("update {0} set ORDE_ID = null where ORDE_ID not in (select id from {1})", PdTableName.PD_FAMILIE, PdTableName.PD_ORDE);

            try
            {
                this.DbConnection.Open();
                using var tr = this.DbConnection.BeginTransaction();
                SQLiteCommand command = new(updateSql, this.DbConnection);
                command.ExecuteNonQuery();
                command.Dispose();
                tr.Commit();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError(string.Format("Fout bij het update van ORDE_ID in de tabel {0}.", PdTableName.PD_FAMILIE));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        private void UpdGenusFamily()
        {
            string updateSql = string.Format("update {0} set FAMILIE_ID = null where FAMILIE_ID not in (select id from {1})", PdTableName.PD_GESLACHT, PdTableName.PD_FAMILIE);

            try
            {
                this.DbConnection.Open();
                using var tr = this.DbConnection.BeginTransaction();
                SQLiteCommand command = new(updateSql, this.DbConnection);
                command.ExecuteNonQuery();
                command.Dispose();
                tr.Commit();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError(string.Format("Fout bij het update van FAMILIE_ID in de tabel {0}.", PdTableName.PD_GESLACHT));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        private void UpdKingdomDomain()
        {
            string updateSql = string.Format("update {0} set DOMEIN_ID = null where DOMEIN_ID not in (select id from {1})", PdTableName.PD_RIJK, PdTableName.PD_DOMEIN);

            try
            {
                this.DbConnection.Open();
                using var tr = this.DbConnection.BeginTransaction();
                SQLiteCommand command = new(updateSql, this.DbConnection);
                command.ExecuteNonQuery();
                command.Dispose();
                tr.Commit();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError(string.Format("Fout bij het update van DOMEIN_ID inde tabel {0}.", PdTableName.PD_RIJK));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }

        private void UpdOrderClass()
        {
            string updateSql = string.Format("update {0} set KLASSE_ID = null where KLASSE_ID not in (select id from {1})", PdTableName.PD_ORDE, PdTableName.PD_KLASSE);

            try
            {
                this.DbConnection.Open();
                using var tr = this.DbConnection.BeginTransaction();
                SQLiteCommand command = new(updateSql, this.DbConnection);
                command.ExecuteNonQuery();
                command.Dispose();
                tr.Commit();
            }
            catch (Exception ex)
            {
                PdLogging.WriteToLogError(string.Format("Fout bij het update van KLASSE_ID in de tabel {0}.", PdTableName.PD_ORDE));
                PdLogging.WriteToLogError(ex.Message);
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogError(ex.ToString());
                }

                MessageBox.Show("Fout bij het ophalen van alle tabelnamen.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.DbConnection.Close();
            }
        }
    }
}
