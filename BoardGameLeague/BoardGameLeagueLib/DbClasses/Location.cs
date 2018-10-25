using System;

namespace BoardGameLeagueLib.DbClasses
{
    public class Location : DbObjectName
    {
        public Location()
            : base("Default Location Name")
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

        public override string ToString()
        {
            return "ID: " + Id + ", Name: " + Name;
        }
    }
}
