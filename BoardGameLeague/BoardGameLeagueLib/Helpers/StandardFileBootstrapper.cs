using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

            foreach (AppHomeFolder.CreationResults i_Result in v_ResultsFromResourceCopy)
            {
                Console.WriteLine("  * " + i_Result);
            }

            XmlConfigurator.Configure(new FileInfo(v_PathToStandardFolder + "log4netConfig"));
            LogManager.GetLogger("BootstrapWrapperForTests").Info("BootstrapWrapperForTests successfully started the logger.");


            return new List<AppHomeFolder.CreationResults> { AppHomeFolder.CreationResults.Error };
        }

        /// <summary>
        /// Wraps creation of new and empty databases to the specified location and filename.
        /// </summary>
        /// <param name="a_FileName">Path and name to the new database.</param>
        /// <returns>Always returns CreationResults.Created. We don't expect it to go wrong.</returns>
        public static AppHomeFolder.CreationResults WriteEmptyDatabase(String a_FileName)
        {
            Assembly v_Assembly = Assembly.GetExecutingAssembly();
            List<String> v_EmbeddedResourceNames = new List<string>(v_Assembly.GetManifestResourceNames());
            int v_IndexInAssembly = v_EmbeddedResourceNames.FindIndex(x => x.Contains("bgldb.xml"));
            String v_PathToReadFrom = v_EmbeddedResourceNames[v_IndexInAssembly];
            StreamReader v_TextStreamReader = new StreamReader(v_Assembly.GetManifestResourceStream(v_PathToReadFrom));
            StreamWriter v_File = new StreamWriter(a_FileName);
            String v_ConfigFile = v_TextStreamReader.ReadToEnd();
            v_File.WriteLine(v_ConfigFile);
            v_File.Close();
            v_TextStreamReader.Close();

            return AppHomeFolder.CreationResults.Created;
        }
    }
}
