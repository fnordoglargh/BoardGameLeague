using System;

namespace BoardGameLeagueLib.DbClasses
{
    public class Settings
    {
        /// <summary>
        /// Path to the database file which was last used.
        /// </summary>
        public String LastUsedDatabase { get; set; }

        public Settings() { }
    }
}
