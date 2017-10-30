﻿using BoardGameLeagueLib.Converters;
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
        public List<CheckBox> PlayerResultCheckBoxes = new List<CheckBox>();
        private List<Button> m_PlayerResultButtons = new List<Button>();
        private ObservableCollection<Player> m_Players;

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
            if (a_AmountActiveElements < 1)
            {
                a_AmountActiveElements = 1;
            }
            else if (a_AmountActiveElements > m_PlayerAmount - 1)
            {
                a_AmountActiveElements = m_PlayerAmount;
            }

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                PlayerResultCheckBoxes[i].IsEnabled = true;
                PlayerResultComboBoxes[i].IsEnabled = true;
                PlayerResultTextBoxes[i].IsEnabled = true;

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

                if (m_PlayerResultButtons.Count != 0)
                {
                    m_PlayerResultButtons[i].IsEnabled = false;
                }
            }
        }

        public bool TestCheckBoxes(int a_AmountActiveElements)
        {
            bool v_IsAtLeastOneBoxChecked = false;

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                v_IsAtLeastOneBoxChecked |= (bool)PlayerResultCheckBoxes[i].IsChecked;
            }

            return v_IsAtLeastOneBoxChecked;
        }

        public string TestComboBoxes(int a_AmountActiveElements)
        {
            string v_Message = "";
            List<Player> v_PlayersAdded = new List<Player>();

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                if (PlayerResultComboBoxes[i].SelectedItem == null)
                {
                    v_Message += (i + 1) + ", ";
                }
                else
                {
                    Player v_PlayerToTest = (Player)PlayerResultComboBoxes[i].SelectedItem;

                    if (v_PlayersAdded.Contains(v_PlayerToTest)) { v_Message += (i + 1) + ", "; }
                    else
                    {
                        v_PlayersAdded.Add(v_PlayerToTest);
                    }
                }
            }

            if (v_Message.Length > 0)
            {
                v_Message = v_Message.Substring(0, v_Message.Length - 2);
            }

            return v_Message;
        }

        public string TestTextBoxes(int a_AmountActiveElements)
        {
            string v_Message = "";
            int v_Result = 0;

            for (int i = 0; i < a_AmountActiveElements; ++i)
            {
                if (!int.TryParse(PlayerResultTextBoxes[i].Text, out v_Result))
                {
                    v_Message += (i + 1) + ", ";
                }
            }

            if (v_Message.Length > 0)
            {
                v_Message = v_Message.Substring(0, v_Message.Length - 2);
            }

            return v_Message;
        }
    }
}
