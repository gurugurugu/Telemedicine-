using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Telemedicine.Controllers
{
    public class MessageController : Controller
    {
        // 返回用於顯示消息的 Partial View
        public ActionResult ShowMessage(string message, string messageType)
        {
            // 將消息和類型儲存到 ViewBag 或 Model 中
            ViewBag.Message = message;
            ViewBag.MessageType = messageType; // 'success', 'error', 'info' 等

            // 返回 Partial View
            return PartialView("_OperationMessage"); // 返回顯示消息的 Partial View
        }
    }
}