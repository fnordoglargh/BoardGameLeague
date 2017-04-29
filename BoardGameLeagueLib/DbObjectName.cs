﻿using System;
using System.ComponentModel;

namespace BoardGameLeagueLib
{
    public abstract class DbObjectName : DbObject, INotifyPropertyChanged
    {
        private String m_Name;

        public String Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public DbObjectName(String a_Name)
        {
            Name = a_Name;
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}