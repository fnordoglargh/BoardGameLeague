using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using static BoardGameLeagueLib.Game;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRootAttribute("BoardGameLeagueDatabase")]
    public class BglDb
    {
        public ObservableCollection<GameFamily> GameFamilies { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Game> Games { get; set; }

        [XmlIgnore]
        public Game SelectedGame;

        [XmlIgnore]
        public Dictionary<Guid, GameFamily> GameFamiliesById
        {
            get;
            private set;
        }

        public static Dictionary<GameType, String> GameTypeEnumWithCaptions
        {
            get
            {
                return Game.GameTypeEnumWithCaptions;
            }
        }

        public void Init()
        {
            GameFamiliesById = new Dictionary<Guid, GameFamily>();
            
            foreach (GameFamily i_Family in GameFamilies)
            {
                GameFamiliesById.Add(i_Family.Id, i_Family);
            }

            foreach (Game i_Game in Games)
            {
                i_Game.Family = GameFamiliesById[i_Game.IdGamefamily];
            }
        }
    }
}
