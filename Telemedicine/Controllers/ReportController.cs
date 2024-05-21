using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telemedicine.Filters;
using Telemedicine.Models;
using Telemedicine.Viewmodels;

namespace Telemedicine.Controllers
{
    public class ReportController : Controller
    {
        private readonly ReportModel _Model;

        public ReportController()
        {
            _Model = new ReportModel();
        }

        [Auth]
        public ActionResult Index(string recordId)
        {
            try
            {
                List<ReportViewModel> combinedRecord = _Model.GetCombinedRecordByRecordId(recordId); // 根據 Record ID 獲取紀錄

                if (combinedRecord == null) // 如果没有找到紀錄
                {
                    return HttpNotFound("No record found with the specified record ID."); // 返回 404
                }

                return View(combinedRecord); // 返回視圖
            }
            catch (OracleException ex)
            {
                // Oracle 數據庫異常處理
                Console.Error.WriteLine("Oracle Error: " + ex.Message); // 記錄錯誤信息
                ViewBag.Message = "Database error. Please try again later.";
                return View("Error"); // 返回錯誤視圖或其他响應
            }
            catch (Exception ex)
            {
                // 處理通用異常
                Console.Error.WriteLine("Error: " + ex.Message); // 紀錄錯誤訊息
                ViewBag.Message = "An unexpected error occurred. Please try again later.";
                return View("Error"); // 返回錯誤視圖或其他響應
            }
        }

        [HttpPost]
        public ActionResult GenerateWordReport(string recordId, string Diagnosis, string Treatment, DateTime VisitDate, string PatientId, string PatientName, string PatientAge, string PatientPhone, string PatientGender, string DoctorId, string DoctorName, string Specialty)

        {
            try
            {
                // 从 FormCollection 中提取數據
                ReportViewModel record = new ReportViewModel
                {
                    RecordId = recordId,  // 提取值
                    Diagnosis = Diagnosis,
                    Treatment = Treatment,
                    VisitDate = VisitDate,  // 確保解析日期
                    PatientId = PatientId,
                    PatientName = PatientName,
                    PatientAge = PatientAge,
                    PatientPhone = PatientPhone,
                    PatientGender = PatientGender,
                    DoctorId = DoctorId,
                    DoctorName = DoctorName,
                    Specialty = Specialty
                };

                // 生成 Word 報告
                string outputPath = Path.Combine(Server.MapPath("~/App_Data/"), $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.docx");  // 指定保存路径
                // 套用template
                string templatePath = Server.MapPath("~/App_Data/Template_2.docx");

                ReportModel.GenerateWordReport(record, outputPath, templatePath);

                return Json(new { message = "Word 報告生成成功" });
            }
            catch (Exception ex)
            {
                return Json(new { error = $"發生錯誤: {ex.Message}" });
            }
        }
    }
}