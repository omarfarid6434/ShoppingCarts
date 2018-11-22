using ShoppingCarts.Models.Data;
using ShoppingCarts.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCarts.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {

            //Declare list of PageViewModels
            List<PageViewModel> pageList;

           
            using (Db db = new Db())
            {
                //Init list
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageViewModel(x)).ToList();

            }

            return View(pageList);
        }

        // GET: Admin/Pages/AddPages
         [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }


        // Post: Admin/Pages/AddPages
        [HttpPost]
        public ActionResult AddPage(PageViewModel model)
        {

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using(Db db=new Db())
            {
                string slug;

                PageDTO dto = new PageDTO();

                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }


                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That Tile alreay exit");
                    return View(model);
                }

                dto.Slug = model.Slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["Sm"] = "You have add a new page";

                return RedirectToAction("AddPage");
            

           
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Declare pageVM
            PageViewModel model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Init pageVM
                model = new PageViewModel(dto);
            }

            // Return view with model
            return View(model);
        }


        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageViewModel model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {

                //Get page id

                int id = model.Id;

                //Declare slug

                string slug="home";

                //Get the page

                PageDTO dto = db.Pages.Find(id);

                //DTO the title

                dto.Title = model.Title;

                //Check the slug and set it if need be

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                    //Make sure title and slug are uniqe

                    if (db.Pages.Where(x=>x.Id != id).Any(x=>x.Title==model.Title) ||
                   db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                   {

                    ModelState.AddModelError("","The title or slug are allready exist");

                    return View();

                   }

                //DTO the reset

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Save DTO

                db.SaveChanges();
            

            }
            //Set Data message

            TempData["SM"] = "You have eidted page";

            //Redirect

            return RedirectToAction("EditPages");
        }

        // GET: Admin/Pages/PageDetails/id

        public ActionResult PageDetails(int id)
        {
            PageViewModel model;

            using (Db db=new Db())
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("This page does not exist");
                }

                model = new PageViewModel(dto);

            }
            return View(model);
        }
    }
}