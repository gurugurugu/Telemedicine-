using OfficeOpenXml;
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
    public class PatientController : Controller
    {
        private readonly PatientModel _Model;
        public PatientController()
        {
            _Model = new PatientModel ();
        }

        [Auth]
        public ActionResult Index()
        {
            List<PatientViewModel> patientInfo = _Model.GetPatients();
            return View(patientInfo);
        }

        [HttpPost]
        public ActionResult Import()
        {
            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No file uploaded." });
                }

                HttpPostedFileBase excelFile = Request.Files[0];
                List<PatientViewModel> data = new List<PatientViewModel>();

                using (ExcelPackage package = new ExcelPackage(excelFile.InputStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];


                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        PatientViewModel patient = new PatientViewModel
                        {
                            PatientId = worksheet.Cells[i, 1].Text,
                            PatientName = worksheet.Cells[i, 2].Text,
                            PatientAge = worksheet.Cells[i, 3].Text,
                            PatientGender = worksheet.Cells[i, 4].Text,
                            PatientPhone = worksheet.Cells[i, 5].Text,
                        };
                        data.Add(patient);
                    }
                }

                // 將數據導入數據庫
                _Model.ImportPatients(data);

                return Json(new { success = true, message = "Import successful." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Import failed: " + ex.Message });
            }
        }
    }
}