using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueUI.Charts;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BoardGameLeagueUI.Helpers
{
    public class PointsSelectionHelper : ChartHelperBase
    {
        public PointsSelectionHelper(LineChart a_LineChart)
            : base(a_LineChart)
        {
            ActualMode = CalculationMode.Progression;
            m_Logger = LogManager.GetLogger("PointsSelectionHelper");

            m_CalculationModes = new Dictionary<CalculationMode, string>
            {
                { CalculationMode.Progression, "Progression" },
                { CalculationMode.OneByOne, "One by One" }
            };
        }

        public override void GenerateChart()
        {
            // Make sure we have selected Players. We may want to raise a user notification or prevent deselecting the last player.
            if (SelectedPlayers == null || SelectedPlayers.Count < 1) { return; }

            m_LineChart.Progression.Clear();
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
                m_Logger.Warn("No chart generated: No game or game family selected.");
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
                                if (ActualMode == CalculationMode.Progression)
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

                    m_LineChart.Progression.Add(v_LineSeries);
                }
            }
        }
    }
}
