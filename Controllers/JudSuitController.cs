using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ComplantSystem.Controllers
{
    public class JudSuitController : Controller
    {
        private readonly ICompRepository<InvestigationRecords> investReps;
        private readonly ICompRepository<Employee> employeeRepos;
        private readonly ICompRepository<JudicialSuitRecords> judSuitRepos;
        private readonly ICompRepository<PunitiveSuitRecords> punSuitRepos;
        private readonly ICompRepository<InvestigationProcedures> InvestigationPro;
        CompDBContext dbContext;
        public JudSuitController(
             ICompRepository<JudicialSuitRecords> judSuitRepos,
            ICompRepository<PunitiveSuitRecords>punSuitRepos,
            ICompRepository<InvestigationProcedures> InvestigationPro,
            ICompRepository<InvestigationRecords> investReps,
             ICompRepository<Employee> employeeRepos,
            CompDBContext _dbContext)
        {
            this.judSuitRepos = judSuitRepos;
            this.punSuitRepos = punSuitRepos;
            this.InvestigationPro = InvestigationPro;
            this.investReps = investReps;
            this.employeeRepos = employeeRepos;
            dbContext = _dbContext;
           
        }
        // GET: InvestigationProceduresController
        public ActionResult Index()
        {
          
            var Rusults = judSuitRepos.List();
            return View(Rusults);
        }

        // GET: InvestigationProceduresController/Details/5
        public ActionResult Details(int id)
        {
            var Record = judSuitRepos.Find(id,0, "");
            

            return View(Record);
        }

        // GET: InvestigationProceduresController/Create
        public ActionResult Create()
        {
            JudSuitVModel viewModel = new JudSuitVModel
            {
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);

        }

        // POST: InvestigationProceduresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JudSuitVModel ViewModel)
        {
            try
            {
                // var invest = investReps.Find(ViewModel.EmployeesId, 0, "");
                var punSuit = punSuitRepos.Find(ViewModel.EmployeesId, 0, "");
                JudicialSuitRecords NewSuit = new JudicialSuitRecords
                {
                    SuitContent=ViewModel.SuitContent,
                    OrienDate=ViewModel.OrienDate,
                    Orientation=ViewModel.Orientation,  
                    Notes=ViewModel.Notes,
                    Date=ViewModel.Date,
                    Trees=punSuit.InvestigationFk.Trees,
                    punitiveSuit =punSuit

                };
                
                judSuitRepos.Add(NewSuit);  
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestigationProceduresController/Edit/5
        public ActionResult Edit(int id)
        {
            var Suit = judSuitRepos.Find(id,0, "");

            JudSuitVModel viewModel = new JudSuitVModel
            {
                JudicialSuitRecordsId= Suit.JudicialSuitRecordsId,
                SuitContent = Suit.SuitContent,
                OrienDate = Suit.OrienDate,
                Orientation = Suit.Orientation,
                Notes = Suit.Notes,
                Date = Suit.Date,
                Trees = Suit.Trees,
                EmployeesId= Suit.punitiveSuit.PunitiveSuitRecordsId,
                Employees = FillsrepostoryEmployee()
               
            };
            return View(viewModel);

             
        }

        // POST: InvestigationProceduresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, JudSuitVModel ViewModel)
        {
            try
            {
               // var invest = investReps.Find(ViewModel.EmployeesId, 0, "");
                var punSuit = punSuitRepos.Find(ViewModel.EmployeesId, 0, "");
                JudicialSuitRecords EditSuit = new JudicialSuitRecords
                {
                    JudicialSuitRecordsId= ViewModel.JudicialSuitRecordsId,   
                    SuitContent = ViewModel.SuitContent,
                    OrienDate = ViewModel.OrienDate,
                    Orientation = ViewModel.Orientation,
                    Notes = ViewModel.Notes,
                    Date = ViewModel.Date,
                    Trees = punSuit.InvestigationFk.Trees,
                    punitiveSuit = punSuit

                };

                judSuitRepos.Update(id, EditSuit);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestigationProceduresController/Delete/5
        public ActionResult Delete(int id)
        {
            var Record = judSuitRepos.Find(id, 0, "");

            return View(Record);
        }

        // POST: InvestigationProceduresController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
               judSuitRepos.Delete(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        List<Employee> FillsrepostoryEmployee()
        {
            List<Employee> emp = new List<Employee>();

            emp.Insert(0, new Employee { EmployeeId = -1, Name = "جلب" });

            var punSuit = punSuitRepos.List().ToList();
             
            int i = 1;
           
            foreach (var item in punSuit)
            {

                emp.Insert(i, new Employee
                {
                    EmployeeId = item.PunitiveSuitRecordsId,
                    Name = "[م/" + item.PunitiveSuitRecordsId + "]" + item.InvestigationFk.complaintrecord.DefendantName
                });
                i++;
            }

            return emp;

        }

        public IActionResult GetCompInvetPunsData(int PunsSuitId)
        {

            var data =punSuitRepos.Find(PunsSuitId, 0, "");

            return Json(data);

        }
    }
}
