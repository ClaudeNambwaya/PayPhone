using Newtonsoft.Json;
using System.Collections;
using System.Data;
//using System.Web.Mvc;
//using System.Web.Script.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using OfficeOpenXml;

namespace SHERIA.Controllers
{
    public class ReportData
    {
        public string? Name { get; set; }
        public string? ViewName { get; set; }
        public string? Title { get; set; }
        public string? Query { get; set; }
    }

    public class reportrecord
    {
        public Int16 id { get; set; }
        public string? reportname { get; set; }
        public string? viewname { get; set; }
        public int enabled { get; set; }
    }

    //[CheckAuthorization]
    public class ReportCentralController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public ReportCentralController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        [RBAC]
        public ActionResult ReportList()
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
        public ContentResult GetRecords()
        {
            ArrayList details = new ArrayList();
            DataTable datatable = new DataTable();
            //JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            datatable = dbhandler.GetRecords("reports");

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
        public ActionResult CreateReport(reportrecord record)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                try
                {
                    if (record.reportname == null)
                        return Content("Invalid report name");

                    if (record.viewname == null)
                        return Content("Invalid view name");

                    ReportsModel existingrecord = dbhandler.GetReports().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        ReportsModel mymodel = new ReportsModel
                        {
                            id = existingrecord.id,
                            name = record.reportname,
                            view_name = record.viewname,
                            enabled = record.enabled
                        };
                        if (dbhandler.UpdateReport(mymodel))
                        {
                            CaptureAuditTrail("Updated report", "Updated report: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not update report, kindly contact system admin");
                    }
                    else
                    {
                        ReportsModel mymodel = new ReportsModel
                        {
                            id = record.id,
                            name = record.reportname,
                            view_name = record.viewname,
                            enabled = record.enabled,
                            created_by = Convert.ToInt16(HttpContext.Session.GetString("userid"))
                        };
                        if (dbhandler.AddReport(mymodel))
                        {
                            CaptureAuditTrail("Created report", "Created report: " + mymodel.name);
                            ModelState.Clear();
                            return Content("Success");
                        }
                        else
                            return Content("Could not create report, kindly contact system admin");
                    }
                }
                catch
                {
                    return Content("Could not create report, kindly contact system admin");
                }
            }
        }

        public ContentResult ReportViewer(ReportData reportData)
        {
            HttpContext.Session.SetString("datasource", "Transaction");
            HttpContext.Session.SetString("reportname", "Recon.rdlc");
            HttpContext.Session.SetString("query", reportData.Query!);
            HttpContext.Session.SetString("Title", reportData.Title!);

            JObject jobject = new JObject
            {
                { "datasource", HttpContext.Session.GetString("datasource") },
                { "reportname", HttpContext.Session.GetString("reportname") },
                { "query", HttpContext.Session.GetString("query") },
                { "title", HttpContext.Session.GetString("Title") },
                { "status", true }
            };

            return Content(JsonConvert.SerializeObject(jobject, Formatting.Indented));
        }

        [HttpPost]
        public async Task<IActionResult> ExportReport(ReportData reportData)
        {
            var stream = new MemoryStream();
            string rootFolder = ihostingenvironment.WebRootPath;
            string excelName = $"Report-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, excelName));
            string handle = Guid.NewGuid().ToString();
            try
            {
                DataTable datatable = dbhandler.GetRecords("report_data", reportData.ViewName!);

                //using ExcelPackage package = new ExcelPackage(file);
                using ExcelPackage package = new ExcelPackage(stream);
                var excelWorksheet = package.Workbook.Worksheets.Add("Sheet1");

                var isNew = excelWorksheet.Dimension == null;

                if (isNew)
                    excelWorksheet.Cells[1, 1].LoadFromDataTable(datatable, true);
                else
                    excelWorksheet.Cells[excelWorksheet.Dimension!.End.Row + 1, 1].LoadFromDataTable(datatable, true);

                if (excelWorksheet.Dimension != null)
                    excelWorksheet.InsertRow(excelWorksheet.Dimension.End.Row + 2, 5);

                package.Save();
                stream.Position = 0;
                HttpContext.Session.Set(handle, stream.ToArray());
            }
            catch (Exception ex)
            {
                iloggermanager.LogError(ex.Message);
            }
            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
            
            JObject jobject = new JObject
            {
                { "FileGuid", handle },
                { "FileName", excelName }
            };
            return Content(JsonConvert.SerializeObject(jobject, Formatting.Indented), "application/json");
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (HttpContext.Session.Get(fileGuid) != null)
            {
                byte[] data = HttpContext.Session.Get(fileGuid)! as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
                return new EmptyResult();
        }

        //public ActionResult LoadParameters(string ViewName)
        //{
        //    if (HttpContext.Session.GetString("name") == null)
        //        return RedirectToAction("AdminLogin", "AppAuth");
        //    else
        //    {
        //        RequestResultModel _model = new RequestResultModel
        //        {
        //            Title = "",
        //            Message = "Parameters Loaded",
        //            InfoType = UIMessageType.Information
        //        };

        //        DataTable Fields = dbhandler.GetAdhocData("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + ViewName + "'");

        //        var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        //        var Filter = JsonConvert.SerializeObject(ColumnBuilder.GetDefaultFields(Fields, false), jsonSerializerSettings);

        //        ViewBag.FilterDefinition = Filter;
        //        return Json(new
        //        {
        //            Status = _model.InfoType,
        //            NotifyType = 0,//NotifyType.PageInline,
        //            Parameters = Filter
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [RBAC]
        public ActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                ReportsModel reportsmodel = dbhandler.GetReports().Find(mymodel => mymodel.id == id)!;
                dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), "reports");
                CaptureAuditTrail("Deleted report", "Deleted report: " + reportsmodel.name);

            }

            return GetRecords();
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