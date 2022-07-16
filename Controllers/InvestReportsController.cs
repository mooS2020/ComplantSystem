using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComplantSystem.Controllers
{
    public class InvestReportsController : Controller
    {
        
        private readonly ICompRepository<Attachments> attachRpos;
        private readonly ICompRepository<PunVioInvestigation> punVioRepos;

        public ICompRepository<InvestigationReports> InvesReportsRepos { get; }
        public ICompRepository<InvestigationPreparator> InvesPreparRepos { get; }
        public ICompRepository<ComplaintRecord> CompRepos { get; }
        public ICompRepository<InvestigationRecords> InvestRepos { get; }
        public ICompRepository<InvestigationProcedures> InvestProcepos { get; }
        public CompDBContext DbContext { get; }

        public InvestReportsController(
            ICompRepository<InvestigationReports> InvesReportsRepos,
            ICompRepository<InvestigationPreparator> InvesPreparRepos,
            ICompRepository<ComplaintRecord> CompRepos,
              ICompRepository<InvestigationRecords> InvestRepos,
               ICompRepository<InvestigationProcedures> InvestProcepos,
                 ICompRepository<PunVioInvestigation> punVioRepos,
                      CompDBContext _dbContext
           
               
            )
        {
            this.InvesReportsRepos = InvesReportsRepos;
            this.InvesPreparRepos = InvesPreparRepos;
            this.CompRepos = CompRepos;
            this.InvestRepos = InvestRepos;
            this.InvestProcepos = InvestProcepos;
            this.punVioRepos = punVioRepos;
            DbContext = _dbContext;
        }
        // GET: InvestReportsController
        public ActionResult Index()
        {
            var All = InvesReportsRepos.List();
            return View(All);
            
        }

        // GET: InvestReportsController/Details/5
        public ActionResult Details(int id)
        {
            var record = InvesReportsRepos.Find(id, 0, "");
            return View(record);
        }

        // GET: InvestReportsController/Create
        public ActionResult Create()
        {
            InvestReportsVModel viewModel = new InvestReportsVModel
            {
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);

        }

        // POST: InvestReportsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvestReportsVModel model)
        {
            try
            {
                var invest = InvestRepos.Find(model.EmployeesId, 0, "");
                InvestigationReports newRepo=new InvestigationReports()
                {
                    Date=model.Date,
                      Notes=model.Notes,
                      InvestigationFk=invest,
                      ReferenceNumber=invest.ReferenceNumber,
                      Trees=invest.Trees,
                      Pinios=model.Pinios


                };
                
                InvesReportsRepos.Add(newRepo);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestReportsController/Edit/5
        public ActionResult Edit(int id)
        {
            var record = InvesReportsRepos.Find(id, 0, "");
            InvestReportsVModel viewModel = new InvestReportsVModel
            {
                InvestigationReportsId= record.InvestigationReportsId,
                Date = record.Date,
                Notes = record.Notes,
                InvestigationFk = record.InvestigationFk,
                ReferenceNumber = record.ReferenceNumber,
                Trees = record.Trees,
                Pinios = record.Pinios,
                EmployeesId= record.InvestigationFk.InvestigationRecordsId,
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);
        }

        // POST: InvestReportsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InvestReportsVModel model)
        {
            try
            {
                var invest = InvestRepos.Find(model.EmployeesId, 0, "");
                InvestigationReports EditRepo = new InvestigationReports()
                { 
                    InvestigationReportsId=model.InvestigationReportsId,
                    Date = model.Date,
                    Notes = model.Notes,
                    InvestigationFk = invest,
                    ReferenceNumber = invest.ReferenceNumber,
                    Trees = invest.Trees,
                    Pinios = model.Pinios


                };

                InvesReportsRepos.Update(id,EditRepo);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestReportsController/Delete/5
        public ActionResult Delete(int id)
        {
            var record = InvesReportsRepos.Find(id, 0, "");
            return View(record);
        }

        // POST: InvestReportsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                InvesReportsRepos.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        List<Employee> FillsrepostoryEmployee()
        {

            var emp = DbContext.employees.Where(i => i.EmployeeId == 0).ToList();
            emp.DefaultIfEmpty();

            emp.Insert(0, new Employee { EmployeeId = -1, Name = "جلب" });
            var Invest = InvestRepos.List().ToList();

            String[] GetEmpName = { "", "", "", "", "", "", "" };
            int i = 1;
            int y = 1;
            foreach (var item in Invest)
            {
                GetEmpName[i] = item.complaintrecord.DefendantName;

                foreach (var items in GetEmpName)
                {
                    if (y != 1)
                    {

                        if (GetEmpName[y] == GetEmpName[i])
                        {
                            GetEmpName[i] = item.complaintrecord.DefendantName + "/" + item.InvestigationRecordsId;
                            ++y;
                        }

                    }

                    else
                    {
                        ++y;
                    }

                }
                emp.Insert(i, new Employee { EmployeeId = item.InvestigationRecordsId, Name = GetEmpName[i] });
                i++;
            }

            return emp;

        }

        //لاسترجا بيانات الشكوى والتحقيق في التقاري
        public IActionResult GetCompInvetData(int Invest)
        {

            var inves = InvestRepos.Find(Invest, 0, "");

            var Proc = InvestProcepos.Search(Invest, "");

            var VioPun = punVioRepos.Search(Invest, "");

            var prep = InvesPreparRepos.Search(Invest, "");

            InvestReportsVModel viewModel = new InvestReportsVModel
           {
               InvestigationFk = inves,
                Procs = Proc,
                VioPun = VioPun, 
                Preps=prep
           };
            return Json(viewModel);

        }
        //private string ReturnInvestProceures(int Investid)
        //{
        //    var InvProceures = InvestProcepos.Search(Investid, "");
             
        //    int i = 1;
        //    string Proc = "";
        //    foreach (var item in InvProceures)
        //    {
        //        Proc += i + item.ProceuresContent;
                
        //        i++;
        //    }


        //    return Proc;

        //}
    }
}
