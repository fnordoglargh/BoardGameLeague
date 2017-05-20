using System;
using System.Xml.Serialization;

namespace BoardGameLeagueLib
{
    public class Score
    {
        [XmlElement("IdPlayerRef")]
        public Guid IdPlayer
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

        public Score(Guid a_IdPlayer, String a_ActualScore)
        {
            IdPlayer = a_IdPlayer;
            ActualScore = a_ActualScore;
        }
    }
}
