using System;
using BoardGameLeagueLib.DbClasses;
using System.Collections.Generic;

namespace BoardGameLeagueLib
{
    public class Player : DbObjectName
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

        public static List<Genders> GendersList = new List<Genders>
        {
            {Genders.Male },
            {Genders.Female }
        };

        public Player(String a_PlayerName, String a_DisplayName, Genders a_Gender)
            : base(a_PlayerName)
        {
            DisplayName = a_DisplayName;
            Gender = a_Gender;
        }

        public Player()
            : base("No Name")
        {
            DisplayName = "No Display Name";
            Gender = Genders.Male;
        }
    }
}
