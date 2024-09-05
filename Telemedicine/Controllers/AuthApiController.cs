using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Telemedicine.Models;
using Telemedicine.Viewmodels;

namespace Telemedicine.Controllers
{
    public class AuthApiController : Controller
    {
        private readonly AccountModel _Model;

        public AuthApiController()
        {
            _Model = new AccountModel();
        }
        
        public ActionResult Authenticate_App(LoginModel loginModel)
        {
            try
            {
                AccountViewModel user = _Model.GetUserByUsername(loginModel.Username);

                if (user != null)
                {
                    string[] storedPasswordHash = user.VCHPASSWORD.Split(':');
                    byte[] salt = Convert.FromBase64String(storedPasswordHash[0]);
                    byte[] expectedHash = Convert.FromBase64String(storedPasswordHash[1]);

                    byte[] passwordBytes = AccountModel.ConcatenateSaltAndPassword(Encoding.UTF8.GetBytes(loginModel.Password), salt);

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] computedHash = sha256.ComputeHash(passwordBytes);

                        if (computedHash.SequenceEqual(expectedHash))
                        {
                            return Json(new { success = true, message = "Login successful" });
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid username or password.");
                        }
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User not found.");
                }
            }
            catch (FormatException ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Invalid data format. " + ex.Message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request. " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Authenticate(LoginModel loginModel, string connectionId)
        {
            try
            {
                AccountViewModel user = _Model.GetUserByUsername(loginModel.Username);

                if (user != null)
                {
                    string[] storedPasswordHash = user.VCHPASSWORD.Split(':');
                    byte[] salt = Convert.FromBase64String(storedPasswordHash[0]);
                    byte[] expectedHash = Convert.FromBase64String(storedPasswordHash[1]);

                    byte[] passwordBytes = AccountModel.ConcatenateSaltAndPassword(Encoding.UTF8.GetBytes(loginModel.Password), salt);

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] computedHash = sha256.ComputeHash(passwordBytes);

                        if (computedHash.SequenceEqual(expectedHash))
                        {
                            var context = GlobalHost.ConnectionManager.GetHubContext<AuthHub>();
                            context.Clients.Client(connectionId).authSuccess();
                            // 驗證成功，設置 Session
                            HttpContext.Session["User"] = loginModel.Username;
                            HttpContext.Session["Role"] = loginModel.Role;

                            foreach (string key in HttpContext.Session.Keys)
                            {
                                string value = HttpContext.Session[key]?.ToString();
                                Debug.WriteLine($"{key}: {value}");
                            }



                            return Json(new { success = true, message = "Login successful" });
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid username or password.");
                        }
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User not found.");
                }
            }
            catch (FormatException ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Invalid data format. " + ex.Message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request. " + ex.Message);
            }
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}