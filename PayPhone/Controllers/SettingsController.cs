using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using System.Collections;
using System.Data;
using static ComplaintManagement.Helpers.CryptoHelper;
using static ComplaintManagement.Helpers.HttpClientHelper;

namespace ComplaintManagement.Controllers
{
    //[CheckAuthorization]
    public class SettingsController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public SettingsController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class parameterrecord
        {
            public int id { get; set; }
            public string? item_key { get; set; }
            public string? item_value { get; set; }
            public string? remarks { get; set; }
        }

        public class email_template_record
        {
            public Int64 id { get; set; }
            public string? request_type { get; set; }
            public string? email_template { get; set; }
            public string? logo { get; set; }
            public string? remarks { get; set; }
        }

        public class email_template_param_record
        {
            public Int64 id { get; set; }
            public Int64 request_type { get; set; }
            public int param_order { get; set; }
            public string? param_name { get; set; }
        }

        [RBAC]
        public ActionResult Parameters()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
                MenuHandler menuhandler = new MenuHandler(dbhandler);
                IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
                return View(menu);
            }
        }

        [RBAC]
        public ActionResult EmailTemplates()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
                MenuHandler menuhandler = new MenuHandler(dbhandler);
                IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
                return View(menu);
            }
        }

        [RBAC]
        public ActionResult EmailTemplateParams()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
                MenuHandler menuhandler = new MenuHandler(dbhandler);
                IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
                return View(menu);
            }
        }

        [HttpGet]
        public ContentResult GetRecords(string module)
        {
            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");
            ArrayList details = new ArrayList();
            DataTable datatable = new DataTable();
            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            datatable = dbhandler.GetRecords(module);

            if (datatable.Rows.Count > 0)
            {
                Dictionary<string, object> row;
                foreach (DataRow dr in datatable.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in datatable.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
            }
            return Content(JsonConvert.SerializeObject(rows, Formatting.Indented) /*serializer.Serialize(rows)*/, "application/json");
        }

        [RBAC]
        public ActionResult CreateParameter(parameterrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.item_key == null)
                    return Content("Invalid key");

                if (record.item_value == null)
                    return Content("Invalid value");

                try
                {
                    ParametersModel existingrecord = dbhandler.GetParameters().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        ParametersModel mymodel = new ParametersModel
                        {
                            id = existingrecord.id,
                            item_key = record.item_key,
                            item_value = record.item_value,
                            comments = record.remarks
                        };

                        if (dbhandler.UpdateParameter(mymodel))
                        {
                            CaptureAuditTrail("Updated parameter", "Updated parameter: " + mymodel.item_key);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update parameter, kindly contact system admin");
                    }
                    else
                    {
                        ParametersModel mymodel = new ParametersModel
                        {
                            item_key = record.item_key,
                            item_value = record.item_value,
                            comments = record.remarks,
                            created_by = Convert.ToInt16(HttpContext.Session.GetString("userid"))
                        };

                        if (dbhandler.AddParameter(mymodel))
                        {
                            CaptureAuditTrail("Created parameter", "Created parameter: " + mymodel.item_key);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create parameter, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create parameter, kindly contact system admin");
                }
            }
        }

        [RBAC]
        public ActionResult Delete(/*[FromBody] JObject jobject*/ int id, string module)
        {
            //Int32 id = Convert.ToInt32(jobject["id"]);
            //string module = jobject["module"].ToString();

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "Home");
            else
            {
                switch (module)
                {
                    case "parameters":
                        ParametersModel parametersmodel = dbhandler.GetParameters().Find(mymodel => mymodel.id == id)!;
                        if (parametersmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            CaptureAuditTrail("Deleted parameter", "Deleted parameter: " + parametersmodel.item_key);
                        }
                        break;

                    default:
                        break;
                }

                return GetRecords(module);
            }
        }

        [RBAC]
        public ActionResult RegisterMpesaURLs([FromBody] JObject jobject /*int id*/)
        {
            HttpHandler httphandler = new HttpHandler();
            string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string response = "";
            Int32 id = Convert.ToInt32(jobject["id"]);

            try
            {
                string serverapi = dbhandler.GetRecords("parameters", "SCAPI_API_URL").Rows[0]["item_value"].ToString()!;
                string api_user = dbhandler.GetRecords("parameters", "SCAPI_API_USER").Rows[0]["item_value"].ToString()!;
                string api_password = dbhandler.GetRecords("parameters", "SCAPI_API_PASSWORD").Rows[0]["item_value"].ToString()!;

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

                JObject request_data = new JObject
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
                            { "interface", "MPESA_G2" },
                            { "request_type", "REGISTER_URL" },
                            { "external_ref_number", external_ref_num }
                        }
                    },
                    // message_body
                    {
                        "message_body",
                        new JObject
                        {
                            { "business_short_code", dbhandler.GetRecords("mpesa_business_short_codes", Convert.ToString(id)).Rows[0]["mpesa_business_shortcode"].ToString() }
                        }
                    }
                };

                string dits_resp_data = httphandler.HttpClientPost(serverapi, request_data);

                JObject dits_resp_data_json = JObject.Parse(dits_resp_data);

                if (dits_resp_data_json["error_code"]!.ToString() == "00" && dits_resp_data_json["error_desc"]!["ResponseDescription"]!.ToString() == "success")
                    response = "Successfull URL registration";
                else if (dits_resp_data_json["error_code"]!.ToString() == "00" && dits_resp_data_json["error_desc"]!["ResponseDescription"]!.ToString() != "00")
                    response = dits_resp_data_json["error_desc"]!["ResponseDescription"]!.ToString();
                else
                    response = "Transaction could not be processed, kindly contact admin";
            }
            catch
            {
                response = "Could not register mpesa URLs, kindly contact system admin";
            }

            //return Json(response, JsonRequestBehavior.AllowGet);
            return Content(JsonConvert.SerializeObject(response, Formatting.Indented) /*serializer.Serialize(rows)*/, "application/json");
        }

        public bool CaptureAuditTrail(string action_type, string action_description)
        {
            AuditTrailModel audittrailmodel = new AuditTrailModel
            {
                user_name = HttpContext.Session.GetString("email")!.ToString(),
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