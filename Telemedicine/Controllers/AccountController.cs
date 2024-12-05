using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Telemedicine.Models;
using Telemedicine.Resources;
using Telemedicine.Viewmodels;

namespace Telemedicine.Controllers
{
    public class AccountController : Controller
    {

        private readonly AccountModel _Model;

        public AccountController()
        {
            _Model = new AccountModel();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(); // 顯示註冊畫面
        }
        // 用戶註冊
        [HttpPost]
        public ActionResult Register(string username, string password, string confirmPassword, string role)
        {
            try
            {
                if (password != confirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    ViewBag.Message = Strings.Register_ComfirmPassword_Failed;
                    ViewBag.MessageType = Strings.MessageType_Error;

                    return View(); // 密碼返回錯誤時註冊畫面
                }

                string passwordHash = AccountModel.HashPassword(password); // 加密密碼

                AccountViewModel user = new AccountViewModel
                {
                    VCHUSERID = Guid.NewGuid().ToString(),
                    VCHUSERNAME = username,
                    VCHPASSWORD = passwordHash,
                    VCHROLE = role // 設定角色
                };

                _Model.AddUser(user); // 添加用戶數據到資料庫
            }

            catch (Exception)
            {

            }
            TempData["MessageType"] = Strings.MessageType_Success;
            TempData["Message"] = Strings.Register_Success_Msg;
            return RedirectToAction("Login"); // 註冊成功後跳轉回登入畫面
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"]; // 將 TempData 的值傳给 ViewBag
                ViewBag.MessageType = TempData["MessageType"];
            }
            return View(); // 顯示登入畫面
        }

        // 使用者登入
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                AccountViewModel user = _Model.GetUserByUsername(username); // 根據名字查找用戶

                if (user != null)
                {
                    string[] storedPasswordHash = user.VCHPASSWORD.Split(':');
                    byte[] salt = Convert.FromBase64String(storedPasswordHash[0]);
                    byte[] expectedHash = Convert.FromBase64String(storedPasswordHash[1]);

                    byte[] passwordBytes = AccountModel.ConcatenateSaltAndPassword(Encoding.UTF8.GetBytes(password), salt);

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] computedHash = sha256.ComputeHash(passwordBytes);

                        if (computedHash.SequenceEqual(expectedHash))
                        {
                            Session["User"] = username;
                            Session["Role"] = user.VCHROLE; // 保存角色

                            return RedirectToAction("Index", "Home"); // 登入成功後跳轉
                        }
                        else
                        {
                            // 登入失敗，設置錯誤消息
                            ViewBag.Message = "Invalid username or password.";
                            ViewBag.MessageType = "danger";
                            return View();
                        }
                    }
                }
                else
                {
                    // 用户名不存在
                    ViewBag.Message = "User not found.";
                    ViewBag.MessageType = "danger";
                    return View();
                }
            }
            catch (FormatException ex)
            {
                // 處理格式化異常，如 Base64 轉換錯誤
                ViewBag.Message = "Invalid data format.";
                ViewBag.MessageType = "danger";
                Console.Error.WriteLine("Format Error: " + ex.Message); // 記錄錯誤
                return View();
            }
            catch (Exception ex)
            {
                // 處理其他通用異常
                ViewBag.Message = "An error occurred while processing your request.";
                ViewBag.MessageType = "danger";
                Console.Error.WriteLine("Error: " + ex.Message); // 記錄錯誤
                return View();
            }
        }
        // 用户登出
        public ActionResult Logout()
        {
            Session.Clear(); // 清除所有 Session
            return RedirectToAction("Login"); // 登出後跳轉
        }


    }
}