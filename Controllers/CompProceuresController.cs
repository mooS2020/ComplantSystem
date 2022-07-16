using ComplantSystem.Models;
using ComplantSystem.Models.Repository;
using ComplantSystem.viewmodel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ComplantSystem.Controllers
{
    public class CompProceuresController : Controller
    {
        private readonly ICompRepository<CompProceures> compProceRepos;
        private readonly ICompRepository<ComplaintRecord> compRepository;

        public CompProceuresController(ICompRepository<CompProceures> compProceRepos,ICompRepository<ComplaintRecord> compRepository)
        {
            this.compProceRepos = compProceRepos;
            this.compRepository = compRepository;
        }
        // GET: CompProceuresController
        public ActionResult Index( int id)
        {
          //  var id = 1;
            ViewBag.Message = id.ToString();

            var Rusults = compProceRepos.Search(id, "");
            return View(Rusults);
        }


        // GET: CompProceuresController/Details/5
        public ActionResult Details(int id)
        {

            var Proceures = compProceRepos.Find(id,0, "");
            return View(Proceures);
        }

        // GET: CompProceuresController/Create
        public ActionResult Create(int id)
        {
            
           
            ViewBag.CompId = id;
             
           return View();
            
        }

        // POST: CompProceuresController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompProceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var CoRecordId = compRepository.Find(model.ComplaintsId,0, "");
                    CompProceures CompProce = new CompProceures
                    {

                        CompProceuresContent = model.CompProceuresContent,
                        CompProceuresCount = model.CompProceuresCount,
                        ComplaintId = CoRecordId,
                        Date = model.Date,

                    };
                    compProceRepos.Add(CompProce);

                    return RedirectToAction("Index", new { id = CoRecordId.ComplaintRecordId });
                }



                catch
                {
                    return View();

                }
            }
            ModelState.AddModelError("", " you have field reqired");
            return View();
           
        }

      
        // GET: CompProceuresController/Edit/5
        public ActionResult Edit(int id)
        {
            var Proceures = compProceRepos.Find(id,0, "");
            CompProceViewModel Copm = new CompProceViewModel
            {
                CompProceuresId = Proceures.CompProceuresId,
                CompProceuresContent = Proceures.CompProceuresContent,
                CompProceuresCount = Proceures.CompProceuresCount,
                Date = Proceures.Date,
                ComplaintsId = Proceures.ComplaintId.ComplaintRecordId,
                 
            };

            return View(Copm);
        }

        // POST: CompProceuresController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( CompProceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var CoRecordId = compRepository.Find(model.ComplaintsId,0, "");
                    CompProceures CompProce = new CompProceures
                    {
                        CompProceuresId=model.CompProceuresId,
                        CompProceuresContent = model.CompProceuresContent,
                        CompProceuresCount = model.CompProceuresCount,
                        ComplaintId = CoRecordId,
                        Date = model.Date,

                    };
                    compProceRepos.Update(model.CompProceuresId,CompProce);

                    return RedirectToAction("Index", new { id = CoRecordId.ComplaintRecordId });
                }



                catch
                {
                    return View();

                }
            }
            ModelState.AddModelError("", " you have field reqired");
            return View();
            //return RedirectToActio
        }

        // GET: CompProceuresController/Delete/5
        public ActionResult Delete(int id)
        {
            var Proceures = compProceRepos.Find(id,0,"");
            return View(Proceures);
        }

        // POST: CompProceuresController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var GetId = compProceRepos.Find(id, 0, "");
                var sendId = GetId.ComplaintId.ComplaintRecordId;
                compProceRepos.Delete(id);
               
                return RedirectToAction("Index", new {id= sendId });
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Add(int id,int Count, int Content, DateTime Date)
        {
            var CoRecordId = compRepository.Find(id,0, "");
            CompProceures CompProce = new CompProceures
            {

                CompProceuresContent = Count,
                CompProceuresCount = Content,
                ComplaintId = CoRecordId,
                Date =Date,

            };
            compProceRepos.Add(CompProce);

            var result = compProceRepos.Search(id, "");
            //نحتاج الى ايجاد طريقة افضل
            return View("Index", result);

        }
    }
}
