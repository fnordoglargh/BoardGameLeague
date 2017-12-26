using System;
using System.ComponentModel;

namespace BoardGameLeagueUI.Helpers
{
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
                m_ActualStatusText = m_DefaultStatusText + value;
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
