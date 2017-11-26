using System;

namespace BoardGameLeagueLib.ResultRows
{
    public abstract class ResultRow
    {
        public String Name { get; set; }
        public int AmountPlayed { get; set; }
        public int AmountWon { get; set; }
        public double PercentageWon { get; set; }

        public ResultRow(String a_Name, int a_AmountPlayed, int a_AmountWon)
        {
            Name = a_Name;
            AmountPlayed = a_AmountPlayed;
            AmountWon = a_AmountWon;
        }
    }
}
