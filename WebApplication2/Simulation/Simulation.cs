using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using WebApplication2.Models;

namespace WebApplication2.Simulation
{
    public class Game
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CreateModels Createmodels = new CreateModels();
        //private int[] MatchSchema = { 9, 10, 11, 12, 9, 11, 10, 12, 9, 12, 10, 11 };
        private int[] MatchSchema = { 1, 2, 3, 4, 1, 3, 2, 4, 1, 4, 2, 3 };

        private TeamModel TeamAttack;
        private TeamModel TeamDefence;
        private TeamModel HomeTeam;
        private TeamModel AwayTeam;
        private PouleModel a;
        private PouleModel b;

        private int HomeScore = 0;
        private int AwayScore = 0;

        private int changeYellowCard;

        public void StartGame()
        {
            RemoveLastGame();
            CreateGame();
            for (int i = 0; i < MatchSchema.Length; i++)
            {
                changeYellowCard = new Random().Next(1, 10);
                HomeScore = 0;
                AwayScore = 0;
                SimulateMatch(MatchSchema[i], MatchSchema[i + 1]);
                i++;
            }
            decidePosistion();
        }

        public void RemoveLastGame()
        {
            var allMatches = from everthing in db.MatchModels select everthing;
            db.MatchModels.RemoveRange(allMatches);
            var allPoule = from everthing in db.PouleModels select everthing;
            db.PouleModels.RemoveRange(allPoule);

            db.SaveChanges();
        }

        public void CreateGame()
        {
            Createmodels.CreatePoule();
            Createmodels.Createteams();
        }

        public void SimulateMatch(int TeamHomeId, int TeamAwayId)
        {
            HomeTeam = db.TeamModels.Find(TeamHomeId);
            AwayTeam = db.TeamModels.Find(TeamAwayId);

            for (int i = 0; i < 90; i = i + 9)
            {
                choiceAttackingTeam(HomeTeam, AwayTeam, i);
                tryToScore(i);
                Thread.Sleep(50);
            }
            saveMatchResults(HomeTeam.Country, AwayTeam.Country);
            savePoule();
        }

        public void choiceAttackingTeam(TeamModel Home, TeamModel Away, int i)
        {
            if (i % 2 == 0)
            {
                TeamAttack = Home;
                TeamDefence = Away;
            }
            else
            {
                TeamAttack = Away;
                TeamDefence = Home;
            }
        }

        public void tryToScore(int i)
        {
            int Attackpoints = TeamAttack.Attack;
            int Defencepoints = TeamDefence.Defence;
            int totaalpoints = Attackpoints + Defencepoints;

            int choice = new Random().Next(0, totaalpoints);
            int goal = new Random().Next(0, (TeamDefence.Keeper + TeamAttack.Attack));

            if (choice % changeYellowCard == 0) { choice = 0; }

            if (choice > Defencepoints && goal > TeamDefence.Keeper)
            {
                if (TeamAttack.Country == HomeTeam.Country)
                {
                    HomeScore++;
                }
                else
                {
                    AwayScore++;
                }
            }
        }

        public void saveMatchResults(String HomeTeam, String AwayTeam)
        {
            MatchModel match = new MatchModel();
            match.NameHomeTeam = HomeTeam;
            match.Goals = HomeScore;
            match.NameAwayTeam = AwayTeam;
            match.GoalsAgainst = AwayScore;

            db.MatchModels.Add(match);
            db.SaveChanges();

        }

        public void savePoule()
        {
            PouleModel Home = db.PouleModels.FirstOrDefault(C => C.Country.Contains(HomeTeam.Country));
            PouleModel Away = db.PouleModels.FirstOrDefault(C => C.Country.Contains(AwayTeam.Country));

            Home.GamesPlayed++;
            Home.Goals = Home.Goals + HomeScore;
            Home.GoalsAgainst = Home.GoalsAgainst + AwayScore;
            Home.GoalsTotaal = Home.Goals - Home.GoalsAgainst;

            Away.GamesPlayed++;
            Away.Goals = Away.Goals + AwayScore;
            Away.GoalsAgainst = Away.GoalsAgainst + HomeScore;
            Away.GoalsTotaal = Away.Goals - Away.GoalsAgainst;

            if (HomeScore > AwayScore)
            {
                Home.Points = Home.Points + 3;
            }

            if (HomeScore == AwayScore)
            {
                Home.Points++;
                Away.Points++;
            }
            if (HomeScore < AwayScore)
            {
                Away.Points = Away.Points + 3;
            }

            db.SaveChanges();
        }

        public void decidePosistion()
        {

            List<PouleModel> SortedList = db.PouleModels.OrderByDescending(p => p.Points)
                                       .ThenByDescending(p => p.GoalsTotaal).ToList();

            var allPoule = from everthing in db.PouleModels select everthing;
            db.PouleModels.RemoveRange(allPoule);
            db.SaveChanges();

            for (int i = 0; i < SortedList.Count-1; i++)
            {
                int j = i+1;
                a = SortedList[i];
                b = SortedList[j];

                if (a.Points == b.Points && a.GoalsTotaal == b.GoalsTotaal)
                {
                    if (a.Goals == b.Goals)
                    {
                        ComparerMatchresult(i);
                    }
                    else 
                    {
                        a.Position = i + 1;
                    }
                }
                else
                {
                    a.Position = i + 1;
                    b.Position = i + 2;
                }
                db.PouleModels.Add(a);
                db.PouleModels.Add(b);
            }
            db.SaveChanges();
        }

        public void ComparerMatchresult(int i)
        {
            try
            {
                MatchModel compareResult = db.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(a.Country) && C.NameAwayTeam.Contains(b.Country));
                if (compareResult.Goals > compareResult.GoalsAgainst)
                {
                    a.Position = i + 1;
                }
                if (compareResult.Goals < compareResult.GoalsAgainst)
                {
                    b.Position = i + 1;
                    a.Position = i + 2;
                }
            }
            catch
            {
                MatchModel compareResult = db.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(b.Country) && C.NameAwayTeam.Contains(a.Country));
                if (compareResult.Goals > compareResult.GoalsAgainst)
                {
                    a.Position = i + 1;
                }
                if (compareResult.Goals < compareResult.GoalsAgainst)
                {
                    b.Position = i + 1;
                    a.Position = i + 2;
                }
            }
        }


    }
}