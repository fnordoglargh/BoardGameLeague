using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
using BoardGameLeagueLib.ResultRows;
using BoardGameLeagueUI.Charts.Helpers;
using BoardGameLeagueUI.Helpers;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static BoardGameLeagueUI.Charts.Helpers.ChartHelperBase;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class MainWindow : Window
    {
        ILog m_Logger;
        public BglDb BglDatabase { get; private set; }
        UiBuildingHelperScoring m_UiHelperView;
        UiBuildingHelperScoring m_UiHelperNewEntry;
        private SolidColorBrush m_ColorDeactivatedControl = Brushes.White;
        private SolidColorBrush m_ColorActivatedControl = Brushes.Lavender;
        private ControlCategory m_ActualSelection;
        public PointsChartHelper PointSelectionHelper { get; private set; }
        public EloChartHelper EloChartHelper { get; private set; }
        public ResultEditStatusHelper ResultEditStatusHelperInstance { get; private set; }

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

            AppDomain v_CurrentDomain = AppDomain.CurrentDomain;
            v_CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
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

                m_UiHelperView = new UiBuildingHelperScoring(BglDb.c_MaxAmountPlayers, 0);
                m_UiHelperView.GeneratePlayerVariableUiWithRemove(gridResultsView);
                m_UiHelperNewEntry = new UiBuildingHelperScoring(BglDb.c_MaxAmountPlayers, 0);
                m_UiHelperNewEntry.GeneratePlayerVariableUiWithReset(gridResultsEntering);
                m_UiHelperNewEntry.ActivateUiElements(0);

                for (int i = 0; i <= BglDb.c_MaxAmountPlayers; i++)
                {
                    comboBoxPlayerNumber.Items.Add(i);
                }

                m_UiHelperView.RemoveEvent += UiHelperView_RemoveEvent;
                m_UiHelperNewEntry.ChangeEvent += UiHelperNewEntry_ChangeEvent;

                // Without this hack the mouse down events are not registered.
                Players_MouseDown(null, null);

                // Chart Instances
                PointSelectionHelper = new PointsChartHelper();
                PointSelectionHelper.ChartDrawingEvent += PointSelectionHelper_ChartDrawingEvent;
                EloChartHelper = new EloChartHelper();
                EloChartHelper.ChartDrawingEvent += EloChartHelper_ChartDrawingEvent;

                // The status helper for new results is used to change the tab heading.
                ResultEditStatusHelperInstance = new ResultEditStatusHelper("New Result");
                m_Logger.Info("UI Populated. Ready for user actions.");

                CellIndexer.Instance.Reset((BglDatabase.Players.Count+2)*BglDatabase.Games.Count, BglDatabase.Players.Count + 2);

                PlayersOverGames = BglDatabase.GeneratePlayersOverGames2();

                var columns = PlayersOverGames.First()
                    .Properties
                    .Select((x, i) => new { Name = x.Name, Index = i })
                    .ToArray();

                foreach (var column in columns)
                {
                    var binding = new Binding(string.Format("Properties[{0}].Value", column.Index));
                    //binding.Converter = new CellColorConverter();
                    //binding.Path = new PropertyPath("Background");
                    DgPlayersOverGames.Columns.Add(new DataGridTextColumn() { Header = column.Name, Binding = binding });
                }



                //// Create cellstyle
                //Style cellStyle = new Style(typeof(DataGridCell));

                //// Background should be blue
                //cellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Transparent));

                //// If a cell is editing the border should be red
                //Trigger isEditingTrigger = new Trigger();
                //isEditingTrigger.Property = DataGridCell.IsEnabledProperty;
                //isEditingTrigger.Value = true;
                //isEditingTrigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.LightSalmon));

                //cellStyle.Triggers.Add(isEditingTrigger);

                //// Set the cell style for the grid
                //DgPlayersOverGames.CellStyle = cellStyle;

            }
            else
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }
        }

        private void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            m_Logger.Fatal("Logging unhandled exception:", e);
        }

        #region Chart Event Handling

        enum ActiveChart
        {
            Points,
            Elo
        }

        private void ChartDrawingNotification(ActiveChart a_ActiveChart, EventArgs e)
        {
            ChartDrawingEventArgs v_EventArgs = e as ChartDrawingEventArgs;

            if (v_EventArgs.PlayersWithTooFewReults.Count > 0)
            {
                String v_PlayerNamesWithTooFewResults = String.Empty;

                foreach (Player i_Player in v_EventArgs.PlayersWithTooFewReults)
                {
                    v_PlayerNamesWithTooFewResults += i_Player.Name + Environment.NewLine;

                    if (a_ActiveChart == ActiveChart.Elo)
                    {
                        LbPlayersEloSelection.SelectedItems.Remove(i_Player);
                    }
                    else if (a_ActiveChart == ActiveChart.Points)
                    {
                        LbPlayersPointsSelection.SelectedItems.Remove(i_Player);
                    }
                }

                String v_NotificationText = "The following players don't have enough results to be displayed:"
                    + Environment.NewLine
                    + Environment.NewLine
                    + v_PlayerNamesWithTooFewResults;

                MessageBox.Show(v_NotificationText);
            }
        }

        private void PointSelectionHelper_ChartDrawingEvent(object sender, EventArgs e)
        {
            ChartDrawingNotification(ActiveChart.Points, e);
        }

        private void EloChartHelper_ChartDrawingEvent(object sender, EventArgs e)
        {
            ChartDrawingNotification(ActiveChart.Points, e);
        }

        #endregion

        private void UiHelperNewEntry_ChangeEvent(object sender, EventArgs e)
        {
            ResultEditStatusHelperInstance.Changed();
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
            // Without this line the button focus leads to nasty side effects (selecting another item in same category doesn't work).
            if (m_ActualSelection == a_ControlCategory) { return; }

            // Since I added the GetFocus event handlers, applying changed data to underlying objects doesn't work
            // realiably anymore. Giving the focus to the button fixes the problem in a lazy way.
            BtEntityApply.Focus();

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

            BglDatabase.SortCollections();
        }

        private void BtEntitiyNew_Click(object sender, RoutedEventArgs e)
        {
            if (m_ActualSelection == ControlCategory.Location)
            {
                Location v_TempLocation = new Location();
                BglDatabase.Locations.Add(v_TempLocation);
                LbLocations.SelectedItem = v_TempLocation;
                TbLocationName.Focus();
                TbLocationName.SelectAll();
            }
            else if (m_ActualSelection == ControlCategory.Player)
            {
                Player v_TempPlayer = new Player();
                BglDatabase.Players.Add(v_TempPlayer);
                LbPlayers.SelectedItem = v_TempPlayer;
                TbPlayerName.Focus();
                TbPlayerName.SelectAll();
            }
            else if (m_ActualSelection == ControlCategory.Game)
            {
                Game v_TempGame = new Game();
                BglDatabase.Games.Add(v_TempGame);
                LbGames.SelectedItem = v_TempGame;
                TbGameName.Focus();
                TbGameName.SelectAll();
            }
            else if (m_ActualSelection == ControlCategory.GameFamily)
            {
                GameFamily v_TempGameFamily = new GameFamily();
                BglDatabase.GameFamilies.Add(v_TempGameFamily);
                LbGameFamilies.SelectedItem = v_TempGameFamily;
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

        private void BtEntityApply_Click(object sender, RoutedEventArgs e)
        {
            BglDatabase.SortCollections();
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

        /// <summary>
        /// Enables or disbles controls related to games.
        /// </summary>
        /// <param name="a_Status">New IsEnabled status for the controls.</param>
        private void SetGamesControlsEnabledStatus(bool a_Status)
        {
            TbGameName.IsEnabled = a_Status;
            CbGameType.IsEnabled = a_Status;
            SPlayerAmountMin.IsEnabled = a_Status;
            SPlayerAmountMax.IsEnabled = a_Status;
            BtEntityApply.IsEnabled = a_Status;
            BtEntityDelete.IsEnabled = a_Status;
            LbGameFamiliesRefs.IsEnabled = a_Status;
        }

        bool m_IsSelectingGame = false;

        private void LbGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_IsSelectingGame = true;
            LbGameFamiliesRefs.SelectedItem = null;

            if (LbGames.SelectedItem == null)
            {
                SetGamesControlsEnabledStatus(false);
            }
            else
            {
                SetGamesControlsEnabledStatus(true);
                CbGameType_SelectionChanged(null, null);
                Game v_SelectedGame = (Game)LbGames.SelectedItem;

                foreach (Guid i_GameFamilyId in v_SelectedGame.IdGamefamilies)
                {
                    // Select the linked game families of selected game.
                    LbGameFamiliesRefs.SelectedItems.Add(BglDatabase.GameFamiliesById[i_GameFamilyId]);
                }

                // Avoid users from changing the type of a game after it has been used in any results.
                CbGameType.IsEnabled = !BglDatabase.CheckIfGamesIsReferenced(v_SelectedGame.Id);
            }

            m_IsSelectingGame = false;
        }

        private void LbGameFamiliesRefs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Early return if the games list box changed the family selection.
            if (LbGames.SelectedItem == null || m_IsSelectingGame) return;

            Game v_SelectedGame = (Game)LbGames.SelectedItem;
            Guid v_ActualGameFamilyId;

            if (e.RemovedItems.Count > 0)
            {
                // Remove the family from the game if it is linked.
                v_ActualGameFamilyId = ((GameFamily)e.RemovedItems[0]).Id;

                if (v_SelectedGame.IdGamefamilies.Contains(v_ActualGameFamilyId))
                {
                    v_SelectedGame.IdGamefamilies.Remove(v_ActualGameFamilyId);
                }
            }
            else if (e.AddedItems.Count > 0)
            {
                // Add family to the game if is not linked.
                v_ActualGameFamilyId = ((GameFamily)e.AddedItems[0]).Id;

                if (!v_SelectedGame.IdGamefamilies.Contains(v_ActualGameFamilyId))
                {
                    v_SelectedGame.IdGamefamilies.Add(v_ActualGameFamilyId);
                }
            }
        }

        /// <summary>
        /// Conveniently sets the max and min player numbers to 2 if the game type is WinLose and deactivates the sliders.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbGameType.SelectedItem == null) return;

            KeyValuePair<Game.GameType, String> v_SelectedItem = (KeyValuePair<Game.GameType, String>)CbGameType.SelectedItem;

            if (v_SelectedItem.Key == Game.GameType.WinLose)
            {
                SPlayerAmountMin.IsEnabled = false;
                SPlayerAmountMax.IsEnabled = false;
            }
            else
            {
                SPlayerAmountMin.IsEnabled = true;
                SPlayerAmountMax.IsEnabled = true;
            }
        }

        #endregion

        #region Game Families and Locations

        private void SetLocationsControlsEnabledStatus(bool a_Status)
        {
            TbLocationName.IsEnabled = a_Status;
            TbLocationDescription.IsEnabled = a_Status;
            BtEntityApply.IsEnabled = a_Status;
            BtEntityDelete.IsEnabled = a_Status;
        }

        private void LbLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void LbGameFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                MessageBox.Show($"{a_Category} cannot be removed because there are references to the entry. "
                    + "This usually means that the entity is used in a result. If you really want to remove it, "
                    + "all references must be removed by hand before it is possible.");
            }
        }

        /// <summary>
        /// Hides controls depending on the selected game type. WinLooe hides text boxes.
        /// </summary>
        /// <param name="a_SelectedGameType"></param>
        /// <param name="a_IsNewResult">If true the new result tab is used and if false the result editing tab.</param>
        private void SetGameTypeUiActivationStatus(Game.GameType a_SelectedGameType, bool a_IsNewResult)
        {
            if (a_SelectedGameType == Game.GameType.WinLose)
            {
                if (a_IsNewResult)
                {
                    m_UiHelperNewEntry.SwitchTextBoxAndRankVisibility(a_SelectedGameType);
                    LbScore.Visibility = Visibility.Hidden;
                    LbResultEnteringWinner.Visibility = Visibility.Visible;
                }
                else
                {
                    m_UiHelperView.SwitchTextBoxAndRankVisibility(a_SelectedGameType);
                    LbResultViewScore.Visibility = Visibility.Hidden;
                    LbResultViewWinner.Visibility = Visibility.Visible;
                }
            }
            else if (a_SelectedGameType == Game.GameType.VictoryPoints)
            {
                if (a_IsNewResult)
                {
                    m_UiHelperNewEntry.SwitchTextBoxAndRankVisibility(a_SelectedGameType);
                    LbScore.Visibility = Visibility.Visible;
                    LbScore.Content = "Score";
                    LbResultEnteringWinner.Visibility = Visibility.Visible;
                }
                else
                {
                    m_UiHelperView.SwitchTextBoxAndRankVisibility(a_SelectedGameType);
                    LbResultViewScore.Visibility = Visibility.Visible;
                    LbResultViewScore.Content = "Score";
                    LbResultViewWinner.Visibility = Visibility.Visible;
                }
            }
            else if (a_SelectedGameType == Game.GameType.Ranks)
            {
                if (a_IsNewResult)
                {
                    m_UiHelperNewEntry.SwitchTextBoxAndRankVisibility(a_SelectedGameType);
                    LbScore.Visibility = Visibility.Visible;
                    LbScore.Content = "Rank";
                    LbResultEnteringWinner.Visibility = Visibility.Hidden;
                }
                else
                {
                    m_UiHelperView.SwitchTextBoxAndRankVisibility(a_SelectedGameType);
                    LbResultViewScore.Visibility = Visibility.Visible;
                    LbResultViewScore.Content = "Rank";
                    LbResultViewWinner.Visibility = Visibility.Hidden;
                }
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

        private void LbPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        #region Results Editor

        private void UiHelperView_RemoveEvent(object sender, EventArgs e)
        {
            Result v_SelectedResult = ((Result)LbResults.SelectedItem);
            UiBuildingHelperScoring.RemoveEventArgs v_Args = e as UiBuildingHelperScoring.RemoveEventArgs;
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
                    MessageBox.Show(String.Format("This game needs {0} scoring players.", v_PlayedGame.PlayerQuantityMin));
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
                SetGameTypeUiActivationStatus(Game.GameType.VictoryPoints, false);
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

                Game v_ReferencedGame = BglDatabase.GamesById[v_SelectedResult.IdGame];
                SetGameTypeUiActivationStatus(v_ReferencedGame.Type, false);

                if (v_ReferencedGame.Type == Game.GameType.Ranks)
                {
                    int v_ActualScore = 0;
                    for (int i = 0; i < v_SelectedResult.Scores.Count; ++i)
                    {
                        Int32.TryParse(v_SelectedResult.Scores[i].ActualScore, out v_ActualScore);
                        m_UiHelperView.PlayerRanksComboBoxes[i].SelectedItem = v_ActualScore;
                    }
                }

                m_UiHelperView.UpdateBindings(v_SelectedResult);
            }
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
                string v_ActualScoreOrRank = "0";

                if (BglDatabase.GamesById[v_SelectedResult.IdGame].Type == Game.GameType.Ranks || BglDatabase.GamesById[v_SelectedResult.IdGame].Type == Game.GameType.TeamedRanks)
                {
                    // Score needs to be set to a valid value for "ranks". 0 is not valid in that case.
                    v_ActualScoreOrRank = "1";
                }

                Score v_NewScore = new Score(v_NextPlayerId, v_ActualScoreOrRank, false);
                v_SelectedResult.Scores.Add(v_NewScore);
                LbResults.SelectedItem = null;
                LbResults.SelectedItem = v_SelectedResult;
                DbHelper.Instance.IsChanged = true;
                m_UiHelperView.SetFirstButtonEnabledState(true);
            }
        }

        private void BtDeleteResult_Click(object sender, RoutedEventArgs e)
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

        private void BtCopyResult_Click(object sender, RoutedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            if (v_SelectedResult == null) { return; }

            Result v_CopiedResult = v_SelectedResult.Copy();
            BglDatabase.Results.Add(v_CopiedResult);
            LbResults.SelectedItem = v_CopiedResult;
        }

        private void ButtonApplyChangedResult_Click(object sender, RoutedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            if (v_SelectedResult == null) { return; }

            Game v_ReferencedGame = BglDatabase.GamesById[v_SelectedResult.IdGame];
            DbHelper.Instance.IsChanged = true;

            /// If the game type is win/lose, we need to sanitize the text box values. This is done on the button click
            /// because we don't want to delete the values e.g. on a game change alone.
            if (v_ReferencedGame.Type == Game.GameType.WinLose)
            {
                bool v_IsResultADraw = true;

                for (int i = 0; i < (int)comboBoxPlayerNumber.SelectedValue; i++)
                {
                    v_IsResultADraw &= !(bool)m_UiHelperView.PlayerResultCheckBoxes[i].IsChecked;
                }

                for (int i = 0; i < (int)comboBoxPlayerNumber.SelectedValue; i++)
                {
                    if (v_IsResultADraw)
                    {
                        v_SelectedResult.Scores[i].ActualScore = (Result.c_WinLosePointsStalemate).ToString();
                    }
                    else if ((bool)m_UiHelperNewEntry.PlayerResultCheckBoxes[i].IsChecked)
                    {
                        v_SelectedResult.Scores[i].ActualScore = (Result.c_WinLosePointsWin).ToString();
                    }
                    else
                    {
                        v_SelectedResult.Scores[i].ActualScore = (Result.c_WinLosePointsLose).ToString();
                    }
                }
            }
            else if (v_ReferencedGame.Type == Game.GameType.Ranks)
            {
                for (int i = 0; i < v_SelectedResult.Scores.Count; ++i)
                {
                    String v_SelectedRank = m_UiHelperView.PlayerRanksComboBoxes[i].SelectedValue.ToString();
                    v_SelectedResult.Scores[i].ActualScore = v_SelectedRank;

                    if (v_SelectedRank == "1")
                    {
                        m_UiHelperView.PlayerResultCheckBoxes[i].IsChecked = true;
                    }
                    else
                    {
                        m_UiHelperView.PlayerResultCheckBoxes[i].IsChecked = false;
                    }
                }
            }
        }

        private void CbPlayerNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayerNumber.SelectedValue != null)
            {
                int v_SelectedPlayerAmount = (int)comboBoxPlayerNumber.SelectedValue;
                m_UiHelperView.ActivateUiElements(v_SelectedPlayerAmount);
            }
        }

        private void CbGamesForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Result v_SelectedResult = (Result)LbResults.SelectedItem;

            if (v_SelectedResult == null) { return; }

            Game v_ReferencedGame = BglDatabase.GamesById[v_SelectedResult.IdGame];
            SetGameTypeUiActivationStatus(v_ReferencedGame.Type, false);
        }

        #endregion

        #region Results Entering

        private void TbCtr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string v_TabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            if (v_TabItem == (string)tabItemResults.Header)
            {
                ComboBox v_ComboBox = e.Source as ComboBox;
                TabControl v_TabControl = e.Source as TabControl;
                string v_SubTabItemHeader = string.Empty;

                if (e.AddedItems.Count > 0)
                {
                    TabItem v_SubTabItem = e.AddedItems[0] as TabItem;

                    if (v_SubTabItem != null)
                    {
                        v_SubTabItemHeader = v_SubTabItem.Header as string;
                    }
                }

                // A click in the games combo box for new results triggers this event too and needs to be filtered out.
                if (v_ComboBox == null && v_SubTabItemHeader == "Results")
                {
                    Game v_SelecteGame = comboBoxGamesForResultEntering.SelectedItem as Game;

                    // Game could have changed outside so we reselect it it to get the new values applied to the UI.
                    if (v_SelecteGame != null)
                    {
                        comboBoxGamesForResultEntering.SelectedItem = null;
                        comboBoxGamesForResultEntering.SelectedItem = v_SelecteGame;
                        m_Logger.Debug("Refreshed game selection: " + v_SelecteGame.Name);
                    }
                }
            }
        }

        private void CbGamesForResultEntering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;

            if (v_SelectedGame == null) { return; }

            BglDatabase.ChangePlayerNumbers(v_SelectedGame.PlayerQuantityMin, v_SelectedGame.PlayerQuantityMax);
            // Using SelectedValue will cause update errors because the SelectionChanged event will sometimes think the value is null.
            comboBoxPlayerAmountEntering.SelectedIndex = v_SelectedGame.PlayerQuantityMax - v_SelectedGame.PlayerQuantityMin;
            comboBoxPlayerAmountEntering.IsEnabled = true;
            SetGameTypeUiActivationStatus(v_SelectedGame.Type, true);

            // Keep scores if the previously selected game was of the same type.
            if (e.RemovedItems.Count != 0)
            {
                Game v_PreviousGame = e.RemovedItems[0] as Game;

                if (v_PreviousGame != null && v_PreviousGame.Type != v_SelectedGame.Type)
                {
                    // Remove text from text boxes.
                    for (int i = 0; i < BglDb.c_MaxAmountPlayers; i++)
                    {
                        m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text = String.Empty;
                    }
                }
            }

            ResultEditStatusHelperInstance.Changed();
        }

        private void CbPlayerAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayerAmountEntering.SelectedValue != null)
            {
                int v_SelectedPlayerAmount = (int)comboBoxPlayerAmountEntering.SelectedValue;
                m_UiHelperNewEntry.ActivateUiElements(v_SelectedPlayerAmount);
            }
        }

        //private void AddVictoryPointResult()
        //{

        //}

        //private void AddWinLossPointResult()
        //{
        //    bool v_IsP1Winner = (bool)m_UiHelperNewEntry.PlayerResultCheckBoxes[0].IsChecked;
        //    bool v_IsP2Winner = (bool)m_UiHelperNewEntry.PlayerResultCheckBoxes[1].IsChecked;

        //    if (v_IsP1Winner && v_IsP2Winner)
        //    {
        //        MessageBox.Show("Only one winner is possible for " + Game.GameTypeEnumWithCaptions[Game.GameType.WinLose] + " type games."
        //            + Environment.NewLine + Environment.NewLine + "Please select only one player as winner.");
        //    }
        //    else
        //    {

        //    }
        //}

        private void CbLocationsForResultEntering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResultEditStatusHelperInstance.Changed();
        }

        private void BtNewResult_Click(object sender, RoutedEventArgs e)
        {
            Game v_SelectedGame = comboBoxGamesForResultEntering.SelectedValue as Game;

            // Do some early checking: Is a game selected?
            if (v_SelectedGame == null)
            {
                MessageBox.Show("Start by selecting a game.");
                return;
            }

            Location v_Location = CbLocationsForResultEntering.SelectedValue as Location;

            // Do some early checking: Is a location selected?
            if (v_Location == null)
            {
                MessageBox.Show("Select a location for this result.");
                return;
            }

            int v_AmountResultsToAdd = (int)comboBoxPlayerAmountEntering.SelectedValue;
            String v_MessageUser = String.Empty;
            String v_MessageTemp = m_UiHelperNewEntry.TestCheckBoxes(v_AmountResultsToAdd, v_SelectedGame.Type);

            if (v_MessageTemp != String.Empty)
            {
                v_MessageUser += v_MessageTemp + Environment.NewLine + Environment.NewLine;
            }

            v_MessageTemp = m_UiHelperNewEntry.TestComboBoxes(v_AmountResultsToAdd);

            if (v_MessageTemp != String.Empty)
            {
                v_MessageUser += v_MessageTemp + Environment.NewLine;
            }

            v_MessageTemp = m_UiHelperNewEntry.TestTextBoxes(v_AmountResultsToAdd);

            // Only with victory points do we need to report problems with the text box values.
            if (v_MessageTemp != String.Empty && v_SelectedGame.Type == Game.GameType.VictoryPoints)
            {
                v_MessageUser += v_MessageTemp + Environment.NewLine;
            }

            v_MessageTemp = m_UiHelperNewEntry.TestRankComboboxes(v_AmountResultsToAdd);

            if (v_MessageTemp != String.Empty && v_SelectedGame.Type == Game.GameType.Ranks)
            {
                v_MessageUser += v_MessageTemp + Environment.NewLine;
            }

            // We record a result if all checks were fine.
            if (v_MessageUser != String.Empty)
            {
                MessageBox.Show(v_MessageUser);
            }
            else
            {
                String v_ResultDisplay = "";
                ObservableCollection<Score> v_Scores = new ObservableCollection<Score>();
                bool v_IsResultADraw = true;

                for (int i = 0; i < v_AmountResultsToAdd; i++)
                {
                    v_IsResultADraw &= !(bool)m_UiHelperNewEntry.PlayerResultCheckBoxes[i].IsChecked;
                }

                for (int i = 0; i < v_AmountResultsToAdd; i++)
                {
                    v_ResultDisplay += ((Player)m_UiHelperNewEntry.PlayerResultComboBoxes[i].SelectedValue).Name;

                    if (v_SelectedGame.Type == Game.GameType.WinLose)
                    {
                        if (v_IsResultADraw)
                        {
                            m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text = (0.5).ToString();
                        }
                        else if ((bool)m_UiHelperNewEntry.PlayerResultCheckBoxes[i].IsChecked)
                        {
                            m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text = (1).ToString();
                        }
                        else
                        {
                            m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text = (0).ToString();
                        }
                    }
                    else if (v_SelectedGame.Type == Game.GameType.Ranks)
                    {
                        String v_SelectedRank = m_UiHelperNewEntry.PlayerRanksComboBoxes[i].SelectedValue.ToString();
                        m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text = v_SelectedRank;

                        if (v_SelectedRank == "1")
                        {
                            m_UiHelperNewEntry.PlayerResultCheckBoxes[i].IsChecked = true;
                        }
                    }

                    v_ResultDisplay += ": " + m_UiHelperNewEntry.PlayerResultTextBoxes[i].Text + " ";

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

                if (v_SelectedGame.Type == Game.GameType.WinLose && v_IsResultADraw)
                {
                    v_ResultDisplay += Environment.NewLine + "Game is a draw." + Environment.NewLine + Environment.NewLine;
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

                    ResultEditStatusHelperInstance.Reset();
                }
            }
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

            CbEloGames.SelectedItem = null;
            CbEloFamilies.SelectedItem = null;
            CbReportGames.SelectedItem = null;
            CbReportFamilies.SelectedItem = null;
            CbEloGamesChart.SelectedItem = null;
            CbEloFamiliesChart.SelectedItem = null;
            CbPointGamesChart.SelectedItem = null;
            CbPointFamiliesChart.SelectedItem = null;
        }

        private void MiOpenFile_Click(object sender, RoutedEventArgs e)
        {
            DatabaseChangesWarning();

            OpenFileDialog v_OpenFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.xml)|*.xml",
                InitialDirectory = DbHelper.StandardPath
            };

            if (v_OpenFileDialog.ShowDialog() == true)
            {
                DeselectAllEntities();
                string v_FileNameAndPath = v_OpenFileDialog.FileName;
                DbHelper v_DbHelper = DbHelper.Instance;
                v_DbHelper.LoadDataBaseAndRepopulate(v_FileNameAndPath);
                PathAndNameToActiveDb = v_FileNameAndPath;
                m_UiHelperView.UpdateBindings(null);
            }
        }

        private void MiNewDb_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog v_SaveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.xml)|*.xml",
                InitialDirectory = DbHelper.StandardPath
            };

            if (v_SaveFileDialog.ShowDialog() == true)
            {
                string v_FileNameAndPath = v_SaveFileDialog.FileName;
                AppHomeFolder.CreationResults v_DbCreationResult = StandardFileBootstrapper.WriteEmptyDatabase(v_FileNameAndPath);

                if (v_DbCreationResult == AppHomeFolder.CreationResults.Created)
                {
                    DbHelper v_DbHelper = DbHelper.Instance;
                    v_DbHelper.LoadDataBaseAndRepopulate(v_FileNameAndPath);
                    PathAndNameToActiveDb = v_FileNameAndPath;
                }
            }
        }

        private void MiSaveDb_Click(object sender, RoutedEventArgs e)
        {
            DbHelper.WriteDatabase(BglDatabase, PathAndNameToActiveDb);
        }

        private void MiExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MiUsage_Click(object sender, RoutedEventArgs e)
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

        private void MiAbout_Click(object sender, RoutedEventArgs e)
        {
            About v_AboutWindow = new About();
            v_AboutWindow.Show();
        }

        #endregion

        #region Reports Tab

        private void TcReports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TiPlayersOverGames.IsSelected)
            {
                PopulateGamesOverPlayers();
            }
        }

        #region Tables Tab

        private void CbReportGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = CbReportGames.SelectedItem as Game;

            if (v_SelectedGame != null)
            {
                IEnumerable<object> v_ResultRows = BglDatabase.CalculateResultsGamesBase(v_SelectedGame.Id);
                CbReportPlayers.SelectedItem = null;
                CbReportFamilies.SelectedItem = null;
                CbEloGames.SelectedItem = null;
                CbEloFamilies.SelectedItem = null;

                if (v_ResultRows.Count() > 0)
                {
                    dataGrid1.ItemsSource = v_ResultRows;
                }
                else
                {
                    MessageBox.Show("I couldn't find any results for the selected game.");
                    dataGrid1.ItemsSource = null;
                }
            }
            else
            {
                // This is for the unlikely event that a newly created game is used in the reports and then deleted (while still selected).
                CbReportGames.SelectedItem = null;
            }
        }

        private void CbReportFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameFamily v_SelectedGameFamily = CbReportFamilies.SelectedItem as GameFamily;

            if (v_SelectedGameFamily != null)
            {
                // Check if we can make sense of the data.
                var v_AllGamesFromFamily = BglDatabase.Games.Where(p => p.IdGamefamilies.Contains(v_SelectedGameFamily.Id));
                bool v_IsOfSameType = true;

                if (v_AllGamesFromFamily.Count() > 0)
                {
                    Game.GameType v_PreviousType = v_AllGamesFromFamily.First().Type;

                    foreach (Game i_Game in v_AllGamesFromFamily)
                    {
                        if (v_PreviousType != i_Game.Type)
                        {
                            v_IsOfSameType = false;
                        }
                    }

                    // Yes, we can!
                    if (v_IsOfSameType)
                    {
                        CbReportPlayers.SelectedItem = null;
                        CbReportGames.SelectedItem = null;
                        CbEloGames.SelectedItem = null;
                        CbEloFamilies.SelectedItem = null;

                        IEnumerable<object> v_ResultRows = BglDatabase.CalculateResultsGameFamilies(v_SelectedGameFamily.Id);

                        if (v_ResultRows.Count() > 0)
                        {
                            dataGrid1.ItemsSource = v_ResultRows;
                        }
                        else
                        {
                            dataGrid1.ItemsSource = null;
                        }
                    }
                    else
                    {
                        dataGrid1.ItemsSource = null;
                        MessageBox.Show(
                            "All games in a family have to be of the same type for this option to be used in reports."
                            , "Warning"
                            , MessageBoxButton.OK
                            , MessageBoxImage.Warning);
                    }
                }
                else
                {
                    dataGrid1.ItemsSource = null;
                    MessageBox.Show("The selected game family is empty.");
                }
            }
            else
            {
                // This is for the unlikely event that a newly created family is used in the reports and then deleted (while still selected).
                CbReportFamilies.SelectedItem = null;
            }
        }

        private void CbReportPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player v_SelectedPlayer = CbReportPlayers.SelectedItem as Player;

            if (v_SelectedPlayer == null) return;

            IEnumerable<object> v_ResultRows = BglDatabase.GeneratePlayerReport(v_SelectedPlayer.Id);
            CbReportFamilies.SelectedItem = null;
            CbReportGames.SelectedItem = null;
            CbEloGames.SelectedItem = null;
            CbEloFamilies.SelectedItem = null;

            if (v_ResultRows.Count() > 0)
            {
                dataGrid1.ItemsSource = v_ResultRows;
            }
            else
            {
                MessageBox.Show("I couldn't find any results for the selected player.");
                dataGrid1.ItemsSource = null;
            }
        }

        private void EloCalculation(Guid a_GameOrFamilyId)
        {
            Dictionary<Player, Result.ResultHelper> v_EloResults = BglDatabase.CalculateEloResults(a_GameOrFamilyId);
            ObservableCollection<EloCalculator.EloResultRow> v_EloResultRows = new ObservableCollection<EloCalculator.EloResultRow>();

            foreach (KeyValuePair<Player, Result.ResultHelper> i_EloResult in v_EloResults)
            {
                v_EloResultRows.Add(new EloCalculator.EloResultRow(i_EloResult.Key.Name, i_EloResult.Value.EloScore, i_EloResult.Value.AmountGamesPlayed, i_EloResult.Value.IsEstablished));
            }

            dataGrid1.ItemsSource = v_EloResultRows;
        }

        private void BtTestELO_Click(object sender, RoutedEventArgs e)
        {
            EloCalculation(Guid.Empty);
            CbReportGames.SelectedItem = null;
            CbReportFamilies.SelectedItem = null;
            CbEloGames.SelectedItem = null;
            CbEloFamilies.SelectedItem = null;
        }

        private void CbEloGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = CbEloGames.SelectedItem as Game;

            if (v_SelectedGame == null) { return; }

            EloCalculation(v_SelectedGame.Id);

            CbReportGames.SelectedItem = null;
            CbReportFamilies.SelectedItem = null;
            CbEloFamilies.SelectedItem = null;
        }

        private void CbEloFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameFamily v_SelectedGameFamily = CbEloFamilies.SelectedItem as GameFamily;

            if (v_SelectedGameFamily == null) { return; }

            EloCalculation(v_SelectedGameFamily.Id);

            CbReportGames.SelectedItem = null;
            CbReportFamilies.SelectedItem = null;
            CbEloGames.SelectedItem = null;
        }

        public void DG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGrid v_SenderGrid = sender as DataGrid;

            if (v_SenderGrid == null) { return; }

            if (v_SenderGrid.Items.Count == 0) { return; }

            ResultRow v_TempRow = null;
            EloCalculator.EloResultRow v_TempEloRow = null;

            if (v_SenderGrid.Items[0].GetType() == typeof(ResultRowRanks))
            {
                v_TempRow = v_SenderGrid.Items[0] as ResultRowRanks;
            }
            else if (v_SenderGrid.Items[0].GetType() == typeof(ResultRowVictoryPoints))
            {
                v_TempRow = v_SenderGrid.Items[0] as ResultRowVictoryPoints;
            }
            else if (v_SenderGrid.Items[0].GetType() == typeof(ResultRowWinLose))
            {
                v_TempRow = v_SenderGrid.Items[0] as ResultRowWinLose;
            }
            else if (v_SenderGrid.Items[0].GetType() == typeof(EloCalculator.EloResultRow))
            {
                v_TempEloRow = v_SenderGrid.Items[0] as EloCalculator.EloResultRow;
            }
            else if (v_SenderGrid.Items[0].GetType() == typeof(ResultRowPlayer))
            {
                v_TempRow = v_SenderGrid.Items[0] as ResultRowPlayer;
            }

            if (v_TempRow == null && v_TempEloRow == null) { return; }

            string v_Headername = e.Column.Header.ToString();

            //Cancel the column you don't want to generate.
            if (v_Headername == "ColumnNames")
            {
                e.Cancel = true;
            }
            else
            {
                // Ugly, but it works.
                if (v_TempRow != null)
                {
                    e.Column.Header = v_TempRow.ColumnNames[v_Headername].Key;

                    if (v_TempRow.ColumnNames[v_Headername].Value != -1)
                    {
                        e.Column.DisplayIndex = v_TempRow.ColumnNames[v_Headername].Value;
                    }
                }
                else if (v_TempEloRow != null)
                {
                    e.Column.Header = v_TempEloRow.ColumnNames[v_Headername].Key;
                    e.Column.DisplayIndex = v_TempEloRow.ColumnNames[v_Headername].Value;
                }
            }
        }

        #endregion

        #region Elo Charts

        private void CbEloMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbEloMode.SelectedItem != null)
            {
                EloChartHelper.GameOrFamilyId = Guid.Empty;
                CbEloFamiliesChart.SelectedItem = null;
                CbEloGamesChart.SelectedItem = null;
            }
        }

        private void CbEloGamesChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = CbEloGamesChart.SelectedItem as Game;

            if (v_SelectedGame != null)
            {
                EloChartHelper.GameOrFamilyId = v_SelectedGame.Id;
                CbEloFamiliesChart.SelectedItem = null;
                CbEloMode.SelectedItem = null;
            }
        }

        private void CbEloFamiliesChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameFamily v_SelectedGameFamily = CbEloFamiliesChart.SelectedItem as GameFamily;

            if (v_SelectedGameFamily != null)
            {
                EloChartHelper.GameOrFamilyId = v_SelectedGameFamily.Id;
                CbEloGamesChart.SelectedItem = null;
                CbEloMode.SelectedItem = null;
            }
        }

        private void LbPlayersEloSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EloChartHelper.SelectedPlayers = (IList<object>)LbPlayersEloSelection.SelectedItems;
        }

        private void BtZoomToggleElo_Click(object sender, RoutedEventArgs e)
        {
            EloChartHelper.Chart.ToogleZoomingMode();
        }

        private void BtZoomResetElo_Click(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            AxisXElo.MinValue = double.NaN;
            AxisXElo.MaxValue = double.NaN;
            AxisYElo.MinValue = double.NaN;
            AxisYElo.MaxValue = double.NaN;
        }

        #endregion

        #region Point Progression

        private void CbPointGamesChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game v_SelectedGame = CbPointGamesChart.SelectedItem as Game;

            if (v_SelectedGame != null)
            {
                PointSelectionHelper.GameOrFamilyId = v_SelectedGame.Id;
                CbPointFamiliesChart.SelectedItem = null;
            }
        }

        private void CbPointFamiliesChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameFamily v_SelectedGameFamily = CbPointFamiliesChart.SelectedItem as GameFamily;

            if (v_SelectedGameFamily != null)
            {
                PointSelectionHelper.GameOrFamilyId = v_SelectedGameFamily.Id;
                CbPointGamesChart.SelectedItem = null;
            }
        }

        private void CbPointMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PointSelectionHelper.ActualMode = ((KeyValuePair<PointsChartHelper.CalculationMode, String>)CbPointMode.SelectedValue).Key;
        }

        private void LbPlayersPointsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PointSelectionHelper.SelectedPlayers = (IList<object>)LbPlayersPointsSelection.SelectedItems;
        }

        private void BtZoomTogglePoints_Click(object sender, RoutedEventArgs e)
        {
            PointSelectionHelper.Chart.ToogleZoomingMode();
        }

        private void BtZoomResetPoints_Click(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            AxisXPoints.MinValue = double.NaN;
            AxisXPoints.MaxValue = double.NaN;
            AxisYPoints.MinValue = double.NaN;
            AxisYPoints.MaxValue = double.NaN;
        }

        #endregion

        #region Games over Players

        public ObservableCollection<GenericResultRow> PlayersOverGames { get; set; }

        private void PopulateGamesOverPlayers()
        {
            //DgPlayersOverGames.DataContext = BglDatabase.GeneratePlayersOverGames().DefaultView;
        }

        #endregion

        #endregion
    }

    public class CellColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v_Value = -1;

            if (int.TryParse(value.ToString(), out v_Value))
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.Transparent;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
