using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Simulation
{
    public class CreateModels
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string[] Countries = { "Nederland", "Spanje", "Chilli", "Australië" };

        public void CreatePoule()
        {
            if (!db.PouleModels.Any())
            {

                foreach (string Country in Countries)
                {
                    PouleModel poule = new PouleModel();
                    poule.Country = Country;
                    poule.GamesPlayed = 0;
                    poule.Goals = 0;
                    poule.GoalsAgainst = 0;
                    poule.GoalsTotaal = 0;
                    poule.Points = 0;

                    db.PouleModels.Add(poule);
                }

                db.SaveChanges();
            }
        }

        public void Createteams()
        {
            if (!db.TeamModels.Any())
            {
                TeamModel Nederland = new TeamModel();
                Nederland.Country = "Nederland";
                Nederland.Attack = 85;
                Nederland.Defence = 70;
                Nederland.Keeper = 75;
                Nederland.tactic = 90;

                TeamModel Spanje = new TeamModel();
                Spanje.Country = "Spanje";
                Spanje.Attack = 85;
                Spanje.Defence = 80;
                Spanje.Keeper = 80;
                Spanje.tactic = 70;

                TeamModel Chilli = new TeamModel();
                Chilli.Country = "Chilli";
                Chilli.Attack = 75;
                Chilli.Defence = 75;
                Chilli.Keeper = 70;
                Chilli.tactic = 70;

                TeamModel Australië = new TeamModel();
                Australië.Country = "Australië";
                Australië.Attack = 50;
                Australië.Defence = 70;
                Australië.Keeper = 65;
                Australië.tactic = 60;

                db.TeamModels.Add(Spanje);
                db.TeamModels.Add(Nederland);
                db.TeamModels.Add(Chilli);
                db.TeamModels.Add(Australië);

                db.SaveChanges();

            }
        }

    }
}