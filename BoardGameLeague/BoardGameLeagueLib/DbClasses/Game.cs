using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    public class Game : DbObjectName
    {
        private int m_PlayerQuantityMin = BglDb.c_MinAmountPlayers;
        private int m_PlayerQuantityMax = BglDb.c_MaxAmountPlayers;
        private ObservableCollection<Guid> m_IdGamefamilies = new ObservableCollection<Guid>();
        private GameType m_GameType;

        public int PlayerQuantityMin
        {
            get { return m_PlayerQuantityMin; }
            set
            {
                if (m_PlayerQuantityMax >= value)
                {
                    m_PlayerQuantityMin = value;
                }
                else
                {
                    m_PlayerQuantityMin = m_PlayerQuantityMax;
                }

                NotifyPropertyChanged("PlayerQuantityMin");
            }
        }

        public int PlayerQuantityMax
        {
            get { return m_PlayerQuantityMax; }
            set
            {
                if (m_PlayerQuantityMin <= value)
                {
                    m_PlayerQuantityMax = value;
                }
                else
                {
                    m_PlayerQuantityMax = m_PlayerQuantityMin;
                }

                NotifyPropertyChanged("PlayerQuantityMax");
            }
        }

        public GameType Type
        {
            get { return m_GameType; }
            set
            {
                m_GameType = value;

                if (m_GameType == GameType.WinLose)
                {
                    PlayerQuantityMin = 2;
                    PlayerQuantityMax = 2;
                }

                NotifyPropertyChanged("Type");
            }
        }

        [XmlElement("IdGamefamilyRef")]
        public ObservableCollection<Guid> IdGamefamilies
        {
            get { return m_IdGamefamilies; }
            set
            {
                m_IdGamefamilies = value;
                NotifyPropertyChanged("IdGamefamilies");
            }
        }

        [XmlIgnore]
        public GameFamily Family { get; set; }

        public static Dictionary<GameType, String> GameTypeEnumWithCaptions = new Dictionary<GameType, string>()
        {
            {GameType.WinLose, "Win/Lose" },
            {GameType.VictoryPoints, "Victory Points" },
            {GameType.Ranks, "Ranks" },
            {GameType.TeamedRanks, "Teamed Ranks" }
        };

        public enum GameType
        {
            WinLose,
            VictoryPoints,
            Ranks,
            TeamedRanks
        }

        public Game(String a_Name, int a_PlayerQuantityMin, int a_PlayerQuantityMax, GameType a_GameType, Guid a_GameFamilyId)
            : base(a_Name)
        {
            m_PlayerQuantityMax = a_PlayerQuantityMax;
            m_PlayerQuantityMin = a_PlayerQuantityMin;
            Type = a_GameType;
            IdGamefamilies.Add(a_GameFamilyId);
            m_IdGamefamilies.CollectionChanged += IdGamefamilies_CollectionChanged;
        }

        public Game(String a_Name, int a_PlayerQuantityMin, int a_PlayerQuantityMax, GameType a_GameType)
            : base(a_Name)
        {
            m_PlayerQuantityMax = a_PlayerQuantityMax;
            m_PlayerQuantityMin = a_PlayerQuantityMin;
            Type = a_GameType;
            m_IdGamefamilies.CollectionChanged += IdGamefamilies_CollectionChanged;
        }

        public Game()
            : base("Default Game Name")
        {
            Type = GameType.VictoryPoints;
            m_IdGamefamilies.CollectionChanged += IdGamefamilies_CollectionChanged;
        }

        private void IdGamefamilies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DbHelper.Instance.IsChanged = true;
        }

        public override string ToString()
        {
            return "ID: " + Id + ", Name: " + Name + ", min:" + m_PlayerQuantityMin + ", max: " + m_PlayerQuantityMax;
        }
    }
}
