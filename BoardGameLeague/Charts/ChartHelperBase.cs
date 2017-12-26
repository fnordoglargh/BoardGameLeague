using BoardGameLeagueLib.DbClasses;
using log4net;
using System;
using System.Collections.Generic;

namespace BoardGameLeagueUI.Charts.Helpers
{
    /// <summary>
    /// The ChartHelperBase is the base class for getting a chart populated with e.g. ELO data or victory points. Updating the ActualMode,
    /// GameOrFamilyId or SelectedPlayers triggers redrawing if all three have been populated with actual data.
    /// </summary>
    public abstract class ChartHelperBase
    {
        private Guid m_GameOrFamilyId;
        private IList<object> m_SelectedPlayers;
        private CalculationMode m_ActualMode;
        protected ILog m_Logger = LogManager.GetLogger("ChartHelperBase");
        protected BglDb m_BglDatabase;

        public LineChart Chart => m_LineChart;
        protected LineChart m_LineChart;

        public enum CalculationMode
        {
            Progression,
            OneByOne,
            EloAll
        }

        public Dictionary<CalculationMode, string> CalculationModes => m_CalculationModes;
        private Dictionary<CalculationMode, string> m_CalculationModes = new Dictionary<CalculationMode, string>();

        public CalculationMode ActualMode
        {
            get { return m_ActualMode; }
            set
            {
                if (CalculationModes == null || CalculationModes.Count == 0)
                {
                    m_Logger.Error("Setting ActualMode failed. CalculationModes is null or empty. This is normal on program loading.");
                }
                else if (CalculationModes.ContainsKey(value))
                {
                    m_ActualMode = value;
                    GenerateChart();
                }
                else
                {
                    m_Logger.Error("Setting ActualMode failed. Mode is invalid: " + value);
                }
            }
        }

        public Guid GameOrFamilyId
        {
            get { return m_GameOrFamilyId; }
            set
            {
                if (m_BglDatabase.GamesById.ContainsKey(value) || m_BglDatabase.GameFamiliesById.ContainsKey(value) || Guid.Empty == value)
                {
                    m_GameOrFamilyId = value;
                    GenerateChart();
                }
                else
                {
                    m_Logger.Error("Setting game or family failed. ID is invalid: " + value);
                }
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

        public ChartHelperBase()
        {
            m_LineChart = new LineChart();
            m_BglDatabase = DbHelper.Instance.LiveBglDb;
        }

        public abstract void GenerateChart();
    }
}
