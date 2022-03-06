namespace PlantenDatabase
{
    /// <summary>
    /// Return the application database tabelnames (static).
    /// SQLite database table names.
    /// </summary>
    public static class PdTableName
    {
        /// <summary>
        /// Dutch table names.
        /// </summary>
        public enum NlName
        {
            Domein,
            Rijk,
            Stam,
            Klasse,
            Orde,
            Familie,
            Geslacht,
            Settings,
            Categorie,
            Standplaats,
            Grondsoort,
            Kleur,
            Vorm,
        }

        // The "PD_" is used beacause table name ORDER is not pausible and PD_ORDER is pausible. (Order is a reserved word).
        /// <summary>
        ///  Gets table name: SETTINGS_META.
        /// </summary>
        public static string SETTINGS_META
        {
            get { return "SETTINGS_META"; }
        }

        /// <summary>
        ///  Gets table name: PD_DOMEIN. (Domein).
        ///  </summary>
        public static string PD_DOMEIN
        {
            get { return "PD_DOMEIN"; }
        }

        /// <summary>
        ///  Gets table name: PD_RIJK. (Rijk).
        /// </summary>
        public static string PD_RIJK
        {
            get { return "PD_RIJK"; }
        }

        /// <summary>
        ///  Gets table name: PD_STAM. (Afdeling / Stam).
        /// </summary>
        public static string PD_STAM
        {
            get { return "PD_STAM"; }
        }

        /// <summary>
        ///  Gets table name: PD_KLASSE. (KLasse).
        /// </summary>
        public static string PD_KLASSE
        {
            get { return "PD_KLASSE"; }
        }

        /// <summary>
        ///  Gets table name: PD_ORDE. (Orde).
        /// </summary>
        public static string PD_ORDE
        {
            get { return "PD_ORDE"; }
        }

        /// <summary>
        ///  Gets table name: PD_FAMILIE. (Familie).
        /// </summary>
        public static string PD_FAMILIE
        {
            get { return "PD_FAMILIE"; }
        }

        /// <summary>
        ///  Gets table name: PD_GESLACHT. (Geslacht).
        /// </summary>
        public static string PD_GESLACHT
        {
            get { return "PD_GESLACHT"; }
        }

        /// <summary>
        /// Gets table name: PD_PLANTENDATA. (Tabel met de volledige plant gegevens).
        /// </summary>
        public static string PD_PLANTENDATA
        {
            get { return "PD_PLANTENDATA"; }
        }

        // Index names

        /// <summary>
        ///  Gets index name: PD_DOMAIN_ID_IDX.
        /// </summary>
        public static string PD_DOMEIN_ID_IDX
        {
            get { return "PD_DOMEIN_ID_IDX"; }
        }

        /// <summary>
        ///  Gets index name: PD_RIJK_ID_IDX.
        /// </summary>
        public static string PD_RIJK_ID_IDX
        {
            get { return "PD_RIJK_ID_IDX"; }
        }

        /// <summary>
        ///  Gets table name: PD_STAM_ID_IDX.
        /// </summary>
        public static string PD_STAM_ID_IDX
        {
            get { return "PD_STAM_ID_IDX"; }
        }

        /// <summary>
        ///  Gets table name: PD_KLASSE_ID_IDX.
        /// </summary>
        public static string PD_KLASSE_ID_IDX
        {
            get { return "PD_KLASSE_ID_IDX"; }
        }

        /// <summary>
        ///  Gets table name: PD_ORDE_ID_IDX.
        /// </summary>
        public static string PD_ORDE_ID_IDX
        {
            get { return "PD_ORDE_ID_IDX"; }
        }

        /// <summary>
        ///  Gets table name: PD_FAMILIE_ID_IDX.
        /// </summary>
        public static string PD_FAMILIE_ID_IDX
        {
            get { return "PD_FAMILIE_ID_IDX"; }
        }

        /// <summary>
        ///  Gets table name: PD_GESLACHT_ID_IDX.
        /// </summary>
        public static string PD_GESLACHT_ID_IDX
        {
            get { return "PD_GESLACHT_ID_IDX"; }
        }

        // Domain table names

        /// <summary>
        ///  Gets table name: PD_STANDPLAATS.
        /// </summary>
        public static string PD_STANDPLAATS
        {
            get { return "PD_STANDPLAATS"; }
        }

        /// <summary>
        ///  Gets table name: PD_GRONDSOORT.
        /// </summary>
        public static string PD_GRONDSOORT
        {
            get { return "PD_GRONDSOORT"; }
        }

        /// <summary>
        ///  Gets table name: PD_KLEUR.
        /// </summary>
        public static string PD_KLEUR
        {
            get { return "PD_KLEUR"; }
        }

        /// <summary>
        ///  Gets table name: PD_CATEGORIE.
        /// </summary>
        public static string PD_CATEGORIE
        {
            get { return "PD_CATEGORIE"; }
        }

        /// <summary>
        ///  Gets table name: PD_VORM.
        /// </summary>
        public static string PD_VORM
        {
            get { return "PD_VORM"; }
        }

    }
}
