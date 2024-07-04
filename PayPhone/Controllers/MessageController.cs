//using ComplaintManagement.Helpers;
//using ComplaintManagement.Models;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using PayPhone.Models;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;

//namespace PayPhone.Controllers
//{
//    public class MessageController : Controller
//    {
//        private readonly IWebHostEnvironment _hostingEnvironment;
//        private readonly ILoggerManager _loggerManager;
//        private readonly DBHandler _dbHandler;

//        public MessageController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler dbHandler)
//        {
//            _loggerManager = logger;
//            _hostingEnvironment = environment;
//            _dbHandler = dbHandler;
//        }

//        public class UserDetailRequest
//        {
//            public string? RecipientsUsername { get; set; }
//            public string? SendersUsername { get; set; }
//        }

//        public class UserDetails
//        {
//            public int Id { get; set; }
//            public int RoleId { get; set; }
//            public string? Mobile { get; set; }
//            public string? Email { get; set; }
//            public string? Name { get; set; }
//            public string? UserName { get; set; }
//            public string? RoleName { get; set; }
//            public DateTime CreatedOn { get; set; }
//            public DateTime UpdatedAt { get; set; }
//            public bool Approved { get; set; }
//            public long Balance { get; set; }
//        }

//        public class UserDetailsRecords
//        {
//            public int Id { get; set; }
//            public int RoleId { get; set; }
//            public string? Mobile { get; set; }
//            public string? Email { get; set; }
//            public string? Name { get; set; }
//            public string? UserName { get; set; }
//            public string? RoleName { get; set; }
//            public DateTime CreatedOn { get; set; }
//            public DateTime UpdatedAt { get; set; }
//            public bool Approved { get; set; }
//            public long Balance { get; set; }
//        }

//        public class MessageRecord
//        {
//            public int Id { get; set; }
//            public int Sender { get; set; }
//            public string? Content { get; set; }
//            public int Receiver { get; set; }
//            public int Chat { get; set; }
//            public List<int> ReadBy { get; set; } = new List<int>();
//        }

//        public class ProcessingResponse
//        {
//            public string? ErrorCode { get; set; }
//            public string? ErrorDesc { get; set; }
//            public string? SystemRef { get; set; }
//            public string? AccountNumber { get; set; }
//        }

//        public IActionResult SendMessage()
//        {
//            if (HttpContext.Session.GetString("name") == null)
//                return RedirectToAction("AdminLogin", "AppAuth");

//            ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
//            MenuHandler menuHandler = new MenuHandler(_dbHandler);
//            IEnumerable<MenuModel> menu = menuHandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
//            return View(menu);
//        }

//        [HttpPost]
//        public ActionResult SendMessage([FromBody] MessageRecord record)
//        {
//            ProcessingResponse response = new ProcessingResponse
//            {
//                SystemRef = DateTime.Now.ToString("yyyyMMddHHmmssfff")
//            };

//            if (HttpContext.Session.GetString("name") == null)
//                return RedirectToAction("AdminLogin", "AppAuth");

//            try
//            {
//                MessageModel existingRecord = _dbHandler.GetMessage().Find(m => m.Id == record.Id);
//                if (existingRecord != null)
//                {
//                    existingRecord.Sender = record.Sender;
//                    existingRecord.Content = record.Content;
//                    existingRecord.Receiver = record.Receiver;
//                    existingRecord.Chat = record.Chat;
//                    existingRecord.ReadBy = record.ReadBy;

//                    if (_dbHandler.UpdateMessage(existingRecord))
//                    {
//                        CaptureAuditTrail("Updated message", $"Updated message: {existingRecord.Sender}");
//                        response.ErrorCode = "00";
//                        response.ErrorDesc = "Updated message successfully";
//                    }
//                    else
//                    {
//                        response.ErrorCode = "01";
//                        response.ErrorDesc = "Could not update message, kindly contact system admin";
//                    }
//                }
//                else
//                {
//                    MessageModel newRecord = new MessageModel
//                    {
//                        Sender = record.Sender,
//                        Content = record.Content,
//                        Receiver = record.Receiver,
//                        Chat = record.Chat,
//                        ReadBy = record.ReadBy
//                    };

//                    if (_dbHandler.AddMessage(newRecord))
//                    {
//                        CaptureAuditTrail("Created message", $"Created message: {newRecord.Sender}");
//                        response.ErrorCode = "00";
//                        response.ErrorDesc = "Message successfully created";
//                    }
//                    else
//                    {
//                        response.ErrorCode = "01";
//                        response.ErrorDesc = "Could not create message, kindly contact system admin";
//                    }
//                }
//            }
//            catch
//            {
//                response.ErrorCode = "01";
//                response.ErrorDesc = "An error occurred, kindly contact system admin";
//            }

