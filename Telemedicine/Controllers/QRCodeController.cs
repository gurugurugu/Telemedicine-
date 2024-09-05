using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using QRCoder;

namespace Telemedicine.Controllers
{
    public class QRCodeController : Controller
    {
        // 用於儲存令牌的靜態字典
        private static readonly Dictionary<string, (string username, string role, bool validated)> tokenStore = new Dictionary<string, (string username, string role, bool validated)>();

        /*public ActionResult GenerateQRCode()
        {
            // 假設從某處獲取了當前用戶的用户名和角色信息
            string username = "*";
            string role = "*";

            // 創建唯一令牌，並將其初始化為為驗證
            var token = Guid.NewGuid().ToString();
            tokenStore[token] = (username, role, false);

            // 構件帶有令牌的URLL
            var qrCodeUrl = Url.Action("ValidateToken", "QRCode", new { token, username, role }, Request.Url.Scheme);

            // 使用 QR Code 生成器生成 QR Code
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (var bitmap = qrCode.GetGraphic(3)) // 調整生成的 QR Code 大小
                    {
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            return File(stream.ToArray(), "image/png");
                        }
                    }
                }
            }
        }*/


        /* public ActionResult ValidateToken(string token, string username, string role)
         {
             if (tokenStore.ContainsKey(token) && !tokenStore[token].validated)
             {
                 // 標記令牌為以驗證
                 var (storedUsername, storedRole, _) = tokenStore[token];
                 tokenStore[token] = (storedUsername, storedRole, true);

                 // 設置會話（Session）
                 Session["User"] = username;
                 Session["Role"] = role;

                 // 根據角色信息跳轉到不同的頁面
                 if (role == "Doctor")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else if (role == "Nurse")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 // 如果角色未知或未指定特定的頁面，则跳轉到默認頁面
                 else
                 {
                     return RedirectToAction("Index", "Home");
                 }
             }

             return Content("Token is invalid or already used.");
         }*/
        public ActionResult Generate(string connectionId)
        {
            string url = $"http://localhost:8080/AuthApi/Authenticate?connectionId={connectionId}";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitmap = qrCode.GetGraphic(3))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return File(stream.ToArray(), "image/png");
                }
            }
        }

        public ActionResult Show()
        {
            return View();
        }
    }
}






