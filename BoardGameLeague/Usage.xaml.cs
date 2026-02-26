using BoardGameLeagueLib.Helpers;
using Markdig;
using System;
using System.IO;
using System.Reflection;
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
            IsWebbrowserOk = true;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string v_CallingFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string v_BaseHref = new Uri(v_CallingFolder + Path.DirectorySeparatorChar).AbsoluteUri;

            string v_AboutText = BoardGameLeagueUI.Properties.Resources.ABOUT;
            string v_VersionNameAndBuildTime = VersionWrapper.NameVersionCalling + " - " + BoardGameLeagueUI.Properties.Resources.BuildDate;
            string v_SearchText = "# Features";
            v_AboutText = v_AboutText.Replace(v_SearchText, v_SearchText + "\n\n" + v_VersionNameAndBuildTime);
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            string v_AboutHtmlBody = Markdown.ToHtml(v_AboutText, pipeline);
            string v_AboutHtml = $"<!doctype html><html><head><meta charset=\"utf-8\"><base href=\"{v_BaseHref}\"></head><body>{v_AboutHtmlBody}</body></html>";

            wb.NavigateToString(v_AboutHtml);
        }
    }
}
