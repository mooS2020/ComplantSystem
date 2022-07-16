using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ComplantSystem.Models;
using ComplantSystem.viewmodel;
using ComplantSystem.Models.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ComplantSystem.Controllers
{
    public class InvesPreparController : Controller
    {
        private readonly IHostingEnvironment hosting;
        private readonly ICompRepository<Attachments> attachRpos;

        public ICompRepository<InvestigationPreparator> InvesPreparRepos { get; }
        public ICompRepository<ComplaintRecord> CompRepos { get; }
        public ICompRepository<InvestigationRecords> InvestRepos { get; }
        public CompDBContext DbContext { get; }

        public InvesPreparController(
            ICompRepository<InvestigationPreparator> InvesPreparRepos,
            ICompRepository<ComplaintRecord> CompRepos,
              ICompRepository<InvestigationRecords> InvestRepos,
            CompDBContext _dbContext,
             IHostingEnvironment hosting,
               ICompRepository<Attachments> attachRpos
            )
        {
            this.InvesPreparRepos = InvesPreparRepos;
            this.CompRepos = CompRepos;
            this.InvestRepos = InvestRepos;
            this.hosting = hosting;
            this.attachRpos = attachRpos;
            DbContext = _dbContext;
        }
        // GET: InvesPreparController
        public ActionResult Index()
        {
            var All=InvesPreparRepos.List();
            return View(All);
        }

        // GET: InvesPreparController/Details/5
        public ActionResult Details(int id)
        {
            var record = InvesPreparRepos.Find(id, 0, "");
            return View(record);
        }

        // GET: InvesPreparController/Create
        public ActionResult Create()
        {
            InvesPreparVModel viewModel = new InvesPreparVModel
            {
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);
        }

        // POST: InvesPreparController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvesPreparVModel model)
        {
            try
            {
                
                if (model.EmployeesId != -1)
                {
                    model.InvestigationFk = InvestRepos.Find(model.EmployeesId, 0, "");
                  
                   // model.complaintrecord = model.InvestigationFk.complaintrecord;
                }
                else
                {
                    InvesPreparVModel viewModel = new InvesPreparVModel
                    {
                        Employees = FillsrepostoryEmployee()
                    };
                    return View(viewModel);
                }
                string FileName = UploadFile(model.File);
                   

                InvestigationPreparator NewIves = new InvestigationPreparator
                {
                    Text = model.Text,
                    InvestigationFk =model.InvestigationFk,
                    FileName=FileName,
                    Date= model.Date,
                   Trees=model.InvestigationFk.Trees


                };
                InvesPreparRepos.Add(NewIves);

                Attachments attack = new Attachments
                {  
                    Description = model.Text,
                    Type=5,
                    Date = model.Date,
                    ComplaintId=model.complaintrecord,
                    Trees= model.InvestigationFk.Trees,
                    FileName = FileName



                };

                attachRpos.Add(attack);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvesPreparController/Edit/5
        public ActionResult Edit(int id)
        {
            var PrRecord = InvesPreparRepos.Find(id, 0, "");

            InvesPreparVModel viewModel = new InvesPreparVModel
            {   InvestigationPreparatorId=PrRecord.InvestigationPreparatorId,
                InvestigationFk=PrRecord.InvestigationFk,
                EmployeesId= PrRecord.InvestigationFk.InvestigationRecordsId,
                Text=PrRecord.Text, 
                Date=PrRecord.Date,
                FileName=PrRecord.FileName, 
                Trees=PrRecord.InvestigationFk.Trees,   
                Employees = FillsrepostoryEmployee()
            };
            return View(viewModel);

           
        }

        // POST: InvesPreparController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InvesPreparVModel model)
        {
            try
            {
                if (model.EmployeesId != -1)
                {
                    model.InvestigationFk = InvestRepos.Find(model.EmployeesId, 0, "");

                  
                }
                else
                {
                    InvesPreparVModel viewModel = new InvesPreparVModel
                    {
                        Employees = FillsrepostoryEmployee()
                    };
                    return View(viewModel);

                }

               string FileName = UploadFile(model.File, model.FileName);


                InvestigationPreparator EditIves = new InvestigationPreparator
                { 
                    InvestigationPreparatorId = model.InvestigationPreparatorId,
                    Text = model.Text,
                    InvestigationFk = model.InvestigationFk,
                    FileName = FileName,
                    Date = model.Date,
                    Trees = model.InvestigationFk.Trees


                };
                InvesPreparRepos.Update(EditIves.InvestigationPreparatorId,EditIves);

               
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvesPreparController/Delete/5
        public ActionResult Delete(int id)
        {
            var record = InvesPreparRepos.Find(id, 0, "");
            return View(record);
           
        }

        // POST: InvesPreparController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                InvesPreparRepos.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                var LastIdValue = 1;
                var LastPIdValue = 1;
                string[] Type = file.FileName.Split('.');


                // return LastIdValue;
                if (attachRpos.List().Count() != 0)

                {
                     
                    LastIdValue = attachRpos.List().Count() + 1;
                }
                if (InvesPreparRepos.List().Count() != 0)

                {
                   
                    LastPIdValue = InvesPreparRepos.List().Count() + 1;
                }

                //to Replace name of file

                var fileName = file.FileName.Replace(file.FileName, LastPIdValue+"_"+ LastIdValue + "." + Type.LastOrDefault());

                string uploads = Path.Combine(hosting.WebRootPath, "upload");

                string fullPath = Path.Combine(uploads, fileName);
                file.CopyTo(new FileStream(fullPath, FileMode.Create));

                return fileName;
            }
            return null;
        }
        //للتعديل
        string UploadFile(IFormFile file, string OldFileName)
        {
            if (file != null)
            {



                var fileName = OldFileName;

                string uploads = Path.Combine(hosting.WebRootPath, "upload");


                string oldFullPath = Path.Combine(uploads, OldFileName);
                string newFullPath = Path.Combine(uploads, fileName);


                if (oldFullPath != newFullPath)
                {
                    System.IO.File.Delete(oldFullPath);
                    file.CopyTo(new FileStream(newFullPath, FileMode.Create));
                }

                return OldFileName;
            }
            return OldFileName;
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
    }
}
