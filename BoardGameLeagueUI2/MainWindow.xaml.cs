﻿using BoardGameLeagueLib;
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

            listBoxGameFamilies.DataContext = m_BglDatabase.GameFamilies;
            listBoxLocations.DataContext = m_BglDatabase.Locations;

            m_Logger.Info("UI Populated. Ready for user actions.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Logger.Info("Application closed.");
            DbLoader.WriteDatabase(m_BglDatabase, "bgldb_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss") + ".xml");
        }

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
