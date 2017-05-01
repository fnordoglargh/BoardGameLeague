using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRootAttribute("BoardGameLeagueDatabase")]
    public class BglDb
    {
        public ObservableCollection<GameFamily> GameFamilies;
        public ObservableCollection<Location> Locations;
        public ObservableCollection<Game> Games;

        [XmlIgnore]
        public Game SelectedGame;

        [XmlIgnore]
        public Hashtable GameFamiliesById
        {
            get;
            private set;
        }

        public void Init()
        {
            GameFamiliesById = new Hashtable();

            foreach (GameFamily i_Family in GameFamilies)
            {
                GameFamiliesById.Add(i_Family.Id, i_Family);
            }
        }

    }
}
