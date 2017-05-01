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
    }
}
