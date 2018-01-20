using System;
using System.Collections.Generic;

namespace BoardGameLeagueLib.ResultRows
{
    public class ResultRowRanks : ResultRow
    {
        private int m_RankBest = int.MaxValue;
        private int m_RankWorst = int.MinValue;

        public int BestRank
        {
            get { return m_RankBest; }
            set
            {
                if (value < m_RankBest)
                {
                    m_RankBest = value;
                }
            }
        }

        public int WorstRank
        {
            get { return m_RankWorst; }
            set
            {
                if (value > m_RankWorst)
                {
                    m_RankWorst = value;
                }
            }
        }

        public ResultRowRanks(String a_Name, int a_AmountPlayed, int a_AmountWon)
            : base(a_Name, a_AmountPlayed, a_AmountWon)
        {
           ColumnNames.Add("BestRank", new KeyValuePair<string, int>("Best Rank", -1));
           ColumnNames.Add("WorstRank", new KeyValuePair<string, int>("Worst Rank", -1));
        }
    }
}
