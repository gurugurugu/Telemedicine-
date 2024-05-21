using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Telemedicine.Controllers
{
public class LanguageController : Controller
{
        public ActionResult SetLanguage(string lang, string returnUrl)
        {
            // 設置语言
            Session["Culture"] = lang;

            // 設置語言的 Cookie（可選）
            Response.Cookies["Culture"].Value = lang;

            // 重定向到请求的來源頁面
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);  // 返回原始頁面
            }

            // 如果没有返回的 URL，默認重定向到首頁
            return RedirectToAction("Index", "Home");
        }
    }
}