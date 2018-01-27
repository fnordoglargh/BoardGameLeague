using System;

namespace BoardGameLeagueLib.DbClasses
{
    public abstract class DbObjectName : DbObject
    {
        private String m_Name;

        /// <summary>
        /// Gets/sets the Name of the DbObject. All trailing whitespaces will be trimmed.
        /// </summary>
        public String Name
        {
            get { return m_Name; }
            set
            {
                // DataGrid Columns will mess up the binding if therfe are thrailing whitepaces.
                m_Name = value.TrimEnd(' ');
                NotifyPropertyChanged("Name");
            }
        }

        public DbObjectName(String a_Name)
        {
            Name = a_Name;
        }
    }
}
