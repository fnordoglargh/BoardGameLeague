using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoardGameLeagueLib.Tests
{
    [SetUpFixture]
    public class BglSetUp
    {
        private ILog m_Logger;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            List<AppHomeFolder.CreationResults> v_BootStrapResult = StandardFileBootstrapper.BootstrapWrapperForTests();
            m_Logger = LogManager.GetLogger("BglSetUp");
            m_Logger.Info("Starting OneTimeSetup.");

            DbHelper v_DbHelper = DbHelper.Instance;
            String v_TestFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + Path.DirectorySeparatorChar
                + "BoardGameLeagueTest"
                + Path.DirectorySeparatorChar
                + "TestEmptyDbPlayer.xml";

            bool v_IsDbLoaded = v_DbHelper.LoadDataBase(v_TestFilePath);
            Assert.IsTrue(v_IsDbLoaded);
            Assert.IsNotNull(v_DbHelper.LiveBglDb);
            m_Logger.Debug("Database is loaded. Ready for executing tests.");
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            m_Logger.Info("End test execution with OneTimeTearDown.");
        }
    }
}
