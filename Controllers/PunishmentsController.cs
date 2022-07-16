using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.Models.SysSetting;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ComplantSystem.Controllers
{
    public class PunishmentsController : Controller
    {
    private readonly ICompRepository<Violations> violRepos;
    private readonly ICompRepository<Punishments> punisRepos;

    public PunishmentsController(ICompRepository<Violations> violRepos, ICompRepository<Punishments> punisRepos)
    {
        this.violRepos = violRepos;
        this.punisRepos = punisRepos;
    }
    // GET: PunishmentsController
    public ActionResult Index()
        {

        var GetAll = punisRepos.List();
        return View(GetAll);
    }

        // GET: PunishmentsController/Details/5
        public ActionResult Details(int id)
        {
            var Record = punisRepos.Find(id, 0, "");
            return View(Record);
        }

        // GET: PunishmentsController/Create
        public ActionResult Create()
        {  
            var Getviol= violRepos.List().ToList();
            Getviol.Insert(0, new Violations { ViolationsId = -1, Violation = "أختر مخالفة" });
            PunishmentsVModel model = new PunishmentsVModel
            {
                ViolationsFk = Getviol,
               
            };
            return View(model);
        }

        // POST: PunishmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PunishmentsVModel model)
        {
            try
            {
                var Violations = violRepos.Find(model.ViolationsId, 0, "");
                Punishments NewPun = new Punishments
                {
                    
                    Punishment = model.Punishment,
                   
                    ViolationsFk = Violations,


                };

                punisRepos.Add(NewPun);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PunishmentsController/Edit/5
        public ActionResult Edit(int id)
        {
            var Getviol = violRepos.List().ToList();
            var Pun = punisRepos.Find(id, 0, "");
            var ViolId = Pun.ViolationsFk.ViolationsId;

            PunishmentsVModel model = new PunishmentsVModel
            {   PunishmentsId= Pun.PunishmentsId,
                Punishment= Pun.Punishment,

                ViolationsFk = Getviol,
                ViolationsId=ViolId
            };
            return View(model);
            
        }

        // POST: PunishmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PunishmentsVModel model)
        {
            try
            {
               var Violations = violRepos.Find(model.ViolationsId, 0, "");
                Punishments EditPun = new Punishments
                {
                    PunishmentsId = model.PunishmentsId,
                    Punishment = model.Punishment,
                   UtoNumber = model.UtoNumber,
                  ViolationsFk = Violations,


                };

                punisRepos.Update(id,EditPun);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PunishmentsController/Delete/5
        public ActionResult Delete(int id)
        {
            var Record = punisRepos.Find(id, 0, "");
            return View(Record);
           
        }

        // POST: PunishmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                punisRepos.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
