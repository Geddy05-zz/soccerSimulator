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
                TeamModel netherlands = new TeamModel();
                netherlands.Country = "Nederland";
                netherlands.Attack = 85;
                netherlands.Defence = 70;
                netherlands.Keeper = 75;
                netherlands.tactic = 90;

                TeamModel spain = new TeamModel();
                spain.Country = "Spanje";
                spain.Attack = 85;
                spain.Defence = 80;
                spain.Keeper = 80;
                spain.tactic = 70;

                TeamModel chilli = new TeamModel();
                chilli.Country = "Chilli";
                chilli.Attack = 75;
                chilli.Defence = 75;
                chilli.Keeper = 70;
                chilli.tactic = 70;

                TeamModel australia = new TeamModel();
                australia.Country = "Australië";
                australia.Attack = 50;
                australia.Defence = 70;
                australia.Keeper = 65;
                australia.tactic = 60;

                db.TeamModels.Add(spain);
                db.TeamModels.Add(netherlands);
                db.TeamModels.Add(chilli);
                db.TeamModels.Add(australia);

                db.SaveChanges();
            }
        }
    }
}