using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using static ComplaintManagement.Helpers.CryptoHelper;

namespace ComplaintManagement.Controllers
{
    public class profiledataresponse
    {
        public string? username { get; set; }
        public string? phonenumber { get; set; }
        public string? emailaddress { get; set; }
        public string? profile { get; set; }
        public string? createdon { get; set; }
        public string? profilepic { get; set; }
        public string? menulayout { get; set; }
    }

    public class newprofiledata
    {
        public string? name { get; set; }
        public string? value { get; set; }
    }

    //[CheckAuthorization]
    public class UserProfileController : Controller
    {

        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public UserProfileController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
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

        [HttpGet]
        public ActionResult GetMyProfileData()
        {
            ArrayList details = new ArrayList();
            profiledataresponse response = new profiledataresponse();

            DataTable datatable = dbhandler.GetRecordsById("user_profile", Convert.ToInt16(HttpContext.Session.GetString("userid")), HttpContext.Session.GetString("profileid").ToString());

            if (datatable.Rows.Count > 0)
            {
                response.username = datatable.Rows[0]["names"].ToString();
                response.phonenumber = datatable.Rows[0]["phone_number"].ToString();
                response.emailaddress = datatable.Rows[0]["email_address"].ToString();
                response.profile = datatable.Rows[0]["profile_name"].ToString();
                response.createdon = datatable.Rows[0]["created_on"].ToString();
                response.menulayout = datatable.Rows[0]["menu_layout"].ToString();
                response.profilepic = datatable.Rows[0]["profile_pic"].ToString();
                HttpContext.Session.SetString("menulayout", datatable.Rows[0]["menu_layout"].ToString()!);
                HttpContext.Session.SetString("avatar", datatable.Rows[0]["profile_pic"].ToString()!);
            }

            details.Add(response);

            return Content(JsonConvert.SerializeObject(details, Formatting.Indented), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateMyProfileData([FromBody] JObject jobject/*newprofiledata record*/)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                try
                {
                    newprofiledata record = new newprofiledata
                    {
                        name = jobject["name"]!.ToString(),
                        value = jobject["value"]!.ToString()
                    };

                    switch (record.name)
                    {
                        case "password":
                            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");

                            record.value = Cryptographer.Encrypt(record.value).Replace("=", "");
                            break;

                        case "profilepic":
                            string file_ext = "png";
                            switch (record.value.Split(',')[0])
                            {
                                case "data:image/png;base64":
                                    file_ext = ".png";
                                    break;

                                case "data:image/jpeg;base64":
                                    file_ext = ".jpg";
                                    break;

                                default:
                                    //return Json("Invalid file", JsonRequestBehavior.AllowGet);
                                    return Content("Invalid file");
                            }

                            string file_name = DateTime.Now.ToString("ddMMyyyyhhmmssfff") + HttpContext.Session.GetString("userid") + file_ext;
                            //string pic_path = HostingEnvironment.MapPath("~/assets/static/img/profile-pics/" + file_name);
                            string pic_path = Path.Combine(ihostingenvironment.WebRootPath, "assets\\static\\img\\profile-pics\\" + file_name);
                            using (FileStream fs = new FileStream(pic_path, FileMode.Create))
                            {
                                using BinaryWriter bw = new BinaryWriter(fs);
                                byte[] data = Convert.FromBase64String(record.value.Split(',')[1]);
                                bw.Write(data);
                                bw.Close();
                                record.value = file_name;
                            }
                            break;
                    }

                    dbhandler.UpdateProfile(Convert.ToInt16(HttpContext.Session.GetString("userid")), Convert.ToInt16(HttpContext.Session.GetString("profileid")), record.name, record.value);

                    CaptureAuditTrail("Updated my profile data", "Updated my profile data: " + record.name);
                    ModelState.Clear();
                    return Content("Success");
                }
                catch (Exception ex)
                {
                    return Json("Could not update profile data, kindly contact system admin");
                }
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