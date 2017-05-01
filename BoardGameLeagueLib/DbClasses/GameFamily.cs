using System;
using BoardGameLeagueLib.DbClasses;

namespace BoardGameLeagueLib
{
    public class GameFamily : DbObjectName
    {
        public static readonly Guid c_StandardId = Guid.Parse("00000000-0000-4000-0000-000000000000");

        public GameFamily()
            : base("no name")
        {
        }

        public GameFamily(String a_Name)
            : base(a_Name)
        {
        }

        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}
