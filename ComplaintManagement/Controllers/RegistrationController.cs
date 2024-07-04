
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using ComplaintManagement.Controllers;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using static ComplaintManagement.Helpers.CryptoHelper;
using static ComplaintManagement.Helpers.HttpClientHelper;
using static Grpc.Core.Metadata;
using static ComplaintManagement.Controllers.RegistrationController;
using ComplaintManagement.Models.Models;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using PayPhone.Dtos;

namespace ComplaintManagement.Controllers
{
    public class RegistrationController : BaseController
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public RegistrationController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            dbhandler = mydbhandler;
            ihostingenvironment = environment;
        }

        
        public class Portaluserrecord
        {
            public string? user_name { get; set; }
            public string? name { get; set; }
            public string? email { get; set; }
            public string? password { get; set; }
            public Int64 id { get; set; }
            public Int64 role_id { get; set; }
            public string? mobile { get; set; }
            public string? role_name { get; set; }
            public string? avatar { get; set; }
            public string? menu_layout { get; set; }
            public bool locked { get; set; }
            public string? sec_key { get; set; }
            public DateTime created_on { get; set; }
            public DateTime updated_at { get; set; }
            public virtual Int32 created_by { get; set; }
            public bool approved { get; set; }
            public Int64 balance { get; set; }
        }

        

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register([FromBody] Portaluserrecord record)
        {
            iloggermanager.LogInfo("Starting registration process for email: " + record.email);

            HttpHandler httphandler = new HttpHandler();
            string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            RandomKeyGeneratorManagement randomkeymanager = new RandomKeyGeneratorManagement();
            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");

            if (string.IsNullOrEmpty(record.name))
            {
                iloggermanager.LogError("Invalid user name");
                return Json(new { success = false, message = "Invalid user name" });
            }

            if (string.IsNullOrEmpty(record.email))
            {
                iloggermanager.LogError("Invalid user email address");
                return Json(new { success = false, message = "Invalid user email address" });
            }

            // Check if the email already exists
            var existingUser = dbhandler.GetPortalUsers().FirstOrDefault(user => user.email == record.email);
            if (existingUser != null)
            {
                iloggermanager.LogInfo($"Email {record.email} is already registered.");
                return Json(new { success = false, message = $"The email '{record.email}' is already registered." });
            }

            try
            {
                string encryptedPassword = Cryptographer.Encrypt(record.password!).Replace("=", "");

                PortalUsersModel mymodel = new PortalUsersModel
                {
                    role_id = 2, // Assign role_id to 2
                    user_name = record.user_name,
                    mobile = record.mobile!,
                    email = record.email,
                    name = record.name,
                    menu_layout = "Side",
                    password = encryptedPassword,
                    locked = record.locked,
                    sec_key = randomkeymanager.GenerateRandomAlphaNumericString(16).ToUpper(),
                    avatar = "221020200740261.jpg",
                    google_authenticate = true,
                    created_by = 1,
                    created_on = record.created_on,
                    updated_at = record.updated_at,
                    role_name = "CLIENT"
                };

                if (dbhandler.AddPortalUser(mymodel))
                {
                    // Fetch the newly registered user from the database
                    var newUser = dbhandler.GetPortalUsers().FirstOrDefault(user => user.email == record.email);
                    iloggermanager.LogInfo($"Registration successful for email: {record.email}");

                    var userDto = new RegistrationUserDto
                    {
                        Name = record.name,
                        UserName = record.user_name,
                        Email = record.email,
                        RoleName = mymodel.role_name,
                        Mobile = record.mobile,
                        Password = encryptedPassword, // Return the encrypted password
                        Locked = record.locked,
                        CreatedOn = record.created_on,
                        UpdatedAt = record.updated_at
                    };

                    return Json(new
                    {
                        success = true,
                        message = "Your registration has been successful " + record.name,
                        user = userDto
                    });
                }
                else
                {
                    iloggermanager.LogError("Could not create user, kindly contact system admin");
                    return Json(new { success = false, message = "Could not create user, kindly contact system admin" });
                }
            }
            catch (Exception ex)
            {
                iloggermanager.LogError("Exception occurred: " + ex.Message);
                return Json(new { success = false, message = "Could not create user, kindly contact system admin" });
            }
        }


        //[HttpPost]

        //public ActionResult Register([FromBody] Portaluserrecord record)
        //{
        //    iloggermanager.LogInfo("Starting registration process for email: " + record.email);

        //    HttpHandler httphandler = new HttpHandler();
        //    string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //    RandomKeyGeneratorManagement randomkeymanager = new RandomKeyGeneratorManagement();
        //    FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
        //    FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");

        //    if (string.IsNullOrEmpty(record.name))
        //    {
        //        iloggermanager.LogError("Invalid user name");
        //        return Json(new { success = false, message = "Invalid user name" });
        //    }

        //    if (string.IsNullOrEmpty(record.email))
        //    {
        //        iloggermanager.LogError("Invalid user email address");
        //        return Json(new { success = false, message = "Invalid user email address" });
        //    }

        //    // Check if the email already exists
        //    var existingUser = dbhandler.GetPortalUsers().FirstOrDefault(user => user.email == record.email);
        //    if (existingUser != null)
        //    {
        //        iloggermanager.LogInfo($"Email {record.email} is already registered.");
        //        return Json(new { success = false, message = $"The email '{record.email}' is already registered." });
        //    }

        //    try
        //    {
        //        PortalUsersModel mymodel = new PortalUsersModel
        //        {
        //            role_id = 2, // Assign role_id to 2
        //            user_name = record.user_name,
        //            mobile = record.mobile!,
        //            email = record.email,
        //            name = record.name,
        //            menu_layout = "Side",
        //            password = Cryptographer.Encrypt(record.password!).Replace("=", ""),
        //            locked = record.locked,
        //            sec_key = randomkeymanager.GenerateRandomAlphaNumericString(16).ToUpper(),
        //            avatar = "221020200740261.jpg",
        //            google_authenticate = true,
        //            created_by = 1,
        //            created_on = record.created_on,
        //            updated_at = record.updated_at
        //        };

        //        var userDto = new RegistrationUserDto
        //        {

        //            Name = record.name,
        //            UserName = record.user_name,
        //            Email = record.email,
        //            RoleName = record.role_name,
        //            Mobile = record.mobile,
        //            Password = record.password,
        //            Locked = record.locked,
        //            CreatedOn = record.created_on,
        //            UpdatedAt = record.updated_at
        //        };


        //        if (dbhandler.AddPortalUser(mymodel))
        //        {
        //            // Fetch the newly registered user from the database
        //            var newUser = dbhandler.GetPortalUsers().FirstOrDefault(user => user.email == record.email);
        //            iloggermanager.LogInfo($"Registration successful for email: {record.email}");

        //            return Json(new
        //            {
        //                success = true,
        //                message = "Your registration has been successful " + record.name,
        //                user = newUser
        //            });
        //        }
        //        else
        //        {
        //            iloggermanager.LogError("Could not create user, kindly contact system admin");
        //            return Json(new { success = false, message = "Could not create user, kindly contact system admin" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        iloggermanager.LogError("Exception occurred: " + ex.Message);
        //        return Json(new { success = false, message = "Could not create user, kindly contact system admin" });
        //    }
        //}



        public bool CaptureAuditTrail(string user, string action_type, string action_description)
        {
            AuditTrailModel audittrailmodel = new AuditTrailModel
            {
                user_name = user,
                action_type = action_type,
                action_description = action_description,
                page_accessed = String.Format("{0}://{1}{2}{3}", HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.Path, HttpContext.Request.QueryString), /*Request.Url.ToString(),*/
                client_ip_address = Request.HttpContext.Connection.RemoteIpAddress!.ToString(), /*Request.UserHostAddress,*/
                session_id = HttpContext.Session.GetString("userid") /*Session.SessionID*/
            };
            return dbhandler.AddAuditTrail(audittrailmodel);
        }
    }
}
