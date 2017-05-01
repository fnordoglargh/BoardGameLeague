using System;
using BoardGameLeagueLib.DbClasses;

namespace BoardGameLeagueLib
{
    public class Person : DbObjectName
    {
        private String m_DisplayName;

        public String DisplayName
        {
            get
            {
                return m_DisplayName;
            }
            set
            {
                m_DisplayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }

        private Genders m_Gender;

        public Genders Gender
        {
            get
            {
                return m_Gender;
            }
            set
            {
                m_Gender = value;
                NotifyPropertyChanged("Gender");
            }
        }

        public enum Genders
        {
            Male,
            Female
        }

        public Person(String a_PlayerName, String a_DisplayName, Genders a_Gender)
            : base(a_PlayerName)
        {
            DisplayName = a_DisplayName;
            Gender = a_Gender;
        }

        public Person()
            : base("No Name")
        {
            DisplayName = "No Display Name";
            Gender = Genders.Male;
        }
    }
}
