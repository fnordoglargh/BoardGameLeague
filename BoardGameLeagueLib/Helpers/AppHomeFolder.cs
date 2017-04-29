using System;
using System.IO;
using log4net;

namespace BoardGameLeagueLib.Helpers
{
    public static class AppHomeFolder
    {
        static ILog m_Logger = LogManager.GetLogger("AppHomeFolder");

        public static bool TestHomeFolder(string a_Path)
        {
            bool v_IsFolderOk = Directory.Exists(a_Path);

            if (!v_IsFolderOk)
            {
                try
                {
                    Directory.CreateDirectory(a_Path);
                }
                catch (Exception e)
                {
                    m_Logger.Fatal("Creating the homefolder in [" + a_Path + "] was NOT successful!" + Environment.NewLine + e.Message);
                    v_IsFolderOk = false;
                }
            }

            return v_IsFolderOk;
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
            }
            catch (Exception e)
            {
                m_Logger.Fatal("Getting the path was NOT successful!" + Environment.NewLine + e.Message);
                m_Logger.Fatal(" a_CompanyName    : [" + a_CompanyName + "]");
                m_Logger.Fatal(" a_ApplicationName: [" + a_ApplicationName + "]");
            }

            return v_Path;
        }
    }
}
