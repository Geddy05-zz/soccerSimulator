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
    public class TeamController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Team
        public ActionResult Index()
        {
            return View(db.TeamModels.ToList());
        }

        // GET: Team/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamModel teamModel = db.TeamModels.Find(id);
            if (teamModel == null)
            {
                return HttpNotFound();
            }
            return View(teamModel);
        }

        // GET: Team/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Country,Attack,Defence,Keeper,tactic")] TeamModel teamModel)
        {
            if (ModelState.IsValid)
            {
                db.TeamModels.Add(teamModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teamModel);
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamModel teamModel = db.TeamModels.Find(id);
            if (teamModel == null)
            {
                return HttpNotFound();
            }
            return View(teamModel);
        }

        // POST: Team/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Country,Attack,Defence,Keeper,tactic")] TeamModel teamModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamModel);
        }

        // GET: Team/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamModel teamModel = db.TeamModels.Find(id);
            if (teamModel == null)
            {
                return HttpNotFound();
            }
            return View(teamModel);
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeamModel teamModel = db.TeamModels.Find(id);
            db.TeamModels.Remove(teamModel);
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
