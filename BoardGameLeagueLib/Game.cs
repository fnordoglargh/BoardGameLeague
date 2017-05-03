using System;
using BoardGameLeagueLib.DbClasses;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BoardGameLeagueLib
{
    public class Game : DbObjectName
    {
        private int m_PlayerQuantityMin = 1;
        private int m_PlayerQuantityMax = Int16.MaxValue;

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
            get;
            set;
        }

        [XmlElement("IdGamefamilyRef")]
        public Guid IdGamefamily
        {
            get;
            set;
        }

        [XmlIgnore]
        public GameFamily Family { get; set; }

        public static Dictionary<GameType, String> GameTypeEnumWithCaptions = new Dictionary<GameType, string>()
        {
            {GameType.WinLoose, "Win/Loose" },
            {GameType.VictoryPoints, "Victory Points" },
            {GameType.Ranks, "Ranks" }
        };

        public enum GameType
        {
            WinLoose,
            VictoryPoints,
            Ranks
        }

        public Game(String a_Name, int a_PlayerQuantityMin, int a_PlayerQuantityMax, GameType a_GameType)
            : base(a_Name)
        {
            m_PlayerQuantityMax = a_PlayerQuantityMax;
            m_PlayerQuantityMin = a_PlayerQuantityMin;
            Type = a_GameType;
            IdGamefamily = GameFamily.c_StandardId;
        }

        public Game()
            : base("No Game Name")
        {
            IdGamefamily = GameFamily.c_StandardId;
        }
    }
}
