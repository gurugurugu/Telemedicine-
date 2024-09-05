using System;
using System.Diagnostics;
using System.Web.Mvc;

public class SessionController : Controller
{
    [HttpPost]
    public ActionResult SetSessionVariables(string username, string role)
    {
        Session["User"] = username;
        Session["Role"] = role;
        return Json(new { success = true });
    }
}