using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telemedicine.Filters;
using Telemedicine.Models;
using Telemedicine.Viewmodels;

namespace Telemedicine.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DoctorModel _Model;
        public DoctorController()
        {
            _Model = new DoctorModel();
        }

        [Auth]
        public ActionResult Index()
        {
            var doctorName = Session["User"] as string;
            string doctorId = _Model.GetDoctorId(doctorName);
            ViewBag.DoctorId = doctorId;
            List<DoctorViewModel> doctors = _Model.GetDoctors();
            return View(doctors);
        }
    }
}