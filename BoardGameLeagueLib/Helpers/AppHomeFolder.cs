using System;
using System.IO;
using log4net;
using System.Collections.Generic;
using System.Reflection;

namespace BoardGameLeagueLib.Helpers
{
    public static class AppHomeFolder
    {
        static ILog m_Logger = LogManager.GetLogger("AppHomeFolder");

        public enum CreationResults
        {
            Created,
            Exists,
            Copied,
            Error
        }

        public static CreationResults TestAndCreateHomeFolder(string a_Path)
        {
            CreationResults v_ActualResult = CreationResults.Error;
            bool v_IsFolderOk = Directory.Exists(a_Path);

            if (!v_IsFolderOk)
            {
                try
                {
                    m_Logger.Debug(String.Format("Creating folder in path [{0}].", a_Path));
                    Directory.CreateDirectory(a_Path);
                    v_ActualResult = CreationResults.Created;
                }
                catch (Exception e)
                {
                    m_Logger.Fatal(String.Format("Creating the homefolder in [{0}] was NOT successful!", a_Path), e);
                    v_ActualResult = CreationResults.Error;
                }
            }
            else
            {
                v_ActualResult = CreationResults.Exists;
            }

            return v_ActualResult;
        }

        public static String GetHomeFolderPath(string a_CompanyName, string a_ApplicationName)
        {
            string v_Path = string.Empty;

            try
            {
                v_Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar;

                if (a_CompanyName != string.Empty)
                {
                    v_Path += a_CompanyName + System.IO.Path.DirectorySeparatorChar;
                }

                v_Path += a_ApplicationName + System.IO.Path.DirectorySeparatorChar;
                m_Logger.Debug(String.Format("Generated path [{0}].", v_Path));
            }
            catch (Exception e)
            {
                m_Logger.Fatal("Getting the path was NOT successful!" + Environment.NewLine + e.Message);
                m_Logger.Fatal(" a_CompanyName    : [" + a_CompanyName + "]");
                m_Logger.Fatal(" a_ApplicationName: [" + a_ApplicationName + "]");
            }

            return v_Path;
        }

        public static CreationResults CreateHomeFolderPath()
        {
            String v_GeneratedPath = GetHomeFolderPath(VersionWrapper.CompanyExecuting, VersionWrapper.NameExecuting);

            if (v_GeneratedPath != String.Empty)
            {
                return TestAndCreateHomeFolder(v_GeneratedPath);
            }

            return CreationResults.Error;
        }

        public static List<CreationResults> CopyStaticResources(List<String> a_ResourcePaths, String a_Location)
        {
            // TODO: Test validity of a_Location.

            String v_AssemblyName = VersionWrapper.NameExecuting + ".";
            //String v_AssemblyName = VersionWrapper.CompanyExecuting + "." + VersionWrapper.NameExecuting + ".";
            Assembly v_Assembly = Assembly.GetExecutingAssembly();

            m_Logger.Debug("Reading from: " + v_AssemblyName);
            m_Logger.Debug("All available assembly resources:");

            foreach (string i_ResourceName in v_Assembly.GetManifestResourceNames())
            {
                m_Logger.Debug("  - " + i_ResourceName);
            }

            // TODO: Change output (it's not just the one file).
            m_Logger.Debug("Trying to write files here: " + a_Location);

            List<CreationResults> v_Results = new List<CreationResults>();

            foreach (String i_FileToLoad in a_ResourcePaths)
            {
                try
                {
                    m_Logger.Debug("Trying to read: " + v_AssemblyName + "DefaultFiles." + i_FileToLoad);
                    if (File.Exists(a_Location + i_FileToLoad))
                    {
                        v_Results.Add(CreationResults.Exists);
                    }
                    else
                    {
                        StreamReader v_TextStreamReader = new StreamReader(v_Assembly.GetManifestResourceStream(v_AssemblyName + "DefaultFiles." + i_FileToLoad));
                        StreamWriter v_File = new StreamWriter(a_Location + i_FileToLoad);
                        String v_ConfigFile = v_TextStreamReader.ReadToEnd();
                        v_File.WriteLine(v_ConfigFile);
                        v_Results.Add(CreationResults.Copied);
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.Fatal("CopyStaticResources failed!", ex);
                    v_Results.Add(CreationResults.Error);
                }
            }

            return v_Results;
        }
    }
}
