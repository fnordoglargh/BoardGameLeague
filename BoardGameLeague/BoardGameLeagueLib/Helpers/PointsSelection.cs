using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueUI.Charts;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BoardGameLeagueUI.Helpers
{
    public class PointsSelectionHelper
    {
        ILog m_Logger = LogManager.GetLogger("PointsSelectionHelper");
        private Guid m_GameOrFamilyId;
        private PointsMode m_ActualMode;
        private IList<object> m_SelectedPlayers;

        public PointsMode ActualMode
        {
            get { return m_ActualMode; }
            set
            {
                m_ActualMode = value;
                GeneratePointProgressionChart();
            }
        }

        public Guid GameOrFamilyId
        {
            get { return m_GameOrFamilyId; }
            set
            {
                m_GameOrFamilyId = value;
                GeneratePointProgressionChart();
            }
        }

        public IList<object> SelectedPlayers
        {
            get { return m_SelectedPlayers; }
            set
            {
                m_SelectedPlayers = value;
                GeneratePointProgressionChart();
            }
        }

        private LineChart m_PointsChart;
        private BglDb m_BglDatabase;

        public Dictionary<PointsMode, string> PointsModes => m_PointsModes;

        public static Dictionary<PointsMode, String> m_PointsModes = new Dictionary<PointsMode, string>
        {
            { PointsMode.Progression, "Progression" },
            { PointsMode.OneByOne, "One by One" }
        };

        public enum PointsMode
        {
            Progression,
            OneByOne
        }

        public PointsSelectionHelper(LineChart a_PointsChart)
        {
            m_PointsChart = a_PointsChart;
            m_ActualMode = PointsMode.Progression;
            m_BglDatabase = DbHelper.Instance.LiveBglDb;
        }

        private void GeneratePointProgressionChart()
        {
            // Make sure we have selected Players. We may want to raise a user notification or prevent deselecting the last player.
            if (SelectedPlayers == null || SelectedPlayers.Count < 1) { return; }
            m_PointsChart.Progression.Clear();

            bool v_IsSelectionFine = true;
            IEnumerable<Result> v_BeginningToEndResults = new ObservableCollection<Result>(m_BglDatabase.Results.OrderBy(p => p.Date));

            if (m_BglDatabase.GameFamiliesById.ContainsKey(GameOrFamilyId))
            {
                m_Logger.Debug("Generating chart for game family: " + m_BglDatabase.GameFamiliesById[GameOrFamilyId].Name);
                var v_AllGamesFromFamily = m_BglDatabase.Games.Where(p => p.IdGamefamily == GameOrFamilyId);

                if (v_AllGamesFromFamily.Count() > 0)
                {
                    // Second: Get all results with games of the given game family.
                    v_BeginningToEndResults = v_BeginningToEndResults.Join(v_AllGamesFromFamily,
                        result => result.IdGame,
                        game => game.Id,
                        (result, game) => result);
                }
                else
                {
                    v_BeginningToEndResults = new ObservableCollection<Result>();
                }
            }
            else if (m_BglDatabase.GamesById.ContainsKey(GameOrFamilyId))
            {
                m_Logger.Debug("Generating chart for game: " + m_BglDatabase.GamesById[GameOrFamilyId].Name);
                v_BeginningToEndResults = v_BeginningToEndResults.Where(p => p.IdGame == GameOrFamilyId);
            }
            else
            {
                m_Logger.Error("No chart generated: No game or game family selected.");
                v_IsSelectionFine = false;
            }

            if (v_IsSelectionFine)
            {
                foreach (object i_Player in SelectedPlayers)
                {
                    Player v_Player = i_Player as Player;

                    LineSeries v_LineSeries = new LineSeries
                    {
                        Title = v_Player.Name,
                        Values = new ChartValues<DateTimePoint>(),
                        LineSmoothness = 0.20,
                        PointGeometrySize = 2,
                    };

                    double v_PreviousScore = 0;

                    foreach (Result i_Result in v_BeginningToEndResults)
                    {
                        foreach (Score i_Score in i_Result.Scores)
                        {
                            if (i_Score.IdPlayer == v_Player.Id)
                            {
                                if (ActualMode == PointsMode.Progression)
                                {
                                    v_PreviousScore += double.Parse(i_Score.ActualScore);
                                }
                                else
                                {
                                    v_PreviousScore = double.Parse(i_Score.ActualScore);
                                }

                                v_LineSeries.Values.Add(new DateTimePoint(i_Result.Date, v_PreviousScore));
                            }
                        }
                    }

                    m_PointsChart.Progression.Add(v_LineSeries);
                }
            }
        }
    }
}
