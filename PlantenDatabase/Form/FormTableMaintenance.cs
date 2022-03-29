using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlantenDatabase
{
    public partial class FormTableMaintenance : Form
    {        
        private BindingSource? bindingSourceDgvTables = new();
        private PdTableMaintenance? tblMaintenance;
        private TreeNode? treeNode;
        private DataTable? changeTableDeleted;
        private DataTable? changeTableAdded;
        private DataTable? changeTableModified;
        private Dictionary<string, string> tableColumnNamesAndFieldTypes = new(); // Field name. For example: ID, Integer
        private int currentFieldLength;     // Holds the length of the field of the selected cell in de the datagridview. (VARCHAR2(100), FieldLength =100)
        private string? currentFieldName;   // Holds the name of the field of the selected cell in de the datagridview.

        public dynamic? JsonObjSettings { get; set; }

        private enum ApplicationAccess
        {
            None,
            Minimal,
            Full,
            RowsRemoved,
            CanNotSave,
            CellValueChanged,
        }

        /// <summary>
        /// Form table maintenance.
        /// </summary>
        public FormTableMaintenance()
        {
            InitializeComponent();
            this.Text = "Onderhoud";
            this.ToolStripStatusLabel1.Text = string.Empty;
            this.LoadSettings();
            this.EnableFunctions(ApplicationAccess.None.ToString());
            this.LoadFormPosition();
        }

        private string? SelectedTableName { get; set; } // Holds the Name of the selected treenode.
        private bool CellValueChanged { get; set; }
        private string? ActiveTable { get; set; }

        public string SetStatusLabelMain
        {
            set { this.ToolStripStatusLabel1.Text = value; }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormTableMaintenance_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            
            this.tblMaintenance = new PdTableMaintenance(this.DataGridViewTables, this.bindingSourceDgvTables);
            this.GetAllTableNames();         // Put the tablenames in a objectlist

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
                    this.ButtonImport.Enabled = false;
                    break;
                case "RowsRemoved":
                    this.ButtonSave.Enabled = true;
                    this.ButtonCancel.Enabled = true;
                    break;
                case "CanNotSave":
                    this.ButtonSave.Enabled = false;
                    this.ButtonCancel.Enabled = true;
                    break;
                case "CellValueChanged":
                    this.ButtonSave.Enabled = true;
                    this.ButtonImport.Enabled = false;
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
            if (this.DataGridViewTables_RowsAdded != null)
            {
                this.DataGridViewTables.RowsAdded -= new DataGridViewRowsAddedEventHandler(this.DataGridViewTables_RowsAdded);
            }

            if (!this.CellValueChanged)
            {
                if (this.TreeViewTableNames.Nodes.Count > 0)
                {
                    TreeNode tn = this.TreeViewTableNames.SelectedNode as TreeNode;
                    PdDatabaseTable dbTable = (PdDatabaseTable)tn.Tag;
                    this.SelectedTableName = tn.Name;  // Used for importing in the right table.

                    if (dbTable != null)
                    {
                        this.ActiveTable = dbTable.TableName;
                        this.treeNode = this.TreeViewTableNames.SelectedNode;  // used when selecting an other node while there are pending changes

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

                this.EnableFunctions(ApplicationAccess.None.ToString());
            }
            else
            {
                // Pending changes
                if (this.TreeViewTableNames.SelectedNode.Text != this.tblMaintenance.Dt.TableName)
                {
                    MessageBox.Show("Sla eerst de huidige mutaties van de tabel " + this.ActiveTable + " op.", "Informatie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.TreeViewTableNames.AfterSelect -= new TreeViewEventHandler(this.TreeViewTableNames_AfterSelect);  // avoid message show twice
                    this.TreeViewTableNames.SelectedNode = this.treeNode;
                    this.TreeViewTableNames.AfterSelect += new TreeViewEventHandler(this.TreeViewTableNames_AfterSelect);
                }
                this.EnableFunctions(ApplicationAccess.Full.ToString());
            }

            this.DataGridViewTables.RowsAdded += new DataGridViewRowsAddedEventHandler(this.DataGridViewTables_RowsAdded);
            
        }

        private void HideDgvColumns(DataGridView dgv)
        {
            try
            {
                // DataGridView1.Columns["ID"].ReadOnly = true;
                this.DataGridViewTables.Columns["GUID"].Visible = false;  // Do not show the column GUID
                this.DataGridViewTables.Columns["ID"].Visible = false;  // Do not show the column ID
                this.DataGridViewTables.Columns["DATUM_AANGEMAAKT"].Visible = false;
                this.DataGridViewTables.Columns["AANGEMAAKT_DOOR"].Visible = false;

                this.DataGridViewTables.Columns["DATE_GEWIJZIGD"].Visible = false;
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
            this.CellValueChanged = true;  // this gets triggert when importing csv files.
        }

        private void DataGridViewTables_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.EnableFunctions(ApplicationAccess.RowsRemoved.ToString());
        }

        private void DataGridViewTables_DataError(object sender, DataGridViewDataErrorEventArgs anError)
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

        private void DataGridViewTables_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Make the ID-Combobox in the datagridview edit able
            if (e.Control is DataGridViewComboBoxEditingControl)
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
            canSave = this.CheckForDuplicateNames(datagridview);

            // When the querygroup name is not unique disable saving.
            if (!canSave)
            {
                this.EnableFunctions(ApplicationAccess.CanNotSave.ToString());
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
                cell.Value = celValue.Substring(0, this.currentFieldLength);
            }
        }

        private bool ValidateStringLength(DataGridViewCellEventArgs e, int maxLength)
        {
            string? name = this.DataGridViewTables[e.ColumnIndex, e.RowIndex].Value.ToString();
            if (name.Length <= maxLength)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckForDuplicateNames(DataGridView dgv)
        {
            List<string> uniqueValues = new();
            bool canSave = true;

            int index = dgv.Columns["CODE"].Index;  // Column CODE must have unique values.

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

                try
                {
                    foreach (string name in query)
                    {
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.Cells[index].Value != null)
                            {
                                if (row.Cells[index].Value.ToString().Equals(name))
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

        private void DataGridViewTables_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Drop down the combobox with 1 mouse click.
            bool validClick = e.RowIndex != -1 && e.ColumnIndex != -1; // Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void DataGridViewTables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                this.CellValueChanged = true;

                // Set the DATE_ALTERED...
                if (e.RowIndex != this.DataGridViewTables.NewRowIndex)
                {
                    this.DataGridViewTables.Rows[e.RowIndex].Cells["DATE_ALTERED"].Value = DateTime.Now;
                    this.ButtonCancel.Enabled = true;
                }

                this.EnableFunctions(ApplicationAccess.CellValueChanged.ToString());
            }
        }

        private void DataGridViewTables_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Add default value when new row is created.
            e.Row.Cells["DATUM_AANGEMAAKT"].Value = DateTime.Now;
            e.Row.Cells["DATE_GEWIJZIGD"].Value = DateTime.Now;
            e.Row.Cells["GUID"].Value = Guid.NewGuid().ToString();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {

        }

        #region closing form
        private void FormTableMaintenance_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveFormPosition();
            this.SaveSettings();
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
    }
}