//            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
//        }

//        [HttpPost]
//        public ActionResult GetUsersDetails([FromBody] UserDetailRequest request)
//        {
//            var users = GetPortalUsers();

//            var sender = users.FirstOrDefault(u => u.user_name.Equals(request.SendersUsername, StringComparison.OrdinalIgnoreCase));
//            var recipient = users.FirstOrDefault(u => u.user_name.Equals(request.RecipientsUsername, StringComparison.OrdinalIgnoreCase));

//            if (sender == null || recipient == null)
//            {
//                return Json(new { success = false, message = "One or both users not found." });
//            }

//            var result = new
//            {
//                success = true,
//                message = "Users found",
//                sender = sender,
//                recipient = recipient
//            };

//            return Json(result);
//        }



//        public List<PortalUsersModel> GetPortalUsers()
//        {
//            List<PortalUsersModel> recordList = new List<PortalUsersModel>();

//            try
//            {
//                DataTable dt = GetRecords("portal_users");

//                foreach (DataRow dr in dt.Rows)
//                {
//                    recordList.Add(new PortalUsersModel
//                    {
//                        id = dr.Field<int>("id"),
//                        role_id = dr.Field<int>("role_id"),
//                        mobile = dr.Field<string>("mobile"),
//                        email = dr.Field<string>("email"),
//                        name = dr.Field<string>("name"),
//                        password = dr.Field<string>("password"),
//                        avatar = dr.Field<string>("avatar"),
//                        locked = dr.Field<bool>("locked"),
//                        google_authenticate = dr.Field<bool>("google_authenticate"),
//                        sec_key = dr.Field<string>("sec_key"),
//                        // Uncomment and ensure the "balance" column exists in your table
//                        // balance = dr.Field<long>("balance"),
//                        created_on = dr.Field<DateTime>("created_on"),
//                        user_name = dr.Field<string>("user_name"),
//                        created_by = dr.Field<int>("created_by"),
//                        menu_layout = dr.Field<string>("menu_layout"),
//                        updated_at = dr.Field<DateTime>("updated_at"),
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                FileLogHelper.log_message_fields("ERROR", "GetPortalUsers | Exception ->" + ex.Message);
//            }

//            return recordList;
//        }


//        [HttpGet]
//        public ContentResult GetRecords(string module, string param = "normal")
//        {
//            //FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
//            //FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");
//            ArrayList details = new ArrayList();
//            DataTable datatable = new DataTable();
//            DataTable datatableI = new DataTable();
//            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
//            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
//            Dictionary<string, object> row;
//            JObject jobject = new JObject();
//            JArray jarray = new JArray();
//            JArray option_array = new JArray();

//            switch (module)
//            {

//                default:
//                    datatable = _dbHandler.GetRecords(module);
//                    break;
//            }

//            if (datatable.Rows.Count > 0)
//            {
//                foreach (DataRow dr in datatable.Rows)
//                {
//                    row = new Dictionary<string, object>();
//                    foreach (DataColumn col in datatable.Columns)
//                    {
//                        row.Add(col.ColumnName, dr[col]);
//                    }
//                    rows.Add(row);
//                }
//            }
//            return Content(JsonConvert.SerializeObject(rows, Formatting.Indented) /*serializer.Serialize(rows)*/, "application/json");
//        }

//        //private List<PortalUsersModel> GetPortalUsers()
//        //{
//        //    List<PortalUsersModel> recordList = new List<PortalUsersModel>();

//        //    try
//        //    {
//        //        DataTable dt = _dbHandler.GetPortalUsers().FirstOrDefault(user => user.email == recordList.);

//        //        foreach (DataRow dr in dt.Rows)
//        //        {
//        //            recordList.Add(new PortalUsersModel
//        //            {
//        //                id = Convert.ToInt32(dr["id"]),
//        //                role_id = Convert.ToInt32(dr["role_id"]),
//        //                mobile = Convert.ToString(dr["mobile"]),
//        //                email = Convert.ToString(dr["email"]),
//        //                name = Convert.ToString(dr["name"]),
//        //                password = Convert.ToString(dr["password"]),
//        //                avatar = Convert.ToString(dr["avatar"]),
//        //                locked = Convert.ToBoolean(dr["locked"]),
//        //                google_authenticate = Convert.ToBoolean(dr["google_authenticate"]),
//        //                sec_key = Convert.ToString(dr["sec_key"]),
//        //                created_on = Convert.ToDateTime(dr["created_on"]),
//        //                user_name = Convert.ToString(dr["user_name"]),
//        //                created_by = Convert.ToInt32(dr["created_by"]),
//        //                menu_layout = Convert.ToString(dr["menu_layout"]),
//        //                updated_at = Convert.ToDateTime(dr["updated_at"]),
//        //                balance = Convert.ToDecimal(dr["balance"])
//        //            });
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        FileLogHelper.log_message_fields("ERROR", "GetPortalUsers | Exception ->" + ex.Message);
//        //    }

