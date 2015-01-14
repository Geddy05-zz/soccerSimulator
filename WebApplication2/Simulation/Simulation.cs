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
        private ApplicationDbContext applicationdb = new ApplicationDbContext();
        private CreateModels createModels = new CreateModels();
        private PouleManager pouleManager = new PouleManager();
        private ReportManager reportManager = new ReportManager();

        //
        private int[] matchSchema = { 9, 10, 11, 12, 9, 11, 10, 12, 9, 12, 10, 11 };

        private TeamModel teamAttack;
        private TeamModel teamDefence;
        private TeamModel homeTeam;
        private TeamModel awayTeam;

        private List<string> reportMatch = new List<string>();

        private int homeScore = 0;
        private int awayScore = 0;
        private int yellowCardProbability;

        // Random.Next() use systemclock so if we dont use a lock 
        // the random number will almost be the same number
        private static readonly Random random = new Random();
        private static readonly object sLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (sLock)
            { 
                return random.Next(min, max);
            }
        }

        public void StartGame()
        {
            RemoveLastGame();
            CreateGame();

            for (int i = 0; i < matchSchema.Length; i+=2)
            {
                yellowCardProbability = RandomNumber(1, 10);
                homeScore = 0;
                awayScore = 0;
                SimulateMatch(matchSchema[i], matchSchema[i + 1]);
            }
            pouleManager.DecidePosistion();
        }

        public void RemoveLastGame()
        {
            var allMatches = from everything in applicationdb.MatchModels select everything;
            applicationdb.MatchModels.RemoveRange(allMatches);

            var allPoule = from everything in applicationdb.PouleModels select everything;
            applicationdb.PouleModels.RemoveRange(allPoule);
            applicationdb.SaveChanges();
        }

        public void CreateGame()
        {
            createModels.CreatePoule();
            createModels.Createteams();
        }

        public void SimulateMatch(int TeamHomeId, int TeamAwayId)
        {
            homeTeam = applicationdb.TeamModels.Find(TeamHomeId);
            awayTeam = applicationdb.TeamModels.Find(TeamAwayId);

            for (int minute = 0 ; minute < 90; minute = minute + 7)
            {
                ChoiceAttackingTeam(homeTeam, awayTeam, minute);
                TryToScore(minute);
            }
            saveMatchResults();
            savePoule();
        }

        public void ChoiceAttackingTeam(TeamModel Home, TeamModel Away, int minute)
        {
            if (minute % 2 == 0)
            {
                teamAttack = Home;
                teamDefence = Away;
            }
            else
            {
                teamAttack = Away;
                teamDefence = Home;
            }
        }

        public void TryToScore(int minute)
        {
            int totalpoints = teamAttack.Attack + teamDefence.Defence;
            int choice = RandomNumber(0, totalpoints);
            int goal = RandomNumber(0, (teamDefence.Keeper + teamAttack.Attack));
            minute = minute + RandomNumber(1, 8);

            if (choice % yellowCardProbability == 1) 
            {
                string matchEvent = minute + " " + teamAttack.Country + " krijgt gele kaart";
                reportMatch.Add(matchEvent);
                choice = 0;
            }
            else if (choice > totalpoints-(teamAttack.Attack/10))
            {
                string matchEvent = minute + " " + teamAttack.Country + " krijgt een vrije trap";
                reportMatch.Add(matchEvent);
            }
            if (choice > teamDefence.Defence && goal > teamDefence.Keeper)
            {
                goalScored(minute);
            }
            else if (choice < (teamDefence.Defence / 2)&& choice != 0)
            {
                string matchEvent = minute + " " + teamAttack.Country + " neemt een corner";
                reportMatch.Add(matchEvent);
                choice = 1;
            }
        }

        public void goalScored(int minute)
        {
            if (teamAttack.Country == homeTeam.Country) 
            {
                homeScore++;
            }
            else 
            {
                awayScore++;
            }
            string matchEvent = minute + " " + teamAttack.Country + " Heeft gescoord";
            reportMatch.Add(matchEvent);
        }

        public void saveMatchResults()
        {
            MatchModel matchResults = new MatchModel();
            matchResults.NameHomeTeam = homeTeam.Country;
            matchResults.Goals = homeScore;
            matchResults.NameAwayTeam = awayTeam.Country;
            matchResults.GoalsAgainst = awayScore;

            applicationdb.MatchModels.Add(matchResults);
            applicationdb.SaveChanges();

            reportManager.saveMatchReport(reportMatch,homeTeam,awayTeam);
            reportMatch = new List<string>();

        }

        public void savePoule()
        {
            PouleModel Home = applicationdb.PouleModels.FirstOrDefault(C => C.Country.Contains(homeTeam.Country));
            PouleModel Away = applicationdb.PouleModels.FirstOrDefault(C => C.Country.Contains(awayTeam.Country));

            Home.GamesPlayed++;
            Home.Goals = Home.Goals + homeScore;
            Home.GoalsAgainst = Home.GoalsAgainst + awayScore;
            Home.GoalsTotaal = Home.Goals - Home.GoalsAgainst;

            Away.GamesPlayed++;
            Away.Goals = Away.Goals + awayScore;
            Away.GoalsAgainst = Away.GoalsAgainst + homeScore;
            Away.GoalsTotaal = Away.Goals - Away.GoalsAgainst;

            if (homeScore > awayScore) Home.Points = Home.Points + 3;

            if (homeScore == awayScore)
            {
                Home.Points++;
                Away.Points++;
            }

            if (homeScore < awayScore) Away.Points = Away.Points + 3;

            applicationdb.SaveChanges();
        }
    }
}