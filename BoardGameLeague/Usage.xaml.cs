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
            String v_AboutFileName = "about.html";
            String v_PlaceholderString = "ABOUT.md";

            try
            {
                if (File.Exists(v_AboutFileName))
                {
                    string v_Html = File.ReadAllText(v_AboutFileName, Encoding.UTF8);

                    // We want to replace the About.md string with the version name and build time.
                    if (v_Html.Contains(v_PlaceholderString))
                    {
                        String v_VersionNameAndBuildTime = VersionWrapper.NameVersionCalling + " - " + BoardGameLeagueUI.Properties.Resources.BuildDate;
                        v_Html = v_Html.Replace(v_PlaceholderString, v_VersionNameAndBuildTime);
                        File.WriteAllText(v_AboutFileName, v_Html);
                    }

                    if (wb != null)
                    {
                        string curDir = Directory.GetCurrentDirectory();
                        Uri localPath = new Uri(String.Format("file:///{0}/{1}", curDir, v_AboutFileName));
                        wb.Navigate(localPath);
                        //wb.NavigateToString(v_Html);
                    }
                }
                else
                {
                    MessageBox.Show($"The {v_AboutFileName} file is missing. This normally means that grip is not installed and used to build bgl.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("UsageWindow").Error($"Something was not right while opening and displaying [{v_AboutFileName}].", ex);
                MessageBox.Show($"Cannot display {v_AboutFileName}." + Environment.NewLine + Environment.NewLine + ex.Message);
                Close();
            }
        }
    }
}
