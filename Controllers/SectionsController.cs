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
    public class SectionsController : Controller
    {
        private readonly ICompRepository<Sections> SectionReop;
        private readonly ICompRepository<Sides> sideRepos;
        private readonly ICompRepository<Tree> treeRepos;

        public SectionsController(ICompRepository<Sections> SectionReop,ICompRepository<Sides> SideRepos,ICompRepository<Tree> treeRepos)
        {
            this.SectionReop = SectionReop;
            sideRepos = SideRepos;
            this.treeRepos = treeRepos;
        }
        // GET: SectionsController
        public ActionResult Index()
        {
            var section = SectionReop.List();
            return View(section);
        }
        public ActionResult GetAllEmployeeData()
        {
            var section = SectionReop.List();
            return View(section);
        }

        // GET: SectionsController/Details/5
        public ActionResult Details(int id)
        {
            var section = SectionReop.Find(id,0,"");
            return View(section);
            
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

                var c = 0;
                
                var Sparant = SectionReop.Find(model.SictioParantId,0, "");

                var Side = sideRepos.Find(model.SideId,0, "");
                var SideT = treeRepos.Find( 0, model.SideId, "side");
                if (model.SictioParantId != -1)
                {
                    var ParantT = treeRepos.Find(0, model.SictioParantId, "sParant");
                    c = ParantT.TreeId;

                }


                if (model.SideId == -1)
                {
                    ViewBag.Message = "اختر جهه";


                    return View(GetallAuthors());
                }
                 
                 
                Sections NewSections = new Sections
                {
                    Id = model.Id,
                    Name = model.Name,
                    parant = Sparant,
                    sides = Side
                };
                SectionReop.Add(NewSections);

                Tree t = new Tree
                {
                    Name=model.Name,
                    FKName= NewSections.Id,
                    SideTFK=SideT.TreeId,
                    SectionPTFK =c


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
            var section = SectionReop.Find(id,0,"");
            
            var sideId = section.sides==null?section.sides.Id = 0 : section.sides.Id;
             
            if (section.parant ==null)
            {
                SectionSDViewmodel viewmodel = new SectionSDViewmodel()
                {
                    Name = section.Name,
                    Id = section.Id,
                    SideId = sideId,

                    Side = Fillsrepostory(),
                    Sectionparant = FillsrepostorySectionParant()
                };


                return View(viewmodel);
            }
            else
            {
                 

                SectionSDViewmodel viewmodel = new SectionSDViewmodel()
                {
                    Name = section.Name,
                    Id = section.Id,
                    SideId = sideId,
                    SictioParantId = section.parant.Id,
                    Side = Fillsrepostory(),
                    Sectionparant = FillsrepostorySectionParant()
                };


                return View(viewmodel);
            }
        }

        // POST: SectionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( SectionSDViewmodel model )
        {
            try
            {
                var side = sideRepos.Find(model.SideId,0,"");
                Sections s = new Sections
                {
                    Id = model.Id,
                    Name = model.Name,
                    parant = SectionReop.Find(model.SictioParantId,0,""),

                    sides = side
                };
               SectionReop.Update(model.Id,s);
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

            var section = SectionReop.Find(id,0,"");
            return View(section);
        }

        // POST: SectionsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                SectionReop.Delete(id);
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
        List<Sections> FillsrepostorySectionParant()
        {
            var sectionParant = SectionReop.List().ToList();
            sectionParant.Insert(0, new Sections { Id = -1, Name = "هو فرع رئيسي" });

            return sectionParant;

        }
        SectionSDViewmodel GetallAuthors()
        {
            var vmodel = new SectionSDViewmodel()
            {
                Side = Fillsrepostory(),
                Sectionparant = FillsrepostorySectionParant()
            };
            return vmodel;
        }
        public ActionResult Search(string term)
        {
            var result = SectionReop.Search(-1,term);
            return View("Index", result);
        }
    }
}
