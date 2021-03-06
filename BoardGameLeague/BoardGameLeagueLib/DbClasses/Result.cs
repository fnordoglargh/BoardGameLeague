﻿using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using static BoardGameLeagueLib.EloCalculator;

namespace BoardGameLeagueLib.DbClasses
{
    public class Result : DbObject
    {
        public const double c_WinLosePointsLose = 0;
        public const double c_WinLosePointsWin = 1;
        public const double c_WinLosePointsStalemate = 0.5;
        private ILog m_Logger = LogManager.GetLogger("Result");
        private Guid m_IdLocation;
        private Guid m_IdGame;
        private DateTime m_Date;

        internal Dictionary<Guid, Score> ScoresById { get; private set; }

        public ObservableCollection<Score> Scores
        {
            get;
            set;
        }

        [XmlElement("IdLocationRef")]
        public Guid IdLocation
        {
            get { return m_IdLocation; }
            set
            {
                m_IdLocation = value;
                NotifyPropertyChanged("IdLocation");
            }
        }

        [XmlElement("IdGameRef")]
        public Guid IdGame
        {
            get { return m_IdGame; }
            set
            {
                m_IdGame = value;
                NotifyPropertyChanged("IdGame");
                NotifyPropertyChanged("ResultRepresentation");
            }
        }

        public String Comment
        {
            get;
            set;
        }

        public DateTime Date
        {
            get { return m_Date; }
            set
            {
                m_Date = value;
                NotifyPropertyChanged("Date");
                NotifyPropertyChanged("ResultRepresentation");
            }
        }

        public Result()
        {
            Scores = new ObservableCollection<Score>();
            ScoresById = new Dictionary<Guid, Score>();
            Scores.CollectionChanged += Scores_CollectionChanged;
        }

        public Result(Guid a_IdGame, ObservableCollection<Score> a_Scores, DateTime a_ResultDate, Guid a_IdLocation)
        {
            IdGame = a_IdGame;
            Scores = a_Scores;
            Date = a_ResultDate;
            IdLocation = a_IdLocation;
            Init();
            Scores.CollectionChanged += Scores_CollectionChanged;
        }

        private void Scores_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ScoresById.Add(((Score)e.NewItems[0]).IdPlayer, (Score)e.NewItems[0]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ScoresById.Remove(((Score)e.OldItems[0]).IdPlayer);
            }
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
            v_TempResult.Init();

            return v_TempResult;
        }

        public void Update(Result a_UpdateSource)
        {
            int v_ScoreDelta = a_UpdateSource.Scores.Count - Scores.Count;

            if (v_ScoreDelta > 0) // Some new Scores were added.
            {
                for (int i = 0; i < v_ScoreDelta; ++i)
                {
                    Scores.Add(new Score());
                }
            }
            else if (v_ScoreDelta < 0) // Some Scores have been removed.
            {
                for (int i = 0; i < v_ScoreDelta; ++i)
                {
                    m_Logger.Debug("Trying to remove Score at: " + (Scores.Count - 1));
                    Scores.RemoveAt(Scores.Count - 1);
                }
            }
        }

        /// <summary>
        /// Initializes the ScoresById collection after the result has been populated.
        /// </summary>
        internal void Init()
        {
            if (ScoresById == null)
            {
                ScoresById = new Dictionary<Guid, Score>();
            }
            else
            {
                ScoresById.Clear();
            }

            foreach (Score i_Score in Scores)
            {
                ScoresById.Add(i_Score.IdPlayer, i_Score);
            }
        }

