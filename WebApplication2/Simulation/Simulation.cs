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
        private ApplicationDbContext Applicationdb = new ApplicationDbContext();
        private CreateModels createModels = new CreateModels();
        private int[] matchSchema = { 9, 10, 11, 12, 9, 11, 10, 12, 9, 12, 10, 11 };

        private TeamModel teamAttack;
        private TeamModel teamDefence;
        private TeamModel homeTeam;
        private TeamModel awayTeam;
        private PouleModel teamA;
        private PouleModel teamB;
        private List<string> matchReport = new List<string>();

        private int homeScore = 0;
        private int awayScore = 0;
        private int yellowCardProbability;

        // Random.Next() use systemclock so if we dont use a lock 
        // the random number will almost  always be the same number
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
            for (int i = 0; i < matchSchema.Length; i++)
            {
                yellowCardProbability = RandomNumber(1, 10);
                homeScore = 0;
                awayScore = 0;
                SimulateMatch(matchSchema[i], matchSchema[i + 1]);
                i++;
            }
            decidePosistion();
        }

        public void RemoveLastGame()
        {
            var allMatches = from everything in Applicationdb.MatchModels select everything;
            Applicationdb.MatchModels.RemoveRange(allMatches);
            var allPoule = from everything in Applicationdb.PouleModels select everything;
            Applicationdb.PouleModels.RemoveRange(allPoule);
            Applicationdb.SaveChanges();
        }

        public void CreateGame()
        {
            createModels.CreatePoule();
            createModels.Createteams();
        }

        public void SimulateMatch(int TeamHomeId, int TeamAwayId)
        {
            homeTeam = Applicationdb.TeamModels.Find(TeamHomeId);
            awayTeam = Applicationdb.TeamModels.Find(TeamAwayId);

            for (int minute = 0 ; minute < 90; minute = minute + 9)
            {
                choiceAttackingTeam(homeTeam, awayTeam, minute);
                tryToScore(minute);
            }
            saveMatchResults();
            savePoule();
        }

        public void choiceAttackingTeam(TeamModel Home, TeamModel Away, int minute)
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

        public void tryToScore(int Minute)
        {
            int totalpoints = teamAttack.Attack + teamDefence.Defence;
            int choice = RandomNumber(0, totalpoints);
            int goal = RandomNumber(0, (teamDefence.Keeper + teamAttack.Attack));
            Minute = Minute + RandomNumber(1, 8);

            if (choice % yellowCardProbability == 1) 
            {
                string report = Minute+" " + teamAttack.Country + " krijgt gele kaart";
                matchReport.Add(report);
                choice = 0;
            }
            if (choice > teamDefence.Defence && goal > teamDefence.Keeper)
            {
                goalScored(Minute);
            }
            else if (choice + 11 < (teamDefence.Defence / 2))
            {
                string report = Minute + " " + teamAttack.Country + " neemt een corner";
                matchReport.Add(report);
            }
            else if (choice > (teamDefence.Defence / 2)+11 && choice < teamAttack.Attack)
            {
                string report = Minute + " " + teamDefence.Country + " krijgt een vrije trap";
                matchReport.Add(report);
            }
        }

        public void goalScored(int Minute)
        {
            if (teamAttack.Country == homeTeam.Country) {
                homeScore++;
            }
            else {
                awayScore++;
            }
            string report = Minute + " " + teamAttack.Country + " Heeft gescoord";
            matchReport.Add(report);
        }

        public void saveMatchResults()
        {
            MatchModel matchResults = new MatchModel();
            matchResults.NameHomeTeam = homeTeam.Country;
            matchResults.Goals = homeScore;
            matchResults.NameAwayTeam = awayTeam.Country;
            matchResults.GoalsAgainst = awayScore;

            Applicationdb.MatchModels.Add(matchResults);
            Applicationdb.SaveChanges();

            saveMatchReport();

        }

        public void saveMatchReport()
        {
            matchReport.Add("93 Scheidsrechter fluit af");

            MatchModel matchResults = Applicationdb.MatchModels.FirstOrDefault(m => m.NameHomeTeam == homeTeam.Country && m.NameAwayTeam == awayTeam.Country);
            int matchId = matchResults.ID;

            foreach (string report in matchReport)
            {
                ReportModel matchEvent = new ReportModel();
                matchEvent.MatchId = matchId;
                matchEvent.report = report;

                Applicationdb.ReportModels.Add(matchEvent);
                Applicationdb.SaveChanges();
            }
            matchReport = new List<string>();
        }

        public void savePoule()
        {
            PouleModel Home = Applicationdb.PouleModels.FirstOrDefault(C => C.Country.Contains(homeTeam.Country));
            PouleModel Away = Applicationdb.PouleModels.FirstOrDefault(C => C.Country.Contains(awayTeam.Country));

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

            Applicationdb.SaveChanges();
        }

        public void decidePosistion()
        {

            List<PouleModel> SortedList = Applicationdb.PouleModels.OrderByDescending(p => p.Points)
                                       .ThenByDescending(p => p.GoalsTotaal).ToList();
            removePoule();

            for (int i = 0; i < SortedList.Count-1; i++)
            {
                int j = i+1;
                teamA = SortedList[i];
                teamB = SortedList[j];

                if (teamA.Points == teamB.Points && teamA.GoalsTotaal == teamB.GoalsTotaal)
                {
                    if (teamA.Goals == teamB.Goals)
                    {
                        ComparerMatchresult(i);
                    }
                    if(teamA.Goals > teamB.Goals)
                    {
                        teamA.Position = i + 1;
                        teamB.Position = i + 2;
                    }
                    if (teamA.Goals < teamB.Goals)
                    {
                        teamB.Position = i + 1;
                        if (i < SortedList.Count - 1)
                        {
                            teamA.Position = i + 2;
                            i++;
                        }
                    }
                }
                else
                {
                    teamA.Position = i + 1;
                    teamB.Position = i + 2;
                }
                Applicationdb.PouleModels.Add(teamA);
                Applicationdb.PouleModels.Add(teamB);
            }
            Applicationdb.SaveChanges();
        }

        public void removePoule()
        {
            var allPoule = from everything in Applicationdb.PouleModels select everything;
            Applicationdb.PouleModels.RemoveRange(allPoule);
            Applicationdb.SaveChanges();
        }

        public void ComparerMatchresult(int i)
        {
            try
            {
                MatchModel compareResult = Applicationdb.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(teamA.Country) && C.NameAwayTeam.Contains(teamB.Country));
                if (compareResult.Goals > compareResult.GoalsAgainst)
                {
                    teamA.Position = i + 1;
                    teamB.Position = i + 2;
                }
                if (compareResult.Goals < compareResult.GoalsAgainst)
                {
                    teamB.Position = i + 1;
                    teamA.Position = i + 2;
                }
                else
                {
                    teamA.Position = i + 1;
                    teamB.Position = i + 2;
                }
            }
            catch
            {
                MatchModel compareResult = Applicationdb.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(teamB.Country) && C.NameAwayTeam.Contains(teamA.Country));
                if (compareResult.Goals > compareResult.GoalsAgainst)
                {
                    teamA.Position = i + 1;
                }
                else if (compareResult.Goals < compareResult.GoalsAgainst)
                {
                    teamB.Position = i + 1;
                    teamA.Position = i + 2;
                }
                else
                {
                    teamA.Position = i + 1;
                }
            }
        }
    }
}