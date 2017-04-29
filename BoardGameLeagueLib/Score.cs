using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameLeagueLib
{
    public class Score
    {
        public Guid IdPerson
        {
            get;
            set;
        }

        public String ActualScore
        {
            get;
            set;
        }

        public Score() { }

        public Score(Guid a_IdPerson, String a_ActualScore)
        {
            IdPerson = a_IdPerson;
            ActualScore = a_ActualScore;
        }

    }
}
