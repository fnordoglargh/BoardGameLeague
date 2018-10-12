using BoardGameLeagueLib.Helpers;
using System.Windows;
using static BoardGameLeagueLib.Helpers.SettingsHelper;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        Settings m_Preferences;

        public Preferences()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_Preferences = SettingsHelper.Instance.Preferences;

            CbNormalizeDates.IsChecked = m_Preferences.IsDateNormalized;
            CbTransparentGraphArea.IsChecked = m_Preferences.IsGraphAreaTransparent;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsHelper.Instance.SaveSettings();
        }

        private void CbTransparentGraphArea_Checked(object sender, RoutedEventArgs e)
        {
            m_Preferences.IsGraphAreaTransparent = true;
        }

        private void CbTransparentGraphArea_Unchecked(object sender, RoutedEventArgs e)
        {
            m_Preferences.IsGraphAreaTransparent = false;
        }

        private void CbNormalizeDates_Checked(object sender, RoutedEventArgs e)
        {
            m_Preferences.IsDateNormalized = true;
        }

        private void CbNormalizeDates_Unchecked(object sender, RoutedEventArgs e)
        {
            m_Preferences.IsDateNormalized = false;
        }
    }
}
