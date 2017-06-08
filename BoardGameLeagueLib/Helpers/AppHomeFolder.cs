using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BoardGameLeagueLib.Helpers
{
    public static class AppHomeFolder
    {
        static ILog m_Logger = LogManager.GetLogger("AppHomeFolder");

        public enum CreationResults
        {
            /// <summary>File or folder was created.</summary>
            Created,
            /// <summary>File or folder already exists.</summary>
            Exists,
            /// <summary>File was successfully copied.</summary>
            Copied,
            /// <summary>Some error occured. Check the logs.</summary>
            Error = -1
        }

        /// <summary>
        /// Determines if a folder already exists and will create it if it doesn't.
        /// </summary>
        /// <param name="a_Path">Path to be tested or created.</param>
        /// <returns>Created if the folder was not on the disk before, 
        /// Exists if the folder was already on the disk, 
        /// Error if an exception occurred (check the logs).</returns>
        public static CreationResults TestAndCreateHomeFolder(string a_Path)
        {
            CreationResults v_ActualResult = CreationResults.Error;
            bool v_IsFolderOnDisk = Directory.Exists(a_Path);

            if (!v_IsFolderOnDisk)
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
                v_Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar;

                if (a_CompanyName != string.Empty)
                {
                    v_Path += a_CompanyName + Path.DirectorySeparatorChar;
                }

                v_Path += a_ApplicationName + Path.DirectorySeparatorChar;
                m_Logger.Debug(String.Format("Generated path [{0}].", v_Path));
            }
            catch (Exception e)
            {
                v_Path = string.Empty;
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

        /// <summary>
        /// Functions tries to write all files from a given list to a location.
        /// </summary>
        /// <param name="a_ResourcePaths">A list of file names which shall be copied from the embedded resources.</param>
        /// <param name="a_Location">Path to copy the files to.</param>
        /// <returns>A list of results which has the same order as the file list.</returns>
        public static List<CreationResults> CopyStaticResources(List<String> a_ResourcePaths, String a_Location)
        {
            if (!a_Location.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                a_Location += Path.DirectorySeparatorChar;
            }

            String v_AssemblyName = VersionWrapper.NameExecuting + ".";
            Assembly v_Assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine("Reading from: " + v_AssemblyName);
            Console.WriteLine("All available assembly resources:");

            List<String> v_EmbeddedResourceNames = new List<string>(v_Assembly.GetManifestResourceNames());

            foreach (string i_ResourceName in v_EmbeddedResourceNames)
            {
                Console.WriteLine("  - " + i_ResourceName);
            }

            Console.WriteLine("Trying to write files here: " + a_Location);
            List<CreationResults> v_Results = new List<CreationResults>();

            foreach (String i_FileToLoad in a_ResourcePaths)
            {
                try
                {
                    // Is the given file name part of the embedded resources?
                    int v_IndexInAssembly = v_EmbeddedResourceNames.FindIndex(x => x.Contains(i_FileToLoad));

                    if (v_IndexInAssembly > -1)
                    {
                        // Looks like BoardGameLeagueLib.DefaultFiles.i_FileToLoad.
                        String v_PathToReadFrom = v_EmbeddedResourceNames[v_IndexInAssembly];
                        Console.WriteLine("Trying to read: " + v_PathToReadFrom);

                        if (File.Exists(a_Location + i_FileToLoad))
                        {
                            Console.WriteLine(String.Format("CopyStaticResources found that file [{0}] already exists.", i_FileToLoad));
                            v_Results.Add(CreationResults.Exists);
                        }
                        else
                        {
                            StreamReader v_TextStreamReader = new StreamReader(v_Assembly.GetManifestResourceStream(v_PathToReadFrom));
                            StreamWriter v_File = new StreamWriter(a_Location + i_FileToLoad);
                            String v_ConfigFile = v_TextStreamReader.ReadToEnd();
                            v_File.WriteLine(v_ConfigFile);
                            v_File.Close();
                            v_TextStreamReader.Close();
                            Console.WriteLine(String.Format("CopyStaticResources wrote assembly file [{0}].", i_FileToLoad));
                            v_Results.Add(CreationResults.Copied);
                        }
                    }
                    else
                    {
                        Console.WriteLine(String.Format("CopyStaticResources failed for [{0}]!", i_FileToLoad));
                        v_Results.Add(CreationResults.Error);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("CopyStaticResources failed for [{0}]!", i_FileToLoad));
                    Console.WriteLine(ex.Message);
                    v_Results.Add(CreationResults.Error);
                }
            }

            return v_Results;
        }
    }
}
