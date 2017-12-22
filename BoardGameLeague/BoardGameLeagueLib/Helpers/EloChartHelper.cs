﻿using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueUI.Charts;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;

namespace BoardGameLeagueUI.Helpers
{
    public class EloChartHelper : ChartHelperBase
    {
        public EloChartHelper(LineChart a_LineChart)
            : base(a_LineChart)
        {
            m_CalculationModes = new Dictionary<CalculationMode, string>
            {
                { CalculationMode.EloAll, "ELO - Entire Database" }
            };
        }

        public override void GenerateChart()
        {
            if (SelectedPlayers == null || SelectedPlayers.Count < 1) { return; }

            m_LineChart.Progression.Clear();
            Dictionary<Player, Result.ResultHelper> v_EloResults = m_BglDatabase.CalculateEloResults(GameOrFamilyId);

            // Make sure we have selected Players. We may want to raise a user notification or prevent deselecting the last player.
            if (SelectedPlayers.Count < 1) { return; }

            foreach (object i_Player in SelectedPlayers)
            {
                Player v_Player = i_Player as Player;
                m_Logger.Debug(v_Player.Name);

                LineSeries v_LineSeries = new LineSeries
                {
                    Title = v_Player.Name,
                    Values = new ChartValues<DateTimePoint>(),
                    LineSmoothness = 0.20,
                    PointGeometrySize = 2,
                };

                Result.ResultHelper v_ActualResult = v_EloResults[v_Player];
                int v_EloRanking = BglDb.c_EloStartValue;

                foreach (KeyValuePair<DateTime, int> i_ProgressionResult in v_ActualResult.Progression)
                {
                    v_EloRanking += i_ProgressionResult.Value;
                    v_LineSeries.Values.Add(new DateTimePoint(i_ProgressionResult.Key, v_EloRanking));
                }

                m_LineChart.Progression.Add(v_LineSeries);
            }
        }
    }
}
