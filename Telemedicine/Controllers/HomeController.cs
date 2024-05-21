using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telemedicine.Filters;

namespace Telemedicine.Controllers
{
    public class HomeController : Controller
    {
        [Auth]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
                ViewBag.MessageType = TempData["MessageType"];
            }
            return View();
        }
        [Auth]
        public ActionResult Unauthorized()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.MessageType = TempData["MessageType"];

            return View("Index");
        }
        [Auth]
        public ActionResult GenerateQRCode()
        {
            return View();
        }

    }
}