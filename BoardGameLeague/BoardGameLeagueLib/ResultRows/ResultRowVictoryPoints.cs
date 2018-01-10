using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib.ResultRows
{
    public class ResultRowVictoryPoints : ResultRow
    {
        public int AmountPoints { get; set; }
        public double AveragePoints { get; set; }
        public int BestScore { get; set; }
        public int WorstScore { get; set; }

        public ResultRowVictoryPoints(String a_Name, int a_AmountPlayed, int a_AmountWon, int a_AmountPoints)
            : base(a_Name, a_AmountPlayed, a_AmountWon)
        {
            AmountPoints = a_AmountPoints;
            BestScore = 0;

            BestScore = int.MinValue;
            WorstScore = int.MaxValue;

            ColumnNames.Add("AmountPoints", new KeyValuePair<string, int>("Amount Points", -1));
            ColumnNames.Add("AveragePoints", new KeyValuePair<string, int>("Average Points", -1));
            ColumnNames.Add("BestScore", new KeyValuePair<string, int>("Best Score", -1));
            ColumnNames.Add("WorstScore", new KeyValuePair<string, int>("Worst Score", -1));
        }
    }
}
