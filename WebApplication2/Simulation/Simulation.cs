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
        private ApplicationDbContext applicationdb; 
        private CreateModels createModels; 
        private PouleManager pouleManager;
        private ReportManager reportManager;

        public TeamModel teamAttack;
        private TeamModel teamDefence;
        private TeamModel homeTeam;
        private TeamModel awayTeam;

        // this array shows use the match schema. I use this because now we got a logical schema.
        private int[] matchSchema = { 1, 2, 3, 4, 1, 3, 2, 4, 1, 4, 2, 3 };
        private int homeScore = 0;
        private int awayScore = 0;
        private int yellowCardProbability;


        public Game() {
            applicationdb = new ApplicationDbContext();
            createModels = new CreateModels();
            pouleManager = new PouleManager();
            reportManager = new ReportManager();
        }


        // Random.Next() use systemclock so if we dont use a lock 
        // the random number will almost be the same number
        private static readonly Random random = new Random();
        private static readonly object sinckLock = new object();

        public static int RandomNumber(int min, int max)
        {
            lock (sinckLock)
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

            for (int minute = 0 ; minute < 90; minute = minute + 7) {
                ChoiceAttackingTeam(homeTeam, awayTeam, minute);
                TryToScore(minute);
            }

            SaveMatchResults();
            SavePoule();
        }

        public void ChoiceAttackingTeam(TeamModel Home, TeamModel Away, int minute) {
            if (minute % 2 == 0) {
                teamAttack = Home;
                teamDefence = Away;
            }
            else {
                teamAttack = Away;
                teamDefence = Home;
            }
        }

        public void TryToScore(int minute) {
            int totalpoints = teamAttack.Attack + teamDefence.Defence;
            int choice = RandomNumber(0, totalpoints);
            int goal = RandomNumber(0, (teamDefence.Keeper + teamAttack.Attack));
            minute = minute + RandomNumber(1, 8);

            if (choice % yellowCardProbability == 1) {
                reportManager.MatchEventReport(ReportManager.MatchEvent.yellowCard, minute, teamAttack.Country);
                choice = 0;
            }
            else if (choice > totalpoints-(teamAttack.Attack/10)) {
                reportManager.MatchEventReport(ReportManager.MatchEvent.freeKick, minute, teamAttack.Country);
            }
            if (choice > teamDefence.Defence && goal > teamDefence.Keeper) {
                GoalScored(minute);
            }
            else if (choice < (teamDefence.Defence / 2)&& choice != 0) {
                reportManager.MatchEventReport(ReportManager.MatchEvent.freeKick, minute, teamAttack.Country);
                choice = 1;
            }
        }


        public void GoalScored(int minute) {
            if (teamAttack.Country == homeTeam.Country) {
                homeScore++;
            }
            else {
                awayScore++;
            }
            reportManager.MatchEventReport(ReportManager.MatchEvent.goal, minute, teamAttack.Country);
        }


        public void SaveMatchResults() {
            MatchModel matchResults = new MatchModel();
            matchResults.NameHomeTeam = homeTeam.Country;
            matchResults.Goals = homeScore;
            matchResults.NameAwayTeam = awayTeam.Country;
            matchResults.GoalsAgainst = awayScore;

            applicationdb.MatchModels.Add(matchResults);
            applicationdb.SaveChanges();

            reportManager.SaveMatchReport(reportManager.reportMatch, homeTeam, awayTeam);
            reportManager.reportMatch = new List<string>();
        }


        public void SavePoule() {
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

            if (homeScore == awayScore){
                Home.Points++;
                Away.Points++;
            }

            if (homeScore < awayScore) Away.Points = Away.Points + 3;

            applicationdb.SaveChanges();
        }
    }
}