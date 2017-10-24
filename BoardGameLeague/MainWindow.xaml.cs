using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ILog m_Logger;
        public BglDb BglDatabase { get; set; }
        int m_MaxPlayerAmount = BglDb.c_MaxAmountPlayers;
        UiBuildingHelper m_UiHelperView;
        UiBuildingHelper m_UiHelperNewEntry;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.Name = "MainWindow";
            List<AppHomeFolder.CreationResults> v_HomeFolderCreationResults = StandardFileBootstrapper.BootstrapWrapper();

            if (v_HomeFolderCreationResults.Contains(AppHomeFolder.CreationResults.Error))
            {
                throw new Exception("Bootstrapping reported an error. Don't know what to do.");
            }

            Title = VersionWrapper.NameVersionCalling;

            m_Logger = LogManager.GetLogger(Thread.CurrentThread.Name);
            m_Logger.Info("*****************************************************************");
            m_Logger.Info("Welcome to " + VersionWrapper.NameVersionCalling);
            m_Logger.Info("Logger loaded.");
            m_Logger.Debug("Window starts loading.");

            DbHelper v_DbHelper = DbHelper.Instance;
            // Loads from executing folder.
            //bool v_IsDbLoaded = v_DbHelper.LoadDataBase(DbHelper.c_StandardDbName);
            // Loads from Appdata.
            bool v_IsDbLoaded = v_DbHelper.LoadStandardDb();

            if (v_IsDbLoaded == true)
            {
                BglDatabase = v_DbHelper.LiveBglDb;
                m_Logger.Info("Backend loading finished. Populating UI with data.");
                DataContext = this;

                m_UiHelperView = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players, 410);
                m_UiHelperView.GeneratePlayerVariableUiWithRemove(gridResultsView);
                m_UiHelperNewEntry = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players, 248);
                m_UiHelperNewEntry.GeneratePlayerVariableUiWithReset(gridResultsEntering);

                for (int i = 1; i <= BglDb.c_MaxAmountPlayers; i++)
                {
                    comboBoxPlayerNumber.Items.Add(i);
                    m_UiHelperView.RemoveEvent += UiHelperView_RemoveEvent;
                }

                m_Logger.Info("UI Populated. Ready for user actions.");
            }
            else
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }
        }

        private void UiHelperView_RemoveEvent(object sender, EventArgs e)
        {
            UiBuildingHelper.RemoveEventArgs v_Args =e as UiBuildingHelper.RemoveEventArgs;
            m_Logger.Debug("Removing player result at index: " + v_Args.Index);


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DbHelper.Instance.IsChanged)
            {
                if (MessageBox.Show("Save database changes?", "Unsaved database changes detected", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DbHelper.WriteStandardDatabase(BglDatabase);
                }
            }

            m_Logger.Info("Application closed.");
        }

        void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var v_TextBox = sender as System.Windows.Controls.TextBox;

            if (v_TextBox != null && !v_TextBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
            {
                v_TextBox.SelectAll();
            }
        }

        #region Games

        private void SetGamesControlsEnabledStatus(bool a_Status)
        {
            textBoxGameName.IsEnabled = a_Status;
            comboBoxGameFamily.IsEnabled = a_Status;
            // TODO: Make game types work.
            //comboBoxGameType.IsEnabled = a_Status;
            sliderPlayerAmountMin.IsEnabled = a_Status;
            sliderPlayerAmountMax.IsEnabled = a_Status;
            buttonGamesApply.IsEnabled = a_Status;
        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Games.Add(new Game());
            listBoxGames.SelectedIndex = listBoxGames.Items.Count - 1;
            textBoxGameName.Focus();
            textBoxGameName.SelectAll();
        }

        private void buttonDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Game", BglDatabase.RemoveEntity(listBoxGames.SelectedItem));
            SetGamesControlsEnabledStatus(false);
        }

        private void listBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxGames.SelectedItem == null)
            {
                buttonDeleteGame.IsEnabled = false;
                SetGamesControlsEnabledStatus(false);
            }
            else
            {
                buttonDeleteGame.IsEnabled = true;
                SetGamesControlsEnabledStatus(true);
            }
        }

        #endregion

        #region Tab: Game Families and Locations

        private void SetLocationsControlsEnabledStatus(bool a_Status)
        {
            textBoxLocationName.IsEnabled = a_Status;
            textBoxLocationDescription.IsEnabled = a_Status;
            buttonDeleteLocation.IsEnabled = a_Status;
            buttonLocationsApply.IsEnabled = a_Status;
        }

        private void buttonNewLocation_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Locations.Add(new Location());
            listBoxLocations.SelectedIndex = listBoxLocations.Items.Count - 1;
            textBoxLocationName.Focus();
            textBoxLocationName.SelectAll();
        }

        private void buttonDeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Location", BglDatabase.RemoveEntity(listBoxLocations.SelectedItem));
        }

        private void listBoxLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxLocations.SelectedItem == null)
            {
                SetLocationsControlsEnabledStatus(false);
            }
            else
            {
                SetLocationsControlsEnabledStatus(true);
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
                buttonGameFamiliesApply.IsEnabled = false;
            }
            else
            {
                buttonDeleteGameFamily.IsEnabled = true;
                buttonGameFamiliesApply.IsEnabled = true;
            }
        }

        private void buttonNewFamily_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.GameFamilies.Add(new GameFamily());
            listBoxGameFamilies.SelectedIndex = listBoxGameFamilies.Items.Count - 1;
            textBoxFamilyName.Focus();
            textBoxFamilyName.SelectAll();
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

        private void SetPlayerControlsEnabledStatus(bool a_Status)
        {
            comboBoxPlayerGender.IsEnabled = a_Status;
            textBoxPlayerName.IsEnabled = a_Status;
            buttonPlayersApply.IsEnabled = a_Status;
            buttonDeletePlayer.IsEnabled = a_Status;
        }

        private void buttonDeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            EntityStatusMessageBox("Player", BglDatabase.RemoveEntity(listBoxPlayers.SelectedItem));
        }

        private void buttonNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.Players.Add(new Player());
            listBoxPlayers.SelectedItem = BglDatabase.Players[BglDatabase.Players.Count - 1];
            textBoxPlayerName.Focus();
            textBoxPlayerName.SelectAll();
        }

        private void listBoxPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxPlayers.SelectedItem == null)
            {
                SetPlayerControlsEnabledStatus(false);
            }
            else
            {
                SetPlayerControlsEnabledStatus(true);
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

            // Result was deselected.
            if (v_SelectedResult == null)
            {
                buttonCopyResult.IsEnabled = false;
            } 
            else
            {
                int v_ScoreAmount = v_SelectedResult.Scores.Count;
                comboBoxPlayerNumber.SelectedIndex = v_ScoreAmount - 1;
                buttonCopyResult.IsEnabled = true;
            }

            m_UiHelperView.UpdateBindings(v_SelectedResult, BglDatabase.Players);
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
            Result v_SelectedResult = (Result)listBoxResults.SelectedItem;

            if (v_SelectedResult == null) { return; }


        }

        private void comboBoxLocationsForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void checkBoxResultWinnerPlayer_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Results Editor

        private void comboBoxGamesForResultEntering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;
            BglDatabase.ChangePlayerNumbers(v_SelectedGame.PlayerQuantityMin, v_SelectedGame.PlayerQuantityMax);
            // Using SelectedValue will cause update errors because the SelectionChanged event will sometimes think the value is null.
            comboBoxPlayerAmountEntering.SelectedIndex = v_SelectedGame.PlayerQuantityMax - v_SelectedGame.PlayerQuantityMin;
        }

        private void comboBoxPlayerAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayerAmountEntering.SelectedValue != null)
            {
                int v_SelectedPlayerAmount = (int)comboBoxPlayerAmountEntering.SelectedValue;
                m_UiHelperNewEntry.ActivateUiElements(v_SelectedPlayerAmount);
            }
        }

        private void buttonNewResult_Click(object sender, RoutedEventArgs e)
        {
            Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;

            // Do some early checking: Is a game selected?
            if (v_SelectedGame == null)
            {
                MessageBox.Show("Start by selecting a game.");
                return;
            }

            // Do some early checking: Is a location selected?
            if (comboBoxLocationsForResultEntering.SelectedValue == null)
            {
                MessageBox.Show("Select a location for this result.");
                return;
            }

            Location v_Location = comboBoxLocationsForResultEntering.SelectedValue as Location;
            String v_Message = "";
            bool v_IsUserNotificationNecessary = false;
            int v_AmountResultsToAdd = (int)comboBoxPlayerAmountEntering.SelectedValue;
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
                v_Message += "No or same players selected in comboboxes " + v_WrongComboBoxes + "." + Environment.NewLine + Environment.NewLine;
            }

            String v_WrongTextBoxes = m_UiHelperNewEntry.TestTextBoxes(v_AmountResultsToAdd);

            if (v_WrongTextBoxes != "")
            {
                v_IsUserNotificationNecessary = true;
                v_Message += "No numbers in textboxes " + v_WrongTextBoxes + "." + Environment.NewLine + Environment.NewLine;
            }

            if (v_IsUserNotificationNecessary)
            {
                MessageBox.Show(v_Message);
            }
            else
            {
                String v_ResultDisplay = "";
                ObservableCollection<Score> v_Scores = new ObservableCollection<Score>();

                for (int i = 0; i < v_AmountResultsToAdd; i++)
                {
                    v_ResultDisplay += ((Player)m_UiHelperNewEntry.PlayerResultComboBoxes[i].SelectedValue).Name + ": ";
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

                    // If the ItemSource is not refreshed after adding a result and reordering, the result would show up at the end.
                    listBoxResults.ItemsSource = BglDatabase.Results;
                }
            }
        }

        private void comboBoxReportGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = comboBoxReportGames.SelectedItem as Game;

            if (v_SelectedGame != null)
            {
                ObservableCollection<BglDb.ResultRow> v_ResultRows = BglDatabase.CalculateResultsGames(v_SelectedGame.Id);
                dataGrid1.ItemsSource = v_ResultRows;
                comboBoxReportFamilies.SelectedItem = null;
            }
            else
            {
                // This is for the unlikely event that a newly created game is used in the reports and then deleted (while still selected).
                comboBoxReportGames.SelectedItem = null;
            }
        }

        private void comboBoxReportFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameFamily v_SelectedGameFamily = comboBoxReportFamilies.SelectedItem as GameFamily;

            if (v_SelectedGameFamily != null)
            {
                ObservableCollection<BglDb.ResultRow> v_ResultRows = BglDatabase.CalculateResultsGameFamilies(v_SelectedGameFamily.Id);
                dataGrid1.ItemsSource = v_ResultRows;
                comboBoxReportGames.SelectedItem = null;
            }
            else
            {
                // This is for the unlikely event that a newly created family is used in the reports and then deleted (while still selected).
                comboBoxReportFamilies.SelectedItem = null;
            }
        }

        private void btnTestELO_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<Player, Result.ResultHelper> v_EloResults = BglDatabase.CalculateEloResults();
            ObservableCollection<EloCalculator.EloResultRow> v_EloResultRows = new ObservableCollection<EloCalculator.EloResultRow>();
            comboBoxReportGames.SelectedItem = null;
            comboBoxReportFamilies.SelectedItem = null;

            foreach (KeyValuePair<Player, Result.ResultHelper> i_EloResult in v_EloResults)
            {
                v_EloResultRows.Add(new EloCalculator.EloResultRow(i_EloResult.Key.Name, i_EloResult.Value.EloScore, i_EloResult.Value.AmountGamesPlayed, i_EloResult.Value.IsEstablished));
            }

            dataGrid1.ItemsSource = v_EloResultRows;
        }

        #endregion

        #region Menu Bar Events

        private void menuItemSaveDb_Click(object sender, RoutedEventArgs e)
        {
            DbHelper.WriteStandardDatabase(BglDatabase);
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuItemUsage_Click(object sender, RoutedEventArgs e)
        {
            Usage v_UsageWindow = new Usage();
            v_UsageWindow.Show();
        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            About v_AboutWindow = new About();
            v_AboutWindow.Show();
        }

        #endregion

        private void comboBoxPlayerNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayerNumber.SelectedValue != null)
            {
                int v_SelectedPlayerAmount = (int)comboBoxPlayerNumber.SelectedValue;
                m_UiHelperView.ActivateUiElements(v_SelectedPlayerAmount);
            }
        }
    }
}