        /// <summary>
        /// Takes the scores in result object and looks how the given player did againts the other players. The resulting dict
        /// is used in the CalculateEloResults method or in GenerateYearsOverGames.
        /// </summary>
        /// <param name="a_PlayerId">Guid of the player for which the standings will be considered.</param>
        /// <returns>A dict with elements of the  Modifier enum from the EloCalculator as a key and a List of IDs as the value.
        /// </returns>
        internal Dictionary<Modifier, List<Guid>> CalculateStandings(Guid a_PlayerId, Game.GameType a_GameType)
        {
            Dictionary<Modifier, List<Guid>> v_Standings = new Dictionary<Modifier, List<Guid>>
            {
                { Modifier.Lose, new List<Guid>() },
                { Modifier.Stalemate, new List<Guid>() },
                { Modifier.Win, new List<Guid>() }
            };

            // All scores *without* the active player.
            var v_ScoresWithoutActivePlayer = Scores.Where(p => p.IdPlayer != a_PlayerId);
            bool v_IsWinner = ScoresById[a_PlayerId].IsWinner;
            double v_ScoreActivePlayer = double.Parse(ScoresById[a_PlayerId].ActualScore);

            foreach (Score i_Score in v_ScoresWithoutActivePlayer)
            {
                double v_TempScore = double.Parse(i_Score.ActualScore);

                if (v_IsWinner)
                {
                    if (a_GameType == Game.GameType.TeamedRanks)
                    {
                        // Filter out: These are our team members.
                    }
                    else if (i_Score.IsWinner)
                    {
                        v_Standings[Modifier.Stalemate].Add(i_Score.IdPlayer);
                    }
                    else
                    {
                        v_Standings[Modifier.Win].Add(i_Score.IdPlayer);
                    }
                }
                else if (v_ScoreActivePlayer == v_TempScore)
                {
                    if (a_GameType == Game.GameType.TeamedRanks)
                    {
                        // Filter out: These are our team members.
                    }
                    else if (i_Score.IsWinner)
                    {
                        v_Standings[Modifier.Lose].Add(i_Score.IdPlayer);
                    }
                    else
                    {
                        v_Standings[Modifier.Stalemate].Add(i_Score.IdPlayer);
                    }
                }
                else if (v_ScoreActivePlayer > v_TempScore)
                {
                    // In a ranked game the actual score is the rank and if it's greater than the temp we've lost.
                    if (a_GameType == Game.GameType.Ranks || a_GameType == Game.GameType.TeamedRanks)
                    {
                        v_Standings[Modifier.Lose].Add(i_Score.IdPlayer);
                    }
                    else if (i_Score.IsWinner)
                    {
                        v_Standings[Modifier.Lose].Add(i_Score.IdPlayer);
                    }
                    else
                    {
                        v_Standings[Modifier.Win].Add(i_Score.IdPlayer);
                    }
                }
                else if (v_ScoreActivePlayer < v_TempScore)
                {
                    // In a ranked game the actual score is the rank and if it's smaller than the temp we've won.
                    if (a_GameType == Game.GameType.Ranks || a_GameType == Game.GameType.TeamedRanks)
                    {
                        v_Standings[Modifier.Win].Add(i_Score.IdPlayer);
                    }
                    else if (i_Score.IsWinner)
                    {
                        v_Standings[Modifier.Lose].Add(i_Score.IdPlayer);
                    }
                    else
                    {
                        v_Standings[Modifier.Lose].Add(i_Score.IdPlayer);
                    }
                }
            }

            return v_Standings;
        }

        internal void CalculateEloResultsForTeamedRanks(Dictionary<Guid, ResultHelper> a_StartResults, DateTime a_MatchDate)
        {
            CalculateEloResults(a_StartResults, a_MatchDate, Game.GameType.TeamedRanks);
        }

        internal void CalculateEloResultsForRanks(Dictionary<Guid, ResultHelper> a_StartResults, DateTime a_MatchDate)
        {
            CalculateEloResults(a_StartResults, a_MatchDate, Game.GameType.Ranks);
        }

        internal void CalculateEloResults(Dictionary<Guid, ResultHelper> a_StartResults, DateTime a_MatchDate)
        {
            CalculateEloResults(a_StartResults, a_MatchDate, Game.GameType.WinLose);
        }

