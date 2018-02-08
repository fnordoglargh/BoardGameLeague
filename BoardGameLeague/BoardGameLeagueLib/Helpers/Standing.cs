namespace BoardGameLeagueLib.Helpers
{
    public class Standing
    {
        public int Won { get; set; }
        public int Stalemate { get; set; }
        public int Lost { get; set; }
        public bool IsInvalid { get; set; }

        public Standing()
        {
            Won = 0;
            Stalemate = 0;
            Lost = 0;
            IsInvalid = false;
        }

        public override string ToString()
        {
            if (!IsInvalid)
            {
                return Won + "-" + Stalemate + "-" + Lost;
            }
            else
            {
                return "";
            }
        }
    }
}
