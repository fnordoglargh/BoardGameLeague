using System;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using BoardGameLeagueLib.DbClasses;

namespace BoardGameLeagueLib
{
    public static class DbLoader
    {
        private static ILog m_Logger = LogManager.GetLogger("DbLoader");

        /// <summary>
        /// Deserializes the BoardgameLeagueDatabase.
        /// </summary>
        /// <param name="a_FilePathName">Path and name of the XML file to deserialize.</param>
        ///  <returns>Returns the DB as a BglDb instance. It will be null in case of errors (which is
        /// pretty unrecoverable).</returns>
        public static BglDb LoadDatabase(string a_FilePathName)
        {
            XmlSerializer v_Serializer = new XmlSerializer(typeof(BglDb));
            BglDb v_BglDataBase = null;

            try
            {
                XmlReader v_Reader = XmlReader.Create(a_FilePathName);
                v_BglDataBase = (BglDb)v_Serializer.Deserialize(v_Reader);
                v_Reader.Close();
            }
            catch(Exception ex)
            {
                m_Logger.Fatal("Loading of database was not successful." , ex);
            }

            return v_BglDataBase;
        }

        /// <summary>
        /// Writes the BoardGameLeage database to disk.
        /// </summary>
        /// <param name="a_BglDbInstance">BglDb instance to serialze into an XML file.</param>
        /// <param name="a_FilePathName">Path and name of the XML file to serialize.</param>
        /// <returns></returns>
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

        public static bool WriteWithXmlSerializer(string a_FileName,  object a_ObjectStructure)
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
                m_Logger.Fatal(ex.Message+Environment.NewLine+ex.StackTrace);
                v_IsSaved = false;
            }

            return v_IsSaved;
        }
    }
}
