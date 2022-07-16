using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ComplantSystem.Models.Repository;
using ComplantSystem.Models.SysSetting;
using ComplantSystem.Models;
using ComplantSystem.viewmodel;
using System.Linq;

namespace ComplantSystem.Controllers
{
    public class PunVioInvestController : Controller
    {
        private readonly ICompRepository<PunVioInvestigation> punVioRepos;
        private readonly ICompRepository<InvestigationRecords> investRepos;
        private readonly ICompRepository<Violations> viotRepos;
        private readonly ICompRepository<Punishments> punRepos;

        public PunVioInvestController(
            ICompRepository<PunVioInvestigation> punVioRepos,
             ICompRepository<InvestigationRecords> investRepos,
               ICompRepository<Violations> viotRepos,
                 ICompRepository<Punishments> punRepos

            )
        {
            this.punVioRepos = punVioRepos;
            this.investRepos = investRepos;
            this.viotRepos = viotRepos;
            this.punRepos = punRepos;
        }
        // GET: PunVioInvestController
        public ActionResult Index(int id)
        {
            ViewBag.InvestFk = id.ToString();

            var Rusults = punVioRepos.Search(id, "");
            return View(Rusults);
            
        }

        // GET: PunVioInvestController/Details/5
        public ActionResult Details(int id)
        {
            var record = punVioRepos.Find(id, 0, "");
            return View(record);
        }

        // GET: PunVioInvestController/Create
        public ActionResult Create(int id)
        {
            ViewBag.InvestId = id;
            
            return View(GetAllPunsVios());

        }

        private PunVioInvestVModel GetAllPunsVios()
        {
            var Getviol = viotRepos.List().ToList();
            var Getpun = punRepos.List().ToList();

            Getviol.Insert(0, new Violations { ViolationsId = -1, Violation = "أختر مخالفة" });
            Getpun.Insert(0, new Punishments { PunishmentsId = -1, Punishment = "أختر عقوبة" });

            PunVioInvestVModel model = new PunVioInvestVModel
            {
                violationsFk = Getviol,
                PunishmentsFk = Getpun,

            };
            return model;
        }

        // POST: PunVioInvestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PunVioInvestVModel model)
        {
            try
            {
                if(model.PunId ==-1|| model.VioId==-1)
                {
                    ViewBag.InvestId = model.InvestFK;
                    return View(GetAllPunsVios());
                } 

                var Vio = viotRepos.Find(model.VioId, 0, "");
                var Pun = punRepos.Find(model.PunId, 0, "");
                var Invest = investRepos.Find(model.InvestFK, 0, "");
                var InvestRot = Invest.InvestigationRecordsId;

                PunVioInvestigation NewInvest = new PunVioInvestigation
                {
                    violationsFk= Vio,
                    PunishmentsFk= Pun,
                    InvestigationFk= Invest

                };

                punVioRepos.Add(NewInvest);
                return RedirectToAction("Index", new { id = InvestRot });
                
            }
            catch
            {
                return View();
            }
        }

        // GET: PunVioInvestController/Edit/5
        public ActionResult Edit(int id)
        {
            var record = punVioRepos.Find(id, 0, "");
            var Getviol = viotRepos.List().ToList();
            var Getpun = punRepos.List().ToList();
            var viol = record.violationsFk.ViolationsId;
            var pun = record.PunishmentsFk.PunishmentsId;

            ViewBag.InvestId = record.InvestigationFk.InvestigationRecordsId;
            PunVioInvestVModel model = new PunVioInvestVModel
            {  
                PunVioInvestigationId=record.PunVioInvestigationId,
                PunId=pun,
                VioId=viol, 
                Date=record.Date,
                violationsFk = Getviol,
                PunishmentsFk = Getpun,

            };
            return View(model);
            
        }

        // POST: PunVioInvestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PunVioInvestVModel model)
        {
            try
            {
                if (model.PunId == -1 || model.VioId == -1)
                {
                    ViewBag.InvestId = model.InvestFK;
                    return View(GetAllPunsVios());
                }

                var Vio = viotRepos.Find(model.VioId, 0, "");
                var Pun = punRepos.Find(model.PunId, 0, "");
                var Invest = investRepos.Find(model.InvestFK, 0, "");
                var InvestRot = Invest.InvestigationRecordsId;

                PunVioInvestigation EditInvest = new PunVioInvestigation
                {    PunVioInvestigationId = model.PunVioInvestigationId,   
                    violationsFk = Vio,
                    PunishmentsFk = Pun,
                    InvestigationFk = Invest

                };

                punVioRepos.Update(id, EditInvest);
                return RedirectToAction("Index", new { id = InvestRot });
            }
            catch
            {
                return View();
            }
        }

        // GET: PunVioInvestController/Delete/5
        public ActionResult Delete(int id)
        {
            var record = punVioRepos.Find(id, 0, "");
            return View(record);
        }

        // POST: PunVioInvestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
               
                var GetId = punVioRepos.Find(id, 0, "");
                var sendId = GetId.InvestigationFk.InvestigationRecordsId;
                punVioRepos.Delete(id);
                return RedirectToAction("Index", new { id = sendId });
                
            }
            catch
            {
                return View();
            }
        }
    }
}
