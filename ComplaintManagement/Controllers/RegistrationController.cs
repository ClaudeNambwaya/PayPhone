
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
        public class registrationrecord
        {
            public string? name { get; set; }
            public string? email { get; set; }
            public string? password { get; set; }
            public Int64 id { get; set; }
            public Int64 role_id { get; set; }
            public string? mobile { get; set; }

            public string? avatar { get; set; }
            public bool locked { get; set; }
            public string? sec_key { get; set; }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register(registrationrecord record)
        {
            HttpHandler httphandler = new HttpHandler();
            string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            RandomKeyGeneratorManagement randomkeymanager = new RandomKeyGeneratorManagement();
            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");

            if (record.name == null)
                return Content("Invalid user name");

            if (record.mobile == null)
                return Content("Invalid user phone number");

            if (record.email == null)
                return Content("Invalid user email address");

            try
            {
                RegistrationModel existingrecord = dbhandler.GetExternalPortalUsers().Find(mymodel => mymodel.email == record.email);
                if (existingrecord != null)
                {

                    RegistrationModel mymodel = new RegistrationModel
                    {
                        id = existingrecord.id,
                        //    role_id = record.role_id,
                        //    mobile = record.mobile,
                        email = record.email,
                        //    name = record.name
                    };

                    return Json(new { success = true, message = mymodel.email + " is already registered" });

                }

                else
                {
                    RegistrationModel mymodel = new RegistrationModel
                    {

                        role_id = record.role_id,
                        mobile = record.mobile,
                        email = record.email,
                        name = record.name,
                        password = Cryptographer.Encrypt(randomkeymanager.GenerateRandomAlphaNumericString(7)).Replace("=", ""),
                        locked = record.locked,
                        sec_key = randomkeymanager.GenerateRandomAlphaNumericString(16).ToUpper(),
                        avatar = "user.jpg"
                    };

                    if (dbhandler.RegisterUser(mymodel))
                    {

                        string response = "";
                        // CaptureAuditTrail("Created user", "Created user: " + mymodel.name);
                        RandomKeyGeneratorManagement myrandomkeymanager = new RandomKeyGeneratorManagement();
                        FinpayiSecurity.CryptoFactory cryptographyfactory = new FinpayiSecurity.CryptoFactory();
                        string EncryptionAlgorith = "rijndael";
                        FinpayiSecurity.ICrypto cryptographer = cryptographyfactory.MakeCryptographer(EncryptionAlgorith);
                        RegistrationModel externalportaluser = dbhandler.GetExternalPortalUsers().Find(mymodel => mymodel.email == record.email);
                        // CaptureAuditTrail("Approved client user", "Approved client user: " + externalusersmodel.name);

                        //string serverapi = dbhandler.GetRecords("parameters", "SCAPI_API_URL").Rows[0]["item_value"].ToString();
                        //string api_user = dbhandler.GetRecords("parameters", "SCAPI_API_USER").Rows[0]["item_value"].ToString();
                        //string api_password = dbhandler.GetRecords("parameters", "SCAPI_API_PASSWORD").Rows[0]["item_value"].ToString();
                        //string mail_cred_subject = dbhandler.GetRecords("parameters", "CLIENT_MAIL_CREDENTIALS_SUBJECT").Rows[0]["item_value"].ToString();
                        //string portal_url = dbhandler.GetRecords("parameters", "CLIENT_PORTAL_URL").Rows[0]["item_value"].ToString();

                        //JObject token_data = new JObject
                        //			{
                        //				{
                        //					"message_validation",
                        //					new JObject
                        //					{
                        //						{ "api_user", cryptographer.Base64Encode(api_user) },
                        //						{ "api_password", cryptographer.Base64Encode(api_password) }
                        //					}
                        //				},
                        //				{
                        //					"message_route",
                        //					new JObject
                        //					{
                        //						{ "interface", "TOKEN" }
                        //					}
                        //				}
                        //			};

                        //string token_resp_data = httphandler.HttpClientPost(serverapi, token_data);

                        //JObject token_resp_data_json = JObject.Parse(token_resp_data);

                        //JObject email_data = new JObject
                        //			{
                        //				{
                        //					"message_validation",
                        //					new JObject
                        //					{
                        //						{ "api_user", "-" },
                        //						{ "api_password", "-" },
                        //						{ "token", cryptographer.Base64Encode(token_resp_data_json["error_desc"]["token"].ToString()) }
                        //					}
                        //				},
                        //				{
                        //					"message_route",
                        //					new JObject
                        //					{
                        //						{ "interface", "SANDBOX_EMAIL" },
                        //						{ "request_type", "data_ghala_client_credentials" },
                        //						{"external_ref_number", external_ref_num }
                        //					}
                        //				},
                        //				{
                        //					"message_body",
                        //					new JObject
                        //					{
                        //						{ "subject", mail_cred_subject },
                        //						{ "customer", externalportaluser.name },
                        //						{ "email_address", externalportaluser.email },
                        //						{ "user_password", cryptographer.Decrypt(externalportaluser.password + "==") },
                        //						{ "portal_url", portal_url },
                        //						{ "attachment", "" }
                        //					}
                        //				}

                        //			};



                        //string email_resp_data = httphandler.HttpClientPost(serverapi, email_data);

                        //JObject email_resp_data_json = JObject.Parse(email_resp_data);

                        //iloggermanager.LogInfo(email_resp_data_json.ToString());

                        //if (email_resp_data_json["error_code"].ToString().Equals("00"))
                        //{
                        //	response = "Success-";
                        //	CaptureAuditTrail(record.email, "Successful password reset", "Successfully sent password reset to email: " + record.email);
                        //	//Success("New password has been sent to your email address", true);
                        //}
                        //else if (email_resp_data_json["error_code"].ToString() == "00" && email_resp_data_json["error_desc"][0]["response_code"].ToString() != "00")
                        //{
                        //	response = email_resp_data_json["error_desc"][0]["response_desc"].ToString();
                        //	CaptureAuditTrail(record.email, "Error on password reset", response + " on password reset to email: " + record.email);
                        //	Danger(response, true);
                        //}
                        //else
                        //{
                        //	response = "Operation could not be completed, kindly contact system admin";
                        //	CaptureAuditTrail(record.email, "Error on password reset", response + " on password reset to email: " + record.email);
                        //	Danger(response, true);
                        //}

                        //CaptureAuditTrail("Created user", "Created user: " + mymodel.name);

                        return Json(new { success = true, message = " New password has been sent to your email address " + record.name });
                    }

                    else
                        return Content("Could not create user, kindly contact system admin");
                }
            }


            catch
            {
                return Content("Could not create user, kindly contact system admin");
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
    }
}
