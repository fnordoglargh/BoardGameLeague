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
        BglDb m_BglDatabase;

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

            m_BglDatabase = DbLoader.LoadDatabase("bgldb.xml");

            if (m_BglDatabase == null)
            {
                MessageBox.Show("Loading of database was unsucessful. Application will close. See logs for details.");
                this.Close();
            }

            m_Logger.Info("Backend loading finished. Populating UI with data.");

            // Add context for the list boxes.
            listBoxGameFamilies.DataContext = m_BglDatabase.GameFamilies;
            listBoxLocations.DataContext = m_BglDatabase.Locations;
            listBoxGames.DataContext = m_BglDatabase.Games;

            // The comboBox for Gamefamilies in the Game tab is filled with the same.
            comboBoxGameFamily.ItemsSource = m_BglDatabase.GameFamilies;

            // Fill the comboBox with game types from the enum defined in Game class.
            foreach (Game.GameType i_GameType in Enum.GetValues(typeof(Game.GameType)))
            {
                comboBoxGameType.Items.Add(i_GameType);
            }

            m_Logger.Info("UI Populated. Ready for user actions.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Logger.Info("Application closed.");
            DbLoader.WriteDatabase(m_BglDatabase, "bgldb_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss") + ".xml");
        }

        #region Games

        private void listBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_BglDatabase.SelectedGame = (Game)listBoxGames.SelectedItem;

            Binding v_Binding = new Binding();
            v_Binding.Source = m_BglDatabase.SelectedGame;
            v_Binding.Path = new PropertyPath("PlayerQuantityMin");
            sliderPlayerAmountMin.SetBinding(Slider.ValueProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_BglDatabase.SelectedGame;
            v_Binding.Path = new PropertyPath("PlayerQuantityMax");
            sliderPlayerAmountMax.SetBinding(Slider.ValueProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_BglDatabase.SelectedGame;
            v_Binding.Path = new PropertyPath("Name");
            textBoxGameName.SetBinding(TextBox.TextProperty, v_Binding);

            v_Binding = new Binding();
            v_Binding.Source = m_BglDatabase.SelectedGame;
            v_Binding.Path = new PropertyPath("Type");
            comboBoxGameType.SetBinding(ComboBox.SelectedValueProperty, v_Binding);

            if (m_BglDatabase.SelectedGame != null)
            {
                comboBoxGameFamily.SelectedValue = m_BglDatabase.GameFamiliesById[m_BglDatabase.SelectedGame.IdGamefamily];
            }

            FamilyButtonActivation();
        }

        private void comboBoxGameFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttonNewFamily_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            m_BglDatabase.Games.Add(new Game());
            listBoxGames.SelectedIndex = listBoxGames.Items.Count - 1;
        }

        private void buttonDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            m_BglDatabase.Games.Remove((Game)listBoxGames.SelectedItem);
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
            m_BglDatabase.Locations.Add(new Location());
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
    }
}
