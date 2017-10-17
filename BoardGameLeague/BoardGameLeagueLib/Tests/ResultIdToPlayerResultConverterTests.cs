using BoardGameLeagueLib.Converters;
using BoardGameLeagueLib.DbClasses;
using NUnit.Framework;
using System;

namespace BoardGameLeagueLib.Tests
{
    [TestFixture]
    class ResultIdToPlayerResultConverterTests
    {
        ResultIdToPlayerResultConverter m_ConverterObject = new ResultIdToPlayerResultConverter();

        [Test]
        public void ConvertValidPlayer()
        {
            DbHelper v_DbHelper = DbHelper.Instance;

            // Create a new Player and add to DB.
            Player v_PlayerToTest = new Player("A", Player.Genders.Male);
            v_DbHelper.LiveBglDb.Players.Add(v_PlayerToTest);
            Player v_PlayerConverted = (Player)m_ConverterObject.Convert(v_PlayerToTest.Id, null, null, null);

            Assert.AreEqual(v_PlayerConverted, v_PlayerToTest);

            Guid v_GuidToTest = (Guid)m_ConverterObject.ConvertBack(v_PlayerToTest, null, null, null);
            Assert.AreEqual(v_PlayerToTest.Id, v_GuidToTest);
        }

        [Test]
        public void ConvertInvalidPlayer()
        {
            Player v_PlayerConverted = (Player)m_ConverterObject.Convert("SomeInvalidInput", null, null, null);
            Assert.IsNull(v_PlayerConverted);

            v_PlayerConverted = (Player)m_ConverterObject.Convert(null, null, null, null);
            Assert.IsNull(v_PlayerConverted);

            v_PlayerConverted = (Player)m_ConverterObject.Convert(6, null, null, null);
            Assert.IsNull(v_PlayerConverted);

            v_PlayerConverted = (Player)m_ConverterObject.Convert(new Guid(), null, null, null);
            Assert.IsNull(v_PlayerConverted);
        }
    }
}
