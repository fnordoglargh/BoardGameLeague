using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Usage : Window
    {
        public Usage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Assembly v_Assembly = Assembly.GetExecutingAssembly();
            List<String> v_EmbeddedResourceNames = new List<string>(v_Assembly.GetManifestResourceNames());
            int v_IndexInAssembly = v_EmbeddedResourceNames.FindIndex(x => x.Contains("Usage.rtf"));
            String v_PathToReadFrom = v_EmbeddedResourceNames[v_IndexInAssembly];
            TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            range.Load(v_Assembly.GetManifestResourceStream(v_PathToReadFrom), System.Windows.DataFormats.Rtf);
        }

        /// <summary>
        /// This helper lets us click links in the loaded RTF and open the it in the default browser. Found the solution here:
        /// https://stackoverflow.com/questions/762271/clicking-hyperlinks-in-a-richtextbox-without-holding-down-ctrl-wpf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }
    }
}
