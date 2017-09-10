using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRootAttribute("BoardGameLeagueDatabase")]
    public sealed class BglDb : INotifyPropertyChanged
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

        private List<int> m_PlayerNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

        public List<int> PlayerNumbers
        {
            get
            {
                return m_PlayerNumbers;
            }
            private set
            {
                m_PlayerNumbers = value;
                NotifyPropertyChanged("PlayerNumbers");
            }
        }

        public void ChangePlayerNumbers(int a_AmountMinimum, int a_AmountMaximum)
        {
            List<int> v_PlayerNumbers = new List<int>();

            for (int i = a_AmountMinimum; i <= a_AmountMaximum; i++)
            {
                v_PlayerNumbers.Add(i);
            }

            PlayerNumbers = v_PlayerNumbers;
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion

        public static Dictionary<Game.GameType, String> GameTypeEnumWithCaptions { get { return Game.GameTypeEnumWithCaptions; } }

        public static List<Player.Genders> GendersList { get { return Player.GendersList; } }

        public void Init()
        {
            m_Logger.Info("Init Database.");
            PlayersById = new Dictionary<Guid, Player>();

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

            m_Logger.Info(String.Format("[{0}] Games loaded.", Games.Count));
            LocationsById = new Dictionary<Guid, Location>();

            foreach (Location i_Location in Locations)
            {
                LocationsById.Add(i_Location.Id, i_Location);
            }

            m_Logger.Info(String.Format("[{0}] Locationa loaded.", Locations.Count));

            foreach (Result i_Result in Results)
            {
                i_Result.Init();
            }

            Players = new ObservableCollection<Player>(Players.OrderBy(p => p.DisplayName));
            GameFamilies = new ObservableCollection<GameFamily>(GameFamilies.OrderBy(p => p.Name));
            Locations = new ObservableCollection<Location>(Locations.OrderBy(p => p.Name));
            Games = new ObservableCollection<Game>(Games.OrderBy(p => p.Name));
            Results = new ObservableCollection<Result>(Results.OrderByDescending(p => p.Date));

            Players.CollectionChanged += DbClasses_CollectionChanged;
            GameFamilies.CollectionChanged += DbClasses_CollectionChanged;
            Locations.CollectionChanged += DbClasses_CollectionChanged;
            Games.CollectionChanged += DbClasses_CollectionChanged;
            Results.CollectionChanged += DbClasses_CollectionChanged;

            m_Logger.Info(String.Format("[{0}] Games loaded.", Games.Count));
            m_Logger.Info(String.Format("[{0}] Results loaded.", Results.Count));
            m_Logger.Info("Init Database completed.");
        }

        public EntityInteractionStatus AddEntity(object a_EntityToAdd)
        {
            EntityInteractionStatus v_ActualStatus = EntityInteractionStatus.Invalid;

            if (a_EntityToAdd is Player)
            {
                Players.Add((Player)a_EntityToAdd);
                v_ActualStatus = EntityInteractionStatus.Added;
            }
            else
            {
                throw new NotImplementedException(String.Format("Adding entities of type [{0}] is not supported.", a_EntityToAdd.GetType()));
            }

            return v_ActualStatus;
        }

        /// <summary>
        /// Signifies the result status of the encapsulated methods to add or remove entities.
        /// </summary>
        public enum EntityInteractionStatus
        {
            Invalid,
            Removed,
            NotRemoved,
            Added
        }

        /// <summary>
        /// Any database object should be only removed through this method. It encapsulates sanity testing and only deletes
        /// an entity if it is not referenced in any other entity.
        /// </summary>
        /// <param name="a_EntityToRemove">The entity will be tried to converted into a known object.</param>
        /// <returns>Invalid if something went wrong, Removed if the entity was removed and NotRemoved if the entity is still referenced.</returns>
        public EntityInteractionStatus RemoveEntity(object a_EntityToRemove)
        {
            EntityInteractionStatus v_ActualStatus = EntityInteractionStatus.Invalid;

            if (a_EntityToRemove is null)
            {
                return v_ActualStatus;
            }
            else if (a_EntityToRemove is Player)
            {
                Player v_PlayerToRemove = a_EntityToRemove as Player;
                var v_ReferencedPlayer = Results.SelectMany(p => p.Scores).Where(p => p.IdPlayer == v_PlayerToRemove.Id);

                if (v_ReferencedPlayer.ToList().Count == 0)
                {
                    PlayersById.Remove(v_PlayerToRemove.Id);
                    Players.Remove(v_PlayerToRemove);
                    m_Logger.Info(String.Format("Removed Player [{0}].", v_PlayerToRemove));
                    v_ActualStatus = EntityInteractionStatus.Removed;
                }
                else
                {
                    if (v_PlayerToRemove.Gender == Player.Genders.Male)
                    {
                        m_Logger.Error(String.Format("Cannot remove [{0}] because he is referenced in {1} scores.", v_PlayerToRemove.DisplayName, v_ReferencedPlayer.ToList().Count));
                    }
                    else if (v_PlayerToRemove.Gender == Player.Genders.Female)
                    {
                        m_Logger.Error(String.Format("Cannot remove [{0}] because she is referenced in {1} scores.", v_PlayerToRemove.DisplayName, v_ReferencedPlayer.ToList().Count));
                    }
                    else
                    {
                        // Just for the case someone adds another gender...
                        m_Logger.Error(String.Format("Cannot remove [{0}] because it is referenced in {1} scores.", v_PlayerToRemove.DisplayName, v_ReferencedPlayer.ToList().Count));
                    }

                    v_ActualStatus = EntityInteractionStatus.NotRemoved;
                }
            }
            else if (a_EntityToRemove is GameFamily)
            {
                GameFamily v_GameFamilyToRemove = a_EntityToRemove as GameFamily;
                var v_GameWithFamilyToRemove = Games.Where(p => p.IdGamefamily == v_GameFamilyToRemove.Id);

                if (v_GameWithFamilyToRemove.ToList().Count == 0)
                {
                    GameFamilies.Remove(v_GameFamilyToRemove);
                    m_Logger.Info(String.Format("Removed GameFamily [{0}].", v_GameFamilyToRemove));
                    v_ActualStatus = EntityInteractionStatus.Removed;
                }
                else
                {
                    m_Logger.Error(String.Format("Cannot remove [{0}] because it is is referenced in {1} games.", v_GameFamilyToRemove.Name, v_GameWithFamilyToRemove.ToList().Count));
                    v_ActualStatus = EntityInteractionStatus.NotRemoved;
                }
            }
            else if (a_EntityToRemove is Game)
            {
                Game v_GameToRemove = a_EntityToRemove as Game;
                var v_ReferencesToGame = Results.Where(p => p.IdGame == v_GameToRemove.Id);

                if (v_ReferencesToGame.ToList().Count == 0)
                {
                    Games.Remove(v_GameToRemove);
                    m_Logger.Info(String.Format("Removed Game [{0}].", v_GameToRemove.Name));
                    v_ActualStatus = EntityInteractionStatus.Removed;
                }
                else
                {
                    m_Logger.Error(String.Format("Cannot remove [{0}] because it is is referenced in {1} results.", v_GameToRemove.Name, v_ReferencesToGame.ToList().Count));
                    v_ActualStatus = EntityInteractionStatus.NotRemoved;
                }
            }
            else if (a_EntityToRemove is Location)
            {
                Location v_LocationToRemove = a_EntityToRemove as Location;
                var v_ReferencesToLocation = Results.Where(p => p.IdLocation == v_LocationToRemove.Id);

                if (v_ReferencesToLocation.ToList().Count == 0)
                {
                    Locations.Remove(v_LocationToRemove);
                    m_Logger.Info(String.Format("Removed Location [{0}].", v_LocationToRemove.Name));
                    v_ActualStatus = EntityInteractionStatus.Removed;
                }
                else
                {
                    m_Logger.Error(String.Format("Cannot remove {0} because it is is referenced in {1} results.", v_LocationToRemove.Name, v_ReferencesToLocation.ToList().Count));
                    v_ActualStatus = EntityInteractionStatus.NotRemoved;
                }
            }
            else
            {
                throw new NotImplementedException(String.Format("Removing entities of type [{0}] is not supported.", a_EntityToRemove.GetType()));
            }

            return v_ActualStatus;
        }

        public class ResultRow
        {
            public String Name { get; set; }
            public int AmountPlayed { get; set; }
            public int AmountWon { get; set; }
            public int AmountPoints { get; set; }
            public double PercentageWon { get; set; }

            public ResultRow(String a_Name, int a_AmountPlayed, int a_AmountWon, int a_AmountPoints)
            {
                Name = a_Name;
                AmountPlayed = a_AmountPlayed;
                AmountWon = a_AmountWon;
                AmountPoints = a_AmountPoints;
            }
        }

        public ObservableCollection<ResultRow> CalculateResults(Guid a_GameId)
        {
            var v_ReferencesToGame = Results.Where(p => p.IdGame == a_GameId);
            Dictionary<Guid, ResultRow> v_ResultRows = new Dictionary<Guid, ResultRow>();

            foreach (Result i_Result in v_ReferencesToGame)
            {
                foreach (Score i_Score in i_Result.Scores)
                {
                    if (!v_ResultRows.ContainsKey(i_Score.IdPlayer))
                    {
                        v_ResultRows.Add(i_Score.IdPlayer, new ResultRow(PlayersById[i_Score.IdPlayer].Name, 0, 0, 0));
                    }

                    v_ResultRows[i_Score.IdPlayer].AmountPlayed++;

                    if (i_Score.IsWinner)
                    {
                        v_ResultRows[i_Score.IdPlayer].AmountWon++;
                    }

                    v_ResultRows[i_Score.IdPlayer].AmountPoints += int.Parse(i_Score.ActualScore);
                }
            }

            ObservableCollection<ResultRow> v_ResultRowInstances = new ObservableCollection<ResultRow>();

            foreach (KeyValuePair<Guid, ResultRow> i_Row in v_ResultRows)
            {
                // Calculate percentage won before we add the result to the collection.
                i_Row.Value.PercentageWon = Math.Round(100 * i_Row.Value.AmountWon / (double)i_Row.Value.AmountPlayed, 2);
                v_ResultRowInstances.Add(i_Row.Value);
            }

            return v_ResultRowInstances;
        }

        public Dictionary<Player, Result.ResultHelper> CalculateEloResults()
        {
            Dictionary<Player, Result.ResultHelper> v_EloResults = new Dictionary<Player, Result.ResultHelper>();

            foreach (Player i_Player in Players)
            {
                v_EloResults.Add(i_Player, new Result.ResultHelper(i_Player.Id, 1500, 0));
            }

            ObservableCollection<Result> v_BeginningToEndResults= new ObservableCollection<Result>(Results.OrderBy(p => p.Date));

            foreach (Result i_Result in v_BeginningToEndResults)
            {
                Dictionary<Guid, Result.ResultHelper> v_TempEloResults2 = new Dictionary<Guid, Result.ResultHelper>();

                foreach (Score i_Score in i_Result.Scores)
                {
                    v_TempEloResults2.Add(i_Score.IdPlayer, v_EloResults[PlayersById[i_Score.IdPlayer]]);
                }

                i_Result.CalculateEloResults(v_TempEloResults2);
            }

            return v_EloResults;
        }

        #region EventHandlers

        private void DbClasses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DbHelper.Instance.IsChanged = true;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (sender is ObservableCollection<Player>)
                {
                    PlayersById.Add(((Player)e.NewItems[0]).Id, (Player)e.NewItems[0]);
                }
                else if (sender is ObservableCollection<GameFamily>)
                {
                    GameFamiliesById.Add(((GameFamily)e.NewItems[0]).Id, (GameFamily)e.NewItems[0]);
                }
                else if (sender is ObservableCollection<Location>)
                {
                    LocationsById.Add(((Location)e.NewItems[0]).Id, (Location)e.NewItems[0]);
                }
                else if (sender is ObservableCollection<Game>)
                {
                    GamesById.Add(((Game)e.NewItems[0]).Id, (Game)e.NewItems[0]);
                }
                else if (sender is ObservableCollection<Result>)
                {

                }
                else
                {
                    throw new NotImplementedException(String.Format("Adding of [{0}] is not supported.", sender.GetType()));
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (sender is ObservableCollection<Player>)
                {
                    PlayersById.Remove(((Player)e.OldItems[0]).Id);
                }
                else if (sender is ObservableCollection<GameFamily>)
                {
                    GameFamiliesById.Remove(((GameFamily)e.OldItems[0]).Id);
                }
                else if (sender is ObservableCollection<Location>)
                {
                    LocationsById.Remove(((Location)e.OldItems[0]).Id);
                }
                else if (sender is ObservableCollection<Game>)
                {
                    GamesById.Remove(((Game)e.OldItems[0]).Id);
                }
                else if (sender is ObservableCollection<Result>)
                {

                }
                else
                {
                    throw new NotImplementedException(String.Format("Removing of [{0}] is not supported.", sender.GetType()));
                }
            }
            else
            {
                throw new NotImplementedException(String.Format("Action {0} not supported on collection.", e.Action));
            }
        }

        #endregion
    }
}
