using NUnit.Framework;
using System;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameLeagueLib.DbClasses;
using log4net;
using log4net.Config;
using System.IO;
using BoardGameLeagueLib.Helpers;

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
            StandardFileBootstrapper.BootstrapWrapper();
            m_Logger = LogManager.GetLogger("ResultIdToPlayerResultConverterTests");

            DbHelper v_DbHelper = DbHelper.Instance;
            //bool v_IsDbLoaded = v_DbHelper.LoadDataBase("C:\\devel\\BoardGameLeague\\BoardGameLeagueUI2\\bin\\Debug\\bgldb.xml");
            bool v_IsDbLoaded = v_DbHelper.LoadDataBase("BoardGameLeague\\BoardGameLeagueUI2\\bin\\Debug\\bgldb.xml");
            Assert.IsTrue(v_IsDbLoaded);
            Assert.IsNotNull(v_DbHelper.LiveBglDb);

            m_Logger.Info("Db Loaded.");
            System.Diagnostics.Debug.WriteLine("Database is is loaded. Starting real tests.");

            Guid v_PlayerId = new Guid("cde86cad-d719-4a88-bf5c-1e5be87eff35");
            object v_PlayerConverted = m_ConverterObject.Convert(v_PlayerId, null, null, null);
            //Player v_PlayerToTest = v_DbHelper.LiveBglDb.PlayersById[v_PlayerId];
            ////Assert.IsNotNull(v_PlayerToTest);


            //Assert.AreEqual(v_PlayerConverted, v_PlayerToTest);
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

        [Test]
        public void ConvertBackValidPlayer()
        {

        }
    }
}
