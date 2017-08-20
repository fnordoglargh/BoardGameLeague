using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRootAttribute("BoardGameLeagueDatabase")]
    public sealed class BglDb
    {
        private static ILog m_Logger = LogManager.GetLogger("BglDb");

        public const int c_MinAmountPlayers = 1;
        public const int c_MaxAmountPlayers = 8;

        public ObservableCollection<Player> Players { get; set; }
        public ObservableCollection<GameFamily> GameFamilies { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<Result> Results { get; set; }

        [XmlIgnore]
        public Game SelectedGame;

        [XmlIgnore]
        public Dictionary<Guid, Player> PlayersById
        {
            get;
            private set;
        }

        [XmlIgnore]
        public Dictionary<Guid, GameFamily> GameFamiliesById
        {
            get;
            private set;
        }

        [XmlIgnore]
        public Dictionary<Guid, Game> GamesById
        {
            get;
            private set;
        }

        [XmlIgnore]
        public Dictionary<Guid, Location> LocationsById
        {
            get;
            private set;
        }

        public static Dictionary<Game.GameType, String> GameTypeEnumWithCaptions { get { return Game.GameTypeEnumWithCaptions; } }

        public static List<Player.Genders> GendersList { get { return Player.GendersList; } }

        public void Init()
        {
            PlayersById = new Dictionary<Guid, Player>();

            m_Logger.Info("Init Database.");


            foreach (Player i_Player in Players)
            {
                PlayersById.Add(i_Player.Id, i_Player);
            }

            m_Logger.Info(String.Format("[{0}] Players loaded.", Players.Count));


            GameFamiliesById = new Dictionary<Guid, GameFamily>();

            foreach (GameFamily i_Family in GameFamilies)
            {
                GameFamiliesById.Add(i_Family.Id, i_Family);
            }

            m_Logger.Info(String.Format("[{0}] Game Families loaded.", GameFamilies.Count));

            GamesById = new Dictionary<Guid, Game>();

            foreach (Game i_Game in Games)
            {
                i_Game.Family = GameFamiliesById[i_Game.IdGamefamily];
                GamesById.Add(i_Game.Id, i_Game);
            }

            Players.CollectionChanged += Players_CollectionChanged;

            m_Logger.Info(String.Format("[{0}] Games loaded.", Games.Count));
            m_Logger.Info("Init Database completed.");
        }

        private void Players_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                Player v_NewlyAddedPlayer = (Player)e.NewItems[0];
                PlayersById.Add(v_NewlyAddedPlayer.Id, v_NewlyAddedPlayer);
            }
        }
    }
}
