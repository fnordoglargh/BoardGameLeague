using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;
using log4net.Config;

namespace BoardGameLeagueDataSetUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ILog m_Logger;
        DataSet m_BGLDB = new DataSet();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.Name = "UI";
            XmlConfigurator.Configure(new FileInfo("log4netConfig.xml"));
            m_Logger = LogManager.GetLogger("MainWindow");
            m_Logger.Debug("Window starts loading.");

            DataSet v_BGLDB = new DataSet();
            m_BGLDB.ReadXml("db" + System.IO.Path.DirectorySeparatorChar + "bgldb.xml");

            lbGames.DataContext = m_BGLDB.Tables["Game"];
            cbGameFamily.DataContext = m_BGLDB.Tables["GameFamily"];

            lbPlayers.DataContext = m_BGLDB.Tables["Player"];

            comboBoxGender.Items.Add("Male");
            comboBoxGender.Items.Add("Female");

            cbGameType.Items.Add("VictoryPoints");
            cbGameType.Items.Add("Rank");

            lbGameFamilies.DataContext = m_BGLDB.Tables["GameFamily"];
            lbLocations.DataContext = m_BGLDB.Tables["Location"];

            //textBoxGameName.DataContext=

        }

        private void AddRow(String a_Tablename)
        {
            if (m_BGLDB.Tables.Contains(a_Tablename))
            {
                DataRow dr = m_BGLDB.Tables[a_Tablename].NewRow();
                dr["Id"] = new Guid();
                dr["Name"] = "blah";
                dr["Description"] = "blah";
                m_BGLDB.Tables[a_Tablename].Rows.Add(dr);

            }
        }

        private void buttonNewLocation_Click(object sender, RoutedEventArgs e)
        {
            AddRow("Location");
        }

    }
}
