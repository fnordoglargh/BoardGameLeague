using System.Collections.Generic;
using System.Xml.Serialization;

namespace BoardGameLeagueLib.DbClasses
{
    [XmlRootAttribute("BoardGameLeagueDatabase")]
    public class BglDb
    {
        public List<GameFamily> GameFamilies;
        public List<Location> Locations;
    }
}
