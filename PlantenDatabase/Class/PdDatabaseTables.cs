namespace PlantenDatabase
{
    using System.Collections.Generic;

    /// <summary>
    /// Hold the database table names.
    /// </summary>
    public class PdDatabaseTables
    {
        private readonly List<PdDatabaseTable> items = new List<PdDatabaseTable>();

        /// <summary>
        /// Gets a list with the database table names.
        /// </summary>
        public List<PdDatabaseTable> Items { get { return this.items; } }
    }
}
