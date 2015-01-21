using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Simulation
{
    public class ReportManager
    {
        private ApplicationDbContext applicationdb = new ApplicationDbContext();
        public List<string> reportMatch = new List<string>();
        public enum MatchEvent { yellowCard, redCard, corner, freeKick, goal };

        public void saveMatchReport(List<string> reportMatch,TeamModel homeTeam,TeamModel awayTeam)
        {
            reportMatch.Add("93 Scheidsrechter fluit af");

            MatchModel matchResults = applicationdb.MatchModels.FirstOrDefault(m => m.NameHomeTeam == homeTeam.Country && m.NameAwayTeam == awayTeam.Country);
            int matchId = matchResults.ID;

            foreach (string report in reportMatch)
                {
                ReportModel matchReport = new ReportModel();
                matchReport.MatchId = matchId;
                matchReport.report = report;

                applicationdb.ReportModels.Add(matchReport);
                applicationdb.SaveChanges();
                }
            reportMatch = new List<string>();

        }


        public void MatchEventReport(MatchEvent matchEvent, int minute, string teamAttack)
        {
            string matchEventText = " ";

            switch (matchEvent)
            {
                case MatchEvent.yellowCard:
                    matchEventText = minute + " " + teamAttack + " krijgt gele kaart";
                    break;
                case MatchEvent.redCard:
                    matchEventText = minute + " " + teamAttack + " krijgt rode kaart";
                    break;
                case MatchEvent.freeKick:
                    matchEventText = minute + " " + teamAttack + " krijgt een vrije trap";
                    break;
                case MatchEvent.corner:
                    matchEventText = minute + " " + teamAttack + " neemt een corner";
                    break;
                case MatchEvent.goal:
                    matchEventText = minute + " " + teamAttack + " Heeft gescoord";
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            reportMatch.Add(matchEventText);
        }

    }
}