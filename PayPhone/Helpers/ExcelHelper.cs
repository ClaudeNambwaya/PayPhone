using System.Data;

/// <summary>
/// Summary description for ExcelManager
/// </summary>
namespace ComplaintManagement.Helpers
{

    public class ExcelHelper
    {
        private readonly IHttpContextAccessor httpcontextaccessor;

        public ExcelHelper(IHttpContextAccessor httpContextAccessor)
        {
            httpcontextaccessor = httpContextAccessor;
        }

        public void ExporttoExcel(DataTable table)
        {
            httpcontextaccessor.HttpContext!.Response.Clear();
            httpcontextaccessor.HttpContext.Response.ContentType = "application/ms-excel";
            httpcontextaccessor.HttpContext.Response.WriteAsync(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            httpcontextaccessor.HttpContext.Response.WriteAsync("<font style='font-size:10.0pt; font-family:Calibri;'>");
            httpcontextaccessor.HttpContext.Response.WriteAsync("<BR><BR><BR>");
            //sets the table border, cell spacing, border color, font of the text, background, foreground, font height
            httpcontextaccessor.HttpContext.Response.WriteAsync("<table border='1' bgColor='#ffffff' " +
              "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
              "style='font-size:10.0pt; font-family:Calibri; background:white;'> <tr>");
            //am getting my grid's column headers
            int columnscount = table.Columns.Count; 

            for (int j = 0; j < columnscount; j++)
            {      //write in new column
                httpcontextaccessor.HttpContext.Response.WriteAsync("<td>");
                //Get column headers  and make it as bold in excel columns
                httpcontextaccessor.HttpContext.Response.WriteAsync("<b>");
                httpcontextaccessor.HttpContext.Response.WriteAsync(table.Columns[j].ColumnName.ToString().ToUpperInvariant());
                httpcontextaccessor.HttpContext.Response.WriteAsync("</b>");
                httpcontextaccessor.HttpContext.Response.WriteAsync("</td>");
            }
            httpcontextaccessor.HttpContext.Response.WriteAsync("</tr>");
            foreach (DataRow row in table.Rows)
            {//write in new row
                httpcontextaccessor.HttpContext.Response.WriteAsync("<tr>");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    httpcontextaccessor.HttpContext.Response.WriteAsync("<td>");
                    httpcontextaccessor.HttpContext.Response.WriteAsync(row[i].ToString()!);
                    httpcontextaccessor.HttpContext.Response.WriteAsync("</td>");
                }

                httpcontextaccessor.HttpContext.Response.WriteAsync("</tr>");
            }
            httpcontextaccessor.HttpContext.Response.WriteAsync("</table>");
            httpcontextaccessor.HttpContext.Response.WriteAsync("</font>");
            //httpcontextaccessor.HttpContext.Response.Flush();
            //httpcontextaccessor.HttpContext.Response.End();
            httpcontextaccessor.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        }
    }
}