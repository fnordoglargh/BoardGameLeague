using BoardGameLeagueLib.ResultRows;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRoot("BoardGameLeagueDatabase")]
    public sealed class BglDb : INotifyPropertyChanged
    {
        private static ILog m_Logger = LogManager.GetLogger("BglDb");
        private const String v_RemovalMessage = "Cannot remove [{0}] because {1} is referenced in {2} scores.";

        public const int c_MinAmountPlayers = 1;
        public const int c_MaxAmountPlayers = 8;
        public const int c_EloStartValue = 1500;

        public ObservableCollection<Player> Players { get; set; }
        public ObservableCollection<GameFamily> GameFamilies { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<Result> Results { get; set; }

        /// <summary>
        /// Gets all Results ordered descending by date.
        /// </summary>
        [XmlIgnore]
        public List<Result> ResultsOrdered
        {
            get
            {
                return Results.OrderByDescending(p => p.Date).ToList();
            }
        }

        /// <summary>
        /// Gets the game families but without the standard "no game family".
        /// </summary>
        [XmlIgnore]
        public List<GameFamily> GameFamiliesFiltered
        {
            get
            {
                List<GameFamily> v_TempFamilies = GameFamilies.Where(s => !s.Id.Equals(GameFamily.c_StandardId)).ToList();
                return v_TempFamilies.OrderBy(p => p.Name).ToList();
            }
        }

        /// <summary>
        /// Gets all players sorted by name.
        /// </summary>
        [XmlIgnore]
        public List<Player> PlayersSorted
        {
            get
            {
                return Players.OrderBy(p => p.Name).ToList();
            }
        }

        /// <summary>
        /// Gets all locations sorted by name.
        /// </summary>
        [XmlIgnore]
        public List<Location> LocationsSorted
        {
            get
            {
                return Locations.OrderBy(p => p.Name).ToList();
            }
        }

        /// <summary>
        /// Gets all games sorted by name.
        /// </summary>
        [XmlIgnore]
        public List<Game> GamesSorted
        {
            get
            {
                return Games.OrderBy(p => p.Name).ToList();
            }
        }

        /// <summary>
        /// Gets all point based games.
        /// </summary>
        [XmlIgnore]
        public List<Game> GamesPointBased
        {
            get
            {
                List<Game> v_TempGames = Games.Where(s => s.Type.Equals(Game.GameType.VictoryPoints)).ToList();
                return v_TempGames.OrderBy(p => p.Name).ToList();
            }
        }

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

        [XmlIgnore]
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
                m_Logger.Debug("GF: " + i_Family.Id + " " + i_Family.Name);
            }

            m_Logger.Info(String.Format("[{0}] Game Families loaded.", GameFamilies.Count));
            GamesById = new Dictionary<Guid, Game>();

            foreach (Game i_Game in Games)
            {
                //i_Game.Family = GameFamiliesById[i_Game.IdGamefamily];
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
                i_Result.PropertyChanged += Result_PropertyChanged;

                foreach (Score i_Score in i_Result.Scores)
                {
                    i_Score.PropertyChanged += Result_PropertyChanged;
                }
            }

            Players = new ObservableCollection<Player>(Players.OrderBy(p => p.Name));
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

        /// <summary>
        /// Is hooked up to result instances to detect changes in existing results so that the user can be notified.
        /// </summary>
        private void Result_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // We want to see the result move to the right spot after changing the date.
            NotifyPropertyChanged("ResultsOrdered");
            DbHelper.Instance.IsChanged = true;
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
                        m_Logger.Error(String.Format(v_RemovalMessage, v_PlayerToRemove.Name, "he", v_ReferencedPlayer.ToList().Count));
                    }
                    else if (v_PlayerToRemove.Gender == Player.Genders.Female)
                    {
                        m_Logger.Error(String.Format(v_RemovalMessage, v_PlayerToRemove.Name, "she", v_ReferencedPlayer.ToList().Count));
                    }
                    else
                    {
                        // Just for the case someone adds another gender...
                        m_Logger.Error(String.Format(v_RemovalMessage, v_PlayerToRemove.Name, "insert pronoun here", v_ReferencedPlayer.ToList().Count));
                    }

                    v_ActualStatus = EntityInteractionStatus.NotRemoved;
                }
            }
            else if (a_EntityToRemove is GameFamily)
            {
                GameFamily v_GameFamilyToRemove = a_EntityToRemove as GameFamily;
                var v_GamesWithFamilyToRemove = Games.Where(p => p.IdGamefamilies.Contains(v_GameFamilyToRemove.Id));

                if (v_GamesWithFamilyToRemove.ToList().Count == 0)
                {
                    GameFamilies.Remove(v_GameFamilyToRemove);
                    m_Logger.Info(String.Format("Removed GameFamily [{0}].", v_GameFamilyToRemove));
                    v_ActualStatus = EntityInteractionStatus.Removed;
                }
                else
                {
                    m_Logger.Error(String.Format("Cannot remove [{0}] because it is is referenced in {1} games.", v_GameFamilyToRemove.Name, v_GamesWithFamilyToRemove.ToList().Count));
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

        /// <summary>
        /// Checks if a game ID is referenced in any result.
        /// </summary>
        /// <param name="a_GameId">Game ID to check.</param>
        /// <returns>True if the game ID is used in any result.</returns>
        public bool CheckIfGamesIsReferenced(Guid a_GameId)
        {
            List<Result> v_IsGameReferenced = Results.Where(p => p.IdGame == a_GameId).ToList();

            if (v_IsGameReferenced.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Calculates results of all games a player finished for the given game id.
        /// </summary>
        /// <param name="a_GameId">Id of the game to calculate results for.</param>
        /// <returns>Returns collections of types ResultRowVictoryPoints, ResultRowRanks or ResultRowWinLose depending on the type of the given game.</returns>
        public IEnumerable<object> CalculateResultsGamesBase(Guid a_GameId)
        {
            var v_ReferencesToGame = Results.Where(p => p.IdGame == a_GameId);

            if (GamesById[a_GameId].Type == Game.GameType.VictoryPoints)
            {
                return CalculateResultsVictoryPoints(v_ReferencesToGame);
            }
            else if (GamesById[a_GameId].Type == Game.GameType.Ranks)
            {
                return CalculateResultsRanks(v_ReferencesToGame);
            }
            else if (GamesById[a_GameId].Type == Game.GameType.WinLose)
            {
                return CalculateResultsWinLose(v_ReferencesToGame);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates individual player ResultRows for the given game family ID.
        /// </summary>
        /// <param name="a_GameFamilyId">Id of the game family to calculate results for.</param>
        /// <returns>A collection of result row objects filled with players which played games from the family of given ID.</returns>
        public IEnumerable<object> CalculateResultsGameFamilies(Guid a_GameFamilyId)
        {
            // First: Get all games from the given game family.
            var v_AllGamesFromFamily = Games.Where(p => p.IdGamefamilies.Contains(a_GameFamilyId));

            if (v_AllGamesFromFamily.Count() < 1) return null;

            // Second: Get all results with games of the given game family.
            var v_Results = Results.Join(v_AllGamesFromFamily,
                result => result.IdGame,
                game => game.Id,
                (result, game) => result);

            // Peek at the game type.
            Game.GameType v_ActualType = v_AllGamesFromFamily.First().Type;
            IEnumerable<object> v_ResultRows = null;

            if (v_ActualType == Game.GameType.VictoryPoints)
            {
                v_ResultRows = CalculateResultsVictoryPoints(v_Results);
            }
            else if (v_ActualType == Game.GameType.Ranks || v_ActualType == Game.GameType.TeamedRanks)
            {
                v_ResultRows = CalculateResultsRanks(v_Results);
            }
            else if (v_ActualType == Game.GameType.WinLose)
            {
                v_ResultRows = CalculateResultsWinLose(v_Results);
            }
            else
            {
                // Do nothing, result rows have been init'd with null anyway.
            }

            return v_ResultRows;
        }

        /// <summary>
        /// Calculates the scores for all players which played in the given collcetion of results.
        /// </summary>
        /// <param name="a_Results">A collection with all results which are used to calculate the individial player results.</param>
        /// <returns>A collection with the individual results of all players which were part of the referenced game sessions.</returns>
        private ObservableCollection<ResultRowVictoryPoints> CalculateResultsVictoryPoints(IEnumerable<object> a_Results)
        {
            Dictionary<Guid, ResultRowVictoryPoints> v_ResultRows = new Dictionary<Guid, ResultRowVictoryPoints>();

            foreach (Result i_Result in a_Results)
            {
                foreach (Score i_Score in i_Result.Scores)
                {
                    // Add a new ResultRow if there is none.
                    if (!v_ResultRows.ContainsKey(i_Score.IdPlayer))
                    {
                        v_ResultRows.Add(i_Score.IdPlayer, new ResultRowVictoryPoints(PlayersById[i_Score.IdPlayer].Name, 0, 0, 0));
                    }

                    v_ResultRows[i_Score.IdPlayer].AmountPlayed++;

                    if (i_Score.IsWinner)
                    {
                        v_ResultRows[i_Score.IdPlayer].AmountWon++;
                    }

                    int v_ActualScore = int.Parse(i_Score.ActualScore);
                    v_ResultRows[i_Score.IdPlayer].AmountPoints += v_ActualScore;

                    if (v_ResultRows[i_Score.IdPlayer].BestScore < v_ActualScore)
                    {
                        v_ResultRows[i_Score.IdPlayer].BestScore = (int)v_ActualScore;
                    }

                    if (v_ResultRows[i_Score.IdPlayer].WorstScore > v_ActualScore)
                    {
                        v_ResultRows[i_Score.IdPlayer].WorstScore = (int)v_ActualScore;
                    }
                }
            }

            ObservableCollection<ResultRowVictoryPoints> v_ResultRowInstances = new ObservableCollection<ResultRowVictoryPoints>();

            foreach (KeyValuePair<Guid, ResultRowVictoryPoints> i_Row in v_ResultRows)
            {
                // Calculate percentage won before we add the result to the collection.
                i_Row.Value.PercentageWon = Math.Round(100 * i_Row.Value.AmountWon / (double)i_Row.Value.AmountPlayed, 2);
                i_Row.Value.AveragePoints = Math.Round(i_Row.Value.AmountPoints / (double)i_Row.Value.AmountPlayed, 2);
                v_ResultRowInstances.Add(i_Row.Value);
            }

            return v_ResultRowInstances;
        }

        private ObservableCollection<ResultRowRanks> CalculateResultsRanks(IEnumerable<object> a_Results)
        {
            Dictionary<Guid, ResultRowRanks> v_ResultRows = new Dictionary<Guid, ResultRowRanks>();

            foreach (Result i_Result in a_Results)
            {
                foreach (Score i_Score in i_Result.Scores)
                {
                    // Add a new ResultRow if there is none.
                    if (!v_ResultRows.ContainsKey(i_Score.IdPlayer))
                    {
                        v_ResultRows.Add(i_Score.IdPlayer, new ResultRowRanks(PlayersById[i_Score.IdPlayer].Name, 0, 0));
                    }

                    v_ResultRows[i_Score.IdPlayer].AmountPlayed++;

                    if (i_Score.IsWinner)
                    {
                        v_ResultRows[i_Score.IdPlayer].AmountWon++;
                    }

                    int v_ActualScore = int.Parse(i_Score.ActualScore);

                    v_ResultRows[i_Score.IdPlayer].WorstRank = v_ActualScore;
                    v_ResultRows[i_Score.IdPlayer].BestRank = v_ActualScore;
                }
            }

            ObservableCollection<ResultRowRanks> v_ResultRowInstances = new ObservableCollection<ResultRowRanks>();

            foreach (KeyValuePair<Guid, ResultRowRanks> i_Row in v_ResultRows)
            {
                // Calculate percentage won before we add the result to the collection.
                i_Row.Value.PercentageWon = Math.Round(100 * i_Row.Value.AmountWon / (double)i_Row.Value.AmountPlayed, 2);
                v_ResultRowInstances.Add(i_Row.Value);
            }

            return v_ResultRowInstances;
        }

        private ObservableCollection<ResultRowWinLose> CalculateResultsWinLose(IEnumerable<object> a_Results)
        {
            Dictionary<Guid, ResultRowWinLose> v_ResultRows = new Dictionary<Guid, ResultRowWinLose>();

            foreach (Result i_Result in a_Results)
            {
                foreach (Score i_Score in i_Result.Scores)
                {
                    // Add a new ResultRow if there is none.
                    if (!v_ResultRows.ContainsKey(i_Score.IdPlayer))
                    {
                        v_ResultRows.Add(i_Score.IdPlayer, new ResultRowWinLose(PlayersById[i_Score.IdPlayer].Name, 0, 0));
                    }

                    v_ResultRows[i_Score.IdPlayer].AmountPlayed++;

                    if (i_Score.IsWinner)
                    {
                        v_ResultRows[i_Score.IdPlayer].AmountWon++;
                    }

                    double v_ActualScore = double.Parse(i_Score.ActualScore);

                    if (v_ActualScore == 0.5)
                    {
                        v_ResultRows[i_Score.IdPlayer].AmountDraw++;
                    }
                }
            }

            ObservableCollection<ResultRowWinLose> v_ResultRowInstances = new ObservableCollection<ResultRowWinLose>();

            foreach (KeyValuePair<Guid, ResultRowWinLose> i_Row in v_ResultRows)
            {
                // Calculate percentage won before we add the result to the collection.
                i_Row.Value.PercentageWon = Math.Round(100 * i_Row.Value.AmountWon / (double)i_Row.Value.AmountPlayed, 2);
                v_ResultRowInstances.Add(i_Row.Value);
            }

            return v_ResultRowInstances;
        }

        public ObservableCollection<ResultRowPlayer> GeneratePlayerReport(Guid a_PlayerId)
        {
            // We want to start with the oldest results. The UI shows newest results on the top so we need to reverse order here.
            IEnumerable<Result> v_BeginningToEndResults = new ObservableCollection<Result>(Results.OrderBy(p => p.Date));

            Dictionary<String, ResultRowPlayer> v_ResultsRows = new Dictionary<string, ResultRowPlayer>();

            foreach (Result i_Result in v_BeginningToEndResults)
            {
                foreach (Score i_Score in i_Result.Scores)
                {
                    if (i_Score.IdPlayer == a_PlayerId)
                    {
                        string v_GameName = GamesById[i_Result.IdGame].Name;

                        if (!v_ResultsRows.ContainsKey(v_GameName))
                        {
                            v_ResultsRows.Add(v_GameName, new ResultRowPlayer(v_GameName, 0, 0));
                        }

                        v_ResultsRows[v_GameName].AmountPlayed++;

                        if (i_Score.IsWinner)
                        {
                            v_ResultsRows[v_GameName].AmountWon++;
                        }
                    }
                }
            }

            ObservableCollection<ResultRowPlayer> v_ResultRowInstances = new ObservableCollection<ResultRowPlayer>();
            ResultRowPlayer v_Totals = new ResultRowPlayer("Total", 0, 0);

            foreach (KeyValuePair<String, ResultRowPlayer> i_ResultRow in v_ResultsRows)
            {
                i_ResultRow.Value.CalculatePercentageWon();
                v_ResultRowInstances.Add(i_ResultRow.Value);
                v_Totals.AmountPlayed += i_ResultRow.Value.AmountPlayed;
                v_Totals.AmountWon += i_ResultRow.Value.AmountWon;
            }

            v_Totals.CalculatePercentageWon();
            v_ResultRowInstances.Add(v_Totals);

            return v_ResultRowInstances;
        }

        /// <summary>
        /// Calculates the ELO score for all players and their results. By default we assume a score of 1500 for a new player.
        /// </summary>
        /// <param name="a_GameOrFamilyId">Three possible ways to use it: Either a Guid for a single game or a game family works.
        /// If the ID is null or invalid the whole results database will be used.</param>
        /// <returns>A dictionary with the Player as key and a ResultHelper object containing the calculated results.</returns>
        public Dictionary<Player, Result.ResultHelper> CalculateEloResults(Guid a_GameOrFamilyId)
        {
            Dictionary<Player, Result.ResultHelper> v_EloResults = new Dictionary<Player, Result.ResultHelper>();

            // Start with the standard ELO number for every player.
            foreach (Player i_Player in Players)
            {
                v_EloResults.Add(i_Player, new Result.ResultHelper(i_Player.Id, c_EloStartValue, 0));
            }

            // We want to start with the oldest results. The UI shows newest results on the top so we need to reverse order here.
            IEnumerable<Result> v_BeginningToEndResults = new ObservableCollection<Result>(Results.OrderBy(p => p.Date));

            if (GameFamiliesById.ContainsKey(a_GameOrFamilyId))
            {
                // First: Get all games from the given game family.
                var v_AllGamesFromFamily = Games.Where(p => p.IdGamefamilies.Contains(a_GameOrFamilyId));

                if (v_AllGamesFromFamily.Count() > 0)
                {
                    // Second: Get all results with games of the given game family.
                    v_BeginningToEndResults = v_BeginningToEndResults.Join(v_AllGamesFromFamily,
                        result => result.IdGame,
                        game => game.Id,
                        (result, game) => result);
                }
                else
                {
                    v_BeginningToEndResults = new ObservableCollection<Result>();
                }
            }
            else if (GamesById.ContainsKey(a_GameOrFamilyId))
            {
                v_BeginningToEndResults = v_BeginningToEndResults.Where(p => p.IdGame == a_GameOrFamilyId);
            }

            foreach (Result i_Result in v_BeginningToEndResults)
            {
                Dictionary<Guid, Result.ResultHelper> v_TempEloResults = new Dictionary<Guid, Result.ResultHelper>();

                // We do not consider results of solo games.
                if (i_Result.Scores.Count == 1)
                {
                    continue;
                }

                foreach (Score i_Score in i_Result.Scores)
                {
                    v_TempEloResults.Add(i_Score.IdPlayer, v_EloResults[PlayersById[i_Score.IdPlayer]]);
                }

                if (GamesById[i_Result.IdGame].Type == Game.GameType.Ranks)
                {
                    i_Result.CalculateEloResultsForRanks(v_TempEloResults, i_Result.Date);
                }
                else
                {
                    i_Result.CalculateEloResults(v_TempEloResults, i_Result.Date);
                }
            }

            return v_EloResults;
        }

        public ObservableCollection<ResultRowGeneric> GeneratePlayersOverGames()
        {
            String v_KeyTotalPlayed = "Total Played";

            Dictionary<string, Dictionary<string, int>> v_PlayerOverGamesDict = new Dictionary<string, Dictionary<string, int>>();

            foreach (Game i_Game in Games)
            {
                v_PlayerOverGamesDict.Add(i_Game.Name, new Dictionary<string, int>());
                v_PlayerOverGamesDict[i_Game.Name].Add(v_KeyTotalPlayed, 0);

                foreach (Player i_Player in PlayersSorted)
                {
                    v_PlayerOverGamesDict[i_Game.Name].Add(i_Player.Name, 0);
                }
            }

            foreach (Result i_Result in Results)
            {
                v_PlayerOverGamesDict[GamesById[i_Result.IdGame].Name][v_KeyTotalPlayed]++;

                foreach (Score i_Score in i_Result.Scores)
                {
                    v_PlayerOverGamesDict[GamesById[i_Result.IdGame].Name][PlayersById[i_Score.IdPlayer].Name]++;
                }
            }

            var v_ResultRows = new ObservableCollection<ResultRowGeneric>();

            foreach (KeyValuePair<string, Dictionary<string, int>> i_Outer in v_PlayerOverGamesDict)
            {
                ResultRowGeneric v_ActualRow = new ResultRowGeneric();
                v_ActualRow.Properties.Add(new GenericProperty("GameName", i_Outer.Key));

                foreach (KeyValuePair<string, int> i_Inner in i_Outer.Value)
                {
                    v_ActualRow.Properties.Add(new GenericProperty(i_Inner.Key, i_Inner.Value));
                }

                v_ResultRows.Add(v_ActualRow);
            }

            return v_ResultRows;
        }

        #region DatabaseChanged EventHandlers

        public void SortCollections()
        {
            NotifyPropertyChanged("GamesPointBased");
            NotifyPropertyChanged("GamesSorted");
            NotifyPropertyChanged("GameFamiliesFiltered");
            NotifyPropertyChanged("LocationsSorted");
            NotifyPropertyChanged("PlayersSorted");
            NotifyPropertyChanged("ResultsOrdered");
        }

        private void DbClasses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DbHelper.Instance.IsChanged = true;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (sender is ObservableCollection<Player>)
                {
                    PlayersById.Add(((Player)e.NewItems[0]).Id, (Player)e.NewItems[0]);
                    NotifyPropertyChanged("PlayersSorted");
                }
                else if (sender is ObservableCollection<GameFamily>)
                {
                    GameFamiliesById.Add(((GameFamily)e.NewItems[0]).Id, (GameFamily)e.NewItems[0]);
                    NotifyPropertyChanged("GameFamiliesFiltered");
                }
                else if (sender is ObservableCollection<Location>)
                {
                    LocationsById.Add(((Location)e.NewItems[0]).Id, (Location)e.NewItems[0]);
                    NotifyPropertyChanged("LocationsSorted");
                }
                else if (sender is ObservableCollection<Game>)
                {
                    GamesById.Add(((Game)e.NewItems[0]).Id, (Game)e.NewItems[0]);
                    NotifyPropertyChanged("GamesPointBased");
                    NotifyPropertyChanged("GamesSorted");
                }
                else if (sender is ObservableCollection<Result>)
                {
                    Result v_Result = (Result)e.NewItems[0];

                    // Hooking up the notifications for the new object.
                    v_Result.PropertyChanged += Result_PropertyChanged;

                    foreach (Score i_Score in v_Result.Scores)
                    {
                        i_Score.PropertyChanged += Result_PropertyChanged;
                    }

                    NotifyPropertyChanged("ResultsOrdered");
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
                    NotifyPropertyChanged("PlayersSorted");
                }
                else if (sender is ObservableCollection<GameFamily>)
                {
                    GameFamiliesById.Remove(((GameFamily)e.OldItems[0]).Id);
                    NotifyPropertyChanged("GameFamiliesFiltered");
                }
                else if (sender is ObservableCollection<Location>)
                {
                    LocationsById.Remove(((Location)e.OldItems[0]).Id);
                    NotifyPropertyChanged("LocationsSorted");
                }
                else if (sender is ObservableCollection<Game>)
                {
                    GamesById.Remove(((Game)e.OldItems[0]).Id);
                    NotifyPropertyChanged("GamesPointBased");
                    NotifyPropertyChanged("GamesSorted");
                }
                else if (sender is ObservableCollection<Result>)
                {
                    NotifyPropertyChanged("ResultsOrdered");
                }
                else
                {
                    throw new NotImplementedException(String.Format("Removing of [{0}] is not supported.", sender.GetType()));
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                m_Logger.Debug("Resetting collection for [" + sender.GetType() + "].");

                if (sender is ObservableCollection<Player>)
                {
                    PlayersById.Clear();
                    NotifyPropertyChanged("PlayersSorted");
                }
                else if (sender is ObservableCollection<GameFamily>)
                {
                    GameFamiliesById.Clear();
                    NotifyPropertyChanged("GameFamiliesFiltered");
                }
                else if (sender is ObservableCollection<Location>)
                {
                    LocationsById.Clear();
                    NotifyPropertyChanged("LocationsSorted");
                }
                else if (sender is ObservableCollection<Game>)
                {
                    GamesById.Clear();
                    NotifyPropertyChanged("GamesPointBased");
                    NotifyPropertyChanged("GamesSorted");
                }
                else if (sender is ObservableCollection<Result>)
                {
                    NotifyPropertyChanged("ResultsOrdered");
                }
                else
                {
                    throw new NotImplementedException(String.Format("Resetting of [{0}] is not supported.", sender.GetType()));
                }
            }
            else
            {
                throw new NotImplementedException(String.Format("Action {0} not supported on collection.", e.Action));
            }
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String a_PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a_PropertyName));
        }

        #endregion
    }
}
