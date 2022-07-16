using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace ComplantSystem.Controllers
{
    public class ComplaintRecordController : Controller
    {
        private readonly ICompRepository<ComplaintRecord> compRepository;
        private readonly ICompRepository<Departments> departmentRepos;
        private readonly ICompRepository<Sections> SectionReop;
        private readonly ICompRepository<Sides> sideRepos;
        private readonly ICompRepository<Tree> treeRepos;
        public ICompRepository<CompProceures> compProceuresRepos;
        CompDBContext dbContext;
        private readonly ICompRepository<Employee> employeeRepos;
        private readonly ICompRepository<Transactions> transaRepos;

        public ComplaintRecordController(
            ICompRepository<ComplaintRecord> compRepository,
            ICompRepository<Departments> departmentRepos,
             ICompRepository<Tree> treeRepos,
             ICompRepository<CompProceures> compProceuresRepos,
            ICompRepository<Sections> SectionReop,
            ICompRepository<Sides> sideRepos,
            CompDBContext _dbContext,
             ICompRepository<Employee> employeeRepos,
            ICompRepository<Transactions> transaRepos
            )
        {
            this.compRepository = compRepository;
            this.departmentRepos = departmentRepos;
            this.SectionReop = SectionReop;
            this.sideRepos = sideRepos;
            this.treeRepos = treeRepos;
            this.compProceuresRepos = compProceuresRepos;
            dbContext = _dbContext;
            this.employeeRepos = employeeRepos;
            this.transaRepos = transaRepos;
        }


        // GET: ComplaintRecordController
        public ActionResult Index()
        {
          
            var record = compRepository.List();
             
            return View(record);
        }


        



        // GET: ComplaintRecordController/Details/5
        public ActionResult Details(int id)
        {
            var ComProceures = compProceuresRepos.Search(id, "");

            ViewBag.ProceuresArr = ReturnCompProceures(id);
            var model = compRepository.Find(id,0, "");

            

            CompViewmodel comp = new CompViewmodel
            {
                ComplaintRecordId = model.ComplaintRecordId,
                AutoNumber = model.AutoNumber,
                ReferenceNumber = model.ReferenceNumber,
                JobType = model.JobType,
                DefendantName = model.DefendantName,
                // Employees=model.EmployeesId,
                ComplainantName = model.ComplainantName,
                ComplainantSex = model.ComplainantSex,
                ComplainantPhone = model.ComplainantPhone,
                ComplainantGmail = model.ComplainantGmail,
                ComplainantCardNumber = model.ComplainantCardNumber,
                ComplainantType = model.ComplainantType,
                AgentName = model.AgentName,
                AgentCardNumber = model.AgentCardNumber,
                ComplaintContent = model.ComplaintContent,
                ComplaintDeta = model.ComplaintDeta,
                Orientation = model.Orientation,
                OrientationSours = model.OrientationSours,
                OrientationDeta = model.OrientationDeta,
                Notes = model.Notes,
                Trees = treeRepos.Find(0,0, model.Trees.Name),
                compProceures = ComProceures
            };

            return View(comp);
        }

        private int[] ReturnCompProceures(int id)
        {
            var ComProceures = compProceuresRepos.Search(id, "");
            int[] ProceuresArr = new int[15];
            int i = 1;
            foreach (var item in ComProceures)
            {
                ProceuresArr[i] = item.CompProceuresContent;
                i++;
            }


          return ProceuresArr;
          
        }

        public ActionResult Create()
        {

            return View(GetallAuthors());
        }

        // POST: ComplaintRecordController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompViewmodel model)
        {
            if (ModelState.IsValid)
            {
                try
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

                    var emp = employeeRepos.Find(model.EmployeesId,0, "");

                    ComplaintRecord s = new ComplaintRecord
                    {
                         
                        ReferenceNumber = model.ReferenceNumber,
                        JobType = model.JobType,
                        DefendantName = emp.Name,
                        Employees = emp,
                        ComplainantName = model.ComplainantName,
                        ComplainantSex = model.ComplainantSex,
                        ComplainantPhone = model.ComplainantPhone,
                        ComplainantGmail = model.ComplainantGmail,
                        ComplainantCardNumber = model.ComplainantCardNumber,
                        ComplainantType = model.ComplainantType,
                        AgentName = model.AgentName,
                        AgentCardNumber = model.AgentCardNumber,
                        ComplaintContent = model.ComplaintContent,
                        ComplaintDeta = model.ComplaintDeta,
                        Orientation = model.Orientation,
                        OrientationSours = model.OrientationSours,
                        OrientationDeta = model.OrientationDeta,
                        Notes = model.Notes,
                        Trees = treeRepos.Find(0,0, TreeeId),
                    };
                    compRepository.Add(s);
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

        // GET: ComplaintRecordController/Edit/5
        public ActionResult Edit(int id)
        {
            var model = compRepository.Find(id,0, "");

            var TreeR = treeRepos.Find(0,0, model.Trees.Name);
            var SideId =-1;
            var SectionId =-1;
            var DepartId =-1;


            if (TreeR.SideTFK == 0 )
            {
                var side = sideRepos.Find(model.Trees.FKName,0, "");
                SideId = side.Id;
            }
            else if (TreeR.SideTFK != 0 && TreeR.SectionTFK == 0)
            {
                var section = SectionReop.Find(model.Trees.FKName,0, "");
                SectionId = section.Id;
                SideId = section.sides.Id;
            }
            else  
            {
                var department = departmentRepos.Find(model.Trees.FKName,0, "");
                DepartId = department.Id;
                SectionId = department.Id;
                SideId = department.sides.Id;
            }



            CompViewmodel comp = new CompViewmodel
            {
                ComplaintRecordId = model.ComplaintRecordId,
                ReferenceNumber = model.ReferenceNumber,
                JobType = model.JobType,
                DefendantName = model.DefendantName,
                EmployeesId=model.Employees.EmployeeId,
               // Employees=model.Employees,
                ComplainantName = model.ComplainantName,
                ComplainantSex = model.ComplainantSex,
                ComplainantPhone = model.ComplainantPhone,
                ComplainantGmail = model.ComplainantGmail,
                ComplainantCardNumber = model.ComplainantCardNumber,
                ComplainantType = model.ComplainantType,
                AgentName = model.AgentName,
                AgentCardNumber = model.AgentCardNumber,
                ComplaintContent = model.ComplaintContent,
                ComplaintDeta = model.ComplaintDeta,
                Orientation = model.Orientation,
                OrientationSours = model.OrientationSours,
                OrientationDeta = model.OrientationDeta,
                Notes = model.Notes,
                Trees = TreeR,
                SideId=SideId,
                SictionId=SectionId,
                DepartmentId=DepartId,
                Side = Fillsrepostory(),
                Section = Fillsrepostorysection(),
                Department = FillsrepostoryDepartment(),
                Employees = FillsrepostoryEmployee()


            };

            return View(comp);


             
        }

        // POST: ComplaintRecordController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CompViewmodel model)
        {
             
            
                try
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
                var emp = employeeRepos.Find(model.EmployeesId,0, "");

                ComplaintRecord UpdateRecord = new ComplaintRecord
                    {
                        ComplaintRecordId = model.ComplaintRecordId,
                        ReferenceNumber = model.ReferenceNumber,
                        JobType = model.JobType,
                        DefendantName = emp.Name,
                         Employees=emp,
                        ComplainantName = model.ComplainantName,
                        ComplainantSex = model.ComplainantSex,
                        ComplainantPhone = model.ComplainantPhone,
                        ComplainantGmail = model.ComplainantGmail,
                        ComplainantCardNumber = model.ComplainantCardNumber,
                        ComplainantType = model.ComplainantType,
                        AgentName = model.AgentName,
                        AgentCardNumber = model.AgentCardNumber,
                        ComplaintContent = model.ComplaintContent,
                        ComplaintDeta = model.ComplaintDeta,
                        Orientation = model.Orientation,
                        OrientationSours = model.OrientationSours,
                        OrientationDeta = model.OrientationDeta,
                        Notes = model.Notes,
                        Trees = treeRepos.Find(0,0, TreeeId),
                    };
                    compRepository.Update(model.ComplaintRecordId, UpdateRecord);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                } 
        }

        // GET: ComplaintRecordController/Delete/5
        public ActionResult Delete(int id)
        {
            var CompRecord = compRepository.Find(id,0, "");
            return View(CompRecord);
        }

        // POST: ComplaintRecordController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
               // var ComProceures = compProceuresRepos.Find(id, "SetNull");
                
               // compProceuresRepos.Delete( id);
                compRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
        List<Employee> FillsrepostoryEmployee()
        {

            var emp = dbContext.employees.ToList();
            emp.Insert(0, new Employee { EmployeeId = -1, Name = "أختر موظف" });

            return emp;

        }

        CompViewmodel GetallAuthors()
        {

            var vmodel = new CompViewmodel()
            {
                Side = Fillsrepostory(),

                Section = Fillsrepostorysection(),
                Department = FillsrepostoryDepartment(),
                Employees = FillsrepostoryEmployee()




            };
            return vmodel;
        }
        public ActionResult Search(string term)
        {
            var result = compRepository.Search(-1, term);
            return View("Index", result);
        }

       


         

        public JsonResult GetSideOrSection(int SideId,int sectionId)   
        {
            if(sectionId != 0)
            {
                var Departs = departmentRepos.List().Where(x => x.sections.Id == sectionId).ToList();
                Departs.Insert(0, new Departments { Id = -1, Name = "أختر القسم" });
                return Json(Departs);
            }
            //   List<Sections> sections = new List<Sections>(); 
            var Sections = SectionReop.List().Where(x=>x.sides.Id==SideId).ToList();
                 Sections.Insert(0, new Sections { Id = -1, Name = "أختر الفرع" });
            return Json(Sections);
            
        }


        public ActionResult StartTrans()
        {
            TransactionVModel Trans = new TransactionVModel
            {

                Side = Fillsrepostory(),
                Section = Fillsrepostorysection(),
                Department = FillsrepostoryDepartment(),



            };
            return View(Trans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartTrans(TransactionVModel model)
        {
            var treeUserId = -1;
            if (model.SideId != -1 && model.SictionId == -1 && model.DepartmentId == -1)
            {
                var SideT = treeRepos.Find(0, model.SideId, "side");
                treeUserId = SideT.TreeId;
 
            }
            else if (model.SideId != -1 && model.SictionId != -1 && model.DepartmentId == -1)
            {
                var SectionT = treeRepos.Find(0, model.SictionId, "section");
                treeUserId = SectionT.TreeId;
            }
            else if (model.SideId != -1 && model.SictionId != -1 && model.DepartmentId != -1)
            {
                var DepT = treeRepos.Find(0, model.DepartmentId, "deprt");
                treeUserId = DepT.TreeId;
            }

           
            var Comps = compRepository.List()
           .Where(x => x.Trees.TreeId == treeUserId ||
           x.Trees.SideTFK == treeUserId ||
           x.Trees.SectionTFK == treeUserId ||
            x.Trees.SectionPTFK == treeUserId);


            return View("Index", Comps);
        }


        public ActionResult Transaction(int id)
        {
            var model = compRepository.Find(id,0, "");

            var TreeR = treeRepos.Find(0,0, model.Trees.Name);
            var SideId = -1;
            var SectionId = -1;
            var DepartId = -1;


            if (TreeR.SideTFK == 0)
            {
                var side = sideRepos.Find(model.Trees.FKName,0, "");
                SideId = side.Id;
            }
            else if (TreeR.SideTFK != 0 && TreeR.SectionTFK == 0)
            {
                var section = SectionReop.Find(model.Trees.FKName,0, "");
                SectionId = section.Id;
                SideId = section.sides.Id;
            }
            else
            {
                var department = departmentRepos.Find(model.Trees.FKName,0, "");
                DepartId = department.Id;
                SectionId = department.Id;
                SideId = department.sides.Id;
            }



            TransactionVModel Trans = new TransactionVModel
            {
                CompId = model.ComplaintRecordId,
                ReferenceNumber = model.ReferenceNumber,
                
                DefendantName = model.DefendantName,
                EmployeesId = model.Employees.EmployeeId,
                // Employees=model.Employees,
                TreeInId= TreeR.TreeId,
               // TreeIn = TreeR,

                SideId = SideId,
                SictionId = SectionId,
                DepartmentId = DepartId,
                Side = Fillsrepostory(),
                Section = Fillsrepostorysection(),
                Department = FillsrepostoryDepartment(),
                Employees = FillsrepostoryEmployee()


            };

            return View(Trans);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction(TransactionVModel model)
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
            var emp = employeeRepos.Find(model.EmployeesId,0, "");
            var comp = compRepository.Find(model.CompId,0, "");

            

            var treeIn = treeRepos.Find(model.TreeInId,0, "");
            var treeTo = treeRepos.Find(-1,0, TreeeId);

            Transactions trans = new Transactions
            {
                ComplaintFID =comp,  
                ReferenceNumber=model.ReferenceNumber,
                TreeIn= treeIn,
                TreeTo= treeTo,
                Orientation=model.Orientation,
                Date=model.Date, 
                TransType=model.TransType,
            };
            transaRepos.Add(trans);

            //to change the state
            comp.ComplaintState = model.TransType;
            compRepository.Update(comp.ComplaintRecordId,comp);  
 
            var  comps = compRepository.List().Where(comp => comp.ComplaintState != 1).ToList();
            return View("Index", comps);

        }
    }

}
    

