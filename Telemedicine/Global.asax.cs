using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OfficeOpenXml; // 引入 EPPlus 命名空间

namespace Telemedicine
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {


            // 设置 EPPlus 的许可证上下文
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


        }
        protected void Application_BeginRequest()
        {
            var lang = Request.Cookies["Culture"]?.Value ?? "en";  // 从 Cookie 获取语言
            var cultureInfo = new CultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

    }
}
