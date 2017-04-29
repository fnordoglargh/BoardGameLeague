using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameLeagueLib
{
    [Serializable]
    public class ResultDatabase
    {
        //private List<Game> m_Games = new List<Game>();
        //private List<Person> m_Persons = new List<Person>();
        //private List<Result> m_Rsults = new List<Result>();
        private Custodian m_Custodian;

        public List<Game> Games
        {
            get;
            set;
        }

        public List<Person> Persons
        {
            get;
            set;
        }

        //public List<Result> Results
        //{
        //    get;
        //    set;
        //}

        public ResultDatabase()
        {
            m_Custodian = Custodian.Instance;
            Games = new List<Game>();
            Persons = new List<Person>();
            //Results = new List<Result>();
        }


    }
}
