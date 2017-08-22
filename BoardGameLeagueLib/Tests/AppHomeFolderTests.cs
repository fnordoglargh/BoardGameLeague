using log4net;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoardGameLeagueLib.Helpers.Tests
{
    class AppHomeFolderTests
    {
        String m_TestFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "BglTest" + Path.DirectorySeparatorChar;
        private ILog m_Logger = LogManager.GetLogger("ResultIdToPlayerResultConverterTests");

        [Test]
        public void InitEnvironmentTest()
        {
            AppHomeFolder.CreationResults v_BootStrapResult = StandardFileBootstrapper.BootstrapWrapper();
            bool v_IsCreatedOrCopied = v_BootStrapResult == AppHomeFolder.CreationResults.Exists || v_BootStrapResult == AppHomeFolder.CreationResults.Copied;
            Assert.IsTrue(v_IsCreatedOrCopied);

            m_Logger = LogManager.GetLogger("AppHomeFolderTests");

            // First delete all files and dirs from earlier test run (if there are any).
            if (Directory.Exists(m_TestFilePath))
            {
                Directory.Delete(m_TestFilePath, true);
            }

            // First call creates the directory.
            AppHomeFolder.CreationResults v_Result = AppHomeFolder.TestAndCreateHomeFolder(m_TestFilePath);
            Assert.AreEqual(AppHomeFolder.CreationResults.Created, v_Result);

            // Second call reports existing directory.
            v_Result = AppHomeFolder.TestAndCreateHomeFolder(m_TestFilePath);
            Assert.AreEqual(AppHomeFolder.CreationResults.Exists, v_Result);

            m_Logger.Debug(VersionWrapper.NameExecuting);

            List<String> v_FilesToCopy = new List<string>() { "log4netConfig.xml" };

            // Copy the file to the empty folder.
            List<AppHomeFolder.CreationResults> v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(v_FilesToCopy, m_TestFilePath);
            Assert.AreEqual(new List<AppHomeFolder.CreationResults>() { AppHomeFolder.CreationResults.Copied }, v_ResultsFromResourceCopy);

            // Calling the same again will tell us that the file was already there.
            v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(v_FilesToCopy, m_TestFilePath);
            Assert.AreEqual(new List<AppHomeFolder.CreationResults>() { AppHomeFolder.CreationResults.Exists }, v_ResultsFromResourceCopy);

            // Files which don't exist will report an error.
            v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(new List<string>() { "invalidFileName.txt", "log4netConfig.xml", "i3.txt" }, m_TestFilePath);
            Assert.AreEqual(new List<AppHomeFolder.CreationResults>() { AppHomeFolder.CreationResults.Error, AppHomeFolder.CreationResults.Exists, AppHomeFolder.CreationResults.Error }, v_ResultsFromResourceCopy);
        }
    }
}