//        //    return recordList;
//        //}

//        //private DataTable GetRecords(string tableName)
//        //{
//        //    // Your implementation to fetch records from the database
//        //}

//        public bool CaptureAuditTrail(string actionType, string actionDescription)
//        {
//            AuditTrailModel auditTrailModel = new AuditTrailModel
//            {
//                user_name = HttpContext.Session.GetString("email"),
//                action_type = actionType,
//                action_description = actionDescription,
//                page_accessed = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}",
//                session_id = HttpContext.Session.Id
//            };

//            return _dbHandler.AddAuditTrail(auditTrailModel);
//        }

//        //[RBAC]
//        //public ActionResult Delete(int id, string module)
//        //{
//        //    if (HttpContext.Session.GetString("name") == null)
//        //        return RedirectToAction("AdminLogin", "AppAuth");

//        //    switch (module)
//        //    {
//        //        case "diagnosis":
//        //            MessageModel diagnosisModel = _dbHandler.GetMessage().Find(m => m.Id == id);
//        //            if (diagnosisModel != null)
//        //            {
//        //                _dbHandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
//        //                CaptureAuditTrail("Deleted message record", $"Deleted message record: {diagnosisModel.Sender}");
//        //            }
//        //            break;

//        //        default:
//        //            break;
//        //    }

//        //    return GetRecords(module);
//        //}
//    }
//}





////using ComplaintManagement.Helpers;
////using ComplaintManagement.Models;
////using Microsoft.AspNetCore.Mvc;
////using Newtonsoft.Json.Linq;
////using Newtonsoft.Json;
////using static ComplaintManagement.Helpers.CryptoHelper;
////using System.Collections;
////using System.Data;
////using PayPhone.Models;

////namespace PayPhone.Controllers
////{
////    public class MessageController : Controller
////    {
////        private IWebHostEnvironment ihostingenvironment;
////        private ILoggerManager iloggermanager;
////        private DBHandler dbhandler;

////        public MessageController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
////        {
////            iloggermanager = logger;
////            ihostingenvironment = environment;
////            dbhandler = mydbhandler;
////        }

////        public class UserDetailRequest
////        {
////            public string? RecipientsUsername { get; set; }
////            public string? SendersUsername { get; set; }
////        }

////        public class UserDetails
////        {
////            public int Id { get; set; }
////            public int RoleId { get; set; }
////            public string? Mobile { get; set; }
////            public string? Email { get; set; }
////            public string? Name { get; set; }
////            public string? UserName { get; set; }
////            public string? RoleName { get; set; }
////            public DateTime CreatedOn { get; set; }
////            public DateTime UpdatedAt { get; set; }
////            public bool Approved { get; set; }
////            public long Balance { get; set; }
////        }



////        public class message_record
////        {
////            public int Id { get; set; }

////            public int Sender { get; set; }

////            public string? Content { get; set; }

////            public int Receiver { get; set; }

////            public int Chat { get; set; }

////            // To represent JSON in a C# object
////            public List<int> ReadBy { get; set; } = new List<int>();
////        }

////        public class Processingresponse
////        {
////            public string? error_code { get; set; }
////            public string? error_desc { get; set; }
////            public string? system_ref { get; set; }
////            public string? account_number { get; set; }
////        }

////        public IActionResult SendMessage()
////        {
////            if (HttpContext.Session.GetString("name") == null)
////                return RedirectToAction("AdminLogin", "AppAuth");
////            else
////            {
////                ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
////                MenuHandler menuhandler = new MenuHandler(dbhandler);
////                IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
////                return View(menu);
////            }
////        }


////        [HttpPost]
////        public ActionResult SendMessage([FromBody] message_record record)
////        {
////            Processingresponse response = new Processingresponse
////            {
////                system_ref = DateTime.Now.ToString("yyyyMMddHHmmssfff")
////            };

