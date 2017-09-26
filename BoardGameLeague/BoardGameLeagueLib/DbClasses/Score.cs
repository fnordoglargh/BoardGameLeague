using System;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
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

        public bool IsWinner
        {
            get;
            set;
        }

        public Score() { }

        public Score(Guid a_IdPlayer, String a_ActualScore, bool a_IsWinner)
        {
            IdPlayer = a_IdPlayer;
            ActualScore = a_ActualScore;
            IsWinner = a_IsWinner;
        }
    }
}
