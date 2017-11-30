using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib
{
    public class EloCalculator
    {
        public enum Modifier
        {
            Win,
            Loose,
            Stalemate
        };

        public static Dictionary<Modifier, double> ModifierProvisional = new Dictionary<Modifier, double>
        {
            { Modifier.Win, 1.0 },
            { Modifier.Loose, -1.0 },
            { Modifier.Stalemate, 0.0 }
        };

        public static Dictionary<Modifier, double> ModifierEstablished = new Dictionary<Modifier, double>
        {
            { Modifier.Win, 1.0 },
            { Modifier.Loose, 0.0 },
            { Modifier.Stalemate, 0.5 }
        };

        public static Dictionary<bool, Dictionary<Modifier, double>> EstablishedStatusToModifier = new Dictionary<bool, Dictionary<Modifier, double>>
        {
            { false, ModifierProvisional },
            { true, ModifierEstablished }
        };

        /// <summary>
        /// Calculates the ELO score for player A who played against player B if both players have provisional ranks. 
        /// See https://www.daysofwonder.com/online/en/play/ranking/ for details.
        /// </summary>
        /// <param name="a_StartingScorePlayerA"></param>
        /// <param name="a_AmountGamesPlayerA"></param>
        /// <param name="a_StartingScorePlayerB"></param>
        /// <param name="a_AmountGamesPlayerB"></param>
        /// <returns>Elo score of player A.</returns>
        public static double CalcProvisionalVsProvisionalRanking(int a_StartingScorePlayerA, int a_AmountGamesPlayerA, int a_StartingScorePlayerB, double a_Modifier)
        {
            if (a_AmountGamesPlayerA > 20) { throw new ArgumentException("a_AmountGamesPlayerA > 20"); }
            if (a_AmountGamesPlayerA < 1) { throw new ArgumentException("a_AmountGamesPlayerA < 1"); }

            return (a_StartingScorePlayerA * a_AmountGamesPlayerA + (a_StartingScorePlayerA + a_StartingScorePlayerB) / 2 + (int)(100 * a_Modifier)) / (a_AmountGamesPlayerA + 1);
        }

        public static double CalcProvisionalVsEstablishedRanking(int a_StartingScorePlayerA, int a_AmountGamesPlayerA, int a_StartingScorePlayerB, double a_Modifier)
        {
            return (a_StartingScorePlayerA * a_AmountGamesPlayerA + a_StartingScorePlayerB + (int)(200 * a_Modifier)) / (a_AmountGamesPlayerA + 1);
        }

        public static double CalcEstablishedVsProvisionalRanking(int a_StartingScorePlayerA, int a_StartingScorePlayerB, int a_AmountGamesPlayerB, double a_Modifier)
        {
            return a_StartingScorePlayerA + 32 * a_AmountGamesPlayerB / 20 * (a_Modifier - 1 / (1 + Math.Pow(10, (a_StartingScorePlayerB - a_StartingScorePlayerA) / 400)));
        }

        public static double CalcEstablishedVsEstablishedRanking(int a_StartingScorePlayerA, int a_StartingScorePlayerB, double a_Modifier)
        {
            return a_StartingScorePlayerA + 32 * (a_Modifier - 1 / (1 + Math.Pow(10, (a_StartingScorePlayerB - a_StartingScorePlayerA) / 400)));
        }

        public static double CalculateEloRanking(
            int a_StartingScorePlayerA, int a_AmountGamesPlayerA, bool a_IsEstablishedPlayerA,
            int a_StartingScorePlayerB, int a_AmountGamesPlayerB, bool a_IsEstablishedPlayerB,
            double a_Modifier)
        {
            double v_EloScore = 0;

            if (a_IsEstablishedPlayerA && a_IsEstablishedPlayerB)
            {
                v_EloScore = CalcEstablishedVsEstablishedRanking(a_StartingScorePlayerA, a_StartingScorePlayerB, a_Modifier);
            }
            else if (a_IsEstablishedPlayerA && !a_IsEstablishedPlayerB)
            {
                v_EloScore = CalcEstablishedVsProvisionalRanking(a_StartingScorePlayerA, a_StartingScorePlayerB, a_AmountGamesPlayerB, a_Modifier);
            }
            else if (!a_IsEstablishedPlayerA && a_IsEstablishedPlayerB)
            {
                v_EloScore = CalcProvisionalVsEstablishedRanking(a_StartingScorePlayerA, a_AmountGamesPlayerA, a_StartingScorePlayerB, a_Modifier);
            }
            else if (!a_IsEstablishedPlayerA && !a_IsEstablishedPlayerB)
            {
                v_EloScore = CalcProvisionalVsProvisionalRanking(a_StartingScorePlayerA, a_AmountGamesPlayerA, a_StartingScorePlayerB, a_Modifier);
            }

            return v_EloScore;
        }

        public class EloResultRow
        {
            public string Name { get; set; }
            public int EloRating { get; set; }
            public int GamesPlayed { get; set; }
            public bool IsEstablished { get; set; }
            public Dictionary<String, KeyValuePair<String, int>> ColumnNames { get; set; }

            public EloResultRow(String a_Name, int a_EloRating, int a_GamesPlayed, bool a_IsEstablished)
            {
                Name = a_Name;
                EloRating = a_EloRating;
                GamesPlayed = a_GamesPlayed;
                IsEstablished = a_IsEstablished;
                ColumnNames = new Dictionary<string, KeyValuePair<string, int>>();
                ColumnNames.Add("Name", new KeyValuePair<string, int>("Name", ColumnNames.Count));
                ColumnNames.Add("EloRating", new KeyValuePair<string, int>("Elo Rating", ColumnNames.Count));
                ColumnNames.Add("GamesPlayed", new KeyValuePair<string, int>("Games Played", ColumnNames.Count));
                ColumnNames.Add("IsEstablished", new KeyValuePair<string, int>("Established Player (over 20 results)", ColumnNames.Count));
            }
        }
    }
}
