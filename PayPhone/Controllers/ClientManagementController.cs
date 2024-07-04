using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using System.Collections;
using static ComplaintManagement.Helpers.CryptoHelper;
using static ComplaintManagement.Helpers.HttpClientHelper;
using System.Data;
using static ComplaintManagement.Controllers.AccessControlController;

namespace ComplaintManagement.Controllers
{
    public class ClientManagementController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public ClientManagementController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class onboardrecord
        {
            public onboardclientrecord[]? applicant_details { get; set; }
            public string? client_files { get; set; }
        }

        public class onboardclientrecord
        {
            public Int64 id { get; set; }
            public string? client_type { get; set; }
            public string? client_name { get; set; }
            public string? phone_number { get; set; }
            public string? email { get; set; }
            public string? id_number { get; set; }
            public string? kra_pin { get; set; }
            public string? physical_address { get; set; }
            public string? postal_address { get; set; }
            public string? industry { get; set; }
            public string? remarks { get; set; }
        }
        public class processingresponse
        {
            public string? code { get; set; }
            public string? desc { get; set; }
            public string? system_ref { get; set; }
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Onboard()
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
        public IActionResult OnboardUnapproved()
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
        public IActionResult OnboardList()
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
        public ActionResult OnboardClient(onboardrecord record)
        {
            ArrayList details = new ArrayList();
            processingresponse response = new processingresponse
            {
                system_ref = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            try
            {
                if (HttpContext.Session.GetString("userid") == null)
                    return RedirectToAction("AdminLogin", "AppAuth");
                else
                {
                    var user = HttpContext.Session.GetString("userid");
                    //1. create primary registration
                    ClientRecordModel clientModel = new ClientRecordModel
                    {
                        client_type = record.applicant_details![0].client_type,
                        client_name = record.applicant_details![0].client_name,
                        phone_number = record.applicant_details[0].phone_number,
                        email = record.applicant_details[0].email,
                        id_number = record.applicant_details[0].id_number,
                        kra_pin = record.applicant_details[0].kra_pin,
                        physical_address = record.applicant_details[0].physical_address,
                        postal_address = record.applicant_details[0].postal_address,
                        industry = record.applicant_details[0].industry,
                        remarks = record.applicant_details[0].remarks,
                        created_by = Convert.ToInt64(user)
                    };

                    Int64 client_id = dbhandler.AddClient(clientModel);


                    if (client_id > 0)
                    {
                        JArray jarray = JArray.FromObject(record.applicant_details);

                        //2. Create customer files records

                        // PatientRecordModel personal_rec = dbhandler.GetPatientRecord()!.Find(model => model.id!.Equals(patient_id))!;


                        //var personal_rec = dbhandler.GetRecordsById("account_no", patient_id);

                        string[] client_files = record.client_files!.Split('|');
                        //remove last item
                        //client_files = client_files.Take(client_files.Count() - 1).ToArray();

                        for (int i = 0; i < client_files.Length; i++)
                        {
                            ClientFilesModel filesmodel = new ClientFilesModel
                            {
                                client_id = client_id,
                                file_name = client_files[i],
                            };

                            if (dbhandler.AddClientFiles(filesmodel))
                            {
                                ModelState.Clear();
                                response.code = "00";
                                response.desc = "Registration was success, you can proceed";
                            }
                            else
                            {
                                dbhandler.DeleteRecord(client_id, Convert.ToInt16(HttpContext.Session.GetString("userid")), "patient_register_fail_delete");
                                ModelState.Clear();
                                response.code = "01";
                                response.desc = "File Upload Failed , kindly contact system admin";
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                iloggermanager.LogError(ex.Message);
                response.code = "01";
                response.desc = "Could not create client, kindly contact system admin";
            }

            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");

        }

        [RBAC]
        public IActionResult Upload(List<IFormFile> postedFiles)
        {
            JArray jarray = new JArray();
            string wwwPath = ihostingenvironment.WebRootPath;
            string contentPath = ihostingenvironment.ContentRootPath;

            string path = Path.Combine(ihostingenvironment.WebRootPath, "Uploads");
            //string path = dbhandler.GetRecords("parameters", "UPLOAD_FILE_PATH").Rows[0]["item_value"].ToString()!;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = DateTime.Now.ToFileTimeUtc().ToString() + Path.GetExtension(postedFile.FileName);
                using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                postedFile.CopyTo(stream);

                jarray.Add(new JObject {
                    { "original_file_name",  postedFile.FileName },
                    { "new_file_name",  fileName },
                    { "message",  "success" }
                });
            }

            //return Content("Success");
            return Content(JsonConvert.SerializeObject(jarray, Formatting.Indented), "application/json");
        }

        [HttpGet]
        public ContentResult GetRecords(string module, string param = "normal")
        {
            ArrayList details = new ArrayList();
            DataTable datatable = new DataTable();
            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            if (param == "unapproved")
                datatable = dbhandler.GetUnapprovedRecords(module);
            else
            {
                if (param.Contains("|"))
                {
                    string[] parameters = param.Split('|');
                    switch (parameters.Length)
                    {
                        case 1:
                            datatable = dbhandler.GetRecords(module, parameters[0]);
                            break;

                        case 2:
                            datatable = dbhandler.GetRecords(module, parameters[0], parameters[1]);
                            break;

                        default:
                            datatable = dbhandler.GetRecords(module);
                            break;
                    }
                }
                else
                {
                    switch (module)
                    {
                        case "roles":
                            datatable = dbhandler.GetRecords(module);
                            break;

                        case "role_unallocated_permissions":
                        case "role_allocated_permissions":
                        case "user_unallocated_roles":
                        case "user_allocated_roles":
                            datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                            break;

                        default:
                            datatable = dbhandler.GetRecords(module);
                            break;
                    }
                }
            }

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
        public ActionResult Delete(/*[FromBody] JObject jobject*/int id, string module)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                //Int32 id = Convert.ToInt32(jobject["id"]);
                //string module = jobject["module"].ToString();

                switch (module)
                {
                    case "roles":
                        RolesModel profilesmodel = dbhandler.GetRoles().Find(mymodel => mymodel.id == id)!;
                        if (profilesmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            CaptureAuditTrail("Deleted role", "Deleted role: " + profilesmodel.role_name);
                        }
                        break;

                    case "portal_users":
                        PortalUsersModel usersmodel = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == id)!;
                        if (usersmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            CaptureAuditTrail("Deleted user", "Deleted user: " + usersmodel.name);
                        }
                        break;

                    case "permissions":
                        PermissionsModel permissionsmodel = dbhandler.GetPermissions().Find(mymodel => mymodel.id == id)!;
                        if (permissionsmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;

                    case "client_record":
                        ClientRecordModel clientrecordmodel = dbhandler.GetClientRecord().Find(mymodel => mymodel.id == id)!;
                        if (clientrecordmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            CaptureAuditTrail("Deleted client", "Deleted client: " + clientrecordmodel.client_name);
                        }
                        break;

                    default:
                        break;
                }

                return GetRecords(module);
            }
        }

        [RBAC]
        public ActionResult Approve(/*[FromBody] JObject jobject*/int id, string module)
        {
            string response = "";
            //Int32 id = Convert.ToInt32(jobject["id"]);
            //string module = jobject["module"].ToString();

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                try
                {
                    switch (module)
                    {
                        case "roles":
                            RolesModel profilesmodel = dbhandler.GetRoles().Find(mymodel => mymodel.id == id)!;
                            if (profilesmodel != null)
                            {
                                dbhandler.ApproveRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                                CaptureAuditTrail("Approved role", "Approved role: " + profilesmodel.role_name);
                            }
                            break;
                        case "client_record":
                            ClientRecordModel clientrecordmodel = dbhandler.GetClientRecord().Find(mymodel => mymodel.id == id)!;
                            if (clientrecordmodel != null)
                            {
                                dbhandler.ApproveRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                                CaptureAuditTrail("Approved client record", "Approved clientrecordmodel: " + clientrecordmodel.client_name);
                            }
                            break;

                        case "portal_users":
                            PortalUsersModel usersmodel = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == id)!;
                            if (usersmodel != null)
                            {
                                string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                HttpHandler httphandler = new HttpHandler();
                                RandomKeyGeneratorManagement myrandomkeymanager = new RandomKeyGeneratorManagement();
                                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");
                                CommunicationManagement mycomman = new CommunicationManagement(dbhandler);
                                dbhandler.ApproveRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                                PortalUsersModel portaluser = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == id)!;
                                CaptureAuditTrail("Approved user", "Approved user: " + usersmodel.name);

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
                                            { "customer", portaluser.name },
                                            { "email_address", portaluser.email },
                                            { "user_password", Cryptographer.Decrypt(portaluser.password + "==") },
                                            { "portal_url", portal_url },
                                            { "attachment", "" }
                                        }
                                    }
                                };

                                string email_resp_data = httphandler.HttpClientPost(serverapi, email_data);

                                JObject email_resp_data_json = JObject.Parse(email_resp_data);

                                if (email_resp_data_json["error_code"]!.ToString() == "00" && email_resp_data_json["error_desc"]![0]!["response_code"]!.ToString() == "00")
                                {
                                    response = "Success-" + external_ref_num.Substring(2, 12);
                                }
                                else if (email_resp_data_json["error_code"]!.ToString() == "00" && email_resp_data_json["error_desc"]![0]!["response_code"]!.ToString() != "00")
                                {
                                    response = email_resp_data_json["error_desc"]![0]!["response_desc"]!.ToString();
                                }
                                else
                                {
                                    response = "Operation could not be completed, kindly contact system admin";
                                }
                            }
                            response = GetRecords(module, "unapproved").ToString()!;
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    iloggermanager.LogError(ex.Message);
                }

                return GetRecords(module, "unapproved");
                //return Json(response, JsonRequestBehavior.AllowGet);
                //return Content(response, "application/json");
            }
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
