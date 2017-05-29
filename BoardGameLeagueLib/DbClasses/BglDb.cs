using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using static BoardGameLeagueLib.Game;
using static BoardGameLeagueLib.Player;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRootAttribute("BoardGameLeagueDatabase")]
    public class BglDb
    {
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

        public static Dictionary<GameType, String> GameTypeEnumWithCaptions { get { return Game.GameTypeEnumWithCaptions; } }

        public static List<Genders> GendersList { get { return Player.GendersList; } }

        public void Init()
        {
            PlayersById = new Dictionary<Guid, Player>();

            foreach (Player i_Player in Players)
            {
                PlayersById.Add(i_Player.Id, i_Player);
            }

            GameFamiliesById = new Dictionary<Guid, GameFamily>();

            foreach (GameFamily i_Family in GameFamilies)
            {
                GameFamiliesById.Add(i_Family.Id, i_Family);
            }

            GamesById = new Dictionary<Guid, Game>();

            foreach (Game i_Game in Games)
            {
                i_Game.Family = GameFamiliesById[i_Game.IdGamefamily];
                GamesById.Add(i_Game.Id, i_Game);
            }
        }
    }
}
