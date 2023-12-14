using System.Data;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using static ComplaintManagement.Helpers.CryptoHelper;
using static ComplaintManagement.Helpers.HttpClientHelper;
using Newtonsoft.Json.Linq;

namespace ComplaintManagement.Controllers
{
    public class AppAuthController : BaseController
    {
        private static int saltLengthLimit = 32;
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public AppAuthController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        #region LaunchPad GET Method 
        [HttpGet]
        public ActionResult LaunchPad(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Pre Required Method For Login
        //GET: EnsureLoggedOut
        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            //if (Request.IsAuthenticated)
            if (HttpContext.User.Identity!.IsAuthenticated)
                Logout();
        }

        //POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            try
            {
                // First we clean the authentication ticket like always
                //required NameSpace: using System.Web.Security;
                //FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                //required NameSpace: using System.Security.Principal;
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                HttpContext.Session.Clear();
                //System.Web.HttpContext.Current.Session.RemoveAll();

                // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
                // this clears the Request.IsAuthenticated flag since this triggers a new request
                return RedirectToLocal();
            }
            catch
            {
                throw;
            }
        }

        //GET: RedirectToLocal
        private ActionResult RedirectToLocal(string returnURL = "")
        {
            try
            {
                // If the return url starts with a slash "/" we assume it belongs to our site
                // so we will redirect to this "action"
                if (!string.IsNullOrWhiteSpace(returnURL) && Url.IsLocalUrl(returnURL))
                    return Redirect(returnURL);

                // If we cannot verify if the url is local to our host we redirect to a default location
                return RedirectToAction("LaunchPad", "Home");
            }
            catch
            {
                throw;
            }
        }

