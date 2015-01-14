using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Simulation
{
    public class PouleManager
    {
        private ApplicationDbContext applicationdb = new ApplicationDbContext();
        private PouleModel teamA;
        private PouleModel teamB;

        public void DecidePosistion()
        {

            List<PouleModel> SortedList = applicationdb.PouleModels.OrderByDescending(p => p.Points)
                                       .ThenByDescending(p => p.GoalsTotaal).ToList();
            RemovePoule();

            for (int i = 0; i < SortedList.Count - 1; i++)
            {
                int j = i + 1;
                teamA = SortedList[i];
                teamB = SortedList[j];

                if (teamA.Points == teamB.Points && teamA.GoalsTotaal == teamB.GoalsTotaal)
                {
                    if (teamA.Goals == teamB.Goals)
                    {
                        ComparerMatchresult(i);
                    }
                    if (teamA.Goals > teamB.Goals)
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
                applicationdb.PouleModels.Add(teamA);
                applicationdb.PouleModels.Add(teamB);
            }
            applicationdb.SaveChanges();
        }

        public void RemovePoule()
        {
            var allPoule = from everything in applicationdb.PouleModels select everything;
            applicationdb.PouleModels.RemoveRange(allPoule);
            applicationdb.SaveChanges();
        }

        public void ComparerMatchresult(int i)
        {
            try
            {
                MatchModel compareResult = applicationdb.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(teamA.Country) && C.NameAwayTeam.Contains(teamB.Country));
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
                MatchModel compareResult = applicationdb.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(teamB.Country) && C.NameAwayTeam.Contains(teamA.Country));
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