using System;

namespace BoardGameLeagueLib.DbClasses
{
    public abstract class DbObjectName : DbObject
    {
        private String m_Name;

        public String Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public DbObjectName(String a_Name)
        {
            Name = a_Name;
        }
    }
}
