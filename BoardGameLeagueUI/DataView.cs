﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameLeagueLib;
using System.ComponentModel;

namespace BoardGameLeagueUI
{
    class DataView : INotifyPropertyChanged
    {
        private Person m_SelectedPerson;

        public Person SelectedItem
        {
            get
            {
                return m_SelectedPerson;
            }
            set
            {
                m_SelectedPerson = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }
        public List<Person> Items { get; set; }

        public DataView(List<Person> a_Persons)
        {
            Items = a_Persons;
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}