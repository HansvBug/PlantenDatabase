namespace PlantenDatabase
{
    using System.Data;
    using System.Data.SQLite;

    public partial class FormTableMaintenance : Form
    {        
        private BindingSource bindingSourceDgvTables;
        private PdTableMaintenance tblMaintenance;
        private TreeNode tn;
        private DataTable? changeTableDeleted;
        private DataTable? changeTableAdded;
        private DataTable? changeTableModified;
        private Dictionary<string, string> tableColumnNamesAndFieldTypes; // Field name. For example: ID, Integer
        private int currentFieldLength;     // Holds the length of the field of the selected cell in de the datagridview. (VARCHAR2(100), FieldLength =100)
        private string currentFieldName;   // Holds the name of the field of the selected cell in de the datagridview.
        private bool afterSelect;

        public dynamic? JsonObjSettings { get; set; }

        private enum ApplicationAccess
        {
            None,
            Minimal,
            Full,
            RowsRemoved,
            CanNotSave,
            CellValueChanged,
            AddRecord,
            Saved,
        }

        /// <summary>
        /// Form table maintenance.
        /// </summary>
        public FormTableMaintenance()
        {
            this.InitializeComponent();
            this.Text = "Onderhoud";
            this.ToolStripStatusLabel1.Text = string.Empty;
            this.LoadSettings();
            this.EnableFunctions(ApplicationAccess.None.ToString());
            this.LoadFormPosition();

            bindingSourceDgvTables = new();
            tblMaintenance = new();
            tn = new();
            changeTableAdded = new();
            changeTableModified = new();
            tableColumnNamesAndFieldTypes = new();

            this.changeTableDeleted = new();
            this.currentFieldName = string.Empty;
        }

        private string SelectedTableName { get; set; } = string.Empty; // Holds the Name of the selected treenode.
        private bool CellValueChanged { get; set; }
        private string ActiveTable { get; set; } = string.Empty;

        public string SetStatusLabelMain
        {
            set { this.ToolStripStatusLabel1.Text = value; }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            if (this.CellValueChanged)
            {
                MessageBox.Show("Niet opgeslagen wijzigingen gevonden. Kies eerst Opslaan.", "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.ActiveControl = this.ButtonSave;
            }
            else
            {
                this.Close();
            }
        }

        private void FormTableMaintenance_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            this.tblMaintenance = new PdTableMaintenance(this.DataGridViewTables, this.bindingSourceDgvTables);
            this.GetAllTableNames();         // Put the table names in a objectlist

            Cursor.Current = Cursors.Default;
        }

        private void LoadSettings()
        {
            using PdSettingsManager set = new();
            set.LoadSettings();
            this.JsonObjSettings = set.JsonObjSettings;
        }

        private void GetAllTableNames()
        {
            this.tblMaintenance.GetAllTableNames();
            this.tblMaintenance.LoadTreeviewTableNames(this.TreeViewTableNames);
        }

        private void LoadFormPosition()
        {
            using PdFormPosition frmPosition = new(this);
            frmPosition.LoadMaintainTablesFormPosition();
        }

        private void EnableFunctions(string ApplicationAccess)
        {
            switch (ApplicationAccess)
            {
                case "Full":
                    this.ButtonSave.Enabled = true;
                    this.ButtonCancel.Enabled = true;
                    this.ButtonImport.Enabled = true;
                    break;
                case "Minimal":
                    break;
                case "None":
                    this.ButtonSave.Enabled = false;
                    this.ButtonCancel.Enabled = false;

                    if (this.tn != null)
                    {
                        if (this.tn.Text == PdTableName.NlName.Domein.ToString() ||
                            this.tn.Text == PdTableName.NlName.Geslacht.ToString() ||
                            this.tn.Text == PdTableName.NlName.Grondsoort.ToString() ||
                            this.tn.Text == PdTableName.NlName.Familie.ToString() ||
                            this.tn.Text == PdTableName.NlName.Orde.ToString()
                            )
                        {
                            this.ButtonImport.Enabled = true;
                        }
                    }
                    else
                    {
                        this.ButtonImport.Enabled = false;
                    }

                    break;
                case "RowsRemoved":
                    this.ButtonSave.Enabled = true;
                    this.ButtonCancel.Enabled = true;
                    this.ButtonImport.Enabled = false;
                    break;
                case "CanNotSave":
                    this.ButtonSave.Enabled = false;
                    this.ButtonCancel.Enabled = true;
                    this.ButtonImport.Enabled = false;
                    break;
                case "CellValueChanged":
                    this.ButtonSave.Enabled = true;
                    this.ButtonCancel.Enabled = true;
                    this.ButtonImport.Enabled = false;
                    break;
                case "AddRecord":
                    this.ButtonCancel.Enabled = true;
                    this.ButtonSave.Enabled = true;
                    this.ButtonImport.Enabled = false;
                    break;
                case "Saved":
                    this.ButtonSave.Enabled = false;
                    this.ButtonCancel.Enabled = false;

                    if (this.tn != null)
                    {
                        if (this.tn.Text == PdTableName.NlName.Domein.ToString() ||
                            this.tn.Text == PdTableName.NlName.Geslacht.ToString() ||
                            this.tn.Text == PdTableName.NlName.Grondsoort.ToString() ||
                            this.tn.Text == PdTableName.NlName.Familie.ToString() ||
                            this.tn.Text == PdTableName.NlName.Orde.ToString()
                            )
                        {
                            this.ButtonImport.Enabled = true;
                        }
                        else
                        {
                            this.ButtonImport.Enabled = false;
                        }
                    }
                    break;
                default:
                    this.ButtonSave.Enabled = false;
                    this.ButtonCancel.Enabled = false;
                    this.ButtonImport.Enabled = false;
                    break;
            }
        }
        private void TreeViewTableNames_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SetStatusLabelMain = "Ophalen gegevens...";
            this.Refresh();
            DataGridViewTables.SuspendLayout();
            DataGridViewTables.Enabled = false;
            if (this.DataGridViewTables_RowsAdded != null)
            {
                this.DataGridViewTables.RowsAdded -= new DataGridViewRowsAddedEventHandler(this.DataGridViewTables_RowsAdded);
            }

            afterSelect = true; // used to disable rowsadded check value changed

            if (!this.CellValueChanged)
            {
                if (this.TreeViewTableNames.Nodes.Count > 0)
                {
                    tn = this.TreeViewTableNames.SelectedNode as TreeNode;
                    PdDatabaseTable dbTable = (PdDatabaseTable)tn.Tag;
                    this.SelectedTableName = tn.Name;  // Used for importing in the right table.

                    if (dbTable != null)
                    {
                        this.ActiveTable = dbTable.TableName;
                        this.tn = this.TreeViewTableNames.SelectedNode;  // used when selecting an other node while there are pending changes

                        this.DataGridViewTables.DataSource = null;
                        this.bindingSourceDgvTables.DataSource = null;
                        this.DataGridViewTables.AutoGenerateColumns = true;

                        this.DataGridViewTables.DataError -= new DataGridViewDataErrorEventHandler(this.DataGridViewTables_DataError);
                        this.DataGridViewTables.EditingControlShowing -= new DataGridViewEditingControlShowingEventHandler(this.DataGridViewTables_EditingControlShowing);  // Make de ID dgv combobox text editable
                        this.DataGridViewTables.CellValidated -= new DataGridViewCellEventHandler(this.DataGridViewTables_CellValidated);

                        if (!string.IsNullOrEmpty(dbTable.TableName))
                        {
                            this.tblMaintenance.SelectFromTable(dbTable.TableName);
                        }

                        // Display
                        this.DataGridViewTables.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                        this.HideDgvColumns(this.DataGridViewTables);  // Hide some columns from the datagrid view

                        this.DataGridViewTables.DataError += new DataGridViewDataErrorEventHandler(this.DataGridViewTables_DataError);
                        this.DataGridViewTables.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(this.DataGridViewTables_EditingControlShowing);  // Make de ID dgv combobox text editable
                        this.DataGridViewTables.CellValidated += new DataGridViewCellEventHandler(this.DataGridViewTables_CellValidated);

                        // Get the column names and the types and lengths
                        PdTableMaintenance tableColumnnamesAndTypes = new(this.DataGridViewTables, this.bindingSourceDgvTables);
                        this.tableColumnNamesAndFieldTypes = tableColumnnamesAndTypes.GetFieldnamesAndtypes(tn.Name);
                    }
                }

                if (tn.Text != PdTableMaintenance.TrvRootNodeName)
                {
                    this.EnableFunctions(ApplicationAccess.Saved.ToString());
                }
                else
                {
                    this.EnableFunctions(ApplicationAccess.None.ToString());
                }                                
            }
            else
            {
                // Pending changes
                if (this.tblMaintenance.Dt != null)
                {
                    if (this.TreeViewTableNames.SelectedNode.Text != this.tblMaintenance.Dt.TableName)
                    {
                        MessageBox.Show("Sla eerst de huidige mutaties van de tabel " + this.ActiveTable + " op.", "Informatie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.TreeViewTableNames.AfterSelect -= new TreeViewEventHandler(this.TreeViewTableNames_AfterSelect);  // avoid message show twice
                        this.TreeViewTableNames.SelectedNode = this.tn;
                        this.TreeViewTableNames.AfterSelect += new TreeViewEventHandler(this.TreeViewTableNames_AfterSelect);

                        this.EnableFunctions(ApplicationAccess.Full.ToString());
                    }                    
                }
            }

            DataGridViewTables.ResumeLayout();
            DataGridViewTables.Enabled = true;

            this.DataGridViewTables.RowsAdded += new DataGridViewRowsAddedEventHandler(this.DataGridViewTables_RowsAdded);
            SetStatusLabelMain = string.Empty;
            this.Refresh();
            Cursor.Current = Cursors.Default;
        }

        private void HideDgvColumns(DataGridView dgv)
        {
            try
            {
                // DataGridView1.Columns["ID"].ReadOnly = true;
                this.DataGridViewTables.Columns["CODE"].Visible = false;  // Do not show the column CODE
                this.DataGridViewTables.Columns["GUID"].Visible = false;  // Do not show the column GUID
                this.DataGridViewTables.Columns["ID"].Visible = false;  // Do not show the column ID
                this.DataGridViewTables.Columns["DATUM_AANGEMAAKT"].Visible = false;
                this.DataGridViewTables.Columns["AANGEMAAKT_DOOR"].Visible = false;

                this.DataGridViewTables.Columns["DATUM_GEWIJZIGD"].Visible = false;
                this.DataGridViewTables.Columns["GEWIJZIGD_DOOR"].Visible = false;
            }
            catch (NullReferenceException nullRefEx)
            {
                // Not all tables have an ID column. The metadata table has different columns
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug("De functie 'HideDgvColumns' geeft een NullReferenceException.");
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogDebug(nullRefEx.Message);
                    PdLogging.WriteToLogDebug(nullRefEx.ToString());
                }
            }
            catch (Exception ex)
            {
                if (PdDebugMode.DebugMode)
                {
                    PdLogging.WriteToLogDebug("De functie 'HideDgvColumns' geeft een Exception.");
                    PdLogging.WriteToLogError("Melding :");
                    PdLogging.WriteToLogDebug(ex.Message);
                    PdLogging.WriteToLogDebug(ex.ToString());
                }
            }
        }
        private void DataGridViewTables_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!afterSelect)
            {
                this.CellValueChanged = true;
            }
            else 
            {
                afterSelect = false;
            }
        }

        private void DataGridViewTables_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (afterSelect && CellValueChanged)
            {
                this.CellValueChanged = true;
            }
            else if (!afterSelect && !CellValueChanged)
            {
                this.CellValueChanged = false;
            }

            // Check for duplicate code names
            bool canSave;
            canSave = this.CheckForDuplicates(DataGridViewTables, "CODE");
            this.CheckForDuplicates(DataGridViewTables, "NAAM");

            // When the CODE name is not unique disable saving.
            if (!canSave)
            {
                this.EnableFunctions(ApplicationAccess.CanNotSave.ToString());
            }
            else
            {
                this.EnableFunctions(ApplicationAccess.RowsRemoved.ToString());
            }
        }

        private void DataGridViewTables_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            if (anError != null)
            {
                // Show error when saving changes in Datagridview fail.
                MessageBox.Show("Error happened " + anError.Context.ToString());

                if (anError.Context == DataGridViewDataErrorContexts.Commit)
                {
                    MessageBox.Show("Commit error" + anError.Context.ToString());

                    PdLogging.WriteToLogError("Er heeft een 'commit error' plaats gevonden.");
                    PdLogging.WriteToLogError(anError.Context.ToString());
                    PdLogging.WriteToLogError(anError.Exception.Message);
                    PdLogging.WriteToLogError(anError.Exception.ToString());
                }

                if (anError.Context == DataGridViewDataErrorContexts.CurrentCellChange)
                {
                    MessageBox.Show("Cell change" + anError.Context.ToString());

                    PdLogging.WriteToLogError("Er heeft een 'Cell change error' plaats gevonden.");
                    PdLogging.WriteToLogError(anError.Context.ToString());
                    PdLogging.WriteToLogError(anError.Exception.Message);
                    PdLogging.WriteToLogError(anError.Exception.ToString());
                }

                if (anError.Context == DataGridViewDataErrorContexts.Parsing)
                {
                    MessageBox.Show("parsing error" + anError.Context.ToString());

                    PdLogging.WriteToLogError("Er heeft een 'parsing error' plaats gevonden.");
                    PdLogging.WriteToLogError(anError.Context.ToString());
                    PdLogging.WriteToLogError(anError.Exception.Message);
                    PdLogging.WriteToLogError(anError.Exception.ToString());
                }

                if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
                {
                    MessageBox.Show("leave control error" + anError.Context.ToString());

                    PdLogging.WriteToLogError("Er heeft een 'leave control error' plaats gevonden.");
                    PdLogging.WriteToLogError(anError.Context.ToString());
                    PdLogging.WriteToLogError(anError.Exception.Message);
                    PdLogging.WriteToLogError(anError.Exception.ToString());
                }

                if (anError.Exception is ConstraintException)
                {
                    DataGridView view = (DataGridView)sender;
                    view.Rows[anError.RowIndex].ErrorText = "an error";
                    view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";

                    MessageBox.Show("Constraint exception error" + anError.Exception.ToString());

                    PdLogging.WriteToLogError("Er heeft een 'constraint error' plaats gevonden.");
                    PdLogging.WriteToLogError(anError.Exception.ToString());
                    PdLogging.WriteToLogError(anError.Exception.Message);
                    PdLogging.WriteToLogError(anError.Exception.ToString());

                    anError.ThrowException = false;
                }
            }
        }

        private void DataGridViewTables_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Make the ID-Combobox in the datagridview edit able
            if (e != null && e.Control is DataGridViewComboBoxEditingControl)
            {
                if (e.Control is not ComboBox combo)
                {
                    return;
                }

                combo.DropDownStyle = ComboBoxStyle.DropDown;
                combo.AutoCompleteMode = AutoCompleteMode.Suggest;  // SuggestAppend;

                // combo.Sorted = true;  //Not possible when the list is a datasource
            }
        }
        private void DataGridViewTables_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var datagridview = sender as DataGridView;
            if (datagridview != null && e != null)
            {
                this.currentFieldName = datagridview.Columns[e.ColumnIndex].HeaderText;

                string fieldLength;
                foreach (KeyValuePair<string, string> entry in this.tableColumnNamesAndFieldTypes)
                {
                    if (entry.Key == this.currentFieldName)
                    {
                        // if type is VARCHAR then get the number of the chars.
                        if (entry.Value.StartsWith("VARCHAR"))
                        {
                            fieldLength = entry.Value.Replace("VARCHAR(", string.Empty);
                            fieldLength = fieldLength.Replace(")", string.Empty);
                            if (!string.IsNullOrEmpty(fieldLength))
                            {
                                this.currentFieldLength = int.Parse(fieldLength);
                            }
                        }

                        // Expand if NUMBER(xx,xx) is needed.
                    }
                }

                this.CheckForCellValueLength(e);

                // Check for duplicate code names
                bool canSave;
                canSave = this.CheckForDuplicates(datagridview, "CODE");
                this.CheckForDuplicates(datagridview, "NAAM");

                // When the CODE name is not unique disable saving.
                if (!canSave)
                {
                    this.EnableFunctions(ApplicationAccess.CanNotSave.ToString());
                }
            }
        }

        private void CheckForCellValueLength(DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this.DataGridViewTables.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string? celValue = this.DataGridViewTables[e.ColumnIndex, e.RowIndex].Value.ToString();

            if (this.ValidateStringLength(e, this.currentFieldLength))
            {
                if (celValue != string.Empty && celValue != null)
                {
                    cell.Value = celValue;
                }
            }
            else
            {
                MessageBox.Show(string.Format("Maximaal {0} tekens toegestaan.", this.currentFieldLength.ToString()), "Waarschuwing.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (celValue != null)
                {
                    cell.Value = celValue.Substring(0, this.currentFieldLength);
                }
            }
        }
        private bool ValidateStringLength(DataGridViewCellEventArgs e, int maxLength)
        {
            string? name = this.DataGridViewTables[e.ColumnIndex, e.RowIndex].Value.ToString();
            if (name != null)
            {
                if (name.Length <= maxLength)
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

        private bool CheckForDuplicates(DataGridView dgv, string searchColumn)
        {
            List<string?> uniqueValues = new();
            bool canSave = true;
            
            if (dgv.Rows.Count > 1)
            {
                int index = dgv.Columns[searchColumn.ToUpper()].Index;  // Column 'searchColumn' must have unique values.

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[index].Value != null)
                    {
                        uniqueValues.Add(row.Cells[index].Value.ToString());
                    }
                }

                // Find duplicate values is the list:
                var query = uniqueValues.GroupBy(x => x)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList();

                // Color the duplcate values ins the datagridview.
                // First make all values black. And then change the color for the double values.
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    dgv[index, row.Index].Style.ForeColor = Color.Black;
                }

                dgv.DefaultCellStyle.ForeColor = Color.Black;
                this.Refresh();

                if (query.Count > 0)
                {
                    canSave = false;
                    string? cellValue;
                    try
                    {
                        foreach (string? name in query)
                        {
                            foreach (DataGridViewRow row in dgv.Rows)
                            {
                                if (row.Cells[index].Value != null)
                                {
                                    cellValue = row.Cells[index].Value.ToString();
                                    if (cellValue != null)
                                        if (cellValue.Equals(name))
                                        {
                                            dgv[index, row.Index].Style.ForeColor = Color.Red;
                                        }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                return canSave;
            }
            else
            {
                return true;
            }
        }

        private void DataGridViewTables_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Drop down the combobox with 1 mouse click.
            bool validClick = e.RowIndex != -1 && e.ColumnIndex != -1; // Make sure the selected cell is valid.
            var datagridview = sender as DataGridView;

            if (datagridview != null)
            {
                // Check to make sure the selected cell is the cell containing the combobox 
                if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
                {
                    datagridview.BeginEdit(true);
                    ((ComboBox)datagridview.EditingControl).DroppedDown = true;
                }
            }
        }

        private void DataGridViewTables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                // Set the DATE_ALTERED...
                if (e.RowIndex != this.DataGridViewTables.NewRowIndex)
                {
                    this.CellValueChanged = true;
                    this.DataGridViewTables.Rows[e.RowIndex].Cells["DATUM_GEWIJZIGD"].Value = DateTime.Now;
                    this.DataGridViewTables.Rows[e.RowIndex].Cells["GEWIJZIGD_DOOR"].Value = Environment.UserName;
                }

                if (this.CellValueChanged)
                {
                    this.EnableFunctions(ApplicationAccess.CellValueChanged.ToString());
                }
                else
                {
                    this.EnableFunctions(ApplicationAccess.Saved.ToString());
                }
            }
        }

        private void DataGridViewTables_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Add default value when new row is created.
            e.Row.Cells["DATUM_AANGEMAAKT"].Value = DateTime.Now;
            e.Row.Cells["GUID"].Value = Guid.NewGuid().ToString();
            e.Row.Cells["AANGEMAAKT_DOOR"].Value = Environment.UserName;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (this.tblMaintenance.Dt != null)
            {
                SetStatusLabelMain = "Bezig met ongedaan maken...";
                this.Refresh();
                DataGridViewTables.SuspendLayout();
                DataGridViewTables.Enabled = false;

                this.tblMaintenance.Dt.RejectChanges();
                this.CellValueChanged = false;
                this.EnableFunctions(ApplicationAccess.None.ToString());

                DataGridViewTables.ResumeLayout();
                DataGridViewTables.Enabled = true;

                SetStatusLabelMain = "";
                this.Refresh();
            }
        }

        #region closing form
        private void FormTableMaintenance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.CellValueChanged)
            {
                MessageBox.Show("Niet opgeslagen wijzigingen gevonden. Kies eerst 'Opslaan'." + Environment.NewLine +
                                "Of maak de wijzigingen ongedaan door op 'Opnieuw' te klikken.",                 
                                "Waarschuwing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = this.CellValueChanged;
                this.ActiveControl = this.ButtonSave;
            }
            else
            {
                this.SaveFormPosition();
                this.SaveSettings();
            }
        }

        private void SaveFormPosition()
        {
            using PdFormPosition frmPosition = new(this);
            frmPosition.SaveMaintainTablesFormPosition();
        }

        private void SaveSettings()
        {
            PdSettingsManager.SaveSettings(this.JsonObjSettings);
        }
        #endregion closing form

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (this.tblMaintenance.Dt != null)
            {
                SetStatusLabelMain = "Bezig met opslaan...";
                this.Refresh();
                Cursor.Current = Cursors.WaitCursor;

                this.EnableFunctions(String.Empty);  //Disable all buttons

                // Get the changes in the data table
                this.changeTableDeleted = this.tblMaintenance.Dt.GetChanges(DataRowState.Deleted);
                this.changeTableAdded = this.tblMaintenance.Dt.GetChanges(DataRowState.Added);
                this.changeTableModified = this.tblMaintenance.Dt.GetChanges(DataRowState.Modified);

                Dictionary<int, string> newData = new();

                // Make a list of all the names. Used for checking duplicates in the NAME column
                int emptyId = -1;
                string? rowId;
                string? rowName;
                foreach (DataRow row in this.tblMaintenance.Dt.Rows)
                {
                    // Skip deleted rows
                    if (row != null && row.RowState != DataRowState.Deleted)
                    {
                        try
                        {
                            rowId = row["ID"].ToString();
                            rowName = row["NAAM"].ToString();

                            if (rowId != null && !string.IsNullOrEmpty(rowId) && rowName != null && !string.IsNullOrEmpty(rowName))
                            {
                                newData.Add(int.Parse(rowId), rowName.Trim());
                            }
                        }
                        catch // Not all tables have a NAME column
                        {
                            try
                            {
                                newData.Add(emptyId, string.Empty);  // New row. Does not have an ID yet.
                                emptyId--;  // emptyId = emptyId - 1.
                            }
                            catch
                            {                         
                            }
                        }
                    }
                }

                // Check for duplicates
                var duplicateValues = newData.GroupBy(x => x.Value).Where(x => x.Count() > 1);

                bool abortUpdate = false;
                string aDuplicateValue = string.Empty;
                foreach (var item in duplicateValues)
                {
                    // If found something dan abort updating
                    abortUpdate = true;

                    aDuplicateValue += item.Key + "; ";
                }

                if (abortUpdate)
                {
                    PdLogging.WriteToLogWarning("Dubbele waarden aangetroffenin de kolom NAAM");
                    PdLogging.WriteToLogWarning("Deze zijn:");
                    PdLogging.WriteToLogWarning(aDuplicateValue);

                    SetStatusLabelMain = "Opslaan onderbreken.";
                    this.Refresh();
                    DialogResult dialogResult = MessageBox.Show(
                        "Dubbele waarde aangetroffen in de kolom naam." + Environment.NewLine +
                        Environment.NewLine +
                        "De dubbele namen staanin het log bestand" + Environment.NewLine + Environment.NewLine +
                        "Wilt u de wijzigingen toch opslaan?",
                        "Waarschuwing",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                    if (dialogResult == DialogResult.Yes)
                    {
                        abortUpdate = false;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        abortUpdate = true;
                    }
                }

                if (!abortUpdate)
                {
                    SetStatusLabelMain = "Bezig met opslaan...";
                    this.Refresh();
                    try
                    {
                        // Update the database with changes.
                        DataGridViewTables.SuspendLayout();
                        DataGridViewTables.Enabled = false;

                        if (this.changeTableModified != null)
                        {
                            this.tblMaintenance.Da.Update(this.tblMaintenance.Dt.Select(null, null, DataViewRowState.ModifiedCurrent));
                        }

                        if (this.changeTableAdded != null)
                        {
                            this.tblMaintenance.Da.Update(this.tblMaintenance.Dt.Select(null, null, DataViewRowState.Added));
                        }

                        if (this.changeTableDeleted != null)
                        {
                            this.tblMaintenance.Da.Update(this.tblMaintenance.Dt.Select(null, null, DataViewRowState.Deleted));
                        }

                        DataGridViewTables.ResumeLayout();
                        DataGridViewTables.Enabled = true;

                        this.LogTableUpdate();

                        this.ClearChangeDataTables();
                        this.CellValueChanged = false;

                        this.ButtonSave.Enabled = false;
                        this.ButtonCancel.Enabled = false;

                        this.TreeViewTableNames.Select();

                        this.TreeViewTableNames.Refresh();
                        this.tn.TreeView.Focus();  // Keep treenode selected in treeview after save.
                        this.EnableFunctions(ApplicationAccess.Saved.ToString());

                    }
                    catch (SQLiteException ex)
                    {
                        SetStatusLabelMain = "Fout bij het opslaan...";
                        this.Refresh();
                        PdLogging.WriteToLogError("Opslaan wijzigingen in de tabel zijn mislukt.");
                        PdLogging.WriteToLogError(ex.Message);
                        PdLogging.WriteToLogError(ex.ToString());

                        if (ex.Message.Contains("FOREIGN KEY constraint failed"))
                        {
                            MessageBox.Show("Mutaties worden niet verwerkt." + Environment.NewLine + "Domeinwaarden die in gebruik zijn kunnen niet verwijderd worden.", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Mutaties worden niet verwerkt. Er is een onverwachte fout opgetreden .", "Fout.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        this.DataGridViewTables.DataSource = null;
                        this.tblMaintenance.BndSource.DataSource = null;
                        this.tblMaintenance.SelectFromTable(this.ActiveTable);  // Reload the data
                        this.EnableFunctions(ApplicationAccess.CellValueChanged.ToString());

                    }
                    catch (Exception ex)
                    {
                        SetStatusLabelMain = "Fout bij het opslaan...";
                        this.Refresh();
                        PdLogging.WriteToLogError(string.Format("Opslaan wijzigingen in de tabel {0} zijn mislukt.", this.ActiveTable));
                        PdLogging.WriteToLogError(ex.Message);

                        if (PdDebugMode.DebugMode)
                        {
                            PdLogging.WriteToLogError(ex.ToString());
                        }

                        this.tblMaintenance.SelectFromTable(this.ActiveTable);  // Reload the data
                        this.EnableFunctions(ApplicationAccess.CellValueChanged.ToString());
                    }
                }                

                // Update domain values in all tables. If a record in Class is deleted then in Orde the attrib. class_id must be updated.
                this.CheckForUpdatedRelations();

                SetStatusLabelMain = "";
                this.Refresh();
                Cursor.Current = Cursors.Default;
            }
        }

        private void CheckForUpdatedRelations()
        {
            // Update domain values in all tables. Example, If a record in class is deleted then in Orde the attrib. class_id must be updated.
            PdTableMaintenance updatereltions = new();
            updatereltions.CheckForUpdatedRelations();
        }
        private void LogTableUpdate()
        {
            if (this.changeTableDeleted != null)
            {
                if (this.changeTableDeleted.Rows.Count > 0)
                {
                    PdLogging.WriteToLogInformation(string.Format("Er zijn {0} records verwijderd uit de tabel: {1}.", this.changeTableDeleted.Rows.Count.ToString(), this.ActiveTable));
                }
            }

            if (this.changeTableAdded != null)
            {
                if (this.changeTableAdded.Rows.Count > 0)
                {
                    PdLogging.WriteToLogInformation(string.Format("Er zijn {0} records toegevoegd aan de tabel : {1}.", this.changeTableAdded.Rows.Count.ToString(), this.ActiveTable));
                }
            }

            if (this.changeTableModified != null)
            {
                if (this.changeTableModified.Rows.Count > 0)
                {
                    PdLogging.WriteToLogInformation(string.Format("Er zijn {0} mutaties doorgevoerd in de tabel : {1}", this.changeTableModified.Rows.Count.ToString(), this.ActiveTable));
                }
            }
        }

        private void ClearChangeDataTables()
        {
            if (this.changeTableModified != null)
            {
                this.changeTableModified = null;
            }

            if (this.changeTableDeleted != null)
            {
                this.changeTableDeleted = null;
            }

            if (this.changeTableAdded != null)
            {
                this.changeTableAdded = null;
            }
        }

        private void ButtonImport_Click(object sender, EventArgs e)
        {
            // Save the current changes before importing new data.
            if (!this.CellValueChanged)
            {
                SetStatusLabelMain = "Bezig met importeren...";
                this.Refresh();
                using PdImportCsv importCsv = new(this.SelectedTableName, this.tblMaintenance.Dt);
                if (importCsv.ImportCsv())
                {
                    this.CellValueChanged = true;

                    int nColumnIndex = 0;

                    if (this.DataGridViewTables.Columns.Count > 3)
                    {
                        nColumnIndex = 3;
                    }

                    SetStatusLabelMain = String.Empty;
                    int nRowIndex = this.DataGridViewTables.Rows.Count - 2;
                    DataGridViewTables.CurrentCell = DataGridViewTables.Rows[nRowIndex].Cells[nColumnIndex];

                    if (this.CellValueChanged)
                    {
                        this.EnableFunctions(ApplicationAccess.CellValueChanged.ToString());
                    }

                }
            }
        }
    }
}
