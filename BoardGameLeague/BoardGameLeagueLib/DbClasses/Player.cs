using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib.DbClasses
{
    public class Player : DbObjectName
    {
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
            {Genders.Male},
            {Genders.Female}
        };

        public Player(String a_PlayerName, Genders a_Gender)
            : base(a_PlayerName)
        {
            Gender = a_Gender;
        }

        public Player()
            : base("Default Player Name")
        {
            Gender = Genders.Male;
        }

        public override string ToString()
        {
            return Id + ", " +  Name + ", " + Gender;
        }
    }
}
