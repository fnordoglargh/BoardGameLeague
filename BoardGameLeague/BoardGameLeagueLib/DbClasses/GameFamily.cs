using System;

namespace BoardGameLeagueLib.DbClasses
{
    public class GameFamily : DbObjectName
    {
        /// <summary>
        /// Invalid standard id with the value "00000000-0000-4000-0000-000000000000"-
        /// </summary>
        public static readonly Guid c_StandardId = Guid.Parse("00000000-0000-4000-0000-000000000000");

        public GameFamily()
            : base("Default Game Family Name")
        {
        }

        public GameFamily(String a_Name)
            : base(a_Name)
        {
        }

        public override string ToString()
        {
            return "ID: " + Id + ", Name: " + Name;
        }
    }
}
