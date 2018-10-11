using BoardGameLeagueLib.DbClasses;
using log4net;
using System;
using System.IO;

namespace BoardGameLeagueLib.Helpers
{
    public sealed class SettingsHelper
    {
        private static ILog m_Logger = LogManager.GetLogger("Settings");
        private static readonly Lazy<SettingsHelper> m_SettingsHelper = new Lazy<SettingsHelper>(() => new SettingsHelper());

        public static SettingsHelper Instance { get { return m_SettingsHelper.Value; } }
        public Settings m_Preferences;
        public Settings Preferences { get { return m_Preferences; } }

        private SettingsHelper()
        {
            LoadSettings();
        }

        public Settings LoadSettings()
        {
            m_Preferences = (Settings)DbHelper.ReadWithXmlSerializer(Settings.SettingsPath, typeof(Settings));

            if (m_Preferences == null)
            {
                m_Preferences = new Settings();
                m_Logger.Info("Settings were not loaded. We're starting with the default database.");
            }
            else
            {
                m_Logger.Debug("Settings loaded.");
            }

            return m_Preferences;
        }

        public bool SaveSettings()
        {
            bool v_IsSaved = DbHelper.WriteWithXmlSerializer(Settings.SettingsPath, m_Preferences);

            if (v_IsSaved)
            {
                m_Logger.Debug("Saved the settings.");
            }
            else
            {
                m_Logger.Warn("Settings were NOT saved.");
            }

            return v_IsSaved;
        }

        /// <summary>
        /// Gets the standard folder of bgl which points to %APPDATA%\BoardGameLeague.
        /// </summary>
        public static String StandardPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + Path.DirectorySeparatorChar
                    + VersionWrapper.NameCalling
                    + Path.DirectorySeparatorChar;
            }
        }

        public class Settings
        {
            public static readonly String SettingsPath = StandardPath + "settings.dat";

            /// <summary>
            /// Path to the database file which was last used.
            /// </summary>
            public String LastUsedDatabase { get; set; }

            public bool IsDateNormalized { get; set; }
            public bool IsGraphAreaTransparent { get; set; }

            public Settings()
            {
                IsDateNormalized = true;
                IsGraphAreaTransparent = true;
            }
        }
    }
}
