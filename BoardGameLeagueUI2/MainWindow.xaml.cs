﻿using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        UiBuildingHelper m_UiHelperView;
        UiBuildingHelper m_UiHelperNewEntry;
        Info m_InfoWindow = new Info();

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

                m_UiHelperView = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players);
                m_UiHelperView.GeneratePlayerVariableUi(gridResultsView);
                m_UiHelperNewEntry = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players);
                m_UiHelperNewEntry.GeneratePlayerVariableUiWithReset(gridResultsEntering);


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
            if (DbHelper.Instance.IsChanged)
            {
                if (MessageBox.Show("Save database changes?", "Unsaved database changes detected", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DbHelper.WriteDatabase(BglDatabase, "bgldb.xml");
                }
            }

            m_InfoWindow.Close();
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
            // Creates a new game family with the same name as the game and selects the same.
            Game v_SelectedGame = (Game)listBoxGames.SelectedItem;
            String v_SelectedGameName = v_SelectedGame.Name;
            GameFamily v_NewGameFamily = new GameFamily(v_SelectedGameName);
            BglDatabase.GameFamilies.Add(v_NewGameFamily);
            v_SelectedGame.IdGamefamily = v_NewGameFamily.Id;
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

            m_UiHelperView.UpdateBindings((Result)listBoxResults.SelectedItem, BglDatabase.Players);

            // Create bindings manually.
        }

        private void comboBoxGamesForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void calendarResult_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
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

        private void buttonCopyResult_Click(object sender, RoutedEventArgs e)
        {

        }



        private void comboBoxLocationsForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void checkBoxResultWinnerPlayer_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Results Editor

        bool m_IsGameSelected = false;
        bool m_IsLocationSelected = false;

        private void comboBoxGamesForResultEntering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = ((ComboBox)comboBoxGamesForResultEntering).SelectedValue as Game;
            m_IsGameSelected = true;
            ActivateAddRusultButton();
            m_UiHelperNewEntry.ActivateUiElements(v_SelectedGame.PlayerQuantityMax);
            BglDatabase.ChangePlayerNumbers(v_SelectedGame.PlayerQuantityMin, v_SelectedGame.PlayerQuantityMax);
            comboBoxPlayerAmountEntering.SelectedValue = v_SelectedGame.PlayerQuantityMin;
        }

        private void comboBoxLocationsForResultEntering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_IsLocationSelected = true;
            ActivateAddRusultButton();
        }

        private void ActivateAddRusultButton()
        {
            if (m_IsGameSelected && m_IsLocationSelected)
            {
                buttonNewResultEntering.IsEnabled = true;
            }
        }

        private void comboBoxPlayerAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;

            //if (v_SelectedGame == null) return;

            if (((ComboBox)sender).SelectedValue != null)
            {

                int v_SelectedPlayerAmount = (int)((ComboBox)sender).SelectedValue;
                m_Logger.Debug("Selected playeramount: " + v_SelectedPlayerAmount);

                foreach (int i in ((ComboBox)sender).Items)
                {
                    m_Logger.Debug("Items: " + i);
                }
            }
            else
            {
                m_Logger.Debug("comboBoxPlayerAmount_SelectionChanged FIRED");
            }
        }

        private void buttonNewResult_Click(object sender, RoutedEventArgs e)
        {
            Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;
            int v_AmountResultsToAdd = (int)comboBoxPlayerAmountEntering.SelectedValue;

            if (comboBoxLocationsForResultEntering.SelectedValue == null)
            {
                MessageBox.Show("Select a location for this result.");
                return;
            }

            Location v_Location = comboBoxLocationsForResultEntering.SelectedValue as Location;
            String v_Message = "";
            bool v_IsUserNotificationNecessary = false;

            if (comboBoxPlayerAmountEntering.SelectedValue == null)
            {
                MessageBox.Show("Select the amount of players for this result.");
                return;
            }

            int v_SelectedPlayerAmount = (int)comboBoxPlayerAmountEntering.SelectedValue;

            bool v_IsEverythingOk = m_UiHelperNewEntry.TestCheckBoxes(v_AmountResultsToAdd);

            if (!v_IsEverythingOk)
            {
                v_IsUserNotificationNecessary = true;
                v_Message += "Check at least one winner checkbox." + Environment.NewLine + Environment.NewLine;
            }

            String v_WrongComboBoxes = m_UiHelperNewEntry.TestComboBoxes(v_AmountResultsToAdd);

            if (v_WrongComboBoxes != "")
            {
                v_IsUserNotificationNecessary = true;
                v_Message += "No players selected in comboboxes " + v_WrongComboBoxes + "." + Environment.NewLine + Environment.NewLine;
            }

            String v_WrongTextBoxes = m_UiHelperNewEntry.TestTextBoxes(v_AmountResultsToAdd);

            if (v_WrongTextBoxes != "")
            {
                v_IsUserNotificationNecessary = true;
                v_Message += "Not a number in textboxes " + v_WrongTextBoxes + "." + Environment.NewLine + Environment.NewLine;
            }

            if (v_IsUserNotificationNecessary)
            {
                MessageBox.Show(v_Message);
            }
            else
            {
                String v_ResultDisplay = "";
                ObservableCollection<Score> v_Scores = new ObservableCollection<Score>();

                for (int i = 0; i < v_SelectedPlayerAmount; i++)
                {
                    v_ResultDisplay += ((Player)m_UiHelperNewEntry.PlayerResultComboBoxes[i].SelectedValue).DisplayName + ": ";
                    v_ResultDisplay += m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text + " ";

                    if ((bool)m_UiHelperNewEntry.PlayerResultCheckBoxes[i].IsChecked)
                    {
                        v_ResultDisplay += "(Winner)";
                        v_Scores.Add(new Score(((Player)m_UiHelperNewEntry.PlayerResultComboBoxes[i].SelectedValue).Id, m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text, true));
                    }
                    else
                    {
                        v_Scores.Add(new Score(((Player)m_UiHelperNewEntry.PlayerResultComboBoxes[i].SelectedValue).Id, m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text, false));
                    }

                    v_ResultDisplay += Environment.NewLine;
                }

                // Prevents display of time.
                String v_SelectedDate = calendarResultEntering.SelectedDate.ToString();
                v_ResultDisplay += v_SelectedDate.Substring(0, v_SelectedDate.IndexOf(' '));

                if (MessageBox.Show(
                    "Is this result correct?" + Environment.NewLine + Environment.NewLine + v_ResultDisplay
                    , "New Result"
                    , MessageBoxButton.YesNo
                    , MessageBoxImage.Warning) == MessageBoxResult.Yes
                    )
                {
                    Result v_Result = new Result(v_SelectedGame.Id, v_Scores, (DateTime)calendarResultEntering.SelectedDate, v_Location.Id);
                    BglDatabase.Results.Add(v_Result);
                }
            }
        }

        private void comboBoxReportGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Guid v_SelectedGameId = (comboBoxReportGames.SelectedItem as Game).Id;

            ObservableCollection<BglDb.ResultRow> v_ResultRows = BglDatabase.CalculateResults(v_SelectedGameId);
            dataGrid1.ItemsSource = v_ResultRows;
        }

        private void btnTestELO_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<Player, Result.ResultHelper> v_EloResults = BglDatabase.CalculateEloResults();
            ObservableCollection<EloCalculator.EloResultRow> v_EloResultRows = new ObservableCollection<EloCalculator.EloResultRow>();

            foreach (KeyValuePair<Player, Result.ResultHelper> i_EloResult in v_EloResults)
            {
                v_EloResultRows.Add(new EloCalculator.EloResultRow(i_EloResult.Key.DisplayName, i_EloResult.Value.EloScore));
            }

            dataGrid1.ItemsSource = v_EloResultRows;
        }

        #endregion

        #region Menu Bar Events

        private void menuItemSaveDb_Click(object sender, RoutedEventArgs e)
        {
            DbHelper.WriteDatabase(BglDatabase, "bgldb.xml");
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuItemInfo_Click(object sender, RoutedEventArgs e)
        {
            m_InfoWindow.Show();
        }

        #endregion
    }
}