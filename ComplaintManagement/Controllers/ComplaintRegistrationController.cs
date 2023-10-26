using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using System.Data;
using Microsoft.AspNetCore.Http;
using Nancy;

namespace ComplaintManagement.Controllers
{
    public class ComplaintRegistrationController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public ComplaintRegistrationController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class onboarding_record
        {
            public complaint_record[]? applicant_details { get; set; }
            public string? complainant_files { get; set; }
        }

        public class complaint_record
        {
            public Int64 id { get; set; }
            public Int64 category_id { get; set; }
            public Int64 subcategory_id { get; set; }
            public Int64 complaint_type { get; set; }
            public string? nature_of_complaint { get; set; }
            public string? complaint_description { get; set; }
            public Int64 county_id { get; set; }
            public Int64 sub_county_id { get; set; }
            public Int64 ward_id { get; set; }
            public string? address { get; set; }
            public bool isanonymous { get; set; }
            public string? remarks { get; set; }
        }
        public class processing_response
        {
            public string? error_code { get; set; }
            public string? error_desc { get; set; }
            public string? system_ref { get; set; }
            public string? account_number { get; set; }
        }


        public IActionResult Complaint()
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

        [HttpPost]
        public ActionResult CreateComplaint(onboarding_record record)
        {
            processing_response response = new processing_response
            {
                system_ref = DateTime.Now.ToString("yyyyMMddHHssfff")
            };

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.applicant_details![0].category_id == -1 || record.applicant_details![0].category_id == 0 )
                    return Content("Invalid Category");
                if (record.applicant_details![0].nature_of_complaint == null)
                    return Content("Invalid nature_of_complaint");