        private void CalculateEloResults(Dictionary<Guid, ResultHelper> a_StartResults, DateTime a_MatchDate, Game.GameType a_GameType)
        {
            // We need to increment the count for all players.
            foreach (KeyValuePair<Guid, ResultHelper> i_Kvp in a_StartResults)
            {
                i_Kvp.Value.AmountGamesPlayed++;
            }

            Dictionary<Guid, KeyValuePair<DateTime, int>> v_TempResults = new Dictionary<Guid, KeyValuePair<DateTime, int>>();

            // We only want to do it ONCE for each entry.
            foreach (KeyValuePair<Guid, ResultHelper> i_Kvp in a_StartResults)
            {
                // Get the standings for given player.
                Dictionary<Modifier, List<Guid>> v_Standings = CalculateStandings(i_Kvp.Key, a_GameType);
                List<double> v_TempEloScores = new List<double>();

                // Iterates over all standings. Standings contains n-1 individual Ids (where n is the amount of players in a match).
                foreach (KeyValuePair<Modifier, List<Guid>> i_KvpInner in v_Standings)
                {
                    double v_ModifierPlayer = EloCalculator.EstablishedStatusToModifier[i_Kvp.Value.IsEstablished][i_KvpInner.Key];

                    foreach (Guid i_OpponentId in i_KvpInner.Value)
                    {
                        /// The +1 is needed because we only increment the amount of played games inside AddResult.
                        /// In the old implementation was actually a defect because the counter for the active player 
                        /// was incremented and the calculation used the old value for the opponent.
                        double v_TempEloScore = EloCalculator.CalculateEloRanking(
                            i_Kvp.Value.EloScore,
                            i_Kvp.Value.AmountGamesPlayed,
                            i_Kvp.Value.IsEstablished,
                            a_StartResults[i_OpponentId].EloScore,
                            a_StartResults[i_OpponentId].AmountGamesPlayed,
                            a_StartResults[i_OpponentId].IsEstablished,
                            v_ModifierPlayer
                        );

                        v_TempEloScores.Add(v_TempEloScore);
                    }
                }

                double v_NewEloScore = 0;

                foreach (double i_TempScore in v_TempEloScores)
                {
                    v_NewEloScore += i_TempScore;
                }

                if (v_TempEloScores.Count > 0)
                {
                    v_NewEloScore = v_NewEloScore / v_TempEloScores.Count;
                    // Must NOT be applied immediately! We're putting the reults aside and apply them afterwards.
                    v_TempResults.Add(i_Kvp.Key, new KeyValuePair<DateTime, int>(a_MatchDate, (int)Math.Round(v_NewEloScore, 0)));
                }
            }

            // Now we add the new results.
            foreach (KeyValuePair<Guid, ResultHelper> i_Kvp in a_StartResults)
            {
                i_Kvp.Value.AddResult(v_TempResults[i_Kvp.Key].Key, v_TempResults[i_Kvp.Key].Value);
            }
        }

        public String ResultRepresentation
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}.{1:D2}.{2:D2} - {3}", Date.Year, Date.Month, Date.Day, DbHelper.Instance.LiveBglDb.GamesById[IdGame].Name);
        }

        /// <summary>
        /// Holds data for one player and his ELO related results.
        /// </summary>
        public class ResultHelper
        {
            private int m_AmountGamesPlayed = 0;
            private int m_EloScore;

            public List<KeyValuePair<DateTime, int>> Progression { get; private set; }
            public Guid PlayerId { get; private set; }
            public bool IsEstablished { get; private set; }

            public int EloScore
            {
                get { return m_EloScore; }
                private set
                {
                    m_EloScore = value;
                }
            }

            public int AmountGamesPlayed
            {
                get { return m_AmountGamesPlayed; }
                set
                {
                    if (value < 0)
                    {
                        m_AmountGamesPlayed = 0;
                    }
                    else
                    {
                        m_AmountGamesPlayed = value;

                        if (m_AmountGamesPlayed > 19 && !IsEstablished)
                        {
                            Console.WriteLine(PlayerId + " is now ESTABLISHED!");
                            IsEstablished = true;
                        }
                        else if (m_AmountGamesPlayed < 19 && IsEstablished)
                        {
                            Console.WriteLine(PlayerId + " is NOT established anymore. How did that happen?");
                            IsEstablished = false;
                        }
                    }
                }
            }

            public ResultHelper(Guid a_PlayerId, int a_EloScore, int a_AmountGamesPlayed)
            {
                Progression = new List<KeyValuePair<DateTime, int>>();
                PlayerId = a_PlayerId;
                m_EloScore = a_EloScore;
                AmountGamesPlayed = a_AmountGamesPlayed;
            }

            /// <summary>
            /// Wraps changing the actual ELO score and records the progression.
            /// </summary>
            /// <param name="a_Date">Date of latest played match.</param>
            /// <param name="a_EloRankNew">New ELO rank. Is used for both progression and current rank.</param>
            public void AddResult(DateTime a_Date, int a_EloRankNew)
            {
                // This the first element to the collection if there aren't any yet.
                if (Progression.Count == 0)
                {
                    // We consider the January 1st as creation date for a player in the year of their first result. Makes the chart look a bit nicer.
                    Progression.Add(new KeyValuePair<DateTime, int>(new DateTime(a_Date.Year, 1, 1), 0));
                }

                Progression.Add(new KeyValuePair<DateTime, int>(a_Date, a_EloRankNew - m_EloScore));
                EloScore = a_EloRankNew;
            }
        }
    }
}
