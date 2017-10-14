using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    public class Score : INotifyPropertyChanged
    {
        private Guid m_IdPlayer;
        private String m_ActualScore;
        private bool m_IsWinner;

        [XmlElement("IdPlayerRef")]
        public Guid IdPlayer
        {
            get { return m_IdPlayer; }
            set
            {
                m_IdPlayer = value;
                NotifyPropertyChanged("IdPlayer");
            }
        }

        public String ActualScore
        {
            get { return m_ActualScore; }
            set
            {
                m_ActualScore = value;
                NotifyPropertyChanged("ActualScore");
            }
        }

        public bool IsWinner
        {
            get { return m_IsWinner; }
            set
            {
                m_IsWinner = value;
                NotifyPropertyChanged("IsWinner");
            }
        }

        public Score() { }

        public Score(Guid a_IdPlayer, String a_ActualScore, bool a_IsWinner)
        {
            IdPlayer = a_IdPlayer;
            ActualScore = a_ActualScore;
            IsWinner = a_IsWinner;
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
