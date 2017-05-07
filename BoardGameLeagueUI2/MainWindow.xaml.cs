using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
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

namespace BoardGameLeagueUI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ILog m_Logger;
        public BglDb BglDatabase { get; set; }
        int m_MaxPlayerAmount =8;
        UiBuildingHelper m_UiHelper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.Name = "MainWindow";
            XmlConfigurator.Configure(new FileInfo("conf\\log4netConfig.xml"));
            m_Logger = LogManager.GetLogger(Thread.CurrentThread.Name);
            m_Logger.Info("*****************************************************************");
            m_Logger.Info("Welcome to " + VersionWrapper.NameVersionExecuting);
            m_Logger.Info("Logger loaded.");
            m_Logger.Debug("Window starts loading.");

            BglDatabase = DbLoader.LoadDatabase("bgldb.xml");

            if (BglDatabase == null)
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }

            m_Logger.Info("Backend loading finished. Populating UI with data.");

            DataContext = this;

            m_UiHelper = new UiBuildingHelper(m_MaxPlayerAmount);
            m_UiHelper.GeneratePlayerVariableUi(gridResults);

            m_Logger.Info("UI Populated. Ready for user actions.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Logger.Info("Application closed.");
            DbLoader.WriteDatabase(BglDatabase, "bgldb_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss") + ".xml");
        }

        #region Games

        private void listBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBoxGameFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxGameFamily.SelectedItem != null)
            {
                KeyValuePair<Guid, GameFamily> kvp = (KeyValuePair<Guid, GameFamily>)comboBoxGameFamily.SelectedItem;

                if ((Guid)kvp.Key == GameFamily.c_StandardId)
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
            BglDatabase.Games.Remove((Game)listBoxGames.SelectedItem);
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

        private void listBoxGameFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            createBindingFromListBoxToTextBox(listBoxGameFamilies, textBoxFamilyName);
        }

        private void listBoxLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            createBindingFromListBoxToTextBox(listBoxLocations, textBoxLocationName);
        }

        private void buttonNewLocation_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Locations.Add(new Location());
        }

        #endregion

        #region Helpers

        private void createBindingFromListBoxToTextBox(ListBox a_ListBox, TextBox a_TextBox)
        {
            Binding v_Binding = new Binding();
            v_Binding.Source = a_ListBox.SelectedValue;
            v_Binding.Path = new PropertyPath("Name");
            a_TextBox.SetBinding(TextBox.TextProperty, v_Binding);
        }

        #endregion

        #region Players

        private void buttonDeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Players.Remove((Player)listBoxPlayers.SelectedItem);
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
