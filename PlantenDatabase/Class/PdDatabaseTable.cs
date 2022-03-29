namespace PlantenDatabase
{
    using System.Globalization;

    /// <summary>
    /// Hold the database table name.
    /// </summary>
    public class PdDatabaseTable
    {
        private string? text;

        /// <summary>
        /// Gets or sets the tablename.
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the table name. Used als treeview.node.text.
        /// </summary>
        public string Text
        {
            get { return VisibleTableName(this.text); }
            set { this.text = value; }
        }

        private static string VisibleTableName(string tblName)
        {
            if (tblName == PdTableName.PD_DOMEIN)
            {
                return PdTableName.NlName.Domein.ToString();
            }
            else if (tblName == PdTableName.PD_RIJK)
            {
                return PdTableName.NlName.Rijk.ToString();
            }
            else if (tblName == PdTableName.PD_STAM)
            {
                return PdTableName.NlName.Stam.ToString();
            }
            else if (tblName == PdTableName.PD_KLASSE)
            {
                return PdTableName.NlName.Klasse.ToString();
            }
            else if (tblName == PdTableName.PD_ORDE)
            {
                return PdTableName.NlName.Orde.ToString();
            }
            else if (tblName == PdTableName.PD_FAMILIE)
            {
                return PdTableName.NlName.Familie.ToString();
            }
            else if (tblName == PdTableName.PD_GESLACHT)
            {
                return PdTableName.NlName.Geslacht.ToString();
            }
            else if (tblName == PdTableName.PD_CATEGORIE)
            {
                return PdTableName.NlName.Categorie.ToString();
            }
            else if (tblName == PdTableName.PD_STANDPLAATS)
            {
                return PdTableName.NlName.Standplaats.ToString();
            }
            else if (tblName == PdTableName.PD_GRONDSOORT)
            {
                return PdTableName.NlName.Grondsoort.ToString();
            }
            else if (tblName == PdTableName.PD_KLEUR)
            {
                return PdTableName.NlName.Kleur.ToString();
            }
            else if (tblName == PdTableName.PD_VORM)
            {
                return PdTableName.NlName.Vorm.ToString();
            }

            return tblName;
        }
    }
}
