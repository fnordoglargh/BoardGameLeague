using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using log4net;
using BoardGameLeagueLib.DbClasses;

namespace BoardGameLeagueLib
{
    public class DbClass
    {
        public struct CalculatedResult
        {
            public int Rank { get; set; }
            public String Name { get; set; }
            public int AmountPlayedGamed { get; set; }
            public int AmountGamesWon { get; set; }
            public double PercentageWon { get; set; }
            public int Points { get; set; }
            public double PointsAverage { get; set; }
        }

        //private ObservableCollection<Person> m_Persons = null;

        private ILog m_Logger = LogManager.GetLogger("DbClass");

        public ObservableCollection<Person> Persons
        {
            //get
            //{
            //    return m_Persons;
            //}
            //private set
            //{
            //    m_Persons = value;
            //}
            get;
            set;
        }

        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<GameFamily> GameFamilies { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Result> Results { get; set; }

        public Person SelectedPlayer
        {
            get;
            set;
        }

        public Game SelectedGame
        {
            get;
            set;
        }

        public Result SelectedResult
        {
            get;
            set;
        }

        public Location SelectedLocation
        {
            get;
            set;
        }

        public DbClass()
        {
        }

        public bool BootStrap()
        {
            Persons = (ObservableCollection<Person>)DbLoader.ReadWithXmlSerializer("db\\person.xml", typeof(ObservableCollection<Person>));
            GameFamilies = (ObservableCollection<GameFamily>)DbLoader.ReadWithXmlSerializer("db\\gamefamily.xml", typeof(ObservableCollection<GameFamily>));
            Locations = (ObservableCollection<Location>)DbLoader.ReadWithXmlSerializer("db\\location.xml", typeof(ObservableCollection<Location>));
            Games = (ObservableCollection<Game>)DbLoader.ReadWithXmlSerializer("db\\game.xml", typeof(ObservableCollection<Game>));
            Results = (ObservableCollection<Result>)DbLoader.ReadWithXmlSerializer("db\\result.xml", typeof(ObservableCollection<Result>));

            //Results = new ObservableCollection<Result>();
            //List<Score> v_Scores = new List<Score>();
            //v_Scores.Add(new Score(Persons[0].Id, "1"));
            //v_Scores.Add(new Score(Persons[1].Id, "2"));

            //List<Guid> v_Winners = new List<Guid>();
            //v_Winners.Add(Persons[0].Id);

            //Results.Add(new Result(Games[0].Id,v_Scores,v_Winners,DateTime.Now));

            //DbLoader.WriteWithXmlSerializer("resuuuuults.xml",Results);

            bool v_IsEverythingFine = true;

            if (Persons == null || GameFamilies == null || Locations == null || Games == null || Results == null)
            {
                v_IsEverythingFine = false;
            }
            else
            {

                GameFamiliesById = new Hashtable();

                foreach (GameFamily i_Family in GameFamilies)
                {
                    GameFamiliesById.Add(i_Family.Id, i_Family);
                }

                PersonsById = new Hashtable();

                foreach (Person i_Person in Persons)
                {
                    PersonsById.Add(i_Person.Id, i_Person);
                }

                GamesById = new Hashtable();

                foreach (Game i_Game in Games)
                {
                    GamesById.Add(i_Game.Id, i_Game);
                }

                LocationsById = new Hashtable();

                foreach (Location i_Location in Locations)
                {
                    LocationsById.Add(i_Location.Id, i_Location);
                }

                v_IsEverythingFine = ValidateDatabase();
            }

            return v_IsEverythingFine;
        }

        public bool SaveDatabase()
        {
            bool v_IsSavedCorrectly = false;

            v_IsSavedCorrectly = DbLoader.WriteWithXmlSerializer("db\\result.xml", Results);
            v_IsSavedCorrectly &= DbLoader.WriteWithXmlSerializer("db\\person.xml", Persons);
            v_IsSavedCorrectly &= DbLoader.WriteWithXmlSerializer("db\\gamefamily.xml", GameFamilies);
            v_IsSavedCorrectly &= DbLoader.WriteWithXmlSerializer("db\\game.xml", Games);
            v_IsSavedCorrectly &= DbLoader.WriteWithXmlSerializer("db\\location.xml", Locations);

            return v_IsSavedCorrectly;
        }

        public bool ValidateDatabase()
        {
            bool v_IsLoadedCorrectly = true;

            m_Logger.Info("Validating Database.");
            m_Logger.Info(String.Format("Loaded {0} Players.", Persons.Count));
            m_Logger.Info(String.Format("Loaded {0} GameFamilies.", GameFamilies.Count));
            m_Logger.Info(String.Format("Loaded {0} Locations.", Locations.Count));

            //Console.WriteLine("Validating Database.");
            //Console.WriteLine(String.Format("Loaded {0} Players.", Persons.Count));
            //Console.WriteLine(String.Format("Loaded {0} GameFamilies.", GameFamilies.Count));
            //Console.WriteLine(String.Format("Loaded {0} Locations.", Locations.Count));

            //bool v_IsVariablePlausible = false;
            //bool v_IsGamesCollectionPlausible = true;

            foreach (Game i_Game in Games)
            {
                v_IsLoadedCorrectly = GameFamiliesById.ContainsKey(i_Game.IdGamefamily);

                if (!v_IsLoadedCorrectly)
                {
                    m_Logger.Info(String.Format("Game {0} has no valid family!", i_Game.Name));
                }
            }

            if (v_IsLoadedCorrectly)
            {
                m_Logger.Info(String.Format("Loaded {0} Games.", Games.Count));
            }
            else
            {
                m_Logger.Info("Game loading unsucessful!");
            }

            foreach (Result i_Result in Results)
            {
                v_IsLoadedCorrectly &= GamesById.ContainsKey(i_Result.IdGame);

                if (!v_IsLoadedCorrectly)
                {
                    m_Logger.Info("Found result without game! " + i_Result.Id);
                }

                foreach (Score i_Score in i_Result.Scores)
                {
                    v_IsLoadedCorrectly &= PersonsById.ContainsKey(i_Score.IdPerson);

                    if (!v_IsLoadedCorrectly)
                    {
                        m_Logger.Info("Found score without person! " + i_Result.Id + ", " + i_Result.Scores.IndexOf(i_Score));
                    }
                }

                foreach (Guid i_Id in i_Result.Winners)
                {
                    v_IsLoadedCorrectly &= PersonsById.ContainsKey(i_Id);

                    if (!v_IsLoadedCorrectly)
                    {
                        m_Logger.Info("Found winner without person! " + i_Result.Id);
                    }
                }

                v_IsLoadedCorrectly &= LocationsById.ContainsKey(i_Result.IdLocation);

                if (!v_IsLoadedCorrectly)
                {
                    m_Logger.Info("Found invalid location Id! " + i_Result.Id);
                }
            }

            if (v_IsLoadedCorrectly)
            {
                m_Logger.Info(String.Format("Loaded {0} Results.", Results.Count));
            }
            else
            {
                m_Logger.Info("Result loading unsucessful!");
            }

            return v_IsLoadedCorrectly;
        }

        public Hashtable GameFamiliesById
        {
            get;
            private set;
        }

        public Hashtable GamesById
        {
            get;
            private set;
        }

        public Hashtable PersonsById
        {
            get;
            private set;
        }

        public Hashtable LocationsById
        {
            get;
            private set;
        }

        //private Person m_SelectedItem;

        //public Person SelectedItem
        //{
        //    get
        //    {
        //        return m_SelectedItem;
        //    }
        //    set
        //    {
        //        m_SelectedItem = value;
        //        NotifyPropertyChanged("SelectedItem");
        //    }
        //}

        //#region PropertyChanged

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //    }
        //}

        //#endregion
    }
}
