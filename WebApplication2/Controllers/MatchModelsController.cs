using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class MatchModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MatchModels
        public ActionResult Index()
        {
            return View(db.MatchModels.ToList());
        }

        // GET: MatchModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatchModel matchModel = db.MatchModels.Find(id);
            if (matchModel == null)
            {
                return HttpNotFound();
            }
            return View(matchModel);
        }

        // GET: MatchModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MatchModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NameHomeTeam,Goals,NameAwayTeam,GoalsAgainst")] MatchModel matchModel)
        {
            if (ModelState.IsValid)
            {
                db.MatchModels.Add(matchModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(matchModel);
        }

        // GET: MatchModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatchModel matchModel = db.MatchModels.Find(id);
            if (matchModel == null)
            {
                return HttpNotFound();
            }
            return View(matchModel);
        }

        // POST: MatchModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameHomeTeam,Goals,NameAwayTeam,GoalsAgainst")] MatchModel matchModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matchModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(matchModel);
        }

        // GET: MatchModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MatchModel matchModel = db.MatchModels.Find(id);
            if (matchModel == null)
            {
                return HttpNotFound();
            }
            return View(matchModel);
        }

        // POST: MatchModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MatchModel matchModel = db.MatchModels.Find(id);
            db.MatchModels.Remove(matchModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
