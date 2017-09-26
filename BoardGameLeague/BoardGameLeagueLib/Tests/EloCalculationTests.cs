using BoardGameLeagueLib.DbClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static BoardGameLeagueLib.EloCalculator;

namespace BoardGameLeagueLib.Tests
{
    [TestFixture]
    class EloCalculationTests
    {
        [Test]
        public void ProvVsProv()
        {
            int v_ActualScore = (int)EloCalculator.CalcProvisionalVsProvisionalRanking(1500, 1, 1500, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Win]);
            Assert.AreEqual(1550, v_ActualScore);
            v_ActualScore = (int)EloCalculator.CalcProvisionalVsProvisionalRanking(1700, 6, 1100, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Win]);
            Assert.AreEqual(1671, v_ActualScore);
            v_ActualScore = (int)EloCalculator.CalcProvisionalVsProvisionalRanking(1158, 12, 1541, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Win]);
            Assert.AreEqual(1180, v_ActualScore);

            v_ActualScore = (int)EloCalculator.CalcProvisionalVsProvisionalRanking(1500, 1, 1500, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Loose]);
            Assert.AreEqual(1450, v_ActualScore);

            v_ActualScore = (int)EloCalculator.CalcProvisionalVsProvisionalRanking(1500, 1, 1500, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Stalemate]);
            Assert.AreEqual(1500, v_ActualScore);

            Assert.Throws<ArgumentException>(() => EloCalculator.CalcProvisionalVsProvisionalRanking(1500, -1, 1500, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Win]));
            Assert.Throws<ArgumentException>(() => EloCalculator.CalcProvisionalVsProvisionalRanking(1500, 21, 1500, EloCalculator.ModifierProvisional[EloCalculator.Modifier.Win]));
        }

        [Test]
        public void CalculateStandings()
        {
            Player v_P1 = new Player("Player A", "pa", Player.Genders.Male);
            Player v_P2 = new Player("Player B", "pb", Player.Genders.Male);
            Player v_P3 = new Player("Player C", "pc", Player.Genders.Male);
            Player v_P4 = new Player("Player D", "pd", Player.Genders.Male);

            ObservableCollection<Score> v_Scores = new ObservableCollection<Score>
            {
                { new Score(v_P1.Id, "10", true) },
                { new Score(v_P2.Id, "6", false) },
                { new Score(v_P3.Id, "6", false) },
                { new Score(v_P4.Id, "3", false) },
            };

            Result v_Result = new Result(new Guid(), v_Scores, new DateTime(), new Guid());
            v_Result.Init();

            Dictionary<Modifier, List<Guid>> v_StandingResult1 = v_Result.CalculateStandings(v_P1.Id);
            Assert.AreEqual(new List<Guid>(), v_StandingResult1[Modifier.Stalemate]);
            Assert.AreEqual(new List<Guid>(), v_StandingResult1[Modifier.Loose]);
            Assert.AreEqual(v_P2.Id, v_StandingResult1[Modifier.Win][0]);
            Assert.AreEqual(v_P3.Id, v_StandingResult1[Modifier.Win][1]);
            Assert.AreEqual(v_P4.Id, v_StandingResult1[Modifier.Win][2]);

            Dictionary<Modifier, List<Guid>> v_StandingResult2 = v_Result.CalculateStandings(v_P2.Id);
            Assert.AreEqual(v_P1.Id, v_StandingResult2[Modifier.Loose][0]);
            Assert.AreEqual(v_P3.Id, v_StandingResult2[Modifier.Stalemate][0]);
            Assert.AreEqual(v_P4.Id, v_StandingResult2[Modifier.Win][0]);

            Dictionary<Modifier, List<Guid>> v_StandingResult3 = v_Result.CalculateStandings(v_P3.Id);
            Assert.AreEqual(v_P1.Id, v_StandingResult3[Modifier.Loose][0]);
            Assert.AreEqual(v_P2.Id, v_StandingResult3[Modifier.Stalemate][0]);
            Assert.AreEqual(v_P4.Id, v_StandingResult3[Modifier.Win][0]);

            Dictionary<Modifier, List<Guid>> v_StandingResult4 = v_Result.CalculateStandings(v_P4.Id);
            Assert.AreEqual(new List<Guid>(), v_StandingResult4[Modifier.Stalemate]);
            Assert.AreEqual(new List<Guid>(), v_StandingResult4[Modifier.Win]);
            Assert.AreEqual(v_P1.Id, v_StandingResult4[Modifier.Loose][0]);
            Assert.AreEqual(v_P2.Id, v_StandingResult4[Modifier.Loose][1]);
            Assert.AreEqual(v_P3.Id, v_StandingResult4[Modifier.Loose][2]);

            foreach (KeyValuePair<Modifier, List<Guid>> i_Kvp in v_StandingResult4)
            {
                foreach (Guid i_PlayerID in i_Kvp.Value)
                {
                    Assert.AreNotEqual(v_P4.Id, i_PlayerID);
                }
            }
        }
    }
}
