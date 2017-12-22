using System;
using System.Collections.Generic;

namespace BoardGameLeagueUI.Helpers
{
    public abstract class ChartHelperBase
    {
        private Guid m_GameOrFamilyId;
        private IList<object> m_SelectedPlayers;

        public Guid GameOrFamilyId
        {
            get { return m_GameOrFamilyId; }
            set
            {
                m_GameOrFamilyId = value;
                GenerateChart();
            }
        }

        public IList<object> SelectedPlayers
        {
            get { return m_SelectedPlayers; }
            set
            {
                m_SelectedPlayers = value;
                GenerateChart();
            }
        }

        public abstract void GenerateChart();
    }
}
