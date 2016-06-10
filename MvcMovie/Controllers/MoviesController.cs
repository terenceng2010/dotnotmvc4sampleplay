using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcMovie.Models;
using AutoMapper;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private MovieDBContext db = new MovieDBContext();

        // GET: Movies
        public ActionResult Index()
        {
            return View(db.Movies.ToList());
        }

        // GET: Movies/SearchIndex?movieGenre=XXXX&searchString=XXXX
        public ActionResult SearchIndex(string movieGenre, string searchString)
        {


            var GenreLst = new List<string>();
            //get all genres
            var GenreQry = from d in db.Movies
                           orderby d.Genre
                           select d.Genre;
            //distinct them, add them to viewbag for dropdown list consumption
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.movieGenre = new SelectList(GenreLst);

            var movies = from m in db.Movies
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (string.IsNullOrEmpty(movieGenre))
                return View(movies);
            else
            {
                return View(movies.Where(x => x.Genre == movieGenre));
            }

        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            //ref: http://stackoverflow.com/questions/781987/how-can-i-get-this-asp-net-mvc-selectlist-to-work
            //query all actors, for each of them, assign them to select list item (~ map in js)
            List<SelectListItem> actorList = new List<SelectListItem>();
            var queryActionList =

                 from d in db.Actors
                 select new
                 {
                     ID = d.ID,
                     ActorName = d.ActorName,

                 };

            CreateOrEditMovieVM model = new CreateOrEditMovieVM();
            model.ReleaseDate = DateTime.Today;
            model.ActorList = new MultiSelectList(queryActionList.ToList(), "ID", "ActorName");
            return View(model);
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOrEditMovieVM createMoiveVM)
        {

            Movie newMovie = Mapper.Map<Movie>(createMoiveVM);
            newMovie.Actors = new List<Actor>();

            //http://stackoverflow.com/questions/7478570/entity-framework-code-first-adding-to-many-to-many-relationship-by-id
            foreach (int receivedActorId in createMoiveVM.ReceviedActors)
            {
                var receivedActor = db.Actors.Find(receivedActorId);
                newMovie.Actors.Add(receivedActor);
            }
            if (ModelState.IsValid)
            {
                db.Movies.Add(newMovie);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }

                return RedirectToAction("Index");
            }

            return View(newMovie);
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }


            CreateOrEditMovieVM model = Mapper.Map<CreateOrEditMovieVM>(movie);
            //model.ActorList = new MultiSelectList(movie.Actors.ToList(), "ID", "ActorName");
            model.ReceviedActors = movie.Actors.Select(a => a.ID).ToArray();
            List<SelectListItem> actorList = new List<SelectListItem>();
            var queryActionList =

                 from d in db.Actors
                 select new
                 {
                     ID = d.ID,
                     ActorName = d.ActorName,

                 };

            model.ActorList = new MultiSelectList(queryActionList.ToList(), "ID", "ActorName");


            return View(model);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateOrEditMovieVM createMoiveVM)
        {
            //http://stackoverflow.com/questions/9520111/entity-framework-4-not-saving-my-many-to-many-rows?lq=1
            Movie movieToBeUpdated = Mapper.Map<Movie>(createMoiveVM);
            if (ModelState.IsValid)
            {
                // Must set to modified or adding child records does not set to modified
                db.Entry(movieToBeUpdated).State = EntityState.Modified;

                // Force loading of Actors collection due to lazy loading
                db.Entry(movieToBeUpdated).Collection(st => st.Actors).Load();

                // Clear existing Actors
                movieToBeUpdated.Actors.Clear();

                //http://stackoverflow.com/questions/7478570/entity-framework-code-first-adding-to-many-to-many-relationship-by-id
                foreach (int receivedActorId in createMoiveVM.ReceviedActors)
                {
                    var receivedActor = db.Actors.Find(receivedActorId);
                    movieToBeUpdated.Actors.Add(receivedActor);
                }



                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movieToBeUpdated);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
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
