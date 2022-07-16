using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
 
namespace ComplantSystem.Controllers
{
    public class AttachmentsController : Controller
    {
        private readonly ICompRepository<Attachments> attachRpos;
        private readonly ICompRepository<ComplaintRecord> compRepository;
        private readonly ICompRepository<Departments> departmentRepos;
        private readonly ICompRepository<Tree> treeRepos;
        private readonly ICompRepository<Sections> SectionReop;
        private readonly ICompRepository<Sides> sideRepos;
        private readonly CompDBContext dbContext;
        [System.Obsolete]
        private readonly IHostingEnvironment hosting;

        [System.Obsolete]
        public AttachmentsController(
            ICompRepository<Attachments> attachRpos,
            ICompRepository<ComplaintRecord> compRepository,
            ICompRepository<Departments> departmentRepos,
             ICompRepository<Tree> treeRepos,  
            ICompRepository<Sections> SectionReop,
            ICompRepository<Sides> sideRepos,
            CompDBContext dbContext,
            IHostingEnvironment hosting)
        {
            this.attachRpos = attachRpos;
            this.compRepository = compRepository;
            this.departmentRepos = departmentRepos;
            this.treeRepos = treeRepos;
            this.SectionReop = SectionReop;
            this.sideRepos = sideRepos;
            this.dbContext = dbContext;
            this.hosting = hosting;
        }
        // GET: AttachmentsController
        public ActionResult Index()
        {
            var Attachments = attachRpos.List();
           
            return View(Attachments);
        }

        // GET: AttachmentsController/Details/5
        public ActionResult Details(int id)
        {
            var Attach = attachRpos.Find(id,0, "");
            
            return View(Attach);
        }

        // GET: AttachmentsController/Create
        public ActionResult Create()
        {
             
            return View(GetallAuthors());
        }

        // POST: AttachmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AttachmentsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {



                  string TreeeId = GetTreeId(model);




                    var ComplantRefs = compRepository.List().ToList();
                    if (model.AtachId != -1)
                    {
                        model.ComplaintId = ComplantRefs.SingleOrDefault(x => x.ComplaintRecordId == model.AtachId);
                    }
                    else
                    {
                        model.ComplaintId = ComplantRefs.SingleOrDefault(x => x.Employees.EmployeeId == model.EmployeesId);
                    }

                    string FileName = UploadFile(model.File);

                    Attachments AttachmentRecord = new Attachments
                    {
                        AttachmentsId = model.AttachmentsId,
                        Type = model.Type,
                        Description = model.Description,
                        FileName = FileName,
                        ComplaintId = model.ComplaintId,
                        // Date =System.DateTime.Now,
                        Date = model.Date,
                        Trees = treeRepos.Find(0,0, TreeeId),
                    };

                    attachRpos.Add(AttachmentRecord);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            ModelState.AddModelError("", " you have field reqired");
            return View(GetallAuthors());
        }

        private string GetTreeId(AttachmentsViewModel model)
        {
            var TreeeId = "";
            if (model.SideId != -1 && model.SictionId == -1 && model.DepartmentId == -1)
            {
                var side = sideRepos.Find(model.SideId,0, "");
                TreeeId = side.Name;
            }
            else if (model.SideId != -1 && model.SictionId != -1 && model.DepartmentId == -1)
            {
                var section = SectionReop.Find(model.SictionId,0, "");
                TreeeId = section.Name;
            }
            else if (model.SideId != -1 && model.SictionId != -1 && model.DepartmentId != -1)
            {
                var department = departmentRepos.Find(model.DepartmentId,0, "");
                TreeeId = department.Name;
            }

            return TreeeId;
        }

