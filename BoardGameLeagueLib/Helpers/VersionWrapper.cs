using System;
using System.Diagnostics;
using System.Reflection;

namespace BoardGameLeagueLib.Helpers
{
    /// <summary>
    /// Version class
    /// used to read verison number from assembly
    /// </summary>
    public class VersionWrapper
    {
        /// <summary>
        /// static Get Accessor VersionName
        /// </summary>
        public static string NameExecuting
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        /// <summary>
        /// static Get Accessor VersionName
        /// </summary>
        public static string NameCalling
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Name;
            }
        }

        /// <summary>
        /// static Get Accessor VersionName
        /// </summary>
        public static string NameVersionExecuting
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name + " " + VersionNumberExecuting;
            }
        }

        /// <summary>
        /// static Get Accessor VersionName
        /// </summary>
        public static string NameVersionCalling
        {
            get
            {
                string v_Version = Assembly.GetCallingAssembly().GetName().Version.ToString();
                int v_Index = v_Version.LastIndexOf(".");

                if (v_Index > 0)
                {
                    v_Version = v_Version.Substring(0, v_Index);
                }

                return Assembly.GetCallingAssembly().GetName().Name + " " + v_Version;
            }
        }

        /// <summary>
        /// static Get Accesssor VersionNumber
        /// </summary>
        public static string VersionNumberExecuting
        {
            get
            {
                string v_Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                int v_Index = v_Version.LastIndexOf(".");

                if (v_Index > 0)
                {
                    v_Version = v_Version.Substring(0, v_Index);
                }

                return v_Version;
            }
        }

        /// <summary>
        /// static Get Accesssor VersionNumber
        /// </summary>
        public static string VersionNumberCalling
        {
            get
            {
                string v_Version = Assembly.GetCallingAssembly().GetName().Version.ToString();
                int v_Index = v_Version.LastIndexOf(".");

                if (v_Index > 0)
                {
                    v_Version = v_Version.Substring(0, v_Index);
                }

                return v_Version;
            }
        }

        /// <summary>
        /// static Get Accessor VersionComplete
        /// </summary>
        public static string VersionCompleteExecuting
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// static Get Accessor VersionComplete
        /// </summary>
        public static string VersionCompleteCalling
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Version.ToString();
            }
        }

        private static string GetAssemblyCompany(Assembly a_Assembly)
        {
            return FileVersionInfo.GetVersionInfo(a_Assembly.Location).CompanyName;
        }

        public static string CompanyCalling
        {
            get
            {
                return GetAssemblyCompany(Assembly.GetCallingAssembly());
            }
        }

        public static string CompanyExecuting
        {
            get
            {
                return GetAssemblyCompany(Assembly.GetExecutingAssembly());
            }
        }

        public static void PrintAll()
        {
            System.Text.StringBuilder v_Builder = new System.Text.StringBuilder();
            v_Builder.AppendLine("NameExecuting: " + NameExecuting);
            v_Builder.AppendLine("NameCalling: " + NameCalling);
            v_Builder.AppendLine("NameVersionExecuting: " + NameVersionExecuting);
            v_Builder.AppendLine("NameVersionCalling: " + NameVersionCalling);
            v_Builder.AppendLine("VersionNumberExecuting: " + VersionNumberExecuting);
            v_Builder.AppendLine("VersionNumberCalling: " + VersionNumberCalling);
            v_Builder.AppendLine("VersionCompleteExecuting: " + VersionCompleteExecuting);
            v_Builder.AppendLine("VersionCompleteCalling: " + VersionCompleteCalling);
            v_Builder.AppendLine("CompanyCalling: " + CompanyCalling);
            v_Builder.AppendLine("CompanyExecuting: " + CompanyExecuting);

            Console.WriteLine(v_Builder.ToString());
        }
    }
}


