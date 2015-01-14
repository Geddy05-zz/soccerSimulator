using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Simulation;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        private Game simulation = new Game();
        private ApplicationDbContext applicationdb = new ApplicationDbContext();
        public ActionResult Index()
        {         
            simulation.CreateGame();

            ViewBag.FirstPlace = db.PouleModels.Where(p => p.Position == 1).Select(p => p.Country).Single();
            ViewBag.SecondPlace = db.PouleModels.Where(p => p.Position == 2).Select(p => p.Country).Single();

            return View(db.PouleModels.OrderBy(p => p.Position));
        }
        
        [HttpPost]
        public ActionResult Index(string start)
        {
            simulation.StartGame();

            ViewBag.FirstPlace = db.PouleModels.Where(p => p.Position == 1).Select(p => p.Country).Single();
            ViewBag.SecondPlace = db.PouleModels.Where(p => p.Position == 2).Select(p => p.Country).Single();

            return View(db.PouleModels.OrderBy(p => p.Position));
        }

        
        public ActionResult Uitslagen()
        {

            return View(db.MatchModels.ToList());
        }

        public ActionResult Contact()
        {

            return View();
        }

        public ActionResult WedstrijdVerslag(int? id)
        {
            return View(db.ReportModels.Where(R => R.MatchId == id).ToList());
        }
    }
}