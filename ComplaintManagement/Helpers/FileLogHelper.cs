using ComplaintManagement.Models;

namespace ComplaintManagement.Helpers
{
    public class FileLogHelper
    {

        private DBHandler dbhandler;

        public FileLogHelper(DBHandler mydbhandler)
        {
            dbhandler = mydbhandler;
        }

        public static void log_message_fields(string messagetype, string message)
        {
            string filename = null;
            StreamWriter f;
            try
            {
                //string logfilespath = dbhandler.GetRecords("parameters", "LOG_FILES_PATH").Rows[0]["item_value"].ToString();

                //if (messagetype == "ERROR")
                //    filename = logfilespath + "RADMIN_Errorlog.txt" ?? string.Empty;
                //else
                //    filename = logfilespath + "RADMIN_Infolog.txt" ?? string.Empty;

                //f = new StreamWriter(filename, true);
                //f.WriteLine(message);
                //f.Close();
            }
            catch (Exception ex)
            {
                throw ex; //through to avoid system running without logs
            }
        }
    }
}