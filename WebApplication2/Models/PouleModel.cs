using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class PouleModel
    {
        public int ID { get; set; }
        public int Position { get; set; }
        public string Country { get; set; }
        public int GamesPlayed { get; set; }
        public int Goals { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalsTotaal { get; set; }
        public int Points { get; set; }
    }

    public class TeamModel
    {
        public int ID { get; set; }
        public string Country { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int Keeper { get; set; }
        public int tactic { get; set; }
    }
    
    public class MatchModel
    {
        public int ID { get; set; }
        public string NameHomeTeam { get; set; }
        public int Goals { get; set; }
        public string NameAwayTeam { get; set; }
        public int GoalsAgainst { get; set; }
    }

}