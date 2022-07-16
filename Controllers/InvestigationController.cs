using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
 using System;
 

namespace ComplantSystem.Controllers
{
    public class InvestigationController : Controller
    {
        private readonly ICompRepository<InvestigationRecords> investigation;
        private readonly ICompRepository<Tree> treeRepos;
        private readonly ICompRepository<Employee> employeeRepos;
        private readonly ICompRepository<ComplaintRecord> compRepository;
        CompDBContext dbContext;
        public InvestigationController(ICompRepository<InvestigationRecords> investigation,
            ICompRepository<ComplaintRecord> compRepository, ICompRepository<Employee> employeeRepos,
            ICompRepository<Tree>treeRepos ,CompDBContext _dbContext)
        {
            this.investigation = investigation;
            this.compRepository = compRepository;

            this.treeRepos = treeRepos;
            
            dbContext = _dbContext;
            this.employeeRepos = employeeRepos;
        }
        // GET: InvestigationController
        public ActionResult Index()
        {
            var record = investigation.List();
            return View(record);
        }

        // GET: InvestigationController/Details/5
        public ActionResult Details(int id)

        {
            var model = investigation.Find(id,0, "");
            
            return View(model);
        }
       
        // GET: InvestigationController/Create
        public ActionResult Create()
        {
            InvestigationViewModel viewModel = new InvestigationViewModel
            {
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);
           
        }

      

        // POST: InvestigationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvestigationViewModel invest)
        {
            try
            {

               
                if (invest.EmployeesId != -1)
                {
                    invest.complaintrecord = compRepository.Find(invest.EmployeesId, 0, "");
                }
                else
                {
                    InvestigationViewModel viewModel = new InvestigationViewModel
                    {
                        Employees = FillsrepostoryEmployee()
                    };
                    return View(viewModel);
                }

                InvestigationRecords NewIves = new InvestigationRecords
                {
                   
                    
                    InvestigationContent = invest.InvestigationContent,
                   InvestigationState = invest.InvestigationState,
                    InvestigationtDate = invest.InvestigationtDate,
                    Notes = invest.Notes,
                    Orientation= invest.Orientation,
                    OrientationSour= invest.Orientation,
                    ReferenceNumber = invest.complaintrecord.ReferenceNumber,
                    complaintrecord = invest.complaintrecord,
                    Trees = invest.complaintrecord.Trees,

                   
                };

                investigation.Add(NewIves);
                

            return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestigationController/Edit/5
        public ActionResult Edit(int id)
        {
            var Inves = investigation.Find(id,0,"");

            InvestigationViewModel model = new InvestigationViewModel
            {

                InvestigationRecordsId = Inves.InvestigationRecordsId,
                AutoNumber = Inves.AutoNumber,
                InvestigationActive = Inves.InvestigationActive,
                InvestigationContent = Inves.InvestigationContent,
                
                InvestigationState = Inves.InvestigationState,
                InvestigationtDate = Inves.InvestigationtDate,
                Notes = Inves.Notes,
                Orientation = Inves.Orientation,
                OrientationSour = Inves.Orientation,
                ReferenceNumber = Inves.ReferenceNumber,
                
                EmployeesId=Inves.complaintrecord.ComplaintRecordId,
                Employees = FillsrepostoryEmployee()



            };

            return View(model);
        }

        // POST: InvestigationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InvestigationViewModel invest)
        {
            try
            {

                if (invest.EmployeesId != -1)
                {
                    invest.complaintrecord = compRepository.Find(invest.EmployeesId, 0, "");
                }
                else
                {
                    InvestigationViewModel viewModel = new InvestigationViewModel
                    {
                        Employees = FillsrepostoryEmployee()
                    };
                    return View(viewModel);
                }

                InvestigationRecords model = new InvestigationRecords
                {

                    InvestigationRecordsId=invest.InvestigationRecordsId,
                    InvestigationContent = invest.InvestigationContent,
                    InvestigationState = invest.InvestigationState,
                    InvestigationtDate = invest.InvestigationtDate,
                    Notes = invest.Notes,
                    Orientation = invest.Orientation,
                    OrientationSour = invest.Orientation,
                    ReferenceNumber = invest.complaintrecord.ReferenceNumber,
                    complaintrecord = invest.complaintrecord,
                    Trees = invest.complaintrecord.Trees,


                };

                investigation.Update(invest.InvestigationRecordsId,model);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvestigationController/Delete/5
        public ActionResult Delete(int id)
        {
            var Inves = investigation.Find(id,0, "");
            return View(Inves);
        }

        // POST: InvestigationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                investigation.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        List<Employee> FillsrepostoryEmployee()
        {
            List<Employee> emp=new List<Employee>();    
            
            emp.Insert(0, new Employee { EmployeeId = -1, Name = "جلب" });
            var comp = compRepository.List().ToList();

            //String[] GetEmpName = new string[100];
               
            int i = 1;      
            //int y = 1;
            foreach (var item in comp)
            {
                //GetEmpName[i] = item.DefendantName;

                //foreach (var items in GetEmpName )
                //{
                //    if ( y== GetEmpName.Count() || i == 1)
                //    {
                //        break;
                //    }

                    
                //    if (GetEmpName[i] == GetEmpName[y])
                //        {
                           
                          
                //                GetEmpName[i] = item.DefendantName + "/" + item.ComplaintRecordId;
                         
                //        }
                //    ++y;


                //}
                emp.Insert(i, new Employee { EmployeeId = item.ComplaintRecordId,
                    Name ="[م/" +item.ComplaintRecordId+"]"+item.DefendantName });
                i++;
            }
           
            return emp;

        }

        //لاسترجا بيانات الشكوى في التحقيق
        public IActionResult GetCompData(int CompId)
        {

            var comps = compRepository.Find(CompId, 0, "");
           
         return  Json(comps);    

        }

        //لاسترجا بيانات الشكوى والتحقيق في المحاظر
        public IActionResult GetCompInvetData(int Invest)
        {

            var inves = investigation.Find(Invest, 0, "");

            return Json(inves);

        }

         



    }
}
