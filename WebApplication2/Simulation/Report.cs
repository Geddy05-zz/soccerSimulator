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

        public void saveMatchReport(List<string> reportMatch, TeamModel homeTeam, TeamModel awayTeam)
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
        }
    }
}