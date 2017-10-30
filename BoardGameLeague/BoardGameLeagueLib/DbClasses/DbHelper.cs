using BoardGameLeagueLib.Helpers;
using log4net;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    public sealed class DbHelper
    {
        private static ILog m_Logger = LogManager.GetLogger("DbHelper");
        private DbHelper() { m_Logger.Debug("Private ctor DbHelper"); }
        private static readonly Lazy<DbHelper> lazy = new Lazy<DbHelper>(() => new DbHelper());
        public static DbHelper Instance { get { return lazy.Value; } }
        public BglDb LiveBglDb { get; private set; }
        public bool IsChanged { get; set; }
        public const String c_StandardDbName = "bgldb.xml";

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

        /// <summary>
        /// Loads database from %APPDATA%\BoardGameLeague.
        /// </summary>
        /// <returns>True if loading was successful.</returns>
        public bool LoadStandardDb()
        {
            return LoadDataBase(StandardPath + c_StandardDbName);
        }

        public bool LoadDataBase(string a_FilePathName)
        {
            LiveBglDb = LoadDatabase(a_FilePathName);

            if (LiveBglDb != null)
            {
                return true;
            }

            return false;
        }

        public bool LoadDataBaseAndRepopulate(string a_FilePathName)
        {
            BglDb v_TempDatabase= LoadDatabase(a_FilePathName);

            LiveBglDb.Results.Clear();
            LiveBglDb.Locations.Clear();
            LiveBglDb.GameFamilies.Clear();
            LiveBglDb.Games.Clear();
            LiveBglDb.Players.Clear();

            foreach(Player i_Player in v_TempDatabase.Players)
            {
                LiveBglDb.Players.Add(i_Player);
            }

            foreach (GameFamily i_GameFamily in v_TempDatabase.GameFamilies)
            {
                LiveBglDb.GameFamilies.Add(i_GameFamily);
            }

            foreach (Game i_Game in v_TempDatabase.Games)
            {
                LiveBglDb.Games.Add(i_Game);
            }

            foreach (Location i_Location in v_TempDatabase.Locations)
            {
                LiveBglDb.Locations.Add(i_Location);
            }

            foreach(Result i_Result in v_TempDatabase.Results)
            {
                LiveBglDb.Results.Add(i_Result);
                i_Result.Init();
            }

            LiveBglDb.Init();

            return false;
        }

        /// <summary>
        /// Deserializes the BoardgameLeagueDatabase and copies a backup of the database file.
        /// </summary>
        /// <param name="a_FilePathName">Path and name of the XML file to deserialize.</param>
        /// <returns>Returns the DB as a BglDb instance. It will be null in case of errors (which is
        /// pretty unrecoverable).</returns>
        private static BglDb LoadDatabase(string a_FilePathName)
        {
            XmlSerializer v_Serializer = new XmlSerializer(typeof(BglDb));
            BglDb v_BglDataBase = null;

            try
            {
                m_Logger.Info(String.Format("Calling from [{0}].", Directory.GetCurrentDirectory()));
                m_Logger.Info(String.Format("Loading database [{0}].", a_FilePathName));
                XmlReader v_Reader = XmlReader.Create(a_FilePathName);
                v_BglDataBase = (BglDb)v_Serializer.Deserialize(v_Reader);
                v_Reader.Close();
                v_BglDataBase.Init();

                // Get the file name so that we can add a timestamp between name and file extension.
                int v_LastDotPosition = a_FilePathName.LastIndexOf('.');
                String v_BackupFileName = "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                if (v_LastDotPosition != -1)
                {
                    String v_FileNameStart = a_FilePathName.Substring(0, v_LastDotPosition);
                    String v_FileNameEnd = a_FilePathName.Substring(v_LastDotPosition);
                    v_BackupFileName = v_FileNameStart + v_BackupFileName + v_FileNameEnd;
                }
                else
                {
                    v_BackupFileName = a_FilePathName + v_BackupFileName;
                }

                // Prepare to create a the backup directory.
                int v_LastDirectorySeperatorPosition = v_BackupFileName.LastIndexOf(Path.DirectorySeparatorChar);
                String v_PathToBackupDirectory;
                String v_BackupFolderName = "backup";

                if (v_LastDirectorySeperatorPosition == -1)
                {
                    v_BackupFileName = v_BackupFolderName + Path.DirectorySeparatorChar + v_BackupFileName;
                    v_PathToBackupDirectory = v_BackupFolderName;
                }
                else
                {
                    v_PathToBackupDirectory = a_FilePathName.Substring(0, v_LastDirectorySeperatorPosition + 1) + v_BackupFolderName;
                    v_BackupFileName = v_PathToBackupDirectory + v_BackupFileName.Substring(v_LastDirectorySeperatorPosition);
                }

                try
                {
                    Directory.CreateDirectory(v_PathToBackupDirectory);
                    File.Copy(a_FilePathName, v_BackupFileName);
                    m_Logger.Info(String.Format("Wrote backup of [{0}] as [{1}].", a_FilePathName, v_BackupFileName));
                }
                catch (Exception ex)
                {
                    m_Logger.Error(String.Format("Backing up the file [{0}] as [{1}] failed.", a_FilePathName, v_BackupFileName), ex);
                }
            }
            catch (Exception ex)
            {
                m_Logger.Fatal("Loading of database was not successful.", ex);
            }

            return v_BglDataBase;
        }

        public static bool WriteStandardDatabase(BglDb a_BglDbInstance)
        {
            return WriteDatabase(a_BglDbInstance, StandardPath + c_StandardDbName);
        }

        /// <summary>
        /// Writes the BoardGameLeage database to disk.
        /// </summary>
        /// <param name="a_BglDbInstance">BglDb instance to serialze into an XML file.</param>
        /// <param name="a_FilePathName">Path and name of the XML file to serialize.</param>
        /// <returns>True if the passed BglDb instance was saved.</returns>
        public static bool WriteDatabase(BglDb a_BglDbInstance, string a_FilePathName)
        {
            XmlSerializer v_Serializer = new XmlSerializer(typeof(BglDb));
            bool v_IsSaved = true;
            m_Logger.Debug(String.Format("Saving [{0}].", a_FilePathName));

            try
            {
                XmlWriterSettings v_Settings = new XmlWriterSettings();
                v_Settings.Indent = true;
                XmlWriter v_Writer = XmlWriter.Create(a_FilePathName, v_Settings);
                v_Serializer.Serialize(v_Writer, a_BglDbInstance);
                v_Writer.Close();
                m_Logger.Info(String.Format("Saved database as [{0}].", a_FilePathName));
                Instance.IsChanged = false;
            }
            catch (Exception ex)
            {
                m_Logger.Fatal(ex.Message + Environment.NewLine + ex.StackTrace);
                v_IsSaved = false;
            }

            return v_IsSaved;
        }

        /// <summary>
        /// Deserializes an XML file into an collection of the given type.
        /// </summary>
        /// <param name="a_FilePathName">Path and name of the XML file to serialize.</param>
        /// <param name="a_Type">Given type can basically serialize anything. Type should follow the scheme typeof(ObservableCollection&lt;T&gt;).
        /// </param>
        /// <returns>Returns an object that must be cast into an ObservableCollection&lt;T&gt;. It will be null in case of errors (which is
        /// pretty unrecoverable).</returns>
        public static object ReadWithXmlSerializer(string a_FilePathName, Type a_Type)
        {
            XmlSerializer v_Serializer = new XmlSerializer(a_Type);
            object v_ObjectStructure = null;

            m_Logger.Debug(String.Format("Loading [{0}] of type [{1}].", a_FilePathName, a_Type));

            try
            {
                XmlReader reader = XmlReader.Create(a_FilePathName);
                v_ObjectStructure = v_Serializer.Deserialize(reader);
                reader.Close();
            }
            catch (System.IO.FileNotFoundException fno)
            {
                m_Logger.Fatal("Unable to load file " + a_FilePathName, fno);
            }
            catch (Exception ex)
            {
                m_Logger.Fatal(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return v_ObjectStructure;
        }

        public static bool WriteWithXmlSerializer(string a_FileName, object a_ObjectStructure)
        {
            Type v_Type = a_ObjectStructure.GetType();
            XmlSerializer v_Serializer = new XmlSerializer(v_Type);
            bool v_IsSaved = true;
            m_Logger.Debug(String.Format("Saving [{0}] of type [{1}].", a_FileName, a_ObjectStructure.GetType()));

            try
            {
                XmlWriterSettings v_Settings = new XmlWriterSettings();
                v_Settings.Indent = true;
                XmlWriter v_Writer = XmlWriter.Create(a_FileName, v_Settings);
                v_Serializer.Serialize(v_Writer, a_ObjectStructure);
                v_Writer.Close();
            }
            catch (Exception ex)
            {
                m_Logger.Fatal(ex.Message + Environment.NewLine + ex.StackTrace);
                v_IsSaved = false;
            }

            return v_IsSaved;
        }
    }
}
