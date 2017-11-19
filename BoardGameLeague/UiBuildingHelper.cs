using BoardGameLeagueLib.Converters;
using BoardGameLeagueLib.DbClasses;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BoardGameLeagueUI
{
    public class UiBuildingHelper
    {
        private ILog m_Logger = LogManager.GetLogger("UiBuildingHelper");
        private const int m_WidthTextBox = 30;
        private const int m_WidthComboBox = 180;
        private const int m_WidthButton = 75;
        private const int m_HeightTextBox = 23;
        private int m_StartX = 400;
        private int m_XTextBox = 0;
        private int m_XComboBox = 30;
        private int m_XCheckBox = 317;
        private int m_XButton = 352;
        private const int m_FirstLineY = 98;
        private const int m_IncrementY = 29;
        private int m_PlayerAmount = BglDb.c_MaxAmountPlayers;
        public List<TextBox> PlayerResultTextBoxes = new List<TextBox>();
        public List<ComboBox> PlayerResultComboBoxes = new List<ComboBox>();
        public List<ComboBox> PlayerStandingComboBoxes = new List<ComboBox>();
        public List<CheckBox> PlayerResultCheckBoxes = new List<CheckBox>();
        private List<Button> m_PlayerResultButtons = new List<Button>();
        private ObservableCollection<Player> m_Players;
        private ObservableCollection<int> m_Ranks = new ObservableCollection<int>();

        private const string c_MessageNoValue = "No value in text box ";
        private const string c_MessageNoNumber = "No number in text box ";
        private const string c_MessageEmpty = "No player selected in combo box ";
        private const string c_MessageSame = "Player selected more than once in combo box ";

        public UiBuildingHelper(int a_PlayerAmount, ObservableCollection<Player> a_Players, int a_StartX)
        {
            if (a_PlayerAmount >= BglDb.c_MinAmountPlayers || a_PlayerAmount <= BglDb.c_MaxAmountPlayers)
            {
                m_PlayerAmount = a_PlayerAmount;
            }

            m_Players = a_Players;
            m_StartX = a_StartX;
            m_XTextBox = m_StartX + 220;
            m_XComboBox = m_StartX + 30;
            m_XCheckBox = m_StartX + 270;
            m_XButton = m_StartX + 320;

            // Populate the ranks combobox;
            for (int i = 0; i < BglDb.c_MaxAmountPlayers; ++i)
            {
                m_Ranks.Add(i + 1);
            }
        }

        private enum ButtonFunction
        {
            Remove,
            Reset
        }

        public void GeneratePlayerVariableUiWithRemove(Grid a_GridToPopulate)
        {
            GeneratePlayerVariableUi(a_GridToPopulate);
            GenerateButtons(a_GridToPopulate, ButtonFunction.Remove);
            ActivateUiElements(0);
        }

        public void GeneratePlayerVariableUiWithReset(Grid a_GridToPopulate)
        {
            GeneratePlayerVariableUi(a_GridToPopulate);
            GenerateButtons(a_GridToPopulate, ButtonFunction.Reset);
        }

        public void GeneratePlayerVariableUi(Grid a_GridToPopulate)
        {
            GeneratePlayerTextBoxes(a_GridToPopulate);
            GeneratePlayerComboBoxes(a_GridToPopulate);
            GeneratePlayerCheckBoxes(a_GridToPopulate);
            GenerateStandingsComboBoxes(a_GridToPopulate);
        }

        private void GeneratePlayerTextBoxes(Grid a_GridToPopulate)
        {
            TextBox v_TextBoxToAdd;

            for (int i = 0; i < m_PlayerAmount; i++)
            {
                int v_YActual = m_FirstLineY + i * m_IncrementY;
                v_TextBoxToAdd = new TextBox();
                v_TextBoxToAdd.HorizontalAlignment = HorizontalAlignment.Left;
                v_TextBoxToAdd.VerticalAlignment = VerticalAlignment.Top;
                v_TextBoxToAdd.Width = m_WidthTextBox;
                v_TextBoxToAdd.Height = m_HeightTextBox;
                v_TextBoxToAdd.Margin = new Thickness(m_XTextBox, v_YActual, 0, 0);
                a_GridToPopulate.Children.Add(v_TextBoxToAdd);
                PlayerResultTextBoxes.Add(v_TextBoxToAdd);
            }
        }

        private void GenerateStandingsComboBoxes(Grid a_GridToPopulate)
        {
            ComboBox v_ComboBoxToAdd;

            for (int i = 0; i < m_PlayerAmount; i++)
            {
                int v_YActual = m_FirstLineY  + i * m_IncrementY;
                v_ComboBoxToAdd = new ComboBox();
                v_ComboBoxToAdd.HorizontalAlignment = HorizontalAlignment.Left;
                v_ComboBoxToAdd.VerticalAlignment = VerticalAlignment.Top;
                v_ComboBoxToAdd.Width = m_WidthTextBox;
                v_ComboBoxToAdd.Height = m_HeightTextBox;
                v_ComboBoxToAdd.Margin = new Thickness(m_XTextBox, v_YActual, 0, 0);
                v_ComboBoxToAdd.ItemsSource = m_Ranks;
                v_ComboBoxToAdd.Name = "CbResultStandings_" + i;
                v_ComboBoxToAdd.Visibility = Visibility.Hidden;
                a_GridToPopulate.Children.Add(v_ComboBoxToAdd);
                PlayerStandingComboBoxes.Add(v_ComboBoxToAdd);
            }
        }

        private void GeneratePlayerComboBoxes(Grid a_GridToPopulate)
        {
            ComboBox v_ComboBoxToAdd;

            for (int i = 0; i < m_PlayerAmount; i++)
            {
                int v_YActual = m_FirstLineY + i * m_IncrementY;
                v_ComboBoxToAdd = new ComboBox();
                v_ComboBoxToAdd.HorizontalAlignment = HorizontalAlignment.Left;
                v_ComboBoxToAdd.VerticalAlignment = VerticalAlignment.Top;
                v_ComboBoxToAdd.Width = m_WidthComboBox;
                v_ComboBoxToAdd.Height = m_HeightTextBox;
                v_ComboBoxToAdd.Margin = new Thickness(m_XComboBox, v_YActual, 0, 0);
                v_ComboBoxToAdd.ItemsSource = m_Players;
                v_ComboBoxToAdd.DisplayMemberPath = "Name";
                v_ComboBoxToAdd.Name = "cbResultAddPlayer_" + i;
                a_GridToPopulate.Children.Add(v_ComboBoxToAdd);
                PlayerResultComboBoxes.Add(v_ComboBoxToAdd);
            }
        }

        private void GeneratePlayerCheckBoxes(Grid a_GridToPopulate)
        {
            CheckBox v_CheckBoxToAdd;

            for (int i = 0; i < m_PlayerAmount; i++)
            {
                int v_YActual = m_FirstLineY + 4 + i * m_IncrementY;
                v_CheckBoxToAdd = new CheckBox();
                v_CheckBoxToAdd.HorizontalAlignment = HorizontalAlignment.Left;
                v_CheckBoxToAdd.VerticalAlignment = VerticalAlignment.Top;
                v_CheckBoxToAdd.Width = m_WidthTextBox;
                v_CheckBoxToAdd.Height = m_HeightTextBox;
                v_CheckBoxToAdd.Margin = new Thickness(m_XCheckBox, v_YActual, 0, 0);
                a_GridToPopulate.Children.Add(v_CheckBoxToAdd);
                PlayerResultCheckBoxes.Add(v_CheckBoxToAdd);
            }
        }

        private void GenerateButtons(Grid a_GridToPopulate, ButtonFunction a_ButtonFunction)
        {
            Button v_ButtonToAdd;

            for (int i = 0; i < m_PlayerAmount; i++)
            {
                int v_YActual = m_FirstLineY + i * m_IncrementY;
                v_ButtonToAdd = new Button();
                v_ButtonToAdd.HorizontalAlignment = HorizontalAlignment.Left;
                v_ButtonToAdd.VerticalAlignment = VerticalAlignment.Top;
                v_ButtonToAdd.Width = m_WidthButton;
                v_ButtonToAdd.Height = m_HeightTextBox;
                v_ButtonToAdd.Margin = new Thickness(m_XButton, v_YActual, 0, 0);

                if (a_ButtonFunction == ButtonFunction.Reset)
                {
                    v_ButtonToAdd.Click += ResetButton_Click;
                    v_ButtonToAdd.Name = "btReset_" + i;
                    v_ButtonToAdd.Content = "Reset";
                }
                else if (a_ButtonFunction == ButtonFunction.Remove)
                {
                    v_ButtonToAdd.Click += RemoveButton_Click;
                    v_ButtonToAdd.Name = "btRemove_" + i;
                    v_ButtonToAdd.Content = "Remove";
                }

                a_GridToPopulate.Children.Add(v_ButtonToAdd);
                m_PlayerResultButtons.Add(v_ButtonToAdd);
            }
        }

        public void SwitchTextBoxAndRankVisibility(bool a_IsTextBoxVisible)
        {
            for (int i = 0; i < m_PlayerAmount; ++i)
            {
                if (a_IsTextBoxVisible)
                {
                    PlayerStandingComboBoxes[i].Visibility = Visibility.Hidden;
                    PlayerResultTextBoxes[i].Visibility = Visibility.Visible;
                    PlayerResultCheckBoxes[i].Visibility = Visibility.Visible;
                }
                else
                {
                    PlayerStandingComboBoxes[i].Visibility = Visibility.Visible;
                    PlayerResultTextBoxes[i].Visibility = Visibility.Hidden;
                    PlayerResultCheckBoxes[i].Visibility = Visibility.Hidden;
                }
            }
        }

        public void SetFirstButtonEnabledState(bool a_IsEnabled)
        {
            m_PlayerResultButtons[0].IsEnabled = a_IsEnabled;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            int v_ButtonNumber = ButtonNumberHelper(sender);
            PlayerResultCheckBoxes[v_ButtonNumber].IsChecked = false;
            PlayerResultComboBoxes[v_ButtonNumber].SelectedItem = null;
            PlayerResultTextBoxes[v_ButtonNumber].Text = "";
        }

        public class RemoveEventArgs : EventArgs
        {
            public RemoveEventArgs(int a_Index)
            {
                Index = a_Index;
            }

            public int Index { get; set; }
        }

        public event EventHandler RemoveEvent;

        protected virtual void OnRemoveEntity(RemoveEventArgs e)
        {
            RemoveEvent?.Invoke(this, e);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int v_ButtonNumber = ButtonNumberHelper(sender);
            OnRemoveEntity(new RemoveEventArgs(v_ButtonNumber));
        }

        private int ButtonNumberHelper(object a_Button)
        {
            int v_ButtonNumber = -1;
            Button v_PressedResetButton = a_Button as Button;

            if (v_PressedResetButton != null)
            {
                string v_ExtractedNumber = v_PressedResetButton.Name.Substring(v_PressedResetButton.Name.LastIndexOf('_') + 1);
                int.TryParse(v_ExtractedNumber, out v_ButtonNumber);
            }

            return v_ButtonNumber;
        }

        public void UpdateBindings(Result a_ResultToBind)
        {
            for (int i = 0; i < m_PlayerAmount; ++i)
            {
                if (a_ResultToBind == null || i >= a_ResultToBind.Scores.Count)
                {
                    BindingOperations.ClearBinding(PlayerResultTextBoxes[i], TextBox.TextProperty);
                    BindingOperations.ClearBinding(PlayerResultComboBoxes[i], ComboBox.SelectedItemProperty);
                    BindingOperations.ClearBinding(PlayerResultCheckBoxes[i], CheckBox.IsCheckedProperty);
                }
                else if (i < a_ResultToBind.Scores.Count)
                {
                    Binding v_Binding = new Binding();
                    v_Binding.Source = a_ResultToBind.Scores[i];
                    v_Binding.Path = new PropertyPath("ActualScore");
                    PlayerResultTextBoxes[i].SetBinding(TextBox.TextProperty, v_Binding);

                    v_Binding = new Binding();
                    v_Binding.Source = a_ResultToBind.Scores[i];
                    v_Binding.Path = new PropertyPath("IdPlayer");
                    v_Binding.Converter = new EntityIdToEntityInstanceConverter();
                    v_Binding.ConverterParameter = "Player";
                    PlayerResultComboBoxes[i].SetBinding(ComboBox.SelectedItemProperty, v_Binding);

                    v_Binding = new Binding();
                    v_Binding.Source = a_ResultToBind.Scores[i];
                    v_Binding.Path = new PropertyPath("IsWinner");
                    PlayerResultCheckBoxes[i].SetBinding(CheckBox.IsCheckedProperty, v_Binding);
                }
            }
        }

        public void ActivateUiElements(int a_AmountActiveElements)
        {
            if (a_AmountActiveElements < 0)
            {
                a_AmountActiveElements = 0;
            }
            else if (a_AmountActiveElements > m_PlayerAmount - 1)
            {
                a_AmountActiveElements = m_PlayerAmount;
            }

            m_Ranks.Clear();

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                PlayerResultCheckBoxes[i].IsEnabled = true;
                PlayerResultComboBoxes[i].IsEnabled = true;
                PlayerResultTextBoxes[i].IsEnabled = true;
                m_Ranks.Add(i + 1);
                PlayerStandingComboBoxes[i].IsEnabled = true;

                if (m_PlayerResultButtons.Count != 0)
                {
                    m_PlayerResultButtons[i].IsEnabled = true;
                }
            }

            for (int i = a_AmountActiveElements; i < m_PlayerAmount; ++i)
            {
                PlayerResultCheckBoxes[i].IsEnabled = false;
                PlayerResultComboBoxes[i].IsEnabled = false;
                PlayerResultTextBoxes[i].IsEnabled = false;
                PlayerStandingComboBoxes[i].IsEnabled = false;

                if (m_PlayerResultButtons.Count != 0)
                {
                    m_PlayerResultButtons[i].IsEnabled = false;
                }
            }
        }

        public void SetTextBoxesVisibility(Visibility a_Visibility)
        {
            for (int i = 0; i < m_PlayerAmount; ++i)
            {
                PlayerResultTextBoxes[i].Visibility = a_Visibility;
            }
        }

        public String TestCheckBoxes(int a_AmountActiveElements, Game.GameType a_GameType)
        {
            if (a_GameType == Game.GameType.VictoryPoints)
            {
                return TestAtLeastOneBoxChecked(a_AmountActiveElements);
            }
            else if (a_GameType == Game.GameType.WinLoose)
            {
                return TestNotAllBoxesChecked(a_AmountActiveElements);
            }
            else
            {
                return "Not Implemented.";
            }
        }

        private String TestNotAllBoxesChecked(int a_AmountActiveElements)
        {
            bool v_IsAllBoxesChecked = true;

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                v_IsAllBoxesChecked &= (bool)PlayerResultCheckBoxes[i].IsChecked;
            }

            if (v_IsAllBoxesChecked)
            {
                return "A draw must not have all winners.";
            }
            else
            {
                return String.Empty;
            }
        }

        private String TestAtLeastOneBoxChecked(int a_AmountActiveElements)
        {
            bool v_IsAtLeastOneBoxChecked = false;

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                v_IsAtLeastOneBoxChecked |= (bool)PlayerResultCheckBoxes[i].IsChecked;
            }

            if (v_IsAtLeastOneBoxChecked)
            {
                return String.Empty;
            }
            else
            {
                return "Check at least one winner checkbox.";
            }
        }

        public string TestComboBoxes(int a_AmountActiveElements)
        {
            string v_MessageEmpty = "";
            string v_MessageSame = "";
            List<Player> v_PlayersAdded = new List<Player>();

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                if (PlayerResultComboBoxes[i].SelectedItem == null)
                {
                    v_MessageEmpty += (i + 1) + ", ";
                }
                else
                {
                    Player v_PlayerToTest = (Player)PlayerResultComboBoxes[i].SelectedItem;

                    if (v_PlayersAdded.Contains(v_PlayerToTest))
                    {
                        v_MessageSame += (i + 1) + ", ";
                    }
                    else
                    {
                        v_PlayersAdded.Add(v_PlayerToTest);
                    }
                }
            }

            RemoveComma(ref v_MessageEmpty);
            RemoveComma(ref v_MessageSame);

            String v_Message = String.Empty;

            if (v_MessageEmpty.Length > 0)
            {
                v_Message += c_MessageEmpty + v_MessageEmpty + "." + Environment.NewLine;
            }

            if (v_MessageSame.Length > 0)
            {
                v_Message += c_MessageSame + v_MessageSame + "." + Environment.NewLine;
            }

            return v_Message;
        }

        public string TestTextBoxes(int a_AmountActiveElements)
        {
            string v_MessageNoValue = String.Empty;
            string v_MessageNoNumber = String.Empty;
            int v_Result = 0;

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                if (PlayerResultTextBoxes[i].Text == String.Empty)
                {
                    v_MessageNoValue += (i + 1) + ", ";
                }
                else if (!int.TryParse(PlayerResultTextBoxes[i].Text, out v_Result))
                {
                    v_MessageNoNumber += (i + 1) + ", ";
                }
            }

            RemoveComma(ref v_MessageNoValue);
            RemoveComma(ref v_MessageNoNumber);

            String v_Message = String.Empty;

            if (v_MessageNoNumber.Length > 0)
            {
                v_Message += c_MessageNoNumber + v_MessageNoNumber + "." + Environment.NewLine;
            }

            if (v_MessageNoValue.Length > 0)
            {
                v_Message += c_MessageNoValue + v_MessageNoValue + "." + Environment.NewLine;
            }

            return v_Message;
        }

        private void RemoveComma(ref String a_MessageWithComma)
        {
            if (a_MessageWithComma == null)
            {
                a_MessageWithComma = String.Empty;
            }

            if (a_MessageWithComma.Length > 0)
            {
                a_MessageWithComma = a_MessageWithComma.Substring(0, a_MessageWithComma.Length - 2);
            }
        }
    }
}
