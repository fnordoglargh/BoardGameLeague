using System;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace BoardGameLeagueLib
{
    public static class DbLoader
    {
        private static ILog m_Logger = LogManager.GetLogger("DbLoader");

        public static object ReadWithXmlSerializer(string a_FileName, Type a_Type)
        {
            XmlSerializer v_Serializer = new XmlSerializer(a_Type);
            object v_ObjectStructure = null;

            m_Logger.Debug(String.Format("Loading [{0}] of type [{1}].",a_FileName,a_Type));

            try
            {
                XmlReader reader = XmlReader.Create(a_FileName);
                v_ObjectStructure = v_Serializer.Deserialize(reader);
                reader.Close();

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
