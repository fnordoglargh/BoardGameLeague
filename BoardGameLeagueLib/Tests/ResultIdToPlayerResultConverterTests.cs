using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BoardGameLeagueLib.Tests
{
    [TestFixture]
    class ResultIdToPlayerResultConverterTests
    {
        ResultIdToPlayerResultConverter m_ConverterObject = new ResultIdToPlayerResultConverter();

        private ILog m_Logger;

        [Test]
        public void ConvertValidPlayer()
        {
            Console.WriteLine("Starting ConvertValidPlayer");
            List<AppHomeFolder.CreationResults> v_BootStrapResult = StandardFileBootstrapper.BootstrapWrapperForTests();
            m_Logger = LogManager.GetLogger("ResultIdToPlayerResultConverterTests");

            DbHelper v_DbHelper = DbHelper.Instance;
            String v_TestFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                + Path.DirectorySeparatorChar 
                + "BoardGameLeagueTest" 
                + Path.DirectorySeparatorChar 
                + "TestEmptyDbPlayer.xml";

            m_Logger.Debug("Trying to load file from: " + v_TestFilePath);

            // Test if the database is loaded and working.
            bool v_IsDbLoaded = v_DbHelper.LoadDataBase(v_TestFilePath);
            Assert.IsTrue(v_IsDbLoaded);
            Assert.IsNotNull(v_DbHelper.LiveBglDb);
            m_Logger.Debug("Database is loaded. Starting real tests.");

            // Create a new Player and add to DB.
            Player v_PlayerToTest = new Player("A", "B", Player.Genders.Male);
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
