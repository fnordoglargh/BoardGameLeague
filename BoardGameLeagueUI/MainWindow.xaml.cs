using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using BoardGameLeagueLib;
using System.Data;
using System.Collections;
using log4net;
using log4net.Config;
using System.IO;
using BoardGameLeagueLib.Helpers;
using System.Threading;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbClass m_Database;
        DataSet m_DataSet = new DataSet();
        List<TextBox> m_PlayerResultBoxes = new List<TextBox>();
        List<ComboBox> m_PlayerResultCombos = new List<ComboBox>();
        List<CheckBox> m_PlayerResultChecks = new List<CheckBox>();
        ILog m_Logger;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.Name = "MainWindow";
            XmlConfigurator.Configure(new FileInfo("log4netConfig.xml"));
            m_Logger = LogManager.GetLogger(Thread.CurrentThread.Name);
            m_Logger.Info("*****************************************************************");
            m_Logger.Info("Welcome to " + VersionWrapper.NameVersionExecuting);
            m_Logger.Info("Logger loaded.");
            m_Logger.Debug("Window starts loading.");
            m_Database = new DbClass();

            if (!m_Database.BootStrap())
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }

            m_Logger.Info("Backend loading finished. Populating UI with data.");

            //List<GameFamily> v_Games = new List<GameFamily>();
            //v_Games.Add(new GameFamily("asdfgajsldgh"));
            //DbLoader.WriteWithXmlSerializer("g.xml", v_Games);

            //v_Games = null;
            //v_Games = (List<GameFamily>)DbLoader.ReadWithXmlSerializer("g.xml", typeof(GameFamily));

            listBoxPlayers.DataContext = m_Database.Persons;
            listBoxGames.DataContext = m_Database.Games;

            comboBoxGameType.Items.Add(Game.GameType.VictoryPoints);
            comboBoxGameType.Items.Add(Game.GameType.Ranks);
            comboBoxGameType.Items.Add(Game.GameType.WinLoose);

            comboBoxGender.Items.Add(Person.Genders.Male);
            comboBoxGender.Items.Add(Person.Genders.Female);

            //comboBoxGameFamily.DataContext = m_Database.GameFamilies;

            //foreach (GameFamily i_GhameFamily in m_Database.GameFamilies)
            //{
            //    comboBoxGameFamily.Items.Add(i_GhameFamily);
            //}


            comboBoxGameFamily.ItemsSource = m_Database.GameFamilies;

            //comboBoxGameFamily.DataContext = m_Database.GameFamilies;

            listBoxGameFamilies.DataContext = m_Database.GameFamilies;
            listBoxLocations.DataContext = m_Database.Locations;
            listBoxResults.DataContext = m_Database.Results;

            comboBoxLocationsForResult.ItemsSource = m_Database.Locations;
            comboBoxGamesForResult.ItemsSource = m_Database.Games;
            comboBoxReportGames.ItemsSource = m_Database.Games;

            m_PlayerResultBoxes.Add(textBoxResultScorePlayer1);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer2);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer3);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer4);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer5);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer6);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer7);
            m_PlayerResultBoxes.Add(textBoxResultScorePlayer8);

            m_PlayerResultCombos.Add(comboBoxResultScorePlayer1);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer2);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer3);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer4);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer5);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer6);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer7);
            m_PlayerResultCombos.Add(comboBoxResultScorePlayer8);

            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer1);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer2);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer3);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer4);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer5);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer6);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer7);
            m_PlayerResultChecks.Add(checkBoxResultWinnerPlayer8);

            foreach (ComboBox i_Box in m_PlayerResultCombos)
            {
                i_Box.ItemsSource = m_Database.Persons;
            }

            // TODO: The maximum should come from a central location.
            for (int i = 1; i < 9; i++)
            {
                comboBoxPlayerAmount.Items.Add(i);
            }

            comboBoxPlayerAmount.SelectedValue = 1;

            // Dataset Versuch
            /*
            m_DataSet.ReadXml("db\\bgldb.xml");
            DataTable v_Table1=m_DataSet.Tables["GameFamily"];
            DataTable v_Table2=m_DataSet.Tables["Game"];
            DataColumn v_Column1 = v_Table1.Columns["Id"];
            DataColumn v_Column2 = v_Table2.Columns["IdGamefamily"];

            DataRelation v_DataRelation = new DataRelation("FamiliesToGame", v_Column1, v_Column2);
            m_DataSet.Relations.Add(v_DataRelation);
            */

            m_Logger.Info("UI Populated. Ready for user actions.");
        }

        //private bool Bootstrap()
        //{
        //    bool v_IsLoadable = false;
        //    string v_LogConfigPathAndName = AppHomeFolder.GetHomeFolderPath(VersionWrapper.CompanyCalling, VersionWrapper.NameCalling);

        //    if (v_LogConfigPathAndName != string.Empty)
        //    {
        //        bool v_IsPathValid = AppHomeFolder.TestHomeFolder(v_LogConfigPathAndName);

        //        if (v_IsPathValid)
        //        {
        //            v_LogConfigPathAndName = v_LogConfigPathAndName + "log4net.xml";
        //            bool v_IsLogConfigPresent = File.Exists(v_LogConfigPathAndName);

        //            if (v_IsLogConfigPresent)
        //            {
        //                XmlConfigurator.Configure(new FileInfo(v_LogConfigPathAndName));
        //                m_Logger = LogManager.GetLogger("AppHomeFolder");
        //                v_IsLoadable = true;
        //                m_Logger.Info("Logger loaded.");
        //            }
        //            else
        //            {
        //                Debug.WriteLine(v_LogConfigPathAndName + " is missing!");
        //            }
        //        }
        //        else
        //        {
        //            Debug.WriteLine(v_LogConfigPathAndName + " is not valid!");
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine(v_LogConfigPathAndName + " is not valid!");
        //    }

        //    return v_IsLoadable;
        //}

        private void listBoxPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedPlayer = (Person)listBoxPlayers.SelectedItem;

            Binding v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedPlayer;
            v_Binding.Path = new PropertyPath("Gender");
            comboBoxGender.SetBinding(ComboBox.SelectedValueProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedPlayer;
            v_Binding.Path = new PropertyPath("Name");
            textBoxName.SetBinding(TextBox.TextProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedPlayer;
            v_Binding.Path = new PropertyPath("DisplayName");
            textBoxDisplayName.SetBinding(TextBox.TextProperty, v_Binding);
        }

        private void buttonDeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            Person v_Person = (Person)listBoxPlayers.SelectedItem;
            m_Database.Persons.Remove(v_Person);
            //MessageBox.Show("Action not implemented yet!");
        }

        private void buttonNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            Person v_Person = new Person();
            m_Database.Persons.Add(v_Person);
            listBoxPlayers.SelectedIndex = listBoxPlayers.Items.Count - 1;
        }

        private void listBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedGame = (Game)listBoxGames.SelectedItem;

            Binding v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedGame;
            v_Binding.Path = new PropertyPath("PlayerQuantityMin");
            sliderPlayerAmountMin.SetBinding(Slider.ValueProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedGame;
            v_Binding.Path = new PropertyPath("PlayerQuantityMax");
            sliderPlayerAmountMax.SetBinding(Slider.ValueProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedGame;
            v_Binding.Path = new PropertyPath("Name");
            textBoxGameName.SetBinding(TextBox.TextProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedGame;
            v_Binding.Path = new PropertyPath("Type");
            comboBoxGameType.SetBinding(ComboBox.SelectedValueProperty, v_Binding);

            comboBoxGameFamily.SelectedValue = m_Database.GameFamiliesById[m_Database.SelectedGame.IdGamefamily];

            FamilyButtonActivation();
        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            Game v_GameToAdd = new Game();
            m_Database.Games.Add(v_GameToAdd);
            listBoxGames.SelectedIndex = listBoxGames.Items.Count - 1;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            m_Database.SelectedGame.Name = textBoxGameName.Text;
        }

        private void buttonNewGame_Click_1(object sender, RoutedEventArgs e)
        {
            Game v_Game = new Game();
            m_Database.Games.Add(v_Game);
            listBoxGames.SelectedIndex = listBoxGames.Items.Count - 1;
        }

        private void comboBoxGameFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedGame = (Game)listBoxGames.SelectedItem;
            m_Database.SelectedGame.IdGamefamily = ((GameFamily)comboBoxGameFamily.SelectedItem).Id;
            FamilyButtonActivation();
            //Console.WriteLine("comboBoxGameFamily_SelectionChanged"+((GameFamily)comboBoxGameFamily.SelectedItem).Id);
        }

        private void listBoxGameFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Binding v_Binding = new Binding();
            v_Binding.Source = listBoxGameFamilies.SelectedValue;
            v_Binding.Path = new PropertyPath("Name");
            textBoxFamilyName.SetBinding(TextBox.TextProperty, v_Binding);
        }

        private void buttonNewFamily_Click(object sender, RoutedEventArgs e)
        {
            GameFamily v_TempFamily = new GameFamily(m_Database.SelectedGame.Name);
            m_Database.GameFamilies.Add(v_TempFamily);
            m_Database.GameFamiliesById.Add(v_TempFamily.Id, v_TempFamily);
            m_Database.SelectedGame.IdGamefamily = v_TempFamily.Id;
            comboBoxGameFamily.SelectedIndex = comboBoxGameFamily.Items.Count - 1;
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

        #region ResultsTab

        private void listBoxResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedResult = (Result)listBoxResults.SelectedItem;
            Console.WriteLine(m_Database.SelectedResult.Date + " " + m_Database.LocationsById[m_Database.SelectedResult.IdLocation]);

            comboBoxGamesForResult.SelectedValue = m_Database.GamesById[m_Database.SelectedResult.IdGame];
            comboBoxLocationsForResult.SelectedValue = m_Database.LocationsById[m_Database.SelectedResult.IdLocation];

            Binding v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedResult;
            v_Binding.Path = new PropertyPath("Date");
            calendarResult.SetBinding(Calendar.SelectedDateProperty, v_Binding);
            calendarResult.DisplayDate = m_Database.SelectedResult.Date;

            foreach (TextBox i_TextBox in m_PlayerResultBoxes)
            {
                BindingOperations.ClearBinding(i_TextBox, TextBox.TextProperty);
            }

            foreach (ComboBox i_Box in m_PlayerResultCombos)
            {
                i_Box.SelectedValue = null;
            }

            foreach (CheckBox i_CheckBox in m_PlayerResultChecks)
            {
                i_CheckBox.IsChecked = false;
            }

            int i = 0;

            foreach (Score i_Score in m_Database.SelectedResult.Scores)
            {
                v_Binding = new Binding();
                v_Binding.Source = i_Score;
                v_Binding.Path = new PropertyPath("ActualScore");
                m_PlayerResultBoxes[i].SetBinding(TextBox.TextProperty, v_Binding);

                m_PlayerResultCombos[i].SelectedValue = m_Database.PersonsById[i_Score.IdPerson];

                if (m_Database.SelectedResult.Winners.Contains(i_Score.IdPerson))
                {
                    m_PlayerResultChecks[i].IsChecked = true;
                }

                i++;
            }

            comboBoxPlayerAmount.SelectedItem = m_Database.SelectedResult.Scores.Count;
        }

        private void calendarResult_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //bool v_IsSearchingForIndex = false;
            //int v_IndexToInsertAt = -1;

            //Debug.WriteLine("Index of Result: " + m_Database.Results.IndexOf(m_Database.SelectedResult));

            //m_Database.Results.Remove(m_Database.SelectedResult);

            //while (v_IsSearchingForIndex)
            //{
            //    foreach (Result i_Result in m_Database.Results)
            //    {
            //        if (i_Result.Date > m_Database.SelectedResult.Date)
            //        {
            //            v_IndexToInsertAt = m_Database.Results.IndexOf(i_Result);
            //            m_Database.Results.Insert(v_IndexToInsertAt, m_Database.SelectedResult);
            //            return;
            //        }
            //    }
            //}
        }

        private void comboBoxGamesForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //m_Database.SelectedGame = (Game)comboBoxGamesForResult.SelectedItem;
            //m_Database.SelectedResult.IdGame = m_Database.SelectedGame.Id;
        }

        private void comboBoxLocationsForResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedLocation = (Location)comboBoxLocationsForResult.SelectedItem;
            m_Database.SelectedResult.IdLocation = m_Database.SelectedLocation.Id;
        }

        private void buttonNewResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonAddResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonDeleteResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCopyResult_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxResults.SelectedItem != null)
            {
                Result v_TempResult = (Result)listBoxResults.SelectedItem;
                m_Database.Results.Add(v_TempResult.Copy());
                listBoxResults.SelectedIndex = listBoxResults.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("No result selected!");
            }
        }

        private void buttonResultSave_Click(object sender, RoutedEventArgs e)
        {
            m_Database.SaveDatabase();
        }


        private void checkBoxResultWinnerPlayer_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox v_Checkbox = (CheckBox)sender;
            Person v_Person = GetPersonFromCheckBox(v_Checkbox);

            if (v_Person != null)
            {
                Debug.WriteLine(v_Person.Id + " is no more winner!");
            }
        }

        private Person GetPersonFromCheckBox(CheckBox a_Checkbox)
        {
            int v_Index = m_PlayerResultChecks.IndexOf(a_Checkbox);
            Person v_Person = ((Person)m_PlayerResultCombos[v_Index].SelectedItem);

            return v_Person;
        }

        private void checkBoxResultWinnerPlayer_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox v_Checkbox = (CheckBox)sender;

        }

        private void comboBoxPlayerAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayerAmount.SelectedValue != null)
            {
                Console.WriteLine((int)comboBoxPlayerAmount.SelectedValue);

                for (int i = 0; i < 8; i++)
                {
                    if (i < (int)comboBoxPlayerAmount.SelectedValue)
                    {
                        m_PlayerResultBoxes[i].IsEnabled = true;
                        m_PlayerResultCombos[i].IsEnabled = true;
                    }
                    else
                    {
                        m_PlayerResultBoxes[i].IsEnabled = false;
                        m_PlayerResultCombos[i].IsEnabled = false;
                        //m_PlayerResultCombos[i].SelectedValue = null;
                    }
                }
            }
            else
            {
                m_PlayerResultBoxes[0].IsEnabled = true;
            }
        }

        #endregion

        private void comboBoxReportGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedGame = (Game)comboBoxReportGames.SelectedItem;

            // Dataset Versuch
            DataTable v_Table2 = new DataTable();
            v_Table2.Columns.Add("Rank");
            v_Table2.Columns.Add("Name");
            v_Table2.Columns.Add("Played");
            v_Table2.Columns.Add("Won");
            v_Table2.Columns.Add("Percentage");
            v_Table2.Columns.Add("Points");
            v_Table2.Columns.Add("Average");

            Dictionary<Guid, DbClass.CalculatedResult> v_CalculatedResults = new Dictionary<Guid, DbClass.CalculatedResult>();
            DbClass.CalculatedResult v_TempResult;

            foreach (Result i_Result in m_Database.Results)
            {
                if (i_Result.IdGame.Equals(m_Database.SelectedGame.Id))
                {

                    foreach (Score i_Score in i_Result.Scores)
                    {
                        if (!v_CalculatedResults.ContainsKey(i_Score.IdPerson))
                        {
                            v_TempResult = new DbClass.CalculatedResult();
                            v_TempResult.Name = ((Person)m_Database.PersonsById[i_Score.IdPerson]).DisplayName;
                            v_CalculatedResults.Add(i_Score.IdPerson, v_TempResult);
                        }

                        v_TempResult = (DbClass.CalculatedResult)v_CalculatedResults[i_Score.IdPerson];
                        v_TempResult.Points += int.Parse(i_Score.ActualScore);
                        v_TempResult.AmountPlayedGamed++;
                        v_CalculatedResults[i_Score.IdPerson] = v_TempResult;
                    }

                    foreach (Guid i_Id in i_Result.Winners)
                    {
                        v_TempResult = (DbClass.CalculatedResult)v_CalculatedResults[i_Id];
                        v_TempResult.AmountGamesWon++;
                        v_CalculatedResults[i_Id] = v_TempResult;
                    }
                }
            }

            foreach (KeyValuePair<Guid, DbClass.CalculatedResult> i_CalculatedcResult in v_CalculatedResults)
            {
                v_TempResult = i_CalculatedcResult.Value;
                double v_AveragePoints = (double)v_TempResult.Points / v_TempResult.AmountPlayedGamed;
                double v_Percentage;

                if (v_TempResult.AmountGamesWon != 0)
                {
                    v_Percentage = (double)v_TempResult.AmountGamesWon / v_TempResult.AmountPlayedGamed;
                }
                else
                {
                    v_Percentage = 0;
                }

                v_Table2.Rows.Add(1, v_TempResult.Name, v_TempResult.AmountPlayedGamed, v_TempResult.AmountGamesWon, Math.Round(v_Percentage, 2), v_TempResult.Points, Math.Round(v_AveragePoints, 2));
            }

            dataGrid1.DataContext = v_Table2;
        }

        private void buttonNewLocation_Click(object sender, RoutedEventArgs e)
        {
            Location v_Location = new Location();
            m_Database.Locations.Add(v_Location);
            listBoxLocations.SelectedIndex = listBoxLocations.Items.Count - 1;
        }

        private void listBoxLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Database.SelectedLocation = (Location)listBoxLocations.SelectedItem;

            Binding v_Binding = new Binding();
            v_Binding.Source = m_Database.SelectedLocation;
            v_Binding.Path = new PropertyPath("Name");
            textBoxLocationNmae.SetBinding(TextBox.TextProperty, v_Binding);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //m_Database.SaveDatabase();
            m_Logger.Info("Application closed.");
        }

        private void buttonSaveDatabase_Click(object sender, RoutedEventArgs e)
        {
            String v_DisplayResult = String.Empty;

            bool v_IsDatabaseSaved = m_Database.SaveDatabase();

            if (v_IsDatabaseSaved)
            {
                MessageBox.Show("Database have been saved (or so it seems).");
            }
            else
            {
                MessageBox.Show("Database has NOT been saved.");
            }
        }

        #region Helpers

        public int GetIndexFromElementName(String a_ElementName)
        {
            int v_Index;
            int.TryParse(a_ElementName[a_ElementName.Length - 1].ToString(), out v_Index);

            return v_Index;
        }

        #endregion
    }
}
