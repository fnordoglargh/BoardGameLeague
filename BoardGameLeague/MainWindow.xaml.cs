using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        private SolidColorBrush m_ColorDeactivatedControl = Brushes.White;
        private SolidColorBrush m_ColorActivatedControl = Brushes.Lavender;
        private ControlCategory m_ActualSelection;

        public enum ControlCategory
        {
            Location,
            Player,
            Game,
            GameFamily
        }

        private String PathAndNameToActiveDb
        {
            get { return DbHelper.Instance.Settings.LastUsedDatabase; }
            set
            {
                Title = VersionWrapper.NameVersionCalling + " - " + value;
                DbHelper.Instance.Settings.LastUsedDatabase = value;
                DbHelper.Instance.SaveSettings();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Prevent tooltips from vanishing.
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
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
            bool v_IsDbLoaded = v_DbHelper.LoadDataBase(v_DbHelper.Settings.LastUsedDatabase);

            if (v_IsDbLoaded == true)
            {
                PathAndNameToActiveDb = v_DbHelper.Settings.LastUsedDatabase;
                BglDatabase = v_DbHelper.LiveBglDb;
                m_Logger.Info("Backend loading finished. Populating UI with data.");
                DataContext = this;

                m_UiHelperView = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players, 410);
                m_UiHelperView.GeneratePlayerVariableUiWithRemove(gridResultsView);
                m_UiHelperNewEntry = new UiBuildingHelper(m_MaxPlayerAmount, BglDatabase.Players, 248);
                m_UiHelperNewEntry.GeneratePlayerVariableUiWithReset(gridResultsEntering);
                m_UiHelperNewEntry.ActivateUiElements(0);

                for (int i = 0; i <= BglDb.c_MaxAmountPlayers; i++)
                {
                    comboBoxPlayerNumber.Items.Add(i);
                }

                m_UiHelperView.RemoveEvent += UiHelperView_RemoveEvent;
                BglDatabase.PropertyChanged += BglDatabase_PropertyChanged;

                // Without this hack the mouse down events are not registered.
                Players_MouseDown(null, null);

                m_Logger.Info("UI Populated. Ready for user actions.");
            }
            else
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }
        }

        private void Locations_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UiFocusHelper(ControlCategory.Location);
        }

        private void Players_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UiFocusHelper(ControlCategory.Player);
        }

        private void GameFamilies_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UiFocusHelper(ControlCategory.GameFamily);
        }

        private void Games_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UiFocusHelper(ControlCategory.Game); 
        }

        private void Locations_Control_GotFocus(object sender, RoutedEventArgs e)
        {
            UiFocusHelper(ControlCategory.Location);
        }

        private void Players_Control_GotFocus(object sender, RoutedEventArgs e)
        {
            UiFocusHelper(ControlCategory.Player);
        }

        private void Games_Control_GotFocus(object sender, RoutedEventArgs e)
        {
            UiFocusHelper(ControlCategory.Game);
        }

        private void GameFamilies_Control_GotFocus(object sender, RoutedEventArgs e)
        {
            UiFocusHelper(ControlCategory.GameFamily);
        }

        private void UiFocusHelper(ControlCategory a_ControlCategory)
        {
            if (a_ControlCategory == ControlCategory.Location)
            {
                m_ActualSelection = ControlCategory.Location;
                GbLocations.Background = m_ColorActivatedControl;
                GbPlayers.Background = m_ColorDeactivatedControl;
                GbGameFamilies.Background = m_ColorDeactivatedControl;
                GbGames.Background = m_ColorDeactivatedControl;
                BtEntitiyNew.Content = "New Location";
                BtEntityDelete.Content = "Delete Location";
                LbPlayers.SelectedItem = null;
                LbGameFamilies.SelectedItem = null;
                LbGames.SelectedItem = null;
            }
            else if (a_ControlCategory == ControlCategory.Player)
            {
                m_ActualSelection = ControlCategory.Player;
                GbLocations.Background = m_ColorDeactivatedControl;
                GbPlayers.Background = m_ColorActivatedControl;
                GbGameFamilies.Background = m_ColorDeactivatedControl;
                GbGames.Background = m_ColorDeactivatedControl;
                BtEntitiyNew.Content = "New Player";
                BtEntityDelete.Content = "Delete Player";
                LbLocations.SelectedItem = null;
                LbGameFamilies.SelectedItem = null;
                LbGames.SelectedItem = null;
            }
            else if (a_ControlCategory == ControlCategory.GameFamily)
            {
                m_ActualSelection = ControlCategory.GameFamily;
                GbLocations.Background = m_ColorDeactivatedControl;
                GbPlayers.Background = m_ColorDeactivatedControl;
                GbGameFamilies.Background = m_ColorActivatedControl;
                GbGames.Background = m_ColorDeactivatedControl;
                BtEntitiyNew.Content = "New Game Family";
                BtEntityDelete.Content = "Delete Game Family";
                LbPlayers.SelectedItem = null;
                LbLocations.SelectedItem = null;
                LbGames.SelectedItem = null;
            }
            else if (a_ControlCategory == ControlCategory.Game)
            {
                m_ActualSelection = ControlCategory.Game;
                GbLocations.Background = m_ColorDeactivatedControl;
                GbPlayers.Background = m_ColorDeactivatedControl;
                GbGameFamilies.Background = m_ColorDeactivatedControl;
                GbGames.Background = m_ColorActivatedControl;
                BtEntitiyNew.Content = "New Game";
                BtEntityDelete.Content = "Delete Game";
                LbPlayers.SelectedItem = null;
                LbLocations.SelectedItem = null;
                LbGameFamilies.SelectedItem = null;
            }
        }

        private void BtEntitiyNew_Click(object sender, RoutedEventArgs e)
        {
            if (m_ActualSelection == ControlCategory.Location)
            {
                BglDatabase.Locations.Add(new Location());
                LbLocations.SelectedIndex = LbLocations.Items.Count - 1;
                TbLocationName.Focus();
                TbLocationName.SelectAll();
            }
            else if (m_ActualSelection == ControlCategory.Player)
            {
                BglDatabase.Players.Add(new Player());
                LbPlayers.SelectedItem = BglDatabase.Players[BglDatabase.Players.Count - 1];
                TbPlayerName.Focus();
                TbPlayerName.SelectAll();
            }
            else if (m_ActualSelection == ControlCategory.Game)
            {
                BglDatabase.Games.Add(new Game());
                LbGames.SelectedIndex = LbGames.Items.Count - 1;
                TbGameName.Focus();
                TbGameName.SelectAll();
            }
            else if (m_ActualSelection == ControlCategory.GameFamily)
            {
                BglDatabase.GameFamilies.Add(new GameFamily());
                LbGameFamilies.SelectedIndex = LbGameFamilies.Items.Count - 1;
                textBoxFamilyName.Focus();
                textBoxFamilyName.SelectAll();
            }
        }

        private void BtEntityDelete_Click(object sender, RoutedEventArgs e)
        {
            if (m_ActualSelection == ControlCategory.Location)
            {
                EntityStatusMessageBox(ControlCategory.Location, BglDatabase.RemoveEntity(LbLocations.SelectedItem));
            }
            else if (m_ActualSelection == ControlCategory.Player)
            {
                EntityStatusMessageBox(ControlCategory.Player, BglDatabase.RemoveEntity(LbPlayers.SelectedItem));
            }
            else if (m_ActualSelection == ControlCategory.Game)
            {
                EntityStatusMessageBox(ControlCategory.Game, BglDatabase.RemoveEntity(LbGames.SelectedItem));
            }
            else if (m_ActualSelection == ControlCategory.GameFamily)
            {
                EntityStatusMessageBox(ControlCategory.GameFamily, BglDatabase.RemoveEntity(LbGameFamilies.SelectedItem));
            }
        }

        private void DatabaseChangesWarning()
        {
            if (DbHelper.Instance.IsChanged)
            {
                if (MessageBox.Show("Save database changes?", "Unsaved database changes detected", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DbHelper.WriteDatabase(BglDatabase, PathAndNameToActiveDb);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DatabaseChangesWarning();
            m_Logger.Info("Application closed.");
        }

        /// <summary>
        /// Helper to select the contents of a TextBox in case it got focus through us of the Tab key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            TbGameName.IsEnabled = a_Status;
            CbGameFamily.IsEnabled = a_Status;
            // TODO: Make game types work.
            //comboBoxGameType.IsEnabled = a_Status;
            SPlayerAmountMin.IsEnabled = a_Status;
            SPlayerAmountMax.IsEnabled = a_Status;
            BtEntityApply.IsEnabled = a_Status;
            BtEntityDelete.IsEnabled = a_Status;
        }

        private void listBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbGames.SelectedItem == null)
            {
                SetGamesControlsEnabledStatus(false);
            }
            else
            {
                SetGamesControlsEnabledStatus(true);
            }
        }

        #endregion

        #region Tab: Game Families and Locations

        private void SetLocationsControlsEnabledStatus(bool a_Status)
        {
            TbLocationName.IsEnabled = a_Status;
            TbLocationDescription.IsEnabled = a_Status;
            BtEntityApply.IsEnabled = a_Status;
            BtEntityDelete.IsEnabled = a_Status;
        }

        private void listBoxLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbLocations.SelectedItem == null)
            {
                SetLocationsControlsEnabledStatus(false);
            }
            else
            {
                SetLocationsControlsEnabledStatus(true);
            }
        }

        private void listBoxGameFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbGameFamilies.SelectedItem == null)
            {
                textBoxFamilyName.IsEnabled = false;
                BtEntityApply.IsEnabled = false;
                BtEntityDelete.IsEnabled = false;
            }
            else
            {
                textBoxFamilyName.IsEnabled = true;
                BtEntityApply.IsEnabled = true;
                BtEntityDelete.IsEnabled = true;
            }
        }

        #endregion

        #region Helpers

        private void EntityStatusMessageBox(ControlCategory a_Category, BglDb.EntityInteractionStatus a_InteractionStatus)
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
            CbPlayerGender.IsEnabled = a_Status;
            TbPlayerName.IsEnabled = a_Status;
            BtEntityApply.IsEnabled = a_Status;
            BtEntityDelete.IsEnabled = a_Status;
        }

        private void listBoxPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbPlayers.SelectedItem == null)
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

        private void BglDatabase_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void UiHelperView_RemoveEvent(object sender, EventArgs e)
        {
            Result v_SelectedResult = ((Result)LbResults.SelectedItem);
            UiBuildingHelper.RemoveEventArgs v_Args = e as UiBuildingHelper.RemoveEventArgs;
            m_Logger.Debug("Removing player result at index: " + v_Args.Index);
            Game v_PlayedGame = BglDatabase.GamesById[v_SelectedResult.IdGame];
            int v_WinnerCounter = 0;

            // Check if we still have at least one winner.
            if (v_SelectedResult.Scores[v_Args.Index].IsWinner)
            {
                foreach (Score i_Score in v_SelectedResult.Scores)
                {
                    if (i_Score.IsWinner)
                    {
                        ++v_WinnerCounter;
                    }
                }
            }

            // We need to keep the sole winner.
            if (v_WinnerCounter == 1)
            {
                MessageBox.Show("We need at least one winner.");
            }
            else
            {
                // Check if removing Score would get us under the minimum player number for that game.
                if (v_SelectedResult.Scores.Count - 1 < v_PlayedGame.PlayerQuantityMin)
                {
                    MessageBox.Show(String.Format("This game needs {0} results.", v_PlayedGame.PlayerQuantityMin));
                }
                else
                {
                    v_SelectedResult.Scores.RemoveAt(v_Args.Index);
                    LbResults.SelectedItem = null;
                    LbResults.SelectedItem = v_SelectedResult;
                    DbHelper.Instance.IsChanged = true;
                }
            }
        }

        private void LbResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            // Result was deselected.
            if (v_SelectedResult == null)
            {
                buttonCopyResult.IsEnabled = false;
                ButtonAddScoreToResult.IsEnabled = false;
                buttonDeleteResult.IsEnabled = false;
                comboBoxGamesForResult.IsEnabled = false;
                comboBoxLocationsForResult.IsEnabled = false;
                calendarResult.IsEnabled = false;
                ButtonApplyChangedResult.IsEnabled = false;
                comboBoxPlayerNumber.SelectedItem = 0;
            }
            else
            {
                buttonCopyResult.IsEnabled = true;
                buttonDeleteResult.IsEnabled = true;
                comboBoxGamesForResult.IsEnabled = true;
                comboBoxLocationsForResult.IsEnabled = true;
                calendarResult.IsEnabled = true;
                ButtonApplyChangedResult.IsEnabled = true;
                comboBoxPlayerNumber.SelectedItem = v_SelectedResult.Scores.Count;

                // We already have the maximum amount of scores.
                if (v_SelectedResult.Scores.Count <= BglDb.c_MaxAmountPlayers)
                {
                    ButtonAddScoreToResult.IsEnabled = true;
                }

                if (v_SelectedResult.Scores.Count == 1)
                {
                    m_UiHelperView.SetFirstButtonEnabledState(false);
                }
            }

            m_UiHelperView.UpdateBindings(v_SelectedResult);
        }

        private void ButtonAddScoreToResult_Click(object sender, RoutedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;
            Guid v_NextPlayerId = Guid.Empty;

            // Find next player which has not already played in the active result.
            foreach (Player i_Player in BglDatabase.Players)
            {
                if (!v_SelectedResult.ScoresById.ContainsKey(i_Player.Id))
                {
                    v_NextPlayerId = i_Player.Id;
                    break;
                }
            }

            if (BglDatabase.GamesById[v_SelectedResult.IdGame].PlayerQuantityMax == v_SelectedResult.Scores.Count)
            {
                MessageBox.Show(String.Format("This result already contains the maximum number of scores ({0}).", v_SelectedResult.Scores.Count));
            }
            else if (v_NextPlayerId == Guid.Empty)
            {
                MessageBox.Show("No players available to add (players can only be assigned once to a result).");
            }
            else
            {
                Score v_NewScore = new Score(v_NextPlayerId, "0", false);
                v_SelectedResult.Scores.Add(v_NewScore);
                LbResults.SelectedItem = null;
                LbResults.SelectedItem = v_SelectedResult;
                DbHelper.Instance.IsChanged = true;
                m_UiHelperView.SetFirstButtonEnabledState(true);
            }
        }

        private void buttonDeleteResult_Click(object sender, RoutedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            if (v_SelectedResult == null) { return; }

            if (MessageBox.Show(
                "Do you really want to delete this result?" + Environment.NewLine + Environment.NewLine + "This action is not reversible."
                , "Delete Result"
                , MessageBoxButton.YesNo
                , MessageBoxImage.Warning) == MessageBoxResult.Yes
                )
            {
                LbResults.SelectedItem = null;
                BglDatabase.Results.Remove(v_SelectedResult);

                if (v_SelectedResult.Scores.Count == 1)
                {
                    m_UiHelperView.SetFirstButtonEnabledState(false);
                }
            }
        }

        private void buttonCopyResult_Click(object sender, RoutedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            if (v_SelectedResult == null) { return; }

            Result v_CopiedResult = v_SelectedResult.Copy();
            BglDatabase.Results.Add(v_CopiedResult);
            BglDatabase.SortResults();

            LbResults.SelectedItem = v_CopiedResult;
        }

        private void ButtonApplyChangedResult_Click(object sender, RoutedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            if (v_SelectedResult == null) { return; }

        }

        private void comboBoxPlayerNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayerNumber.SelectedValue != null)
            {
                int v_SelectedPlayerAmount = (int)comboBoxPlayerNumber.SelectedValue;
                m_UiHelperView.ActivateUiElements(v_SelectedPlayerAmount);
            }
        }

        #endregion

        #region Results Editor

        private void comboBoxGamesForResultEntering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;
            BglDatabase.ChangePlayerNumbers(v_SelectedGame.PlayerQuantityMin, v_SelectedGame.PlayerQuantityMax);
            // Using SelectedValue will cause update errors because the SelectionChanged event will sometimes think the value is null.
            comboBoxPlayerAmountEntering.SelectedIndex = v_SelectedGame.PlayerQuantityMax - v_SelectedGame.PlayerQuantityMin;
            comboBoxPlayerAmountEntering.IsEnabled = true;
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
                    LbResults.ItemsSource = BglDatabase.Results;
                }
            }
        }

        #endregion

        #region Reports Tab

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

        private void DeselectAllEntities()
        {
            LbPlayers.SelectedItem = null;
            LbGames.SelectedItem = null;
            LbGameFamilies.SelectedItem = null;
            LbLocations.SelectedItem = null;
            LbResults.SelectedItem = null;
        }

        private void menuItemOpenFile_Click(object sender, RoutedEventArgs e)
        {
            DatabaseChangesWarning();
            OpenFileDialog v_OpenFileDialog = new OpenFileDialog();
            v_OpenFileDialog.Filter = "Text files (*.xml)|*.xml";
            v_OpenFileDialog.InitialDirectory = DbHelper.StandardPath;
            string v_FileNameAndPath = String.Empty;

            if (v_OpenFileDialog.ShowDialog() == true)
            {
                DeselectAllEntities();
                v_FileNameAndPath = v_OpenFileDialog.FileName;
                DbHelper v_DbHelper = DbHelper.Instance;
                v_DbHelper.LoadDataBaseAndRepopulate(v_FileNameAndPath);
                PathAndNameToActiveDb = v_FileNameAndPath;
            }
        }

        private void menuItemNewDb_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog v_SaveFileDialog = new SaveFileDialog();
            v_SaveFileDialog.Filter = "Text files (*.xml)|*.xml";
            v_SaveFileDialog.InitialDirectory = DbHelper.StandardPath;
            string v_FileNameAndPath = String.Empty;

            if (v_SaveFileDialog.ShowDialog() == true)
            {
                v_FileNameAndPath = v_SaveFileDialog.FileName;
                AppHomeFolder.CreationResults v_DbCreationResult = StandardFileBootstrapper.WriteEmptyDatabase(v_FileNameAndPath);

                if (v_DbCreationResult == AppHomeFolder.CreationResults.Created)
                {
                    DbHelper v_DbHelper = DbHelper.Instance;
                    v_DbHelper.LoadDataBaseAndRepopulate(v_FileNameAndPath);
                    PathAndNameToActiveDb = v_FileNameAndPath;
                }
            }
        }

        private void menuItemSaveDb_Click(object sender, RoutedEventArgs e)
        {
            DbHelper.WriteDatabase(BglDatabase, PathAndNameToActiveDb);
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuItemUsage_Click(object sender, RoutedEventArgs e)
        {
            Usage v_UsageWindow = new Usage();

            if (v_UsageWindow.IsWebbrowserOk)
            {
                v_UsageWindow.Show();
            }
            else
            {
                System.Diagnostics.Process.Start("about.html");
            }
        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            About v_AboutWindow = new About();
            v_AboutWindow.Show();
        }

        #endregion

    }
}
