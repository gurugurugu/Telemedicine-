using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Telemedicine.Filters
{
    public class AuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["User"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "Controller", "Account" },
                    { "Action", "Login" }
                    }
                );
            }
            else
            {
                var userName = filterContext.HttpContext.Session["User"] as string;
                var role = filterContext.HttpContext.Session["Role"] as string;

                // 將角色和用戶名直接設置到Session中
                filterContext.HttpContext.Session["Role"] = role;
                filterContext.HttpContext.Session["DoctorName"] = userName; // 直接將用戶名當作醫生姓名
            }
        }
    }

    public class RoleAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public RoleAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }
      
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userRole = filterContext.HttpContext.Session["Role"] as string;
            var doctorName = filterContext.HttpContext.Session["DoctorName"] as string;

            if (userRole == null || userRole != _requiredRole || (_requiredRole == "Doctor" && !IsDoctorAuthorized(doctorName)))
            {
                filterContext.Controller.TempData["Message"] = "你沒有權限使用該功能";
                filterContext.Controller.TempData["MessageType"] = "warning";
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "Controller", "Home" },
                    { "Action", "Unauthorized" }
                    }
                );
            }
        }

        // 檢查醫生是否有權限訪問病歷資料
        private bool IsDoctorAuthorized(string doctorName)
        {
            // 根據醫生姓名（用戶名）查詢該醫生負責的病人ID列表
            var patientIds = GetPatientsForDoctor(doctorName);

            return patientIds.Any(); // 如果有任何病人ID，則表示有權限訪問病歷資料
        }

        private List<string> GetPatientsForDoctor(string doctorName)
        {
            List<string> patientIds = new List<string>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

            string query = "SELECT RECORD.VCHPATIENTID " +
                           "FROM RECORD " +
                           "INNER JOIN DOCTOR ON RECORD.VCHDOCTORID = DOCTOR.VCHDOCTORID " +
                           "WHERE DOCTOR.VCHDOCTORNAME = :doctorName";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("doctorName", doctorName));

                    try
                    {
                        connection.Open();
                        OracleDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string patientId = reader["VCHPATIENTID"].ToString(); // 此處應該是 VCHPATIENTID
                            patientIds.Add(patientId);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        // 處理異常
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }

            return patientIds;
        }
    }
}