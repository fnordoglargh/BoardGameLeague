using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using BoardGameLeagueLib;
using log4net;
using log4net.Config;

namespace BoardGameLeagueUIBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public DbClass BGGDatabase { get; set; }
        ILog m_Logger;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.Name = "UI";
            XmlConfigurator.Configure(new FileInfo("log4netConfig.xml"));
            m_Logger = LogManager.GetLogger("MainWindow");
            m_Logger.Debug("Window starts loading.");
            BGGDatabase = new DbClass();
            BGGDatabase.BootStrap();
            DataContext = this;
        }

        #region Player Tab

        public Dictionary<Player.Genders, string> GenderEnumWithCaptions => new Dictionary<Player.Genders, string>() // TODO: Each time new dict?
        {
            {Player.Genders.Female, "Female"},
            {Player.Genders.Male, "Male"},
        };

        private void btAddNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            BGGDatabase.Persons.Add(new Player());
        }

        private void btDeleteSelectedPlayer_Click(object sender, RoutedEventArgs e)
        {
            Player v_SelectedPerson = (Player)lbPlayers.SelectedItem;
            BGGDatabase.Persons.Remove(v_SelectedPerson);
        }

        #endregion

        #region Games Tab

        public Dictionary<Game.GameType, string> GameEnumWithCaptions
        {
            get
            {
                return new Dictionary<Game.GameType, string>() // TODO: Each time new dict?
                {
                    {Game.GameType.VictoryPoints, "Victory Points"},
                    {Game.GameType.Ranks, "Ranks"},
                    {Game.GameType.WinLoose, "Win/Loose"},
                };
            }
        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            Game v_NewGame=new Game();
            BGGDatabase.Games.Add(v_NewGame);
            lbGames.SelectedItem = (v_NewGame);
        }

        private void buttonDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            Game v_SelectedGame = (Game)lbGames.SelectedItem;
            // TODO: Test for delete
            BGGDatabase.Games.Remove(v_SelectedGame);
        }

        private void buttonNewFamily_Click(object sender, RoutedEventArgs e)
        {
            Game v_SelectedGame=(Game)lbGames.SelectedItem;
            String v_SelectedGameName=v_SelectedGame.Name;
            BGGDatabase.GameFamilies.Add(new GameFamily(v_SelectedGameName));
        }

        #endregion

        #region Locations Tab

        private void btNewLocation_Click(object sender, RoutedEventArgs e)
        {
            Location v_NewLocation = new Location();
            BGGDatabase.Locations.Add(v_NewLocation);
            lbLocations.SelectedItem = v_NewLocation;
        }

        private void btDeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            Location v_SelectedLocation = (Location)lbLocations.SelectedItem;
            // TODO: Test for delete
            BGGDatabase.Locations.Remove(v_SelectedLocation);
        }

        #endregion

        

      
    }
}
