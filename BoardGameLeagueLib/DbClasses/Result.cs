using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    public class Result : DbObject
    { 
        public ObservableCollection<Score> Scores
        {
            get;
            set;
        }

        [XmlElement("IdLocationRef")]
        public Guid IdLocation
        {
            get;
            set;
        }

        [XmlElement("IdGameRef")]
        public Guid IdGame
        {
            get;
            set;
        }

        public String Comment
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        //public Dictionary<String,int> 

        public Result()
        {
            Scores = new ObservableCollection<Score>();
        }

        public Result(Guid a_IdGame, ObservableCollection<Score> a_Scores, DateTime a_ResultDate, Guid a_IdLocation)
        {
            IdGame = a_IdGame;
            Scores = a_Scores;
            Date = a_ResultDate;
            IdLocation = a_IdLocation;
        }

        public Result Copy()
        {
            Result v_TempResult = new Result();

            foreach (Score i_Score in Scores)
            {
                v_TempResult.Scores.Add(new Score(i_Score.IdPlayer, i_Score.ActualScore, i_Score.IsWinner));
            }

            v_TempResult.Comment = Comment;
            v_TempResult.IdGame = IdGame;
            v_TempResult.IdLocation = IdLocation;
            v_TempResult.Date = new DateTime(Date.Year, Date.Month, Date.Day);

            return v_TempResult;
        }

        public override string ToString()
        {
            return Date.Year + "." + Date.Month + "." + Date.Day;
        }
    }
}
