using System;

namespace BoardGameLeagueLib
{
    public class Location : DbObjectName
    {
        public Location()
            : base("no name")
        {
        }

        public Location(String a_Name)
            : base(a_Name)
        {
        }
    }
}
