using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace BoardGameLeagueUI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ILog m_Logger;
        public BglDb BglDatabase { get; set; }
        int m_MaxPlayerAmount = 8;
        UiBuildingHelper m_UiHelper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.Name = "MainWindow";
            AppHomeFolder.CreationResults v_HomeFolderCreationResult = StandardFileBootstrapper.BootstrapWrapper();

            m_Logger = LogManager.GetLogger(Thread.CurrentThread.Name);
            m_Logger.Info("*****************************************************************");
            m_Logger.Info("Welcome to " + VersionWrapper.NameVersionCalling);
            m_Logger.Info("Logger loaded.");
            m_Logger.Debug("Window starts loading.");

            DbHelper v_DbHelper = DbHelper.Instance;
            bool v_IsDbLoaded = v_DbHelper.LoadDataBase("bgldb.xml");

            if (v_IsDbLoaded == true)
            {
                BglDatabase = v_DbHelper.LiveBglDb;

                m_Logger.Info("Backend loading finished. Populating UI with data.");

                DataContext = this;

                m_UiHelper = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players);
                m_UiHelper.GeneratePlayerVariableUi(gridResults);

                for (int i = 1; i <= BglDb.c_MaxAmountPlayers; i++)
                {
                    comboBoxPlayerAmount.Items.Add(i);
                }

                m_Logger.Info("UI Populated. Ready for user actions.");
            }
            else
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DbHelper.WriteDatabase(BglDatabase, "bgldb_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xml");
            m_Logger.Info("Application closed.");
        }

        #region Games

        private void comboBoxGameFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxGameFamily.SelectedItem != null)
            {
                GameFamily v_SelectedFamily = (GameFamily)comboBoxGameFamily.SelectedItem;

                if (v_SelectedFamily.Id == GameFamily.c_StandardId)
                {
                    buttonNewFamily.IsEnabled = true;
                }
                else
                {
                    buttonNewFamily.IsEnabled = false;
                }
            }
        }

        private void buttonNewFamily_Click(object sender, RoutedEventArgs e)
        {
            Game v_SelectedGame = (Game)listBoxGames.SelectedItem;
            String v_SelectedGameName = v_SelectedGame.Name;
            BglDatabase.GameFamilies.Add(new GameFamily(v_SelectedGameName));
        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Games.Add(new Game());
            listBoxGames.SelectedIndex = listBoxGames.Items.Count - 1;
        }

        private void buttonDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Game", BglDatabase.RemoveEntity(listBoxGames.SelectedItem));
        }

        private void FamilyButtonActivation()
        {
            GameFamily v_CurrentFamily = (GameFamily)comboBoxGameFamily.SelectedItem;

            if (v_CurrentFamily.Id.Equals(GameFamily.c_StandardId))
            {
                buttonNewFamily.IsEnabled = true;
            }
            else
            {
                buttonNewFamily.IsEnabled = false;
            }
        }

        #endregion

        #region Tab: Game Families and Locations

        private void buttonNewLocation_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Locations.Add(new Location());
        }

        private void buttonDeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Location", BglDatabase.RemoveEntity(listBoxLocations.SelectedItem));
        }

        private void listBoxLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxLocations.SelectedItem == null)
            {
                buttonDeleteLocation.IsEnabled = false;
            }
            else
            {
                buttonDeleteLocation.IsEnabled = true;
            }
        }

        private void buttonDeleteGameFamily_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Game Family", BglDatabase.RemoveEntity(listBoxGameFamilies.SelectedItem));
        }

        private void listBoxGameFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxGameFamilies.SelectedItem == null)
            {
                buttonDeleteGameFamily.IsEnabled = false;
            }
            else
            {
                buttonDeleteGameFamily.IsEnabled = true;
            }
        }

        #endregion

        #region Helpers

        private void EntityStatusMessageBox(String a_Category, BglDb.EntityInteractionStatus a_InteractionStatus)
        {
            if (a_InteractionStatus == BglDb.EntityInteractionStatus.NotRemoved)
            {
                MessageBox.Show(String.Format("{0} cannot be removed because there are references to the entry.", a_Category));
            }
        }

        #endregion

        #region Players

        private void buttonDeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Player", BglDatabase.RemoveEntity(listBoxPlayers.SelectedItem));
        }

        private void buttonNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Players.Add(new Player());
        }

        private void listBoxPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxPlayers.SelectedItem == null)
            {
                buttonDeletePlayer.IsEnabled = false;
            }
            else
            {
                buttonDeletePlayer.IsEnabled = true;
            }
        }

        private void listBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxGames.SelectedItem == null)
            {
                buttonDeleteGame.IsEnabled = false;
            }
            else
            {
                buttonDeleteGame.IsEnabled = true;
            }
        }

        #endregion

        #region Results

        private void listBoxResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null) { return; } // Can't go further without a sender.

            Result v_SelectedResult = null;

            try
            {
                v_SelectedResult = (Result)((ListBox)sender).SelectedItem;
            }
            catch (InvalidCastException ice)
            {
                m_Logger.Error("Casting the selected listbox item into a Result failed.", ice);
            }
            catch (Exception ex)
            {
                m_Logger.Error("Selection of result was not successful.", ex);
            }

            if (v_SelectedResult == null) { return; } // Can't go further without a result instance.

            int v_ScoreAmount = v_SelectedResult.Scores.Count;

            m_UiHelper.UpdateBindings((Result)listBoxResults.SelectedItem, BglDatabase.Players);

            // Create bindings manually.

        }

        private void comboBoxGamesForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void calendarResult_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBoxPlayerAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBoxResultWinnerPlayer_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void buttonDeleteResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonAddResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonResultSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCopyResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonNewResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboBoxLocationsForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void checkBoxResultWinnerPlayer_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        #endregion


    }
}
