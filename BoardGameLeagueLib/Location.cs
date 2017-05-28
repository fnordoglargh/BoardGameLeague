using System;
using BoardGameLeagueLib.DbClasses;

namespace BoardGameLeagueLib
{
    public class Location : DbObjectName
    {
        public Location()
            : base("no name")
        {
        }

        public Location(String a_Name)
            : base(a_Name)
        {
        }

        private String m_Description;

        public String Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                NotifyPropertyChanged("Description");
            }
        }
    }
}
