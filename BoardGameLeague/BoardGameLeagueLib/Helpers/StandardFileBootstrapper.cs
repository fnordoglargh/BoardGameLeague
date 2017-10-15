using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoardGameLeagueLib.Helpers
{
    public static class StandardFileBootstrapper
    {
        /// <summary>
        /// This function will create %APPDATA%\ProductName and copy the logging configuration if it does not exist.
        /// </summary>
        /// <returns>A AppHomeFolder.CreationResults element. Copied if the file was not in the expected folder, 
        /// Exists if the file was already there and Error if something went wrong.</returns>
        public static List<AppHomeFolder.CreationResults> BootstrapWrapper()
        {
            String v_PathToStandardFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                + Path.DirectorySeparatorChar 
                + VersionWrapper.NameCalling
                + Path.DirectorySeparatorChar;
            AppHomeFolder.CreationResults v_Result = AppHomeFolder.TestAndCreateHomeFolder(v_PathToStandardFolder);
            List<String> v_FilesToCopy = new List<string>() { "log4netConfig", "bgldb.xml" };
            List<AppHomeFolder.CreationResults> v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(v_FilesToCopy, v_PathToStandardFolder);
            Console.WriteLine("BootstrapWrapper result: " + v_ResultsFromResourceCopy[0]);
            XmlConfigurator.Configure(new FileInfo(v_PathToStandardFolder + "log4netConfig"));
            LogManager.GetLogger("StandardFileBootstrapper").Info("Bootstrapper successfully started the logger: " + v_ResultsFromResourceCopy[0]);

            return v_ResultsFromResourceCopy;
        }

        public static List<AppHomeFolder.CreationResults> BootstrapWrapperForTests()
        {
            String v_PathToStandardFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData) 
                + Path.DirectorySeparatorChar 
                + VersionWrapper.NameCalling
                + "Test"
                + Path.DirectorySeparatorChar;
            AppHomeFolder.CreationResults v_Result = AppHomeFolder.TestAndCreateHomeFolder(v_PathToStandardFolder);
            List<String> v_FilesToCopy = new List<string>() { "log4netConfig", "TestEmptyDbPlayer.xml" };
            List<AppHomeFolder.CreationResults> v_ResultsFromResourceCopy = AppHomeFolder.CopyStaticResources(v_FilesToCopy, v_PathToStandardFolder);
            Console.WriteLine("BootstrapWrapperForTests results:");
            
            foreach(AppHomeFolder.CreationResults i_Result in v_ResultsFromResourceCopy)
            {
                Console.WriteLine("  * " + i_Result);
            }

            XmlConfigurator.Configure(new FileInfo(v_PathToStandardFolder + "log4netConfig"));
            LogManager.GetLogger("BootstrapWrapperForTests").Info("BootstrapWrapperForTests successfully started the logger.");


            return new List<AppHomeFolder.CreationResults> { AppHomeFolder.CreationResults.Error };
        }
    }
}
