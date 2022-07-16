using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ComplantSystem.Models.Repository;
using ComplantSystem.Models.SysSetting;
namespace ComplantSystem.Controllers
{
    public class ViolationsController : Controller
    {
        private readonly ICompRepository<Violations> violRepos;
        private readonly ICompRepository<Punishments> punisRepos;

        public ViolationsController(ICompRepository<Violations> violRepos, ICompRepository<Punishments> punisRepos)
        {
            this.violRepos = violRepos;
            this.punisRepos = punisRepos;
        }
        // GET: ViolationsPusController
        public ActionResult Index()
        {
            var GetAll=violRepos.List();
            return View(GetAll);
        }
        // GET: ViolationsPusController
       

        // GET: ViolationsPusController/Details/5
        public ActionResult Details(int id)
        {
            var Record = violRepos.Find(id, 0, "");
            return View(Record);
        }

        // GET: ViolationsPusController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ViolationsPusController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Violations model)
        {
            try
            {
                violRepos.Add(model);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ViolationsPusController/Edit/5
        public ActionResult Edit(int id)
        {

            var Record = violRepos.Find(id, 0, "");
            return View(Record);
        }

        // POST: ViolationsPusController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Violations model)
        {
            try
            {
                violRepos.Update(id, model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ViolationsPusController/Delete/5
        public ActionResult Delete(int id)
        {
            var Record = violRepos.Find(id, 0, "");
            return View(Record);
        }

        // POST: ViolationsPusController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Violations model)
        {
            try
            {
                violRepos.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
