using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
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
        [HttpPost]
        public ActionResult Authenticate(LoginModel loginModel)
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
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}