        //GET: SignInAsync   
        private void SignInRemember(string userName, bool isPersistent = false)
        {
            // Clear any lingering authencation data
            //FormsAuthentication.SignOut();

            // Write the authentication cookie
            //FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }

        #endregion

        #region Admin Login GET Method 
        [HttpGet]
        public ActionResult AdminLogin(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Admin Login POST Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(LoginModel entity)
        {
            try
            {
                string EnctryptionAlgorith = "rijndael";

                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

                if (!ModelState.IsValid)
                    return View(entity);

                //bool isLogin = false; //CompareHashValue(entity.password, entity.email, OldHASHValue, SALT);
                //check licence validity
                string expdateenc = string.Empty;
                string filepath = ihostingenvironment.WebRootPath;
                string fullpath = Path.Combine(filepath, "License.txt");
                using (StreamReader reader = new StreamReader(fullpath))
                {
                    expdateenc = reader.ReadToEnd();
                }
                string expdate = Cryptographer.Decrypt(expdateenc);
                DateTime date1 = DateTime.Parse(expdate);
                DateTime date2 = DateTime.Now;

               

                Int32 daysuntilexpiry = date1.Subtract(date2).Days;
                if (daysuntilexpiry <= 5)
                {
                    if (daysuntilexpiry <= 0)
                    {
                        Danger("The system license has expired, contact administrator", true);
                        return View(entity);
                    }
                }
                string usertype = "";
                DataTable dt = new DataTable();
                dt = dbhandler.ValidateUserLogin("ADMIN", entity.email);
                if (dt.Rows.Count > 0)
                {
                    usertype = "ADMIN";

                    if (Cryptographer.Encrypt(entity.password).Replace("=", "") == dt.Rows[0]["password"].ToString())
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["locked"]))
                        {
                            CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Account is locked for user: " + entity.email);
                            Danger("Access Denied! Account is locked, contact administrator", true);
                            return View(entity);
                        }
                        else if (!Convert.ToBoolean(dt.Rows[0]["approved"]))
                        
                        {
                            CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Account is not approved for user: " + entity.email);
                            Danger("Access Denied! Account is not approved, contact administrator", true);
                            return View(entity);
                        }
                        else
                        {
                            //Login Success
                            HttpContext.Session.SetString("userid", dt.Rows[0]["id"].ToString()!);
                            HttpContext.Session.SetString("email", dt.Rows[0]["email"].ToString()!);
                            HttpContext.Session.SetString("avatar", dt.Rows[0]["avatar"].ToString()!);
                            HttpContext.Session.SetString("name", dt.Rows[0]["name"].ToString()!);
                            HttpContext.Session.SetString("profileid", dt.Rows[0]["role_id"].ToString()!);
                            HttpContext.Session.SetString("menulayout", dt.Rows[0]["menu_layout"].ToString()!);
                            HttpContext.Session.SetString("userType", usertype);

                            //For Set Authentication in Cookie (Remeber ME Option)
                            SignInRemember(entity.email, entity.isRemember);

                            CaptureAuditTrail(entity.email, "Successful Login", "Successfully logged in with user: " + HttpContext.Session.GetString("email"));

                            return RedirectToAction("Index", "Dashboard");
                            //return RedirectToLocal(entity.ReturnURL);
                        }
                    }
                    else
                    {
                        //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                        CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                        Danger("Access Denied! Wrong Credentials", true);
                        return View(entity);
                    }
                }
                else
                {
                    dt = dbhandler.ValidateUserLogin("CLIENT", entity.email);

                    if (dt.Rows.Count > 0)
                    {

                        usertype = "CLIENT";
                        var password = Cryptographer.Encrypt(entity.password);
                        var pass = dt.Rows[0]["password"].ToString();

                        if (Cryptographer.Encrypt(entity.password).Replace("=", "") == dt.Rows[0]["password"].ToString())
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["locked"]))
                            {
                                CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Account is locked for user: " + entity.email);
                                Danger("Access Denied! Account is locked, contact administrator", true);
                                return View(entity);
                            }
                            else if (!Convert.ToBoolean(dt.Rows[0]["approved"]))

                            {
                                CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Account is not approved for user: " + entity.email);
                                Danger("Access Denied! Account is not approved, contact administrator", true);
                                return View(entity);
                            }
                            else
                            {
                                //Login Success
                                HttpContext.Session.SetString("userid", dt.Rows[0]["id"].ToString()!);
                                HttpContext.Session.SetString("email", dt.Rows[0]["email"].ToString()!);
                                HttpContext.Session.SetString("avatar", dt.Rows[0]["avatar"].ToString()!);
                                HttpContext.Session.SetString("name", dt.Rows[0]["name"].ToString()!);
                                HttpContext.Session.SetString("profileid", dt.Rows[0]["role_id"].ToString()!);
                                HttpContext.Session.SetString("menulayout", dt.Rows[0]["menu_layout"].ToString()!);
                                HttpContext.Session.SetString("userType", usertype);


                                //For Set Authentication in Cookie (Remeber ME Option)
                                SignInRemember(entity.email, entity.isRemember);

                                CaptureAuditTrail(entity.email, "Successful Login", "Successfully logged in with user: " + HttpContext.Session.GetString("email"));

                                return RedirectToAction("Client", "Dashboard");
                                //return RedirectToLocal(entity.ReturnURL);
                            }
                        }
                        else
                        {
                            //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                            CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                            Danger("Access Denied! Wrong Credentials", true);
                            return View(entity);
                        }
                    }

                    else
                    {
                        //Login Fail
                        //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                        CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                        Danger("Access Denied! Wrong Credentials", true);
                        return View(entity);
                    }

                   
                }
            }
            catch (Exception ex)
            {
                iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
                //TempData["ErrorMSG"] = "System encountered issue while trying to authenticate";
                CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! System encountered issue while trying to authenticate user: " + entity.email);
                Danger("System encountered issue while trying to authenticate", true);
                return View(entity);
            }

        }

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

        #endregion

        #region Admin Reset Password GET Method 
        [HttpGet]
        public ActionResult ResetPassword(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch (Exception ex)
            {
                iloggermanager.LogError(ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region Admin Reset Password POST Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(LoginModel entity)
        {
            try
            {
                string response = "";
                HttpHandler httphandler = new HttpHandler();
                string EnctryptionAlgorith = "rijndael";
                string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

                //if (!ModelState.IsValid)
                //    return View(entity);

                //bool isLogin = false; //CompareHashValue(entity.password, entity.email, OldHASHValue, SALT);
                DataTable dt = new DataTable();
                dt = dbhandler.ValidateUserLogin("CLIENT", entity.email);
                if (dt.Rows.Count > 0)
                {
                    RandomKeyGeneratorManagement myrandomkeymanager = new RandomKeyGeneratorManagement();

                    PortalUsersModel existingrecord = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == Convert.ToInt32(dt.Rows[0]["id"]))!;
                    if (existingrecord != null)
                    {
                        PortalUsersModel mymodel = new PortalUsersModel
                        {
                            id = existingrecord.id,
                            role_id = existingrecord.role_id,
                            mobile = existingrecord.mobile,
                            email = existingrecord.email,
                            name = existingrecord.name
                        };

                        mymodel.password = Cryptographer.Encrypt(myrandomkeymanager.GenerateRandomAlphaNumericString(7)).Replace("=", "");
                        mymodel.locked = existingrecord.locked;
                        mymodel.google_authenticate = existingrecord.google_authenticate;
                        mymodel.sec_key = existingrecord.sec_key;
                        mymodel.avatar = existingrecord.avatar;

                        if (dbhandler.UpdatePortalUser(mymodel))
                        {
                            //make call to SCAPI
                            //make token request
                            string serverapi = dbhandler.GetRecords("parameters", "SCAPI_API_URL").Rows[0]["item_value"].ToString()!;
                            string api_user = dbhandler.GetRecords("parameters", "SCAPI_API_USER").Rows[0]["item_value"].ToString()!;
                            string api_password = dbhandler.GetRecords("parameters", "SCAPI_API_PASSWORD").Rows[0]["item_value"].ToString()!;
                            string mail_cred_subject = dbhandler.GetRecords("parameters", "MAIL_CREDENTIALS_SUBJECT").Rows[0]["item_value"].ToString()!;
                            string portal_url = dbhandler.GetRecords("parameters", "BACKOFFICE_PORTAL_URL").Rows[0]["item_value"].ToString()!;

                            JObject token_data = new JObject
                            {
                                // message_validation
                                {
                                    "message_validation",
                                    new JObject
                                    {
                                        { "api_user", api_user },
                                        { "api_password", api_password }
                                    }
                                },
                                // message_route
                                {
                                    "message_route",
                                    new JObject
                                    {
                                        { "interface", "TOKEN" }
                                    }
                                }
                            };

                            string token_resp_data = httphandler.HttpClientPost(serverapi, token_data);

                            JObject token_resp_data_json = JObject.Parse(token_resp_data);

                            JObject email_data = new JObject
                            {
                                // message_validation
                                {
                                    "message_validation",
                                    new JObject
                                    {
                                        { "api_user", api_user },
                                        { "api_password", api_password },
                                        { "token", token_resp_data_json["error_desc"]!["token"]!.ToString() },
                                    }
                                },
                                // message_route
                                {
                                    "message_route",
                                    new JObject
                                    {
                                        { "interface", "EMAIL" },
                                        { "request_type", "backoffice_credentials" },
                                        { "external_ref_number", external_ref_num }
                                    }
                                },
                                // message_body
                                {
                                    "message_body",
                                    new JObject
                                    {
                                        { "subject", mail_cred_subject },
                                        { "customer", existingrecord.name },
                                        { "email_address", existingrecord.email },
                                        { "user_password", Cryptographer.Decrypt(mymodel.password + "==") },
                                        { "portal_url", portal_url },
                                        { "attachment", "" }
                                    }
                                }
                            };

                            iloggermanager.LogInfo(email_data.ToString());

                            string email_resp_data = httphandler.HttpClientPost(serverapi, email_data);

                            JObject email_resp_data_json = JObject.Parse(email_resp_data);

                            iloggermanager.LogInfo(email_resp_data_json.ToString());

                            if (email_resp_data_json["error_code"]!.ToString() == "00" && email_resp_data_json["error_desc"]![0]!["response_code"]!.ToString() == "00")
                            {
                                response = "Success-" + external_ref_num.Substring(2, 12);
                                CaptureAuditTrail(entity.email, "Successful password reset", "Successfully sent password reset to email: " + entity.email);
                                Success("New password has been sent to your email address", true);
                            }
                            else if (email_resp_data_json["error_code"]!.ToString() == "00" && email_resp_data_json["error_desc"]![0]!["response_code"]!.ToString() != "00")
                            {
                                response = email_resp_data_json["error_desc"]![0]!["response_desc"]!.ToString();
                                CaptureAuditTrail(entity.email, "Error on password reset", response + " on password reset to email: " + entity.email);
                                Danger(response, true);
                            }
                            else
                            {
                                response = "Operation could not be completed, kindly contact system admin";
                                CaptureAuditTrail(entity.email, "Error on password reset", response + " on password reset to email: " + entity.email);
                                Danger(response, true);
                            }
                            //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                            ModelState.Clear();
                        }
                        else
                        {
                            Danger("Could not reset password, kindly contact system admin");
                        }
                    }
                    else
                    {
                        //Login Fail
                        //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                        CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                        Danger("Access Denied! Wrong Credentials", true);
                    }
                }
                else
                {
                    Danger("Unknown user");
                }
                return View(entity);
            }
            catch (Exception ex)
            {
                iloggermanager.LogError(ex.StackTrace);
                //TempData["ErrorMSG"] = "System encountered issue while trying to authenticate";
                CaptureAuditTrail(entity.email, "Invalid Login", "Access denied! System encountered issue while trying to authenticate user: " + entity.email);
                Danger("System encountered issue while trying to authenticate", true);
                return View(entity);
            }

        }
        #endregion 

        #region --> Generate SALT Key

        private static byte[] Get_SALT()
        {
            return Get_SALT(saltLengthLimit);
        }

        private static byte[] Get_SALT(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];

            //Require NameSpace: using System.Security.Cryptography;
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }

        #endregion

        #region --> Generate HASH Using SHA512
        public static string Get_HASH_SHA512(string password, string username, byte[] salt)
        {
            try
            {
                //required NameSpace: using System.Text;
                //Plain Text in Byte
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(password + username);

                //Plain Text + SALT Key in Byte
                byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + salt.Length];

                for (int i = 0; i < plainTextBytes.Length; i++)
                {
                    plainTextWithSaltBytes[i] = plainTextBytes[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    plainTextWithSaltBytes[plainTextBytes.Length + i] = salt[i];
                }

                HashAlgorithm hash = new SHA512Managed();
                byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
                byte[] hashWithSaltBytes = new byte[hashBytes.Length + salt.Length];

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashWithSaltBytes[i] = hashBytes[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    hashWithSaltBytes[hashBytes.Length + i] = salt[i];
                }

                return Convert.ToBase64String(hashWithSaltBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region --> Comapare HASH Value
        public static bool CompareHashValue(string password, string username, string OldHASHValue, byte[] SALT)
        {
            try
            {
                string expectedHashString = Get_HASH_SHA512(password, username, SALT);

                return (OldHASHValue == expectedHashString);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        //Refer(SALT): http://codereview.stackexchange.com/questions/93614/salt-generation-in-c  
        //Refer(SHA512): http://zurb.com/forrst/posts/C_SHA512_Encryption_Hashing-UaL
        //Refer 1:  https://www.mssqltips.com/sqlservertip/4037/storing-passwords-in-a-secure-way-in-a-sql-server-database/
        //Refer 2: https://crackstation.net/hashing-security.htm

    }
}