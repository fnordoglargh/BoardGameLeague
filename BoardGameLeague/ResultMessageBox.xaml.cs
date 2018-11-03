using System.Windows;

namespace BoardGameLeagueUI
{
    /// <summary>
    /// Interaction logic for ResultMessageBox.xaml
    /// </summary>
    public partial class ResultMessageBox : Window
    {
        public ResultMessageBox(string a_DisplayMessage)
        {
            InitializeComponent();
            TbMessageBlock.Text = a_DisplayMessage;
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
