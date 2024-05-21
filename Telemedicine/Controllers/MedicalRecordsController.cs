using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telemedicine.Filters;
using Telemedicine.Models;
using Telemedicine.Viewmodels;
using Xceed.Words.NET;





namespace Telemedicine.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly MedicalRecordsModel _Model;

        public MedicalRecordsController()
        {
            _Model = new MedicalRecordsModel();

        }

        [Auth]
        [Role("Doctor")]
        public ActionResult Index(string doctorId = null, string patientId = null)
        {
            try
            {
                
                List<MedicalRecordsViewModel> medicalRecords = _Model.GetMedicalRecords(); 

                var doctorName = HttpContext.Session["DoctorName"] as string;
                var doctorIds   = _Model.GetDoctorId(doctorName);
                var patientIds = _Model.GetPatientsForDoctor(doctorName);
                //先以醫生負責的病人
                medicalRecords = medicalRecords.Where(r => patientIds.Contains(r.PatientId)).ToList();
                //如果一個病人看多個醫生，只顯示該醫生寫的病歷
                if (!string.IsNullOrEmpty(doctorIds))
                {
                    medicalRecords = medicalRecords.Where(r => r.DoctorId == doctorIds).ToList();
                }

                if (!string.IsNullOrEmpty(doctorId))
                {
                    medicalRecords = medicalRecords.Where(r => r.DoctorId == doctorId).ToList();
                }

                if (!string.IsNullOrEmpty(patientId))
                {
                    medicalRecords = medicalRecords.Where(r => r.PatientId == patientId).ToList();
                }

                return View(medicalRecords); // 返回視圖
            }
            catch (NullReferenceException ex)
            {
                // 處理空值
                Console.Error.WriteLine("Null Reference Error: " + ex.Message); // 顯示錯誤
                ViewBag.Message = "An error occurred. Please try again later.";
                return View(new List<MedicalRecordsViewModel>()); // 返回空列表
            }
            catch (Exception ex)
            {
                // 處理通用異常
                Console.Error.WriteLine("Error: " + ex.Message); // 顯示錯誤
                ViewBag.Message = "An unexpected error occurred. Please try again later.";
                return View(new List<MedicalRecordsViewModel>()); // 返回空列表
            }
        }



        [HttpPost]
        public ActionResult UpdateRecord(string recordId, string patientId, string doctorId, string diagnosis, string treatment, DateTime visitDate, string isFinished)
        {
            try
            {
                _Model.UpdateMedicalRecord(recordId, patientId, doctorId, diagnosis, treatment, visitDate, isFinished);
                return Json(new { success = true, message = "Record updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating record: {ex.Message}" });
            }
        }

        public ActionResult CreateNewRecord(string patientId, string doctorId, string diagnosis, string treatment, DateTime visitDate, string isFinished)
        {
            try
            {

                string recordId = _Model.CreateMedicalRecord(patientId, doctorId, diagnosis, treatment, visitDate, isFinished);

                return Json(new { success = true, recordId = recordId, message = "Record created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error creating record: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult DeleteRecord(string recordId)
        {
            try
            {
                _Model.DeleteMedicalRecord(recordId); // 調用删除方法

                return Json(new { success = true, message = "Record deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting record: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult ExportToExcel(List<string> recordIds)
        {
            if (recordIds == null || recordIds.Count == 0)
            {
                return Json(new { success = false, message = "No record IDs provided." });
            }

            // 獲取數據
            List<MedicalRecordsViewModel> records = _Model.GetRecordsByIds(recordIds);

            if (records == null || records.Count == 0)
            {
                return Json(new { success = false, message = "No records found for the provided IDs." });
            }

            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Medical Records");

                    // 創建欄位名
                    worksheet.Cell(1, 1).Value = "Record ID";
                    worksheet.Cell(1, 2).Value = "Patient ID";
                    worksheet.Cell(1, 3).Value = "Doctor ID";
                    worksheet.Cell(1, 4).Value = "Diagnosis";
                    worksheet.Cell(1, 5).Value = "Treatment";
                    worksheet.Cell(1, 6).Value = "Visit Date";
                    worksheet.Cell(1, 7).Value = "Is Finished";

                    // 填入數據
                    int row = 2;
                    foreach (MedicalRecordsViewModel record in records)
                    {
                        worksheet.Cell(row, 1).Value = record.RecordId;
                        worksheet.Cell(row, 2).Value = record.PatientId;
                        worksheet.Cell(row, 3).Value = record.DoctorId;
                        worksheet.Cell(row, 4).Value = record.Diagnosis;
                        worksheet.Cell(row, 5).Value = record.Treatment;
                        worksheet.Cell(row, 6).Value = record.VisitDate.ToString("yyyy-MM-dd");
                        worksheet.Cell(row, 7).Value = record.IsFinished;
                        row++;
                    }

                    string filePath = Path.Combine(Server.MapPath("~/ExportFile/"), $"MedicalRecords_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    workbook.SaveAs(filePath);

                    // 返回文件下载
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"MedicalRecords_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    return File(filePath, contentType, fileName); // 返回 Excel 文件
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating Excel file: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ExportToPDF(List<string> recordIds)
        {
            if (recordIds == null || recordIds.Count == 0)
            {
                return Json(new { success = false, message = "No records selected." });
            }

            try
            {
                // 創建 PDF 文檔
                Document document = new Document();
                string filePath = Path.Combine(Server.MapPath("~/ExportFile/"), $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                // 打開文檔
                document.Open();

                // 加載字體，確保支持中文
                BaseFont baseFont = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font titleFont = new Font(baseFont, 12, Font.BOLD);  // 標題字體

                // 添加間距
                document.Add(new Paragraph(" ", new Font(baseFont, 16)));  // 添加空段落以增加間距



                // 添加標題段落
                document.Add(new Paragraph("病歷紀錄表", titleFont));  // 中文標題
                // 創建 PDF 表格
                PdfPTable pdfTable = new PdfPTable(7);  // 表格列數
                pdfTable.WidthPercentage = 100;   // 設置表格寬度為100%
                pdfTable.SpacingBefore = 20;  // 添加表格與前一段的間距

                // 填充表格數據
                // 添加表頭
                string[] headers = { "Record ID", "Patient ID", "Doctor ID", "Diagnosis", "Treatment", "Visit Date", "Is Finished" };
                foreach (string header in headers)
                {
                    pdfTable.AddCell(new PdfPCell(new Phrase(header, titleFont)));  // 使用粗體標題
                }

                // 添加數據
                List<MedicalRecordsViewModel> records = _Model.GetRecordsByIds(recordIds);

                Font textFont = new Font(baseFont, 12);  // 標題字體

                foreach (MedicalRecordsViewModel record in records)
                {
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.RecordId, textFont)));
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.PatientId, textFont)));
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.DoctorId, textFont)));
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.Diagnosis, textFont)));
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.Treatment, textFont)));
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.VisitDate.ToString("yyyy-MM-dd"), textFont)));
                    pdfTable.AddCell(new PdfPCell(new Phrase(record.IsFinished, textFont)));
                }

                // 將表格添加到文檔中
                document.Add(pdfTable);

              
                document.Close();

                string contentType = "application/pdf"; // 文件類型
                string fileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"; // 文件名

                return File(filePath, contentType, fileName); // 返回文件
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating PDF: " + ex.Message });
            }
        }

       

    }
}
