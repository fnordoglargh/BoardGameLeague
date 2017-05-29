using System;
using System.Collections.Generic;
using BoardGameLeagueLib.DbClasses;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace BoardGameLeagueLib
{
    public class Result : DbObject
    {
        public List<Guid> Winners

        {
            get;
            set;
        }

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
            Winners = new List<Guid>();
            Scores = new ObservableCollection<Score>();
        }

        public Result(Guid a_IdGame, ObservableCollection<Score> a_Scores, List<Guid> a_Winners, DateTime a_ResultDate)
        {
            IdGame = a_IdGame;
            Scores = a_Scores;
            Winners = a_Winners;
            Date = a_ResultDate;
            //Game = a_Game;
            //Person = a_Person;
        }

        public Result Copy()
        {
            Result v_TempResult = new Result();

            foreach (Score i_Score in Scores)
            {
                v_TempResult.Scores.Add(new Score(i_Score.IdPlayer, i_Score.ActualScore));
            }

            foreach (Guid i_Winner in Winners)
            {
                v_TempResult.Winners.Add(i_Winner);
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
