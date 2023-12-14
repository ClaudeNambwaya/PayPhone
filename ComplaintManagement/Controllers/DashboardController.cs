using Newtonsoft.Json.Linq;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using static ComplaintManagement.Helpers.CryptoHelper;

namespace ComplaintManagement.Controllers
{
    //[CheckAuthorization]
    public class DashboardController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public DashboardController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class user_record
        {
            public int user_id { get; set; }
        }

        public ActionResult Index()
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

        public ActionResult Client()
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
        public ContentResult GetDashboardData( string param = "")
        {
            string EnctryptionAlgorith = "rijndael";
            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            JObject jobject = new JObject();

            try
            {

                DataTable datatable = dbhandler.GetAdhocData("call get_admin_dashboard_widget_data");

                if (datatable.Rows.Count > 0)
                {
                    foreach (DataRow dr in datatable.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in datatable.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    jobject.Add("widget_data", JsonConvert.SerializeObject(rows, Formatting.Indented));
                }

                rows = new List<Dictionary<string, object>>();

                DataTable datatableI = dbhandler.GetAdhocData("call get_admin_dashboard_doughnut_data");

                if (datatableI.Rows.Count > 0)
                {
                    foreach (DataRow dr in datatableI.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in datatableI.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    jobject.Add("doughnut_data", JsonConvert.SerializeObject(rows));
                }

                rows = new List<Dictionary<string, object>>();

                DataTable datatableII = dbhandler.GetAdhocData("call get_admin_dashboard_line_chart_data");

                if (datatableII.Rows.Count > 0)
                {
                    foreach (DataRow dr in datatableII.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in datatableII.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    jobject.Add("line_chart_data", JsonConvert.SerializeObject(rows));
                }

                rows = new List<Dictionary<string, object>>();

                //check licence validity
                string expdateenc = string.Empty;
                string message = string.Empty;
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
                        message = "Kindly note that the system license has expired, contact administrator";
                    else
                        message = "Kindly note that the system license will expire in " + daysuntilexpiry + " days";
                }

                JObject message_jobject = new JObject
                {
                    { "user", HttpContext.Session.GetString("name") },
                    { "avatar", HttpContext.Session.GetString("avatar") },
                    { "message", message }
                };

                jobject.Add("message_data", message_jobject);
            }
            catch (Exception ex)
            {
                iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
            }
            return Content(jobject.ToString(), "application/json");
        }
        
        [HttpGet]
        public ContentResult GetClientDashboardData( string param = "")
        {
            string EnctryptionAlgorith = "rijndael";
            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            JObject jobject = new JObject();

            try
            {
                string user_id = HttpContext.Session.GetString("userid")!;

                DataTable datatable = dbhandler.GetAdhocData("call get_client_dashboard_widget_data", user_id);

                if (datatable.Rows.Count > 0)
                {
                    foreach (DataRow dr in datatable.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in datatable.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    jobject.Add("widget_data", JsonConvert.SerializeObject(rows, Formatting.Indented));
                }

                rows = new List<Dictionary<string, object>>();

                DataTable datatableI = dbhandler.GetAdhocData("call get_client_dashboard_doughnut_data",user_id);

                if (datatableI.Rows.Count > 0)
                {
                    foreach (DataRow dr in datatableI.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in datatableI.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    jobject.Add("doughnut_data", JsonConvert.SerializeObject(rows));
                }

                rows = new List<Dictionary<string, object>>();

                DataTable datatableII = dbhandler.GetAdhocData("call get__dashboard_line_chart_data", user_id);

                if (datatableII.Rows.Count > 0)
                {
                    foreach (DataRow dr in datatableII.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in datatableII.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    jobject.Add("line_chart_data", JsonConvert.SerializeObject(rows));
                }

                rows = new List<Dictionary<string, object>>();

                //check licence validity
                string expdateenc = string.Empty;
                string message = string.Empty;
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
                        message = "Kindly note that the system license has expired, contact administrator";
                    else
                        message = "Kindly note that the system license will expire in " + daysuntilexpiry + " days";
                }

                JObject message_jobject = new JObject
                {
                    { "user", HttpContext.Session.GetString("name") },
                    { "avatar", HttpContext.Session.GetString("avatar") },
                    { "message", message }
                };

                jobject.Add("message_data", message_jobject);
            }
            catch (Exception ex)
            {
                iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
            }
            return Content(jobject.ToString(), "application/json");
        }
    }
}