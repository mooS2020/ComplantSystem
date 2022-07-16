using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComplantSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ICompRepository<Departments> departmentRepos;
        private readonly ICompRepository<Sections> SectionReop;
        private readonly ICompRepository<Tree> treeRepos;
        private readonly ICompRepository<Sides> sideRepos;

        public DepartmentsController(ICompRepository<Departments> departmentRepos,ICompRepository<Sections> SectionReop,
          ICompRepository<Tree> treeRepos, ICompRepository<Sides> SideRepos)
        {
            this.departmentRepos = departmentRepos;
            this.SectionReop = SectionReop;
            this.treeRepos = treeRepos;
            sideRepos = SideRepos;
        }
        // GET: SectionsController
        public ActionResult Index()
        {
            var dep = departmentRepos.List();
            return View(dep);
        }

        // GET: SectionsController/Details/5
        public ActionResult Details(int id)
        {
            var dep = departmentRepos.Find(id,0,"");
            return View(dep);
            
        }

        // GET: SectionsController/Create
        public ActionResult Create()
        {
            return View(GetallAuthors());
        }

        // POST: SectionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SectionSDViewmodel model)
        {
            try
            {
                var TreeParant = 0;
                var Section = SectionReop.Find(model.SictionId,0,"");
                var SparantId = Section.parant == null ?   0 : Section.parant.Id;
                //var Sparant= SectionReop.Find(Section.parant.Id, "");
                var Side = sideRepos.Find(model.SideId,0, "");


               // var SideT = treeRepos.Find(0, model.SideId, "side");
                var SectionT = treeRepos.Find(0, model.SictionId, "section");
                

                if (SparantId != 0)
                {
                    TreeParant = Section.parant.Id;
                }
                if (model.SideId == -1|| model.SictionId == -1)
                {
                    ViewBag.Message = "اختر الجهه والفرع";


                    return View(GetallAuthors());
                }
                Departments NewDepartments = new Departments
                {
                    Id = model.Id,
                    Name = model.Name,
                    sides = Side,
                    sections =Section
                };
                departmentRepos.Add(NewDepartments);
                
                Tree t = new Tree
                {
                    Name = model.Name,
                    FKName = NewDepartments.Id,
                    SideTFK = SectionT.SideTFK,
                    SectionTFK = SectionT.TreeId,
                     



                };
                treeRepos.Add(t);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SectionsController/Edit/5
        public ActionResult Edit(int id)
        {   
            var dep = departmentRepos.Find(id,0,"");
            
            var sideId =dep.sides==null?dep.sides.Id = 0 : dep.sides.Id;
            var sectionId = dep.sections == null ? dep.sections.Id = 0 : dep.sections.Id;
            SectionSDViewmodel viewmodel = new SectionSDViewmodel()
            {
                Name = dep.Name,
                Id = dep.Id,
                SideId = sideId,
                SictionId = sectionId,
                Side =sideRepos.List().ToList(),
                Section= SectionReop.List().ToList()
            };

            return View(viewmodel);
        }

        // POST: SectionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( SectionSDViewmodel model )
        {
            try
            {
                var side = sideRepos.Find(model.SideId,0,"");
                var sect = SectionReop.Find(model.SictionId,0,"");
                Departments s = new Departments
                {
                    Id = model.Id,
                    Name = model.Name,
                    sides = side,
                    sections = sect,
                };
               departmentRepos.Update(model.Id,s);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SectionsController/Delete/5
        public ActionResult Delete(int id)
        {

            var dep = departmentRepos.Find(id,0,"");
            return View(dep);
        }

        // POST: SectionsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                departmentRepos.Delete(id);
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
        SectionSDViewmodel GetallAuthors()
        {
            var vmodel = new SectionSDViewmodel()
            {
                Side = Fillsrepostory(),
                
               Section = Fillsrepostorysection()



            };
            return vmodel;
        }
        public ActionResult Search(string term)
        {
            var result = departmentRepos.Search(-1,term);
            return View("Index", result);
        }
    }
}
