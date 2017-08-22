using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            GameFamilies.CollectionChanged += GameFamilies_CollectionChanged;

            m_Logger.Info(String.Format("[{0}] Games loaded.", Games.Count));
            m_Logger.Info("Init Database completed.");
        }

        public bool AddEntity(object a_EntityToAdd)
        {
            if (a_EntityToAdd is Player)
            {

            }
            else
            {
                throw new NotImplementedException(String.Format("Adding entities of type [{0}] is not supported.", a_EntityToAdd.GetType()));
            }

            return false;
        }

        public enum EntityStatus
        {
            Invalid,
            Removed,
            NotRemoved
        }

        public EntityStatus RemoveEntity(object a_EntityToRemove)
        {
            EntityStatus v_ActualStatus = EntityStatus.Invalid;

            if (a_EntityToRemove is Player)
            {

            }
            else if (a_EntityToRemove is GameFamily)
            {
                GameFamily v_GameFamilyToRemove = (GameFamily)a_EntityToRemove;
                var v_GameWithFamilyToRemove = Games.Where(p => p.IdGamefamily == v_GameFamilyToRemove.Id);

                if (v_GameWithFamilyToRemove.ToList().Count == 0)
                {
                    GameFamiliesById.Remove(v_GameFamilyToRemove.Id);
                    GameFamilies.Remove(v_GameFamilyToRemove);
                    v_ActualStatus = EntityStatus.Removed;
                }
                else
                {
                    v_ActualStatus = EntityStatus.NotRemoved;
                }
            }
            else
            {
                throw new NotImplementedException(String.Format("Removing entities of type [{0}] is not supported.", a_EntityToRemove.GetType()));
            }

            return v_ActualStatus;
        }

        #region EventHandlers

        private void Players_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                Player v_NewlyAddedPlayer = (Player)e.NewItems[0];
                PlayersById.Add(v_NewlyAddedPlayer.Id, v_NewlyAddedPlayer);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                Player v_RemovedPlayer = (Player)e.OldItems[0];
                PlayersById.Remove(v_RemovedPlayer.Id);
            }
        }

        private void GameFamilies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                GameFamily v_NewlyGameFamily = (GameFamily)e.NewItems[0];
                GameFamiliesById.Add(v_NewlyGameFamily.Id, v_NewlyGameFamily);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                GameFamily v_RemovedGameFamily = (GameFamily)e.OldItems[0];
                GameFamiliesById.Remove(v_RemovedGameFamily.Id);
            }
        }

        #endregion
    }
}
