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
        private PouleModel selectedTeam;
        private PouleModel compareWithTeam;

        public void DecidePosistion()
        {

            List<PouleModel> SortedList = applicationdb.PouleModels.OrderByDescending(p => p.Points)
                                       .ThenByDescending(p => p.GoalsTotaal).ToList();
            RemovePoule();

            for (int i = 0; i < SortedList.Count - 1; i++)
            {
                int j = i + 1;
                selectedTeam = SortedList[i];
                compareWithTeam = SortedList[j];

                if (selectedTeam.Points == compareWithTeam.Points && selectedTeam.GoalsTotaal == compareWithTeam.GoalsTotaal)
                {
                    if (selectedTeam.Goals == compareWithTeam.Goals)
                    {
                        ComparerMatchresult(i);
                        i++;
                    }
                    if (selectedTeam.Goals > compareWithTeam.Goals)
                    {
                        selectedTeam.Position = i + 1;
                        compareWithTeam.Position = i + 2;
                    }
                    if (selectedTeam.Goals < compareWithTeam.Goals)
                    {
                        compareWithTeam.Position = i + 1;
                        selectedTeam.Position = i + 2;
                        if (i < SortedList.Count - 2) i++;
                    }
                }
                
                else
                {
                    selectedTeam.Position = i + 1;
                    compareWithTeam.Position = i + 2;
                }
                applicationdb.PouleModels.Add(selectedTeam);
                applicationdb.PouleModels.Add(compareWithTeam);
            }
            selectedTeam = SortedList[SortedList.Count - 1];
            if (selectedTeam.Position < 1 || selectedTeam.Position > 3)
            {
                selectedTeam.Position = 4;
           }

            applicationdb.SaveChanges();
            applicationdb.PouleModels.Add(selectedTeam);
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
                MatchModel compareResult = applicationdb.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(selectedTeam.Country) && C.NameAwayTeam.Contains(compareWithTeam.Country));
                if (compareResult.Goals > compareResult.GoalsAgainst)
                {
                    selectedTeam.Position = i + 1;
                    compareWithTeam.Position = i + 2;
                }
                if (compareResult.Goals < compareResult.GoalsAgainst)
                {
                    compareWithTeam.Position = i + 1;
                    selectedTeam.Position = i + 2;
                }
                else
                {
                    selectedTeam.Position = i + 1;
                    compareWithTeam.Position = i + 2;
                }
            }
            catch
            {
                MatchModel compareResult = applicationdb.MatchModels.FirstOrDefault(C => C.NameHomeTeam.Contains(compareWithTeam.Country) && C.NameAwayTeam.Contains(selectedTeam.Country));
                if (compareResult.Goals > compareResult.GoalsAgainst)
                {
                    selectedTeam.Position = i + 1;
                    compareWithTeam.Position = i + 2;
                }
                else if (compareResult.Goals < compareResult.GoalsAgainst)
                {
                    compareWithTeam.Position = i + 1;
                    selectedTeam.Position = i + 2;
                }
                else
                {
                    selectedTeam.Position = i + 1;
                    compareWithTeam.Position = i + 2;
                }
            }
        }
    }
}