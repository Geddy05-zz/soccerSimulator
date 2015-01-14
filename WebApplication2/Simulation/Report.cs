using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Simulation
{
    public class RandomNum
    {
         public static class Util
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
    }
}