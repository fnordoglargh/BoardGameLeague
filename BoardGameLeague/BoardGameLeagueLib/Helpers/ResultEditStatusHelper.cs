﻿using System;
using System.ComponentModel;

namespace BoardGameLeagueUI.Helpers
{
    /// <summary>
    /// Used to give a small visual indicator in the result entering tab fi a result has not been saved yet.
    /// </summary>
    public class ResultEditStatusHelper : INotifyPropertyChanged
    {
        private String m_DefaultStatusText = "Status";
        private String m_ActualStatusText = "Status";
        private const String c_InstanceEdited = "*";
        private const String c_InstanceAdded = "+";
        private bool m_IsUnsavedOnce = false;

        public String StatusText
        {
            get { return m_ActualStatusText; }
            private set
            {
                m_ActualStatusText = value + " " + m_DefaultStatusText;
                NotifyPropertyChanged("StatusText");
            }
        }

        public ResultEditStatusHelper(string a_DefaultStatusText)
        {
            m_DefaultStatusText = a_DefaultStatusText;
            m_ActualStatusText = a_DefaultStatusText;
        }

        public void Changed()
        {
            if (m_IsUnsavedOnce)
            {
                StatusText = c_InstanceEdited;
            }
            else
            {
                StatusText = c_InstanceAdded;
            }
        }

        public void Reset()
        {
            m_IsUnsavedOnce = true;
            StatusText = String.Empty;
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
