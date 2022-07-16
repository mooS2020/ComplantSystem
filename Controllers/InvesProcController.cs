using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComplantSystem.Controllers
{
    public class InvesProcController : Controller
    {
        private readonly ICompRepository<InvestigationRecords> investReps;
        private readonly ICompRepository<InvestigationProcedures> InvestigationPro;
        CompDBContext dbContext;
        public InvesProcController(ICompRepository<InvestigationProcedures> InvestigationPro,
            ICompRepository<InvestigationRecords> investReps, CompDBContext _dbContext)
        {  
            this.InvestigationPro = InvestigationPro;
            this.investReps = investReps;
            dbContext = _dbContext;
           
        }
        // GET: InvestigationProceduresController
        public ActionResult Index(int id)
        {
            ViewBag.InvestFk = id.ToString();

            var Rusults = InvestigationPro.Search(id, "");
            return View(Rusults);
        }

        // GET: InvestigationProceduresController/Details/5
        public ActionResult Details(int id)
        {
            var Record = InvestigationPro.Find(id,0, "");
            

            return View(Record);
        }

        // GET: InvestigationProceduresController/Create
        public ActionResult Create(int id)
        {
            ViewBag.InvestId = id;

            return View();
            
        }

        // POST: InvestigationProceduresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvestProcVModel ViewModel)
        {
            try
            {
                var InvestFK = investReps.Find(ViewModel.InvestFK, 0, "");
                InvestigationProcedures NewInvest = new InvestigationProcedures
                {
                   InvestigationProceduresId = ViewModel.InvestigationProceduresId,
                   ProceuresContent = ViewModel.ProceuresContent,
                   ProceuresCount = ViewModel.ProceuresCount,
                   Date = ViewModel.Date,
                   InvestigationId = InvestFK,
                   
                };
                InvestigationPro.Add(NewInvest);
                return RedirectToAction("Index", new { id = InvestFK.InvestigationRecordsId });
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestigationProceduresController/Edit/5
        public ActionResult Edit(int id)
        {
            var Inves = InvestigationPro.Find(id,0, "");



            InvestProcVModel model = new  InvestProcVModel
            {

                InvestigationProceduresId = Inves.InvestigationProceduresId,
                ProceuresContent = Inves.ProceuresContent,
                ProceuresCount = Inves.ProceuresCount,
                Date = Inves.Date,
                InvestFK=Inves.InvestigationId.InvestigationRecordsId,
                InvestigationId = Inves.InvestigationId,

            };

            return View(model);
        }

        // POST: InvestigationProceduresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InvestProcVModel ViewModel)
        {
            try
            {
                var InvestFK = investReps.Find(ViewModel.InvestFK, 0, "");
                InvestigationProcedures EditInvest = new InvestigationProcedures
                {
                    InvestigationProceduresId = ViewModel.InvestigationProceduresId,
                    ProceuresContent = ViewModel.ProceuresContent,
                    ProceuresCount = ViewModel.ProceuresCount,
                    Date = ViewModel.Date,
                    InvestigationId = InvestFK,

                };
                InvestigationPro.Update(id, EditInvest);
                return RedirectToAction("Index", new { id = InvestFK.InvestigationRecordsId });
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestigationProceduresController/Delete/5
        public ActionResult Delete(int id)
        {
            var Inves = InvestigationPro.Find(id,0, "");
            return View(Inves);
        }

        // POST: InvestigationProceduresController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var GetId = InvestigationPro.Find(id, 0, "");
                var sendId = GetId.InvestigationId.InvestigationRecordsId;
                InvestigationPro.Delete(id);

                return RedirectToAction("Index", new { id = sendId });
            }
            catch
            {
                return View();
            }
        }
    }
}
