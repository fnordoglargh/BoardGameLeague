using BoardGameLeagueLib.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Paragraph para = new Paragraph();

            para.Inlines.Add(new Run(VersionWrapper.NameVersionCalling + Environment.NewLine));
            para.Inlines.Add(new Run("Build Date: " + BoardGameLeagueUI.Properties.Resources.BuildDate + Environment.NewLine));
            para.Inlines.Add(new Run("Write the author: "));

            Hyperlink v_LinkToMail = new Hyperlink(new Run("Martin Woelke"));
            v_LinkToMail.NavigateUri = new Uri("mailto:bgl.boardgameleague@gmail.com");
            v_LinkToMail.RequestNavigate += Hyperlink_RequestNavigate;
            para.Inlines.Add(v_LinkToMail);
            para.Inlines.Add(" (bgl.boardgameleague@gmail.com)" + Environment.NewLine + Environment.NewLine);

            para.Inlines.Add(new Run("Icon by mattahan from the "));
            Run v_LinkText = new Run("Buuf icon set.");
            Hyperlink v_LinkToIconSet = new Hyperlink();
            v_LinkToIconSet.IsEnabled = true;
            v_LinkToIconSet.Inlines.Add("Buuf icon set.");
            v_LinkToIconSet.NavigateUri = new Uri("https://mattahan.deviantart.com/art/Buuf-37966044");
            v_LinkToIconSet.RequestNavigate += Hyperlink_RequestNavigate;
            para.Inlines.Add(v_LinkToIconSet);

            para.Inlines.Add(Environment.NewLine + Environment.NewLine + "Licensed under ");
            Hyperlink v_LinkToLicense = new Hyperlink();
            v_LinkToLicense.IsEnabled = true;
            v_LinkToLicense.Inlines.Add("GNU GPLv2.");
            v_LinkToLicense.NavigateUri = new Uri("https://choosealicense.com/licenses/gpl-2.0/");
            v_LinkToLicense.RequestNavigate += Hyperlink_RequestNavigate;
            para.Inlines.Add(v_LinkToLicense);

            rtb.Document.Blocks.Add(para);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri.ToString()));
            e.Handled = true;
        }
    }
}
