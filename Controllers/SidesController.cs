using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComplantSystem.Controllers
{
    public class SidesController : Controller
    {
       
        private readonly ICompRepository<Sides> SidesRepos;
        private readonly ICompRepository<Tree> treeRepos;

        public SidesController(ICompRepository<Sides> SidesRepos,ICompRepository<Tree> treeRepos)
        {
            this.SidesRepos = SidesRepos;
            this.treeRepos = treeRepos;
        }
        // GET: SidesController
        public ActionResult Index()
        {
            var side = SidesRepos.List();
            return View(side);
        }

        // GET: SidesController/Details/5
        public ActionResult Details(int id)
        { var s=SidesRepos.Find(id,0,"");
            return View(s);
        }

        // GET: SidesController/Create
        public ActionResult Create()
        {  
            return View();
        }

        // POST: SidesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sides side)
        {
            try
            { SidesRepos.Add(side);
                
                Tree s = new Tree
                {
                    Name=side.Name,
                    FKName=side.Id
                };
                treeRepos.Add(s);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SidesController/Edit/5
        public ActionResult Edit(int id)
        {
            var s = SidesRepos.Find(id,0,"");
            return View(s);
        }

        // POST: SidesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Sides s)
        {
            try
            {
                SidesRepos.Update(id, s);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SidesController/Delete/5
        public ActionResult Delete(int id)
        { var side = SidesRepos.Find(id,0,"");
            return View(side);
        }

        // POST: SidesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Sides s)
        {
            try
            {
                SidesRepos.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Search(string term)
        {
            var result = SidesRepos.Search(-1,term);
            return View("Index", result);
        }
    }
}
