using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Data;

namespace ComplaintManagement.Controllers
{
    public class ComplaintManagementController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;
        public ComplaintManagementController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }
        public class categoryrecord
        {
            public Int64 id { get; set; }
            public string? category_name { get; set; }
            public string? category_description { get; set; }
        }
        public class complainttyperecord
        {
            public Int64 id { get; set; }
            public string? complaint_name { get; set; }
        }
        public class subcategoryrecord
        {
            public Int64 id { get; set; }
            public string? sub_name { get; set; }
            public Int64 category_id { get; set; }
        }
        public class staterecord
        {
            public Int64 id { get; set; }
            public string? state_name { get; set; }
            public string? description { get; set; }
        }
        public class statusrecord
        {
            public Int64 id { get; set; }
            public string? status_name { get; set; }
        }
        public IActionResult Category()
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

        [HttpPost]
        public ActionResult CreateCategory(categoryrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.category_name == null)
                    return Content("Invalid name");
                if (record.category_description == null)
                    return Content("Invalid name");

                try
                {
                    CategoryModel existingrecord = dbhandler.GetCategory().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        CategoryModel mymodel = new CategoryModel
                        {
                            id = existingrecord.id,
                            category_name = record.category_name,
                            category_description = record.category_description,

                        };

                        if (dbhandler.UpdateCategory(mymodel))
                        {
                            // CaptureAuditTrail("Updated name", "name: " + mymodel.name);

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update DOCTOR, kindly contact system admin");
                    }
                    else
                    {
                        CategoryModel mymodel = new CategoryModel
                        {

                            category_name = record.category_name,
                            category_description = record.category_description

                        };

                        if (dbhandler.AddCategory(mymodel))
                        {
                            // CaptureAuditTrail("Created topic", "Created topic: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create topic, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
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

                    case "category_record":
                        CategoryModel model = dbhandler.GetCategory().Find(mymodel => mymodel.id == id)!;
                        if (model != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;
                    case "complaint_type_record":
                        ComplaintTypeModel complainttypemodel = dbhandler.GetComplaintType().Find(mymodel => mymodel.id == id)!;
                        if (complainttypemodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;
                    case "subcategory_record":
                        SubcategoryModel submodel = dbhandler.GetSubcategory().Find(mymodel => mymodel.id == id)!;
                        if (submodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;

                    case "state_record":
                        StateModel statemodel = dbhandler.GetState().Find(mymodel => mymodel.id == id)!;
                        if (statemodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;

                    case "complaint_record":
                        ComplaintModel compmodel = dbhandler.GetComplaint().Find(mymodel => mymodel.id == id)!;
                        if (compmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            //CaptureAuditTrail("Deleted permission", "Deleted permission: " + permissionsmodel.permission_name);
                        }
                        break;

                    case "statuses_record":
                        StatusModel statusmodel = dbhandler.GetStatus().Find(mymodel => mymodel.id == id)!;
                        if (statusmodel != null)
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
        public IActionResult ComplaintType()
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
        public ActionResult CreateComplaintType(complainttyperecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.complaint_name == null)
                    return Content("Invalid name");
                

                try
                {
                    ComplaintTypeModel existingrecord = dbhandler.GetComplaintType().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        ComplaintTypeModel mymodel = new ComplaintTypeModel
                        {
                            id = existingrecord.id,
                            complaint_name = record.complaint_name

                        };

                        if (dbhandler.UpdateComplaintType(mymodel))
                        {
                            // CaptureAuditTrail("Updated name", "name: " + mymodel.name);

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update DOCTOR, kindly contact system admin");
                    }
                    else
                    {
                        ComplaintTypeModel mymodel = new ComplaintTypeModel
                        {

                            complaint_name = record.complaint_name

                        };

                        if (dbhandler.AddComplaintType(mymodel))
                        {
                            // CaptureAuditTrail("Created topic", "Created topic: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create topic, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
        }

        public IActionResult Subcategory()
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
        public ActionResult CreateSubcategory(subcategoryrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.sub_name == null)
                    return Content("Invalid name");

                try
                {
                    SubcategoryModel existingrecord = dbhandler.GetSubcategory().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        SubcategoryModel mymodel = new SubcategoryModel
                        {
                            id = existingrecord.id,
                            sub_name = record.sub_name,
                            category_id = record.category_id

                        };

                        if (dbhandler.UpdateSubcategory(mymodel))
                        {
                            // CaptureAuditTrail("Updated name", "name: " + mymodel.name);

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update DOCTOR, kindly contact system admin");
                    }
                    else
                    {
                        SubcategoryModel mymodel = new SubcategoryModel
                        {

                            sub_name = record.sub_name,
                            category_id = record.category_id

                        };

                        if (dbhandler.AddSubcategory(mymodel))
                        {
                            // CaptureAuditTrail("Created topic", "Created topic: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create topic, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
        }

        public IActionResult State()
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
        public ActionResult CreateState(staterecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.state_name == null)
                    return Content("Invalid name");

                try
                {
                    StateModel existingrecord = dbhandler.GetState().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        StateModel mymodel = new StateModel
                        {
                            id = existingrecord.id,
                            state_name = record.state_name,
                            description = record.description

                        };

                        if (dbhandler.UpdateState(mymodel))
                        {
                            // CaptureAuditTrail("Updated name", "name: " + mymodel.name);

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update DOCTOR, kindly contact system admin");
                    }
                    else
                    {
                        StateModel mymodel = new StateModel
                        {
                            state_name = record.state_name,
                            description = record.description

                        };

                        if (dbhandler.AddState(mymodel))
                        {
                            // CaptureAuditTrail("Created topic", "Created topic: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create topic, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
        }

        public IActionResult Status()
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
        public ActionResult CreateStatus(statusrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.status_name == null)
                    return Content("Invalid name");

                try
                {
                    StatusModel existingrecord = dbhandler.GetStatus().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        StatusModel mymodel = new StatusModel
                        {
                            id = existingrecord.id,
                            status_name = record.status_name
                            
                        };

                        if (dbhandler.UpdateStatus(mymodel))
                        {
                            // CaptureAuditTrail("Updated name", "name: " + mymodel.name);

                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update DOCTOR, kindly contact system admin");
                    }
                    else
                    {
                        StatusModel mymodel = new StatusModel
                        {
                            status_name = record.status_name
                           
                        };

                        if (dbhandler.AddStatus(mymodel))
                        {
                            // CaptureAuditTrail("Created topic", "Created topic: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create topic, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create topic, kindly contact system admin");
                }
            }
        }

    }
}
