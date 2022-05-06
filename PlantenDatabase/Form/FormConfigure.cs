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
    public partial class FormConfigure : Form
    {
        public dynamic? JsonObjSettings { get; set; }

        public FormConfigure()
        {
            InitializeComponent();
            this.Text = "Instellingen";
            this.LoadSettings();            
            this.LoadFormPosition();
        }

        #region open form
        private void FormConfigure_Load(object sender, EventArgs e)
        {

        }
        private void LoadSettings()
        {
            using PdSettingsManager set = new();
            set.LoadSettings();
            this.JsonObjSettings = set.JsonObjSettings;

            this.TextBoxDefaultImportLocation.Text = this.JsonObjSettings.FormConfigure[0].FolderImportLocation;
        }

        private void LoadFormPosition()
        {
            using PdFormPosition frmPosition = new(this);
            frmPosition.LoadConfigureFormPosition();
        }
        #endregion open form

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Close form
        private void FormConfigure_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveFormPosition();
            this.SaveSettings();
        }

        private void SaveFormPosition()
        {
            using PdFormPosition frmPosition = new(this);
            frmPosition.SaveConfigureFormPosition();
        }

        private void SaveSettings()
        {
            PdSettingsManager.SaveSettings(this.JsonObjSettings);
        }
        #endregion Close form

        private void TextBoxDefaultImportLocation_Leave(object sender, EventArgs e)
        {
            this.JsonObjSettings.FormConfigure[0].FolderImportLocation = this.TextBoxDefaultImportLocation.Text;
        }
    }
}