        // GET: AttachmentsController/Edit/5
        public ActionResult Edit(int id)
        {
            var AttackRecord = attachRpos.Find(id,0, "");
            var TreeR =AttackRecord.Trees;
            var SideId = -1;
            var SectionId = -1;
            var DepartId = -1;
             
            

            if (TreeR.SideTFK == 0)
            {
                var side = sideRepos.Find(AttackRecord.Trees.FKName,0, "");
                SideId = side.Id;
            }
            else if (TreeR.SideTFK != 0 && TreeR.SectionTFK == 0)
            {
                var section = SectionReop.Find(AttackRecord.Trees.FKName,0, "");
                SectionId = section.Id;
                SideId = section.sides.Id;
            }
            else
            {
                var department = departmentRepos.Find(AttackRecord.Trees.FKName,0, "");
                DepartId = department.Id;
                SectionId = department.Id;
                SideId = department.sides.Id;
            }
            AttackRecord.AtachId = AttackRecord.ComplaintId.ComplaintRecordId;
            AttachmentsViewModel Vmodel = new AttachmentsViewModel
            {
                AttachmentsId = AttackRecord.AttachmentsId,
                Type = AttackRecord.Type,
                Description = AttackRecord.Description,
                FileName = AttackRecord. FileName,
                AtachId = AttackRecord.AtachId,
                ComplaintId = AttackRecord.ComplaintId,
                Complaints= FillsrepostoryCompIds(AttackRecord.AtachId),
                // Date =System.DateTime.Now,
                Date = AttackRecord.Date,
                Trees = AttackRecord.Trees,
                SideId = SideId,
                SictionId = SectionId,
                DepartmentId = DepartId,
                EmployeesId= AttackRecord.ComplaintId.Employees.EmployeeId,
                Side = Fillsrepostory( ),
                Section = Fillsrepostorysection(),
                Department = FillsrepostoryDepartment(),
                Employees = FillsrepostoryEmployee()

            };



            return View(Vmodel);
        }

        // POST: AttachmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AttachmentsViewModel model)
        {
            try
            {



                string TreeeId = GetTreeId(model);




                var ComplantRefs = compRepository.List().ToList();
                if (model.AtachId != -1)
                {
                    model.ComplaintId = ComplantRefs.SingleOrDefault(x => x.ComplaintRecordId == model.AtachId);
                }
                else
                {
                    model.ComplaintId = ComplantRefs.SingleOrDefault(x => x.Employees.EmployeeId == model.EmployeesId);
                }

                string FileName = UploadFile(model.File,model.FileName);

                Attachments AttachmentRecord = new Attachments
                {
                    AttachmentsId = model.AttachmentsId,
                    Type = model.Type,
                    Description = model.Description,
                    FileName = FileName,
                    ComplaintId = model.ComplaintId,
                    // Date =System.DateTime.Now,
                    Date = model.Date,
                    Trees = treeRepos.Find(0, 0,TreeeId),
                };

                attachRpos.Update(model.AttachmentsId,AttachmentRecord);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttachmentsController/Delete/5
        public ActionResult Delete(int id)
        {
            var Attach = attachRpos.Find(id,0, "");
            return View(Attach);
        }

        // POST: AttachmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {

                attachRpos.Delete(id);
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
                string[] Type = file.FileName.Split('.');

                
                // return LastIdValue;
                if(attachRpos.List().Count()!=0)

                {
                    var Attachments = attachRpos.List().ToList();
                    LastIdValue = Attachments.Last().AttachmentsId + 1;
               }
                 

                //to Replace name of file

                var fileName = file.FileName.Replace(file.FileName, LastIdValue + "." + Type.LastOrDefault());

                string uploads = Path.Combine(hosting.WebRootPath, "upload");
               
                string fullPath = Path.Combine(uploads, fileName);
                file.CopyTo(new FileStream(fullPath, FileMode.Create));

                return fileName;
            }
            return null;
        }
        //للتعديل
        string UploadFile(IFormFile file,string OldFileName)
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
        List<Sides> Fillsrepostory()
        {
            var side = sideRepos.List().ToList();
            side.Insert(0, new Sides { Id = -1, Name = "أختر جهه" });

            return side;

        }
        List<Sections> Fillsrepostorysection()
        {
            var section = SectionReop.List().ToList();
            section.Insert(0, new Sections { Id = -1, Name = "أختر الفرع" });

            return section;

        }
        List<Departments> FillsrepostoryDepartment()
        {
            var depart = departmentRepos.List().ToList();
            depart.Insert(0, new Departments { Id = -1, Name = "أختر القسم" });

            return depart;

        }
        List<ComplaintRecord> FillsrepostoryCompIds(int EmpId)
        {
            var ComplaintsId = compRepository.List().ToList();
           
            ComplaintsId.Insert(0, new ComplaintRecord { ComplaintRecordId = -1 });



            return ComplaintsId;
        }
        List<Employee> FillsrepostoryEmployee()
        {

            var emp = dbContext.employees.ToList();
            emp.Insert(0, new Employee { EmployeeId = -1, Name = "أختر موظف" });

            return emp;

        }

        AttachmentsViewModel GetallAuthors()
        {
            
            var vmodel = new AttachmentsViewModel()
            {
                Side = Fillsrepostory(),

                Section = Fillsrepostorysection(),
                Department = FillsrepostoryDepartment(),
                Employees = FillsrepostoryEmployee(),
                Complaints= FillsrepostoryCompIds(-1)




            };
            return vmodel;
        }
    }
}
