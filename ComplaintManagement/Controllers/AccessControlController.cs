
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using static ComplaintManagement.Helpers.CryptoHelper;
using static ComplaintManagement.Helpers.HttpClientHelper;

namespace ComplaintManagement.Controllers
{
    //[CheckAuthorization]
    public class AccessControlController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public AccessControlController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }
        public class rolerecord
        {
            public int id { get; set; }
            public string? role_name { get; set; }
            public string? role_type { get; set; }
            public string? remarks { get; set; }
            public bool is_sys_admin { get; set; }
        }

        public class permissionrecord
        {
            public int id { get; set; }
            public string? permission_name { get; set; }
        }

        public class menuaccessrecord
        {
            public string? main_menu_name { get; set; }
            public string? main_menu_url { get; set; }
            public int can_access { get; set; }
            public submenurecord[]? sub_menus { get; set; }
        }

        public class submenurecord
        {
            public string? sub_menu_url { get; set; }
            public int can_access { get; set; }
        }

        public class mappingrecord
        {
            public int id { get; set; }
            public string? mode { get; set; }
            public int parent_id { get; set; }
            public int child_id { get; set; }
        }

        public class userrecord
        {
            public Int32 id { get; set; }

            public Int32 role_id { get; set; }

            public string? mobile { get; set; }

            public string? email { get; set; }

            public string? name { get; set; }

            public string? password { get; set; }

            public virtual string? avatar { get; set; }

            public bool locked { get; set; }

            public bool google_authenticate { get; set; }

            public virtual string? sec_key { get; set; }

            public bool reset_password { get; set; }
        }

        public class menuassign
        {
            public int profileid { get; set; }
            public menuaccessrecord[]? menu { get; set; }
        }

        [RBAC]
        public ActionResult RolesIndex()
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
        public ActionResult RolesUnapproved()
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
        public ActionResult MenuAccess()
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
        public ActionResult Permissions()
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
        public ActionResult RolePermissions()
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
        public ActionResult PortalUsersIndex()
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
        public ActionResult PortalUsersUnapproved()
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
        public ActionResult PortalUserRoles()
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

        [HttpGet]
        //public JArray GetMenusJSTree(int profileid)
        public IActionResult GetMenusJSTree(int profileid)
        {
            DataTable datatable = new DataTable();

            JArray main_menu_jarray = new JArray();
            DataTable main_menu_datatable = dbhandler.GetAdhocData("call get_menu_data (" + profileid + ", 'main', '');");

            if (main_menu_datatable.Rows.Count > 0)
            {
                for (int i = 0; i <= main_menu_datatable.Rows.Count - 1; i++)
                {
                    JObject menu_jobject = new JObject();
                    JObject jo = new JObject();
                    string page_url = "#";

                    menu_jobject.Add("text", main_menu_datatable.Rows[i]["main_menu_name"].ToString());

                    DataTable sub_menu_datatable = dbhandler.GetAdhocData("call get_menu_data (" + profileid + ", 'sub', '" + main_menu_datatable.Rows[i]["main_menu_name"] + "');");

                    if (sub_menu_datatable.Rows.Count > 0)
                    {
                        page_url = "#";
                        jo.Add("href", page_url);
                        menu_jobject.Add("a_attr", jo);

                        JArray sub_menu_jarray = new JArray();
                        for (int j = 0; j <= sub_menu_datatable.Rows.Count - 1; j++)
                        {
                            //add items to json object
                            JObject children = new JObject();
                            children.Add("text", sub_menu_datatable.Rows[j]["sub_menu_name"].ToString());
                            jo = new JObject();
                            page_url = sub_menu_datatable.Rows[j]["page_url"].ToString()!;
                            jo.Add("href", page_url);
                            children.Add("a_attr", jo);
                            jo = new JObject
                            {
                                { "selected", Convert.ToBoolean(sub_menu_datatable.Rows[j]["can_access"]) }
                            };
                            children.Add("state", jo);
                            sub_menu_jarray.Add(children);

                        }
                        menu_jobject.Add("children", sub_menu_jarray);
                    }
                    else
                    {
                        DataTable no_sub_datatable = dbhandler.GetAdhocData("call get_menu_data (" + profileid + ", 'page_url', '" + main_menu_datatable.Rows[i]["main_menu_name"].ToString() + "');");
                        if (no_sub_datatable.Rows.Count > 0)
                        {
                            page_url = no_sub_datatable.Rows[0]["page_url"].ToString()!;
                            jo = new JObject
                            {
                                { "href", page_url }
                            };
                            menu_jobject.Add("a_attr", jo);
                            jo = new JObject
                            {
                                { "selected", Convert.ToBoolean(no_sub_datatable.Rows[0]["can_access"]) }
                            };
                            menu_jobject.Add("state", jo);
                        }
                    }
                    main_menu_jarray.Add(menu_jobject);
                }
            }
            //return main_menu_jarray;
            return Content(JsonConvert.SerializeObject(main_menu_jarray, Formatting.Indented), "application/json");
        }

        [RBAC]
        public ActionResult CreateRole(rolerecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.role_name == null)
                    return Content("Invalid role name");

                try
                {
                    RolesModel existingrecord = dbhandler.GetRoles().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        RolesModel mymodel = new RolesModel
                        {
                            id = existingrecord.id,
                            role_name = record.role_name,
                            role_type = record.role_type,
                            remarks = record.remarks,
                            is_sys_admin = record.is_sys_admin
                        };

                        if (dbhandler.UpdateRole(mymodel))
                        {
                            CaptureAuditTrail("Updated role", "Updated role: " + mymodel.role_name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                        {
                            return Content("Could not update role, kindly contact system admin");
                        }
                    }
                    else
                    {
                        RolesModel mymodel = new RolesModel
                        {
                            role_name = record.role_name,
                            role_type = record.role_type,
                            remarks = record.remarks,
                            is_sys_admin = record.is_sys_admin,
                            created_by = Convert.ToInt16(HttpContext.Session.GetString("userid"))
                        };

                        if (dbhandler.AddRole(mymodel))
                        {
                            CaptureAuditTrail("Created role", "Created role: " + mymodel.role_name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                        {
                            return Content("Could not create role, kindly contact system admin");
                        }
                    }
                }
                catch
                {
                    return Content("Could not create role, kindly contact system admin");
                }
            }
        }

        [RBAC]
        [HttpPost]
        public ActionResult CaptureMenuAccess([FromBody] JObject jobject /*int profileid, menuaccessrecord[] menu*/)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                int profileid = Convert.ToInt32(jobject["profileid"]);
                JArray menu = JArray.Parse(jobject["menu"]!.ToString());
                //menuaccessrecord[] menu = new menuaccessrecord[menuarray.Count];
                try
                {
                    dbhandler.DeleteRecord(profileid, Convert.ToInt16(HttpContext.Session.GetString("userid")), "menu_access");

                    for (int i = 0; i <= menu.Count - 1; i++)
                    {
                        if (JArray.Parse(menu[i]["sub_menus"]!.ToString()).Count > 0)
                        {
                            JArray sub_menus = JArray.Parse(menu[i]["sub_menus"]!.ToString());
                            for (int j = 0; j <= sub_menus.Count - 1; j++)
                                dbhandler.AddMenuAccess(sub_menus[j]["sub_menu_url"]!.ToString(), menu[i]["main_menu_name"]!.ToString(), sub_menus[j]["sub_menu_text"]!.ToString(), profileid, (sub_menus[j]["can_access"]!.ToString().ToLower() == "true" ? 1 : 0 ), i + 1, j + 1);
                        }
                        else
                        {
                            dbhandler.AddMenuAccess(menu[i]["main_menu_url"]!.ToString(), menu[i]["main_menu_name"]!.ToString(), "", profileid, (menu[i]["can_access"]!.ToString() == "true" ? 1 : 0), i + 1, 0);
                        }
                    }
                }
                catch
                {
                }
                CaptureAuditTrail("Role menu allocation", "Allocated menu to role: " + dbhandler.GetRoles().Find(mymodel => mymodel.id == profileid)!.role_name);
                return GetRecords("roles");
            }
        }

        [RBAC]
        public ActionResult CreatePermission(permissionrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.permission_name == null)
                    return Content("Invalid permission name");

                try
                {
                    PermissionsModel existingrecord = dbhandler.GetPermissions().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        PermissionsModel mymodel = new PermissionsModel
                        {
                            id = existingrecord.id,
                            permission_name = record.permission_name
                        };

                        if (dbhandler.UpdatePermission(mymodel))
                        {
                            CaptureAuditTrail("Updated permission", "Updated permission: " + mymodel.permission_name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update permission, kindly contact system admin");
                    }
                    else
                    {
                        PermissionsModel mymodel = new PermissionsModel
                        {
                            permission_name = record.permission_name,
                            created_by = Convert.ToInt16(HttpContext.Session.GetString("userid"))
                        };

                        if (dbhandler.AddPermission(mymodel))
                        {
                            CaptureAuditTrail("Created permission", "Created permission: " + mymodel.permission_name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create permission, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create permission, kindly contact system admin");
                }
            }
        }

        [RBAC]
        public ActionResult CreateRolePermissionMapping(mappingrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                try
                {
                    switch (record.mode)
                    {
                        case "allocate":
                            if (record.parent_id <= 0)
                                return Content("Invalid role");

                            if (record.child_id <= 0)
                                return Content("Invalid permission");

                            RolePermissionModel existingrecord = dbhandler.GetRolePermissions(record.parent_id).Find(mymodel => mymodel.role_id == record.parent_id && mymodel.permission_id == record.child_id)!;
                            if (existingrecord == null)
                            {
                                RolePermissionModel rpmodel = new RolePermissionModel
                                {
                                    role_id = record.parent_id,
                                    permission_id = record.child_id
                                };

                                if (dbhandler.AddRolePermission(rpmodel))
                                {
                                    CaptureAuditTrail("Allocate role permission access", "Allocated access to permission: " + dbhandler.GetPermissions().Find(mymodel => mymodel.id == record.child_id)!.permission_name + " for role: " + dbhandler.GetRoles().Find(mymodel => mymodel.id == record.parent_id)!.role_name);
                                    ModelState.Clear();
                                    return Content("Success");
                                }
                                else
                                {
                                    return Content("Could not allocate permission to role, kindly contact system admin");
                                }
                            }
                            else
                                return Content("Permission already allocated to role");

                        case "unallocate":
                            if (dbhandler.DeleteRecord(record.id, Convert.ToInt16(HttpContext.Session.GetString("userid")), "role_allocated_permissions"))
                            {
                                CaptureAuditTrail("Unallocate role permission access", "Unallocated access to permission: " + dbhandler.GetPermissions().Find(mymodel => mymodel.id == record.child_id)!.permission_name + " for role: " + dbhandler.GetRoles().Find(mymodel => mymodel.id == record.parent_id)!.role_name);
                                return Content("Success");
                            }
                            else
                                return Content("Could not deallocate role permission, kindly contact system admin");

                        default:
                            return Content("Could not perform operation, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not perform operation, kindly contact system admin");
                }
            }
        }

        [RBAC]
        public ActionResult CreatePortalUser(userrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                RandomKeyGeneratorManagement myrandomkeymanager = new RandomKeyGeneratorManagement();
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
                    PortalUsersModel existingrecord = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        PortalUsersModel mymodel = new PortalUsersModel
                        {
                            id = existingrecord.id,
                            role_id = record.role_id,
                            mobile = record.mobile,
                            email = record.email,
                            name = record.name
                        };
                        if (Convert.ToBoolean(record.reset_password))
                            mymodel.password = Cryptographer.Encrypt(myrandomkeymanager.GenerateRandomAlphaNumericString(7)).Replace("=", "");
                        else
                            mymodel.password = existingrecord.password;

                        mymodel.locked = record.locked;
                        mymodel.google_authenticate = record.google_authenticate;
                        mymodel.sec_key = existingrecord.sec_key;
                        mymodel.avatar = existingrecord.avatar;

                        if (dbhandler.UpdatePortalUser(mymodel))
                        {
                            CaptureAuditTrail("Updated user", "Updated user: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update user, kindly contact system admin");
                    }
                    else
                    {
                        PortalUsersModel mymodel = new PortalUsersModel
                        {
                            role_id = record.role_id,
                            mobile = record.mobile,
                            email = record.email,
                            name = record.name,
                            password = Cryptographer.Encrypt(myrandomkeymanager.GenerateRandomAlphaNumericString(7)).Replace("=", ""),
                            locked = record.locked,
                            google_authenticate = record.google_authenticate,
                            sec_key = myrandomkeymanager.GenerateRandomAlphaNumericString(16).ToUpper(),
                            avatar = "user.jpg",
                            created_by = Convert.ToInt16(HttpContext.Session.GetString("userid"))
                        };

                        if (dbhandler.AddPortalUser(mymodel))
                        {
                            CaptureAuditTrail("Created user", "Created user: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
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
        }

        [RBAC]
        public ActionResult CreateUserRoleMapping(mappingrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                try
                {
                    switch (record.mode)
                    {
                        case "allocate":
                            if (record.parent_id <= 0)
                                return Content("Invalid user");

                            if (record.child_id <= 0)
                                return Content("Invalid role");

                            UserRoleModel existingrecord = dbhandler.GetUserRoles(record.parent_id).Find(mymodel => mymodel.user_id == record.parent_id && mymodel.role_id == record.child_id)!;
                            if (existingrecord == null)
                            {
                                UserRoleModel urmodel = new UserRoleModel
                                {
                                    user_id = record.parent_id,
                                    role_id = record.child_id
                                };

                                if (dbhandler.AddUserRole(urmodel))
                                {
                                    CaptureAuditTrail("Allocate user role access", "Allocated access to role: " + dbhandler.GetRoles().Find(mymodel => mymodel.id == record.child_id)!.role_name + " for user: " + dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == record.parent_id)!.name);
                                    ModelState.Clear();
                                    return Content("Success");
                                }
                                else
                                    return Content("Could not allocate role to user, kindly contact system admin");
                            }
                            else
                                return Content("Role already allocated to user");

                        case "unallocate":
                            if (dbhandler.DeleteRecord(record.id, Convert.ToInt16(HttpContext.Session.GetString("userid")), "user_allocated_roles"))
                            {
                                CaptureAuditTrail("Unallocate user role access", "Unallocated access to role: " + dbhandler.GetRoles().Find(mymodel => mymodel.id == record.child_id)!.role_name + " for user: " + dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == record.parent_id)!.name);
                                return Content("Success");
                            }
                            else
                                return Content("Could not deallocate user role, kindly contact system admin");

                        default:
                            return Content("Could not perform operation, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not perform operation, kindly contact system admin");
                }
            }
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