////            if (HttpContext.Session.GetString("name") == null)
////                return RedirectToAction("AdminLogin", "AppAuth");
////            else
////            {
////                try
////                {
////                    MessageModel existingrecord = dbhandler.GetMessage().Find(mymodel => mymodel.Id == record.Id)!;
////                    if (existingrecord != null)
////                    {
////                        MessageModel mymodel = new MessageModel
////                        {
////                            Id = existingrecord.Id,
////                            Sender = record.Sender,
////                            Content = record.Content,
////                            Receiver = record.Receiver,
////                            Chat = record.Chat,
////                            ReadBy = record.ReadBy
////                        };

////                        if (dbhandler.UpdateMessage(mymodel))
////                        {
////                            CaptureAuditTrail("Updated message", "Updated message: " + mymodel.Sender);
////                            ModelState.Clear();
////                            response.error_code = "00";
////                            response.error_desc = "Updated message successfully ";
////                        }
////                        else
////                        {
////                            response.error_code = "01";
////                            response.error_desc = "Could not Update message, kindly contact system admin ";
////                        }
////                    }
////                    else
////                    {
////                        MessageModel mymodel = new MessageModel
////                        {
////                            Id = existingrecord!.Id,
////                            Sender = record.Sender,
////                            Content = record.Content,
////                            Receiver = record.Receiver,
////                            Chat = record.Chat,
////                            ReadBy = record.ReadBy
////                        };

////                        if (dbhandler.AddMessage(mymodel))
////                        {
////                            CaptureAuditTrail("Created message", "Created message: " + mymodel.Sender);
////                            ModelState.Clear();
////                            response.error_code = "00";
////                            response.error_desc = "message successfully created";
////                        }
////                        else
////                        {
////                            response.error_code = "01";
////                            response.error_desc = "Could not Create message, kindly contact system admin";
////                        }

////                    }
////                }
////                catch
////                {
////                    response.error_code = "01";
////                    response.error_desc = "Could not Create diagnosis, kindly contact system admin";
////                }
////            }
////            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
////        }




////        [HttpGet]
////        public ContentResult GetRecords(string module, string param = "normal")
////        {
////            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
////            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");
////            ArrayList details = new ArrayList();
////            DataTable datatable = new DataTable();
////            DataTable datatableI = new DataTable();
////            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
////            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
////            Dictionary<string, object> row;
////            JObject jobject = new JObject();
////            JArray jarray = new JArray();
////            JArray option_array = new JArray();

////            switch (module)
////            {

////                default:
////                    datatable = dbhandler.GetRecords(module);
////                    break;
////            }

////            if (datatable.Rows.Count > 0)
////            {
////                foreach (DataRow dr in datatable.Rows)
////                {
////                    row = new Dictionary<string, object>();
////                    foreach (DataColumn col in datatable.Columns)
////                    {
////                        row.Add(col.ColumnName, dr[col]);
////                    }
////                    rows.Add(row);
////                }
////            }
////            return Content(JsonConvert.SerializeObject(rows, Formatting.Indented) /*serializer.Serialize(rows)*/, "application/json");
////        }

////        public bool CaptureAuditTrail(string action_type, string action_description)
////        {
////            AuditTrailModel audittrailmodel = new AuditTrailModel
////            {
////                user_name = HttpContext.Session.GetString("email")!.ToString(),
////                action_type = action_type,
////                action_description = action_description,
////                page_accessed = String.Format("{0}://{1}{2}{3}", HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.Path, HttpContext.Request.QueryString), /*Request.Url.ToString(),*/
////                //client_ip_address = GetIPAddress(HttpContext), //Request.HttpContext.Connection.RemoteIpAddress.ToString(), /*Request.UserHostAddress,*/
////                session_id = HttpContext.Session.Id //HttpContext.Session.GetString("userid") /*Session.SessionID*/
////            };
////            return dbhandler.AddAuditTrail(audittrailmodel);
////        }

////        [RBAC]
////        public ActionResult Delete(/*[FromBody] JObject jobject*/ int id, string module)
////        {

////            if (HttpContext.Session.GetString("name") == null)
////                return RedirectToAction("AdminLogin", "AppAuth");
////            else
////            {
////                switch (module)
////                {
////                    case "diagnosis":
////                        MessageModel diagnosis_model = dbhandler.GetMessage().Find(mymodel => mymodel.Id == id)!;
////                        if (diagnosis_model != null)
////                        {
////                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
////                            CaptureAuditTrail("Deleted message record", "Deleted message record: " + diagnosis_model.Sender);
////                        }
////                        break;


////                    default:
////                        break;
////                }

////                return GetRecords(module);
////            }
////        }
////    }
////}
