using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ComplantSystem.Controllers
{
    public class PunSuitController : Controller
    {
        private readonly ICompRepository<InvestigationRecords> investReps;
        private readonly ICompRepository<Employee> employeeRepos;
        private readonly ICompRepository<PunitiveSuitRecords> punSuitRepos;
        private readonly ICompRepository<InvestigationProcedures> InvestigationPro;
        CompDBContext dbContext;
        public PunSuitController(ICompRepository<PunitiveSuitRecords>punSuitRepos,
            ICompRepository<InvestigationProcedures> InvestigationPro,
            ICompRepository<InvestigationRecords> investReps,
             ICompRepository<Employee> employeeRepos,
            CompDBContext _dbContext)
        {
            this.punSuitRepos = punSuitRepos;
            this.InvestigationPro = InvestigationPro;
            this.investReps = investReps;
            this.employeeRepos = employeeRepos;
            dbContext = _dbContext;
           
        }
        // GET: InvestigationProceduresController
        public ActionResult Index()
        {
          
            var Rusults = punSuitRepos.List();
            return View(Rusults);
        }

        // GET: InvestigationProceduresController/Details/5
        public ActionResult Details(int id)
        {
            var Record = punSuitRepos.Find(id,0, "");
            

            return View(Record);
        }

        // GET: InvestigationProceduresController/Create
        public ActionResult Create()
        {
            PunSuitVModel viewModel = new PunSuitVModel
            {
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);

        }

        // POST: InvestigationProceduresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PunSuitVModel ViewModel)
        {
            try
            {
                var invest = investReps.Find(ViewModel.EmployeesId, 0, "");
                PunitiveSuitRecords NewSuit = new PunitiveSuitRecords
                {
                    SuitContent=ViewModel.SuitContent,
                    OrienDate=ViewModel.OrienDate,
                    Orientation=ViewModel.Orientation,  
                    Notes=ViewModel.Notes,
                    Date=ViewModel.Date,
                    Trees=invest.Trees,
                    InvestigationFk=invest

                };
                
                punSuitRepos.Add(NewSuit);  
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
            var Suit = punSuitRepos.Find(id,0, "");

            PunSuitVModel viewModel = new PunSuitVModel
            {
                PunitiveSuitRecordsId= Suit.PunitiveSuitRecordsId,
                SuitContent = Suit.SuitContent,
                OrienDate = Suit.OrienDate,
                Orientation = Suit.Orientation,
                Notes = Suit.Notes,
                Date = Suit.Date,
                Trees = Suit.Trees,
                EmployeesId= Suit.InvestigationFk.InvestigationRecordsId,
                Employees = FillsrepostoryEmployee()
               
            };
            return View(viewModel);

             
        }

        // POST: InvestigationProceduresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PunSuitVModel ViewModel)
        {
            try
            {
                var invest = investReps.Find(ViewModel.EmployeesId, 0, "");
                PunitiveSuitRecords EditSuit = new PunitiveSuitRecords
                {
                    PunitiveSuitRecordsId=ViewModel.PunitiveSuitRecordsId,  
                    SuitContent = ViewModel.SuitContent,
                    OrienDate = ViewModel.OrienDate,
                    Orientation = ViewModel.Orientation,
                    Notes = ViewModel.Notes,
                    Date = ViewModel.Date,
                    Trees = invest.Trees,
                    InvestigationFk = invest

                };

                punSuitRepos.Update(id, EditSuit);
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
            var Record = punSuitRepos.Find(id, 0, "");

            return View(Record);
        }

        // POST: InvestigationProceduresController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
               punSuitRepos.Delete(id);

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
            var invest = investReps.List().ToList();
             
            int i = 1;
           
            foreach (var item in invest)
            {

                emp.Insert(i, new Employee
                {
                    EmployeeId = item.InvestigationRecordsId,
                    Name = "[م/" + item.InvestigationRecordsId + "]" + item.complaintrecord.DefendantName
                });
                i++;
            }

            return emp;

        }
    }
}
