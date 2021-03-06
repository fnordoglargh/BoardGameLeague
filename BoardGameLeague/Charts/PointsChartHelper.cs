﻿using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace BoardGameLeagueUI.Charts.Helpers
{
    public class PointsChartHelper : ChartHelperBase
    {
        public PointsChartHelper()
        {
            m_Logger = LogManager.GetLogger("PointsSelectionHelper");
            CalculationModes.Add(CalculationMode.Progression, "Progression");
            CalculationModes.Add(CalculationMode.OneByOne, "One by One");
            ActualMode = CalculationMode.Progression;
        }

        public override List<Player> GenerateChart()
        {
            List<Player> v_PlayersWithTooFewResults = new List<Player>();
            m_LineChart.Progression.Clear();

            // Make sure we have selected Players. We may want to raise a user notification or prevent deselecting the last player.
            if (SelectedPlayers == null || SelectedPlayers.Count < 1)
            {
                m_Logger.Info("Tried to generate a chart but the selected players were either null or empty.");
                return v_PlayersWithTooFewResults;
            }

            bool v_IsSelectionFine = true;
            IEnumerable<Result> v_BeginningToEndResults = new ObservableCollection<Result>(m_BglDatabase.Results.OrderBy(p => p.Date));

            if (m_BglDatabase.GameFamiliesById.ContainsKey(GameOrFamilyId))
            {
                m_Logger.Debug("Generating chart for game family: " + m_BglDatabase.GameFamiliesById[GameOrFamilyId].Name);
                var v_AllGamesFromFamily = m_BglDatabase.Games.Where(p => p.IdGamefamilies.Contains(GameOrFamilyId));

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
                DateTime v_DateFirst = DateTime.MaxValue;
                DateTime v_DateLast = DateTime.MinValue;

                // Get the last and first dates from all results.
                foreach (Result i_Result in v_BeginningToEndResults)
                {
                    if (i_Result.Date > v_DateLast)
                    {
                        v_DateLast = i_Result.Date;
                    }

                    if (i_Result.Date < v_DateFirst)
                    {
                        v_DateFirst = i_Result.Date;
                    }
                }

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

                    // Make graphs transparent based on app settings.
                    if (SettingsHelper.Instance.Preferences.IsGraphAreaTransparent)
                    {
                        v_LineSeries.Fill = Brushes.Transparent;
                    }

                    double v_PreviousScore = 0;

                    // If the date is normalized we put the standard ELO score at the first date we saved.
                    if (SettingsHelper.Instance.Preferences.IsDateNormalized)
                    {
                        v_LineSeries.Values.Add(new DateTimePoint(v_DateFirst, v_PreviousScore));
                    }

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

                    // If the date is normalized we put the standard ELO score at the last date we saved.
                    if (SettingsHelper.Instance.Preferences.IsDateNormalized)
                    {
                        v_LineSeries.Values.Add(new DateTimePoint(v_DateLast, v_PreviousScore));
                    }

                    if (v_LineSeries.Values.Count > 0)
                    {
                        m_LineChart.Progression.Add(v_LineSeries);
                    }
                    else
                    {
                        v_PlayersWithTooFewResults.Add(v_Player);
                    }
                }
            }

            return v_PlayersWithTooFewResults;
        }
    }
}
