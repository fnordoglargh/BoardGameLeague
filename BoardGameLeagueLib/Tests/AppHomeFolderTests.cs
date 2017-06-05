using BoardGameLeagueLib.Helpers;
using log4net;
using log4net.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameLeagueLib.Helpers.Tests
{
    class AppHomeFolderTests
    {
        String m_TestFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "BglTest" + Path.DirectorySeparatorChar;
        String m_TestFileName = "AppHomeTestFile{0}.txt";
        private ILog m_Logger = LogManager.GetLogger("ResultIdToPlayerResultConverterTests");
        List<String> m_TestFilePaths = new List<String>();

        private bool InitEnvironment()
        {
            bool v_IsEverythingOk = true;

            // TODO: Correct this fragile shit.
            XmlConfigurator.Configure(new FileInfo("C:\\devel\\BoardGameLeague\\BoardGameLeagueUI2\\bin\\Debug\\conf\\log4netConfig.xml"));
            m_Logger = LogManager.GetLogger("ResultIdToPlayerResultConverterTests");
            m_Logger.Debug("Initing File Names");

            for (int i = 0; i < 10; i++)
            {
                String v_FileName = String.Format(m_TestFileName, i);
                m_Logger.Debug("  " + v_FileName);
                m_TestFilePaths.Add(m_TestFilePath + v_FileName);
            }

            try
            {
                m_Logger.Debug("Deleting and Creating Files:");

                foreach (String i_TestFilePath in m_TestFilePaths)
                {
                    File.Delete(i_TestFilePath);
                    m_Logger.Debug("  " + i_TestFilePath);
                    String v_RandomText = "**** Start Random Guids:" + Environment.NewLine + Guid.NewGuid() + Environment.NewLine + Guid.NewGuid() + Environment.NewLine + Guid.NewGuid() + Environment.NewLine + "**** End Random Guids";
                    File.AppendAllText(i_TestFilePath, v_RandomText);
                }
            }
            catch (Exception)
            {
                v_IsEverythingOk = false;
            }

            return v_IsEverythingOk;
        }

        [Test]
        public void InitEnvironmentTest()
        {
            AppHomeFolder.CreationResults v_BootStrapResult = StandardFileBootstrapper.BootstrapWrapper();
            bool v_IsCreatedOrCopied = v_BootStrapResult == AppHomeFolder.CreationResults.Created || v_BootStrapResult == AppHomeFolder.CreationResults.Copied;
            Assert.IsTrue(v_IsCreatedOrCopied);

            // First delete all files and dirs from earlier test run.
            Directory.Delete(m_TestFilePath, true);

            // First call creates the directory.
            AppHomeFolder.CreationResults v_Result = AppHomeFolder.TestAndCreateHomeFolder(m_TestFilePath);
            Assert.AreEqual(AppHomeFolder.CreationResults.Created, v_Result);

            // Second call reports existing directory.
            v_Result = AppHomeFolder.TestAndCreateHomeFolder(m_TestFilePath);
            Assert.AreEqual(AppHomeFolder.CreationResults.Exists, v_Result);

            // Create temporary test files.
            //Assert.IsTrue(InitEnvironment());

            m_Logger.Debug(VersionWrapper.NameExecuting);

            List<String> v_FilesToCopy = new List<string>() { "log4netConfig.xml" };

            List<AppHomeFolder.CreationResults> v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(v_FilesToCopy, m_TestFilePath);
            Assert.AreEqual(new List<AppHomeFolder.CreationResults>() { AppHomeFolder.CreationResults.Copied }, v_ResultsFromResourceCopy);

            v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(v_FilesToCopy, m_TestFilePath);
            Assert.AreEqual(new List<AppHomeFolder.CreationResults>() { AppHomeFolder.CreationResults.Exists }, v_ResultsFromResourceCopy);

            v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(new List<string>() { "invalidFileName.txt" }, m_TestFilePath);
            Assert.AreEqual(new List<AppHomeFolder.CreationResults>() { AppHomeFolder.CreationResults.Error }, v_ResultsFromResourceCopy);
        }
    }
}
