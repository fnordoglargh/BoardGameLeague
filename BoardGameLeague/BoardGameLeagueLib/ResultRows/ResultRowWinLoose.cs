using System;

namespace BoardGameLeagueLib.ResultRows
{
    public class ResultRowWinLoose : ResultRow
    {
        public int AmountDraw { get; set; }
        public int AmountLoss { get { return AmountPlayed - AmountDraw - AmountWon; } }

        public ResultRowWinLoose(String a_Name, int a_AmountPlayed, int a_AmountWon)
            : base(a_Name, a_AmountPlayed, a_AmountWon)
        {
            AmountDraw = 0;
        }
    }
}
