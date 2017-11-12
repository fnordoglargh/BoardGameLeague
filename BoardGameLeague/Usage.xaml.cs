using BoardGameLeagueLib.Helpers;
using Cyotek.ApplicationServices.Windows.Forms;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Usage : Window
    {
        public bool IsWebbrowserOk { get; private set; }

        public Usage()
        {
            if (!Cyotek.ApplicationServices.Windows.Forms.InternetExplorerBrowserEmulation.IsBrowserEmulationSet())
            {
                IsWebbrowserOk = InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();
            }
            else
            {
                IsWebbrowserOk = true;
            }

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string v_Html = File.ReadAllText("about.html", Encoding.UTF8);
                String v_VersionNameAndBuildTime = VersionWrapper.NameVersionCalling + " - " + BoardGameLeagueUI.Properties.Resources.BuildDate;
                v_Html = v_Html.Replace("ABOUT.md", v_VersionNameAndBuildTime);

                if (wb != null)
                {
                    wb.NavigateToString(v_Html);
                }
            }
            catch
            {
                MessageBox.Show("The about.html file is missing. This normally means that grip is not installed and used to build bgl.");
                Close();
            }
        }
    }
}