                try
                {
                    string[] complainant_files;

                    ComplaintModel existingrecord = dbhandler.GetComplaint().Find(mymodel => mymodel.id == record.applicant_details![0].id)!;
                    if (existingrecord != null)
                    {
                        ComplaintModel mymodel = new ComplaintModel
                        {
                            id = existingrecord.id,
                            category_id = record.applicant_details![0].category_id,
                            subcategory_id = record.applicant_details![0].subcategory_id,
                            complaint_type = record.applicant_details![0].complaint_type,
                            nature_of_complaint = record.applicant_details![0].nature_of_complaint,
                            complaint_description = record.applicant_details![0].complaint_description,
                            county_id = record.applicant_details![0].county_id,
                            sub_county_id = record.applicant_details![0].sub_county_id,
                            ward_id = record.applicant_details![0].ward_id,
                            address = record.applicant_details![0].address,
                            isanonymous = record.applicant_details![0].isanonymous,
                            remarks = record.applicant_details![0].remarks,
                            user_id = HttpContext.Session.GetString("userid")
                        };

                        if (dbhandler.UpdateComplaint(mymodel))
                        {

                            //2. Update customer files records
                            if (record.complainant_files == null)
                            {
                                response.error_code = "00";
                                response.error_desc = "File updated successfully";
                            }
                            else
                            {
                                complainant_files = record.complainant_files.Trim().Split('|');
                                complainant_files = complainant_files.Take(complainant_files.Count() - 1).ToArray();



                                for (int i = 0; i < complainant_files.Length; i++)
                                {
                                    try
                                    {
                                        string[] filedata = complainant_files[i].Split(',');
                                        string file_number = filedata[0];
                                        string file_name = filedata[1];

                                        ComplaintFilesModel filesmodel = new ComplaintFilesModel
                                        {
                                            complaint_id = existingrecord.id,
                                            file_number = file_number,
                                            file_name = file_name,
                                        };


                                        ComplaintFilesModel existingfilerecord = dbhandler.GetComplaintFiles("complaint_files", Convert.ToString(existingrecord.id)).Find(mymodel1 => mymodel1.file_number != null)!;

                                        if (existingfilerecord != null)
                                        {
                                           // dbhandler.UpdateComplaintFilesDocumentData(Convert.ToInt64(filesmodel.principal_id), Convert.ToInt16(Session["userid"]), filesmodel.file_type, filesmodel.client_files);
                                        }
                                        else
                                        {
                                            try
                                            {
                                                if (dbhandler.Addcomplaint_Files(filesmodel))
                                                {
                                                    response.error_code = "00";
                                                    response.error_desc = "File created successfully";
                                                }
                                                else
                                                {
                                                    response.error_code = "01";
                                                    response.error_desc = "Client not created successfully, kindly contact admin";
                                                }
                                                

                                            }
                                            catch (Exception ex)
                                            {
                                                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                                                response.error_code = "01";
                                                response.error_desc = "Exception raised on customer file (" + file_number[i] + ") saving " + ex;
                                            }
                                            //}
                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                                        response.error_code = "01";
                                        response.error_desc = "Exception raised on customer file (" + complainant_files[i] + ") saving " + ex;
                                    }
                                }

                            }


                            // CaptureAuditTrail("Updated name", "name: " + mymodel.name);

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update Complaint, kindly contact system admin");
                    }
                    else
                    {
                        ComplaintModel mymodel = new ComplaintModel
                        {

                            category_id = record.applicant_details![0].category_id,
                            subcategory_id = record.applicant_details![0].subcategory_id,
                            complaint_type = record.applicant_details![0].complaint_type,
                            nature_of_complaint = record.applicant_details![0].nature_of_complaint,
                            complaint_description = record.applicant_details![0].complaint_description,
                            county_id = record.applicant_details![0].county_id,
                            sub_county_id = record.applicant_details![0].sub_county_id,
                            ward_id = record.applicant_details![0].ward_id,
                            address = record.applicant_details![0].address,
                            isanonymous = record.applicant_details![0].isanonymous,
                            remarks = record.applicant_details![0].remarks,
                            user_id = HttpContext.Session.GetString("userid")

                        };

                        Int64 complaint_id = dbhandler.AddComplaint(mymodel);

                        //2. Update customer files records
                        if (record.complainant_files == null)
                        {
                            response.error_code = "00";
                            response.error_desc = "File updated successfully";
                        }
                        else
                        {
                            complainant_files = record.complainant_files.Trim().Split('|');
                            complainant_files = complainant_files.Take(complainant_files.Count() - 1).ToArray();



                            for (int i = 0; i < complainant_files.Length; i++)
                            {
                                try
                                {
                                    string[] filedata = complainant_files[i].Split(',');
                                    string file_number = filedata[0];
                                    string file_name = filedata[1];

                                    ComplaintFilesModel filesmodel = new ComplaintFilesModel
                                    {
                                        complaint_id = complaint_id,
                                        file_number = file_number,
                                        file_name = file_name,
                                    };


                                   // ComplaintFilesModel existingfilerecord = dbhandler.GetComplaintFiles(Convert.ToString(existingrecord!.id)).Find(model => model.file_number == file_number);

                                    if (filesmodel == null)
                                    {
                                        response.error_code = "00";
                                        response.error_desc = "File created successfully";
                                        // dbhandler.UpdateComplaintFilesDocumentData(Convert.ToInt64(filesmodel.principal_id), Convert.ToInt16(Session["userid"]), filesmodel.file_type, filesmodel.client_files);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (dbhandler.Addcomplaint_Files(filesmodel))
                                            {
                                                response.error_code = "00";
                                                response.error_desc = "File created successfully";
                                            }
                                            else
                                            {
                                                response.error_code = "01";
                                                response.error_desc = "Client not created successfully, kindly contact admin";
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                                            response.error_code = "01";
                                            response.error_desc = "Exception raised on customer file (" + file_number[i] + ") saving " + ex;
                                        }
                                        //}
                                    }


                                }
                                catch (Exception ex)
                                {
                                    FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                                    response.error_code = "01";
                                    response.error_desc = "Exception raised on customer file (" + complainant_files[i] + ") saving " + ex;
                                }
                            }

                        }

                        return Content("Success");

                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
        }
       

        [HttpPost]
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

            var user_id = HttpContext.Session.GetString("userid");

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
                        case "subcategorybyid":
                            datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                            break;
                        case "subcountybyid":
                            datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                            break;
                        case "complaint_record_byId":
                            datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(user_id));
                            break;
                        case "wardbyid":
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
