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
    public class PouleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Poule
        public ActionResult Index()
        {
            return View(db.PouleModels.ToList());
        }

        // GET: Poule/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PouleModel pouleModel = db.PouleModels.Find(id);
            if (pouleModel == null)
            {
                return HttpNotFound();
            }
            return View(pouleModel);
        }

        // GET: Poule/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Poule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Country,GamesPlayed,Goals,Points")] PouleModel pouleModel)
        {
            if (ModelState.IsValid)
            {
                db.PouleModels.Add(pouleModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pouleModel);
        }

        // GET: Poule/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PouleModel pouleModel = db.PouleModels.Find(id);
            if (pouleModel == null)
            {
                return HttpNotFound();
            }
            return View(pouleModel);
        }

        // POST: Poule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Country,GamesPlayed,Goals,Points")] PouleModel pouleModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pouleModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pouleModel);
        }

        // GET: Poule/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PouleModel pouleModel = db.PouleModels.Find(id);
            if (pouleModel == null)
            {
                return HttpNotFound();
            }
            return View(pouleModel);
        }

        // POST: Poule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PouleModel pouleModel = db.PouleModels.Find(id);
            db.PouleModels.Remove(pouleModel);
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
