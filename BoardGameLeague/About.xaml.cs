using BoardGameLeagueLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            var paragraph = new Paragraph();
            string v_BuildDateTime = BoardGameLeagueUI.Properties.Resources.BuildDate;
            paragraph.Inlines.Add(new Run(string.Format(VersionWrapper.NameVersionCalling + Environment.NewLine)));
            paragraph.Inlines.Add(new Run("Build Date: " + v_BuildDateTime + Environment.NewLine));
            paragraph.Inlines.Add(new Run("Author: Martin Woelke" + Environment.NewLine));
            paragraph.Inlines.Add(new Run(Environment.NewLine+ "Icon by mattahan from the Buuf icon set https://mattahan.deviantart.com/art/Buuf-37966044."));

            
            rtbAbout.Document.Blocks.Add(paragraph);
        }
    }
}
