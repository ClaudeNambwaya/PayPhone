using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;
using static ComplaintManagement.Controllers.ManageComplaintController;

namespace ComplaintManagement.Controllers
{
    public class ManageComplaintController : Controller
    {

        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;
        public ManageComplaintController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }
        public class complaintrecord
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

        public ActionResult DownloadPDF(Int64? id)
        {

            List<ComplaintFilesModel> recordstklist = new List<ComplaintFilesModel>();

            DataTable dt = new DataTable();

            var pdfId = Convert.ToInt32(id);

            dt = dbhandler.GetRecordsById("complaint_files", Convert.ToInt64(id));

            //dt = mydb.GetRecordsById("my_pdf", pdfId);
            //dt = mydb.GetRecordsById("my_pdf", Convert.ToInt64(id));

            foreach (DataRow dr in dt.Rows)
            {


                var filename = Convert.ToString(dr["file_number"]);


                JObject jobject = new JObject
                    {
                        { "file", filename }
                    };

                return Content(jobject.ToString(), "application/json");



            }

            return Content("success");

        }

        [HttpPost]
        public ActionResult MakeRemarks(Int64 id, string description)
        {
         
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {

                try
                {
                    ComplaintModel existingrecord = dbhandler.GetComplaintRecord().Find(mymodel => mymodel.id == id)!;
                    if (existingrecord != null)
                    {
                        id = existingrecord.id;

                        if (dbhandler.UpdateRemarks(id, description))
                        {

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update Complaint, kindly contact system admin");
                    }
                    else
                    {
                        return Content("Could not update Complaint, kindly contact system admin");

                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateStatus(Int64 id, string module)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                switch (module)
                {
                    case "open_close_status":
                        if (dbhandler.Update_Open_Complaint_Status(id, module))
                        {
                            return Content("Success");

                        }
                        break;
                    case "closed_open":
                        if (dbhandler.Update_Open_Complaint_Status(id, module))
                        {
                            return Content("Success");

                        }
                        break;
                    default:
                        break;
                }

                return Content("Fail");
            }
        }

        public IActionResult ClosedComplaint()
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
                        case "complaint_files":
                            datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                            break;
                        //case "subcategorybyid":
                        //    datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                        //    break;

                        //case "subcountybyid":
                        //    datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                        //    break;

                        //case "wardbyid":
                        //    datatable = dbhandler.GetRecordsById(module, Convert.ToInt16(param));
                        //    break;

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
            return Content(JsonConvert.SerializeObject(rows, Formatting.Indented), "application/json");
        }

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
                            //CaptureAuditTrail("Deleted role", "Deleted role: " + profilesmodel.role_name);
                        }
                        break;

                    case "portal_users":
                        PortalUsersModel usersmodel = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == id)!;
                        if (usersmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted user", "Deleted user: " + usersmodel.name);
                        }
                        break;

                    case "permissions":
                        PermissionsModel permissionsmodel = dbhandler.GetPermissions().Find(mymodel => mymodel.id == id)!;
                        if (permissionsmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;

                    case "delete_complaint_record":
                        ComplaintModel compmodel = dbhandler.GetComplaint().Find(mymodel => mymodel.id == id)!;
                        if (compmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;


                    default:
                        break;
                }

                return GetRecords(module);
            }
        }

    }
}
