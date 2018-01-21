using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib.ResultRows
{
    // Remember to add any newly implemented class to DG_AutoGeneratingColumn in the MainWindow.
    public abstract class ResultRow
    {
        public String Name { get; set; }
        public int AmountPlayed { get; set; }
        public int AmountWon { get; set; }
        public double PercentageWon { get; set; }
        public Dictionary<String, KeyValuePair<String, int>> ColumnNames { get; set; }

        public ResultRow(String a_Name, int a_AmountPlayed, int a_AmountWon)
        {
            Name = a_Name;
            AmountPlayed = a_AmountPlayed;
            AmountWon = a_AmountWon;
            ColumnNames = new Dictionary<string, KeyValuePair<string, int>>();
            ColumnNames.Add("Name", new KeyValuePair<string, int>("Name", ColumnNames.Count));
            ColumnNames.Add("AmountPlayed", new KeyValuePair<string, int>("Amount Played", ColumnNames.Count));
            ColumnNames.Add("AmountWon", new KeyValuePair<string, int>("Amount Won", ColumnNames.Count));
            ColumnNames.Add("PercentageWon", new KeyValuePair<string, int>("Percentage Won", ColumnNames.Count));
        }

        public void CalculatePercentageWon()
        {
            PercentageWon = Math.Round(100 * AmountWon / (double)AmountPlayed, 2);
        }
    }
}
