using Cdc.Web.BLL;
using Cdc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cdc.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly CdcUserService visitor = new CdcUserService();

        public ActionResult Index() => View(visitor.GetNews());

        public ActionResult Schedule() => View(visitor.GetShedule());

        public ActionResult Subjects() => View(visitor.GetSubjects());

        public ActionResult Teachers() => View(visitor.GetTeachers());

        public ActionResult Responses() => View(visitor.GetResponses());

        [HttpGet]
        public ActionResult AddResponse() => View();

        [HttpPost]
        public ActionResult AddResponse(AddResponseViewModel form)
        {
            visitor.AddResponse(form);
            return RedirectToAction("Responses");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}