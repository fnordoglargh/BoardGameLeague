﻿using BoardGameLeagueLib;
using BoardGameLeagueLib.DbClasses;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BoardGameLeagueUI2
{
    public class UiBuildingHelper
    {
        private const int m_WidthTextBox = 120;
        private const int m_HeightTextBox = 23;
        private const int m_XTextBox = 430;
        private const int m_XComboBox = 577;
        private const int m_XCheckBox = 737;
        private const int m_FirstLineY = 69;
        private const int m_IncrementY = 29;
        private int m_PlayerAmount = BglDb.c_MaxAmountPlayers;
        private List<TextBox> m_PlayerResultTextBoxes = new List<TextBox>();
        private List<ComboBox> m_PlayerResultComboBoxes = new List<ComboBox>();
        private List<CheckBox> m_PlayerResultCheckBoxes = new List<CheckBox>();
        private ObservableCollection<Player> m_Players;

        private ILog m_Logger = LogManager.GetLogger("UiBuildingHelper");

        public UiBuildingHelper(int a_PlayerAmount, ObservableCollection<Player> a_Players)
        {
            if (a_PlayerAmount >= BglDb.c_MinAmountPlayers || a_PlayerAmount <= BglDb.c_MaxAmountPlayers)
            {
                m_PlayerAmount = a_PlayerAmount;
            }

            m_Players = a_Players;
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
                m_PlayerResultTextBoxes.Add(v_TextBoxToAdd);
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
                v_ComboBoxToAdd.Width = m_WidthTextBox;
                v_ComboBoxToAdd.Height = m_HeightTextBox;
                v_ComboBoxToAdd.Margin = new Thickness(m_XComboBox, v_YActual, 0, 0);
                v_ComboBoxToAdd.ItemsSource= m_Players;
                v_ComboBoxToAdd.DisplayMemberPath = "Name";
                //foreach (Player i_Player in m_Players)
                //{
                //    // This means that adding a player won't update the items.
                //    v_ComboBoxToAdd.Items.Add(i_Player);
                //    v_ComboBoxToAdd.DisplayMemberPath = "Name";
                //}

                a_GridToPopulate.Children.Add(v_ComboBoxToAdd);
                m_PlayerResultComboBoxes.Add(v_ComboBoxToAdd);
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
                m_PlayerResultCheckBoxes.Add(v_CheckBoxToAdd);
            }
        }

        public void UpdateBindings(Result a_ResultToBind, ObservableCollection<Player> a_Players)
        {
            for (int i = 0; i < m_PlayerAmount; ++i)
            {
                if (i < a_ResultToBind.Scores.Count)
                {
                    Binding v_Binding = new Binding();
                    v_Binding.Source = a_ResultToBind.Scores[i];
                    v_Binding.Path = new PropertyPath("ActualScore");
                    m_PlayerResultTextBoxes[i].SetBinding(TextBox.TextProperty, v_Binding);

                    v_Binding = new Binding();
                    v_Binding.Source = a_ResultToBind.Scores[i];
                    v_Binding.Path = new PropertyPath("IdPlayer");
                    v_Binding.Converter = new ResultIdToPlayerResultConverter();
                    v_Binding.ConverterParameter = i;
                    m_PlayerResultComboBoxes[i].SetBinding(ComboBox.SelectedItemProperty, v_Binding);

                    v_Binding = new Binding();
                    v_Binding.Source = a_ResultToBind.Scores[i];
                    v_Binding.Path = new PropertyPath("IsWinner");
                    m_PlayerResultCheckBoxes[i].SetBinding(CheckBox.IsCheckedProperty, v_Binding);
                }
                else
                {
                    BindingOperations.ClearBinding(m_PlayerResultTextBoxes[i], TextBox.TextProperty);
                    BindingOperations.ClearBinding(m_PlayerResultComboBoxes[i], ComboBox.SelectedItemProperty);
                    BindingOperations.ClearBinding(m_PlayerResultCheckBoxes[i], CheckBox.IsCheckedProperty);
                }
            }
        }
    }
}
