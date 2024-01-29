using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ComplaintManagement.Helpers;
using static ComplaintManagement.Helpers.CryptoHelper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using NLog;

namespace ComplaintManagement.Models
{
    public class DBHandler : IDisposable
    {
        public readonly IConfiguration config;
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        private MySqlConnection connection;
        private string connectionstring;

        public DBHandler(string connstring)
        {
            connection = new MySqlConnection(connstring);
            this.connection.Open();
            connectionstring = connstring;
        }

        public void Dispose()
        {
            connection.Close();
        }

        #region Databases
        public enum DataBaseObject
        {
            HostDB,
            BrokerDB
        }

        public string GetDataBaseConnection(DataBaseObject databaseobject)
        {
            string connection_string = connectionstring; //config["ConnectionStrings:DefaultConnection"];
            switch (databaseobject)
            {
                case DataBaseObject.HostDB:
                    connection_string = connectionstring; //config["ConnectionStrings:DefaultConnection"];
                    break;
                default:
                    connection_string = connectionstring; //config["ConnectionStrings:DefaultConnection"];
                    break;
            }
            return connection_string;
        }
        #endregion

        //public static string GetConnectionString()
        //{
        //    return Startup.ConnectionString;
        //}

        ////public DataTable ValidateUserLogin(string user_type, string email_address)
        //{
        //    DataTable dt = new DataTable();
        //    string sql = "validate_user_login";
        //    if (user_type.Equals("TELLER"))
        //        sql = "validate_teller_login";

        //    try
        //    {
        //        using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
        //        using MySqlCommand cmd = new MySqlCommand(sql, connect);
        //        using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
        //        connect.Open();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@in_user_name", email_address);
        //        sd.Fill(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        FileLogHelper.log_message_fields("ERROR", "ValidateUserLogin | Exception ->" + ex.Message);
        //    }

        //    return dt;
        //}


        #region Adhoc
        public DataTable ValidateUserLogin(string user_type, string email_address)
        {
            DataTable dt = new DataTable();
            try
            {
                using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));

                using MySqlCommand cmd = new MySqlCommand("validate_login", connect);
                using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                connect.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", email_address);
                cmd.Parameters.AddWithValue("@profiletype", user_type);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "ValidateUserLogin | Exception ->" + ex.Message);
            }

            return dt;
        }

        public List<CategoryModel> GetCategory()
        {
            List<CategoryModel> recordlist = new List<CategoryModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("category_record");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new CategoryModel()
                    {
                        id = Convert.ToInt64(dr["id"]),
                        category_name = Convert.ToString(dr["category_name"]),
                        category_description = Convert.ToString(dr["category_description"]),
                        created_on = Convert.ToDateTime(dr["created_on"]),
                        //updated_date = Convert.ToDateTime(dr["updated_date"])
                    });
                }
                

            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }

        public bool AddCategory(CategoryModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_category", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@category_name", model.category_name);
                    cmd.Parameters.AddWithValue("@category_description", model.category_description);
             
                   
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i >= 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateCategory(CategoryModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_category", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", model.id);
                    cmd.Parameters.AddWithValue("@in_category_name", model.category_name);
                    cmd.Parameters.AddWithValue("@in_category_description", model.category_description);
                  

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public List<SubcategoryModel> GetSubcategory()
        {
            List<SubcategoryModel> recordlist = new List<SubcategoryModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("subcategory_record");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new SubcategoryModel()
                    {
                        id = Convert.ToInt64(dr["id"]),
                        sub_name = Convert.ToString(dr["sub_name"]),
                        category_id = Convert.ToInt64(dr["category_id"])
                        
                        
                    });
                }


            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }

        public bool AddSubcategory(SubcategoryModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_subcategory", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@sub_name", model.sub_name);
                    cmd.Parameters.AddWithValue("@category_id", model.category_id);
                   
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i >= 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateSubcategory(SubcategoryModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_sub_category", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", model.id);
                    cmd.Parameters.AddWithValue("@in_sub_name", model.sub_name);
                    cmd.Parameters.AddWithValue("@in_category_id", model.category_id);
                  
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public List<StateModel> GetState()
        {
            List<StateModel> recordlist = new List<StateModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("state_record");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new StateModel()
                    {
                        id = Convert.ToInt64(dr["id"]),
                        state_name = Convert.ToString(dr["state_name"]),
                        description = Convert.ToString(dr["description"])
                       
                    });
                }


            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }

        public bool AddState(StateModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_state", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@state_name", model.state_name);
                    cmd.Parameters.AddWithValue("@description", model.description);
                
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i >= 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateState(StateModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_state", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", model.id);
                    cmd.Parameters.AddWithValue("@in_state_name", model.state_name);
                    cmd.Parameters.AddWithValue("@in_description", model.description);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public List<ComplaintModel> GetComplaint()
        {
            List<ComplaintModel> recordlist = new List<ComplaintModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("complaint_record");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new ComplaintModel()
                    {
                        id = Convert.ToInt64(dr["id"]),
                        user_id = Convert.ToString(dr["user_id"]),
                        category_id = Convert.ToInt64(dr["category_id"]),
                        subcategory_id = Convert.ToInt64(dr["subcategory_id"]),
                        complaint_type = Convert.ToInt64(dr["complaint_type"]),
                        state_id = Convert.ToBoolean(dr["state_id"]),
                        nature_of_complaint = Convert.ToString(dr["nature_of_complaint"]),
                        complaint_description = Convert.ToString(dr["complaint_description"]),
                        county_id = Convert.ToInt64(dr["county_id"]),
                        sub_county_id = Convert.ToInt64(dr["sub_county_id"]),
                        ward_id = Convert.ToInt64(dr["ward_id"]),
                        address = Convert.ToString(dr["address"]),
                        isanonymous = Convert.ToBoolean(dr["isanonymous"]),
                        remarks = Convert.ToString(dr["remarks"])

                    });
                }


            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }
        public List<ComplaintModel> GetComplaintRecord()
        {
            List<ComplaintModel> recordlist = new List<ComplaintModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("complaint_list");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new ComplaintModel()
                    {
                        id = Convert.ToInt64(dr["id"])

                    });
                }


            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }

        public Int64 AddComplaint(ComplaintModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_complaint", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;

                    cmd.Parameters.AddWithValue("@category_id", model.category_id);
                    cmd.Parameters.AddWithValue("@subcategory_id", model.subcategory_id);
                    cmd.Parameters.AddWithValue("@complaint_type", model.complaint_type);
                    cmd.Parameters.AddWithValue("@nature_of_complaint", model.nature_of_complaint);
                    cmd.Parameters.AddWithValue("@complaint_description", model.complaint_description);
                    cmd.Parameters.AddWithValue("@county_id", model.county_id);
                    cmd.Parameters.AddWithValue("@sub_county_id", model.sub_county_id);
                    cmd.Parameters.AddWithValue("@ward_id", model.ward_id);
                    cmd.Parameters.AddWithValue("@address", model.address);
                    cmd.Parameters.AddWithValue("@isanonymous", model.isanonymous);
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
                    cmd.Parameters.AddWithValue("@user_id", model.user_id);
               
                    


                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                return i;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return 0;
            }
        }
        public Int64 AddRemarks(ComplaintModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_remarks", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
               
                    
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                return i;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return 0;
            }
        }

        public bool AddComplaintFiles(ComplaintFilesModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_files", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@complaint_id", model.complaint_id);
                    cmd.Parameters.AddWithValue("@file_name", model.file_name);

                    cmd.ExecuteNonQuery();
                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i > 0)

                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPost | Exception ->" + ex.Message);
                return false;
            }
        }


        public bool UpdateComplaint(ComplaintModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_complaint", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", model.id);
                    cmd.Parameters.AddWithValue("@in_category_id", model.category_id);
                    cmd.Parameters.AddWithValue("@in_subcategory_id", model.subcategory_id);
                    cmd.Parameters.AddWithValue("@in_complaint_type", model.complaint_type);
                    cmd.Parameters.AddWithValue("@in_nature_of_complaint", model.nature_of_complaint);
                    cmd.Parameters.AddWithValue("@in_complaint_description", model.complaint_description);
                    cmd.Parameters.AddWithValue("@in_county_id", model.county_id);
                    cmd.Parameters.AddWithValue("@in_sub_county_id", model.sub_county_id);
                    cmd.Parameters.AddWithValue("@in_ward_id", model.ward_id);
                    cmd.Parameters.AddWithValue("@in_isanonymous", model.isanonymous);
                    cmd.Parameters.AddWithValue("@in_address", model.address);
                    cmd.Parameters.AddWithValue("@in_remarks", model.remarks);

                    i = (int)cmd.ExecuteNonQuery();
                }
               
                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateComplaint | Exception ->" + ex.Message);
                return false;
            }
        }
        public bool UpdateRemarks(Int64 id, string description)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_remarks", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", id);
                    cmd.Parameters.AddWithValue("@in_remarks", description);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateComplaint | Exception ->" + ex.Message);
                return false;
            }
        }
        public bool Update_Open_Complaint_Status(Int64 id, string module, string param1 = "", DataBaseObject database = DataBaseObject.HostDB)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(database)))
                {
                    using (MySqlCommand cmd = new MySqlCommand("update_complaint_status", connect))
                    {
                        connect.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@in_recordid", id);
                        if (param1 != "")
                            cmd.Parameters.AddWithValue("@param1", param1);
                        cmd.Parameters.AddWithValue("@in_module", module);
                        i = cmd.ExecuteNonQuery();
                    }
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "DeleteRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        public List<ComplaintFilesModel> GetClientProfileDataDetails(string module, string param = "normal")
        {
            List<ComplaintFilesModel> recordlist = new List<ComplaintFilesModel>();

            try
            {
                DataTable datatable = new DataTable();

                switch (module)
                {

                    case "National_ID":
                        if (!param.Equals("normal"))
                            datatable = GetRecordsById("profile_nation_id", Convert.ToInt64(param));
                        break;
                    default:
                        break;
                }

                foreach (DataRow dr in datatable.Rows)
                {
                    recordlist.Add(
                    new ComplaintFilesModel
                    {
                        file_name = Convert.ToString(dr["file_name"])
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //FileLogHelper.log_message_fields("ERROR", "GetPortalRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public List<ComplaintFilesModel> GetComplaintFiles(string module, string param = "normal")
        {
            List<ComplaintFilesModel> recordlist = new List<ComplaintFilesModel>();

            try
            {
                DataTable datatable = new DataTable();

                switch (module)
                {

                    case "complaint_files":
                        if (!param.Equals("normal"))
                            datatable = GetRecordsById(module, Convert.ToInt64(param));
                        break;
                        default:
                        break;
                }

                foreach (DataRow dr in datatable.Rows)
                {
                    recordlist.Add(
                    new ComplaintFilesModel
                    {
                        file_number = Convert.ToString(dr["file_number"])
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //FileLogHelper.log_message_fields("ERROR", "GetPortalRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public List<ComplaintTypeModel> GetComplaintType()
        {
            List<ComplaintTypeModel> recordlist = new List<ComplaintTypeModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("complaint_type_record");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new ComplaintTypeModel()
                    {
                        id = Convert.ToInt64(dr["id"]),
                        complaint_name = Convert.ToString(dr["complaint_name"]),

                    });
                }


            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }

        public bool AddComplaintType(ComplaintTypeModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_complaint_type", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@complaint_name", model.complaint_name);
                 
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i >= 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return false;
            }
        }
        
        public bool Addcomplaint_Files(ComplaintFilesModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_complaint_files", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@file_number", model.file_number);
                    cmd.Parameters.AddWithValue("@file_name", model.file_name);
                    cmd.Parameters.AddWithValue("@complaint_id", model.complaint_id);
                 
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i >= 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateComplaintType(ComplaintTypeModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_complaint_type", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", model.id);
                    cmd.Parameters.AddWithValue("@in_complaint_name", model.complaint_name);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public List<StatusModel> GetStatus()
        {
            List<StatusModel> recordlist = new List<StatusModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("statuses_record");

                foreach (DataRow dr in dt.Rows)
                {

                    recordlist.Add(
                    new StatusModel()
                    {
                        id = Convert.ToInt64(dr["id"]),
                        status_name = Convert.ToString(dr["status_name"]),

                    });
                }


            }
            catch (Exception ex)
            {

                FileLogHelper.log_message_fields("ERROR", "GetCategoryRecords | Exception ->" + ex.Message);

            }
            return recordlist;
        }

        public bool AddStatus(StatusModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_statuses", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@status_name", model.status_name);

                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i >= 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddCategory | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateStatus(StatusModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_statuses", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", model.id);
                    cmd.Parameters.AddWithValue("@in_status_name", model.status_name);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateCategory | Exception ->" + ex.Message);
                return false;
            }
        }



        public bool UpdateProfile(Int16 record_id, Int16 profile_id, string attribute, string new_value)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_my_profile", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_record_id", record_id);
                    cmd.Parameters.AddWithValue("@in_profile_id", profile_id);
                    cmd.Parameters.AddWithValue("@in_attribute", attribute);
                    cmd.Parameters.AddWithValue("@in_new_value", new_value);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateMyProfile | Exception ->" + ex.Message);
                return false;
            }
        }

        public DataTable GetAdhocData(string sql, string userid = "")
        {
            DataTable dt = new DataTable();

            try
            {
                if(userid != "")
                {
                    using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                    using MySqlCommand cmd = new MySqlCommand()
                    {
                        Connection = connect,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "get_client_dashboard_widget_data"
                    };

                    // Add a parameter for user_id
                    cmd.Parameters.AddWithValue("@user_id_param", userid);

                    using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                    sd.Fill(dt);

                    //using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                    ////using MySqlCommand cmd = new MySqlCommand(sql, connect);
                    //using MySqlCommand cmd = new MySqlCommand("get_client_dashboard_widget_data", connect);
                    //// Add a parameter for user_id
                    //cmd.Parameters.AddWithValue("@user_id_param", userid);
                    //using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                    //sd.Fill(dt);
                }
                else
                {
                    using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                    using MySqlCommand cmd = new MySqlCommand(sql, connect);
                    // Add a parameter for user_id
                    //cmd.Parameters.AddWithValue("@user_id_param", userId);
                    using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                    sd.Fill(dt);
                }
               
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetAdhocData | Exception ->" + ex.Message);
            }
            return dt;
        }

        public DataTable GetRecords(string module, string param1 = "", string param2 = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using MySqlCommand cmd = new MySqlCommand("get_records", connect);
                using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                connect.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@module", module);
                cmd.Parameters.AddWithValue("@param1", param1);
                cmd.Parameters.AddWithValue("@param2", param2);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetRecords | Exception ->" + ex.Message);
            }

            return dt;
        }

        public DataTable GetUnapprovedRecords(string module, string param1 = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using MySqlCommand cmd = new MySqlCommand("get_records_unapproved", connect);
                using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@module", module);
                //cmd.Parameters.AddWithValue("@param1", param1);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUnapprovedRecords | Exception ->" + ex.Message);
            }

            return dt;
        }

        public DataTable GetRecordsById(string module, Int64 id, string param1 = "", string param2 = "")
        {
            DataTable dt = new DataTable();

            try
            {
                using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using MySqlCommand cmd = new MySqlCommand("get_records_by_id", connect);
                using MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                connect.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@module", module);
                cmd.Parameters.AddWithValue("@id", id);
                sd.Fill(dt);
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetRecordsById | Exception ->" + ex.Message);
            }

            return dt;
        }

        public string GetScalarItem(string sql)
        {
            string scalaritem = "";

            try
            {
                using MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB));
                using MySqlCommand command = new MySqlCommand(sql, connect);
                connect.Open();
                scalaritem = (string)(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetScalarItem | Exception ->" + ex.Message);
                scalaritem = "";
            }
            return scalaritem;
        }

        public bool DeleteRecord(Int64 id, int deleted_by, string module)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("delete_records", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_recordid", id);
                    cmd.Parameters.AddWithValue("@in_deleted_by", deleted_by);
                    cmd.Parameters.AddWithValue("@in_module", module);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "DeleteRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool ApproveRecord(Int64 id, int approved_by, string module, string action_flag = "")
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("approve_records", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_record_id", id);
                    cmd.Parameters.AddWithValue("@in_approved_by", approved_by);
                    cmd.Parameters.AddWithValue("@in_module", module);
                    cmd.Parameters.AddWithValue("@in_action_flag", action_flag);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "ApproveRecord | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AddAuditTrail(AuditTrailModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_audit_trail", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_user_name", mymodel.user_name);
                    cmd.Parameters.AddWithValue("@in_action_type", mymodel.action_type);
                    cmd.Parameters.AddWithValue("@in_action_description", mymodel.action_description);
                    cmd.Parameters.AddWithValue("@in_page_accessed", mymodel.page_accessed);
                    cmd.Parameters.AddWithValue("@in_client_ip_address", mymodel.client_ip_address);
                    cmd.Parameters.AddWithValue("@in_session_id", mymodel.session_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddAuditTrail | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AllocateDeallocateRolePermission(string action, int role_id, int permission_id)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("allocate_deallocate_role_permission", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    cmd.Parameters.AddWithValue("@permission_id", permission_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AllocateDeallocateRolePermission | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool AllocateDeallocateUserRole(string action, int user_id, int role_id)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("allocate_deallocate_user_role", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AllocateDeallocateUserRole | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool StartStopInterface(int id, string action)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("start_stop_interface", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_interface_id", id);
                    cmd.Parameters.AddWithValue("@in_action", action);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "StartStopInterface | Exception ->" + ex.Message);
                return false;
            }
        }
        #endregion

        #region Access Control
        //Roles
        public List<RolesModel> GetRoles()
        {
            List<RolesModel> recordlist = new List<RolesModel>();

            try
            {
                  DataTable dt = new DataTable();
                dt = GetRecords("roles");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new RolesModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_name = Convert.ToString(dr["role_name"]),
                        role_type = Convert.ToString(dr["role_type"]),
                        remarks = Convert.ToString(dr["remarks"]),
                        is_sys_admin = Convert.ToBoolean(dr["is_sys_admin"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetPortalRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddRole(RolesModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_role", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_role_name", mymodel.role_name);
                    cmd.Parameters.AddWithValue("@in_role_type", mymodel.role_type);
                    cmd.Parameters.AddWithValue("@in_remarks", mymodel.remarks);
                    cmd.Parameters.AddWithValue("@in_is_sys_admin", mymodel.is_sys_admin);
                    cmd.Parameters.AddWithValue("@in_created_by", mymodel.created_by);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddRole | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateRole(RolesModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_role", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", mymodel.id);
                    cmd.Parameters.AddWithValue("@in_role_name", mymodel.role_name);
                    cmd.Parameters.AddWithValue("@in_role_type", mymodel.role_type);
                    cmd.Parameters.AddWithValue("@in_remarks", mymodel.remarks);
                    cmd.Parameters.AddWithValue("@in_is_sys_admin", mymodel.is_sys_admin);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateRole | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Roles

        //Role Menu Access
        public bool AddMenuAccess(string page_url, string main_menu_name, string sub_menu_name, int role_id, int can_access, int menu_order, int sub_menu_order)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_menu_access", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_role_id", role_id);
                    cmd.Parameters.AddWithValue("@in_main_menu_name", main_menu_name);
                    cmd.Parameters.AddWithValue("@in_sub_menu_name", sub_menu_name);
                    cmd.Parameters.AddWithValue("@in_page_url", page_url);
                    cmd.Parameters.AddWithValue("@in_can_access", can_access);
                    cmd.Parameters.AddWithValue("@in_menu_order", menu_order);
                    cmd.Parameters.AddWithValue("@in_sub_menu_order", sub_menu_order);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetPermissions | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Role Menu Access

        //Permissions
        public List<PermissionsModel> GetPermissions()
        {
            List<PermissionsModel> recordlist = new List<PermissionsModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("permissions");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new PermissionsModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        permission_name = Convert.ToString(dr["permission_name"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetPermissions | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddPermission(PermissionsModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_permission", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_permission_name", mymodel.permission_name);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPermission | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdatePermission(PermissionsModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_permission", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", mymodel.id);
                    cmd.Parameters.AddWithValue("@in_permission_name", mymodel.permission_name);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdatePermission | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Permissions

        //Role Permissions
        public List<RolePermissionModel> GetRolePermissions(int role_id)
        {
            List<RolePermissionModel> recordlist = new List<RolePermissionModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecordsById("role_allocated_permissions", role_id);

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new RolePermissionModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_id = Convert.ToInt32(dr["role_id"]),
                        permission_id = Convert.ToInt32(dr["permission_id"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetRolePermissions | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddRolePermission(RolePermissionModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_role_permission_mapping", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_role_id", mymodel.role_id);
                    cmd.Parameters.AddWithValue("@in_permission_id", mymodel.permission_id);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddRolePermission | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Role Permissions

        //Users
        public List<PortalUsersModel> GetPortalUsers()
        {
            List<PortalUsersModel> recordlist = new List<PortalUsersModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("portal_users");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new PortalUsersModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_id = Convert.ToInt32(dr["role_id"]),
                        mobile = Convert.ToString(dr["mobile"]),
                        email = Convert.ToString(dr["email"]),
                        name = Convert.ToString(dr["name"]),
                        password = Convert.ToString(dr["password"]),
                        avatar = Convert.ToString(dr["avatar"]),
                        locked = Convert.ToBoolean(dr["locked"]),
                        google_authenticate = Convert.ToBoolean(dr["google_authenticate"]),
                        sec_key = Convert.ToString(dr["sec_key"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUserRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddPortalUser(PortalUsersModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_portal_user", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_role_id", mymodel.role_id);
                    cmd.Parameters.AddWithValue("@in_mobile", mymodel.mobile);
                    cmd.Parameters.AddWithValue("@in_email", mymodel.email);
                    cmd.Parameters.AddWithValue("@in_name", mymodel.name);
                    cmd.Parameters.AddWithValue("@in_password", mymodel.password);
                    cmd.Parameters.AddWithValue("@in_avatar", mymodel.avatar);
                    cmd.Parameters.AddWithValue("@in_locked", mymodel.locked);
                    cmd.Parameters.AddWithValue("@in_google_authenticate", mymodel.google_authenticate);
                    cmd.Parameters.AddWithValue("@in_created_by", mymodel.created_by);
                    cmd.Parameters.AddWithValue("@in_sec_key", mymodel.sec_key);

                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPortalUser | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdatePortalUser(PortalUsersModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_portal_user", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", mymodel.id);
                    cmd.Parameters.AddWithValue("@in_role_id", mymodel.role_id);
                    cmd.Parameters.AddWithValue("@in_mobile", mymodel.mobile);
                    cmd.Parameters.AddWithValue("@in_email", mymodel.email);
                    cmd.Parameters.AddWithValue("@in_name", mymodel.name);
                    cmd.Parameters.AddWithValue("@in_password", mymodel.password);
                    cmd.Parameters.AddWithValue("@in_avatar", mymodel.avatar);
                    cmd.Parameters.AddWithValue("@in_locked", mymodel.locked);
                    cmd.Parameters.AddWithValue("@in_google_authenticate", mymodel.google_authenticate);
                    cmd.Parameters.AddWithValue("@in_sec_key", mymodel.sec_key);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdatePortalUser | Exception ->" + ex.Message);
                return false;
            }
        }
        //End Users

        //Register
        public List<RegistrationModel> GetExternalPortalUsers()
        {
            List<RegistrationModel> recordlist = new List<RegistrationModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("external_portal_users");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new RegistrationModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        role_id = Convert.ToInt32(dr["role_id"]),
                        mobile = Convert.ToString(dr["mobile"])!,
                        email = Convert.ToString(dr["email"])!,
                        name = Convert.ToString(dr["name"])!,
                        password = Convert.ToString(dr["password"])!,
                        avatar = Convert.ToString(dr["avatar"])!,
                        locked = Convert.ToBoolean(dr["locked"]),
                        google_authenticate = Convert.ToBoolean(dr["google_authenticate"]),
                        sec_key = Convert.ToString(dr["sec_key"])!
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUserRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool RegisterUser(RegistrationModel model)
        {
            try
            {

                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_external_portal_user", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_name", model.name);
                    cmd.Parameters.AddWithValue("@in_email", model.email);
                    cmd.Parameters.AddWithValue("@in_password", model.password);
                    cmd.Parameters.AddWithValue("@in_mobile", model.mobile);
                    cmd.Parameters.AddWithValue("@in_avatar", model.avatar);
                    cmd.Parameters.AddWithValue("@in_locked", model.locked);
                    cmd.Parameters.AddWithValue("@in_sec_key", model.sec_key);
                    cmd.Parameters.AddWithValue("@in_google_authenticate", model.google_authenticate);
                    cmd.Parameters.AddWithValue("@in_role_id", model.role_id);
                    cmd.ExecuteNonQuery();

                    Int64 id = Convert.ToInt64(cmd.Parameters["@id"].Value);

                    if (id > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "RegisterUser | Exception ->" + ex.Message);
                return false;
            }
        }
        public bool UpdateExternalPortalUser(RegistrationModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_external_portal_user", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", mymodel.id);
                    cmd.Parameters.AddWithValue("@in_role_id", mymodel.role_id);
                    cmd.Parameters.AddWithValue("@in_mobile", mymodel.mobile);
                    cmd.Parameters.AddWithValue("@in_email", mymodel.email);
                    cmd.Parameters.AddWithValue("@in_name", mymodel.name);
                    cmd.Parameters.AddWithValue("@in_password", mymodel.password);
                    cmd.Parameters.AddWithValue("@in_avatar", mymodel.avatar);
                    cmd.Parameters.AddWithValue("@in_locked", mymodel.locked);
                    cmd.Parameters.AddWithValue("@in_google_authenticate", mymodel.google_authenticate);
                    cmd.Parameters.AddWithValue("@in_sec_key", mymodel.sec_key);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdatePortalUser | Exception ->" + ex.Message);
                return false;
            }
        }
        //User Roles
        public List<UserRoleModel> GetUserRoles(int user_id)
        {
            List<UserRoleModel> recordlist = new List<UserRoleModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecordsById("user_allocated_roles", user_id);

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new UserRoleModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        user_id = Convert.ToInt32(dr["user_id"]),
                        role_id = Convert.ToInt32(dr["role_id"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetUserRoles | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddUserRole(UserRoleModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_user_role_mapping", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_user_id", mymodel.user_id);
                    cmd.Parameters.AddWithValue("@in_role_id", mymodel.role_id);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddUserRole | Exception ->" + ex.Message);
                return false;
            }
        }
        //End User Roles
        #endregion



        #region Settings
        // Parameters
        public List<ParametersModel> GetParameters()
        {
            List<ParametersModel> recordlist = new List<ParametersModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("parameters");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ParametersModel
                    {
                        id = Convert.ToInt16(dr["Id"]),
                        item_key = Convert.ToString(dr["item_key"]),
                        item_value = Convert.ToString(dr["item_value"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetParameters | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddParameter(ParametersModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_parameter", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@in_item_key", mymodel.item_key);
                    cmd.Parameters.AddWithValue("@in_item_value", mymodel.item_value);
                    cmd.Parameters.AddWithValue("@in_comments", mymodel.comments);
                    cmd.Parameters.AddWithValue("@in_created_by", mymodel.created_by);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddParameter | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateParameter(ParametersModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_parameter", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@in_id", mymodel.id);
                    cmd.Parameters.AddWithValue("@in_item_key", mymodel.item_key);
                    cmd.Parameters.AddWithValue("@in_item_value", mymodel.item_value);
                    cmd.Parameters.AddWithValue("@in_comments", mymodel.comments);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateParameter | Exception ->" + ex.Message);
                return false;
            }
        }
        #endregion Settings

        #region Reports
        public List<ReportsModel> GetReports()
        {
            List<ReportsModel> recordlist = new List<ReportsModel>();

            try
            {
                DataTable dt = new DataTable();
                dt = GetRecords("reports");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ReportsModel
                    {
                        id = Convert.ToInt16(dr["id"]),
                        name = Convert.ToString(dr["name"]),
                        view_name = Convert.ToString(dr["view_name"]),
                        enabled = Convert.ToInt16(dr["enabled"])
                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetReports | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public bool AddReport(ReportsModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using (MySqlCommand cmd = new MySqlCommand("add_report", connect))
                    {
                        connect.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@in_name", mymodel.name);
                        cmd.Parameters.AddWithValue("@in_view_name", mymodel.view_name);
                        cmd.Parameters.AddWithValue("@in_enabled", Convert.ToBoolean(mymodel.enabled));
                        cmd.Parameters.AddWithValue("@in_created_by", mymodel.created_by);

                        i = (int)cmd.ExecuteNonQuery();
                    }
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddReport | Exception ->" + ex.Message);
                return false;
            }
        }

        public bool UpdateReport(ReportsModel mymodel)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using (MySqlCommand cmd = new MySqlCommand("update_report", connect))
                    {
                        connect.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@record_id", mymodel.id);
                        cmd.Parameters.AddWithValue("@in_name", mymodel.name);
                        cmd.Parameters.AddWithValue("@in_view_name", mymodel.view_name);
                        cmd.Parameters.AddWithValue("@in_enabled", Convert.ToBoolean(mymodel.enabled));

                        i = (int)cmd.ExecuteNonQuery();
                    }
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateReport | Exception ->" + ex.Message);
                return false;
            }
        }
        #endregion Reports
      
        //Client
        public List<ClientRecordModel> GetClientRecord()
        {
            List<ClientRecordModel> recordlist = new List<ClientRecordModel>();

            try
            {
                DataTable dt = new DataTable();

                dt = GetRecords("client_record");

                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                    new ClientRecordModel
                    {
                        id = Convert.ToInt32(dr["id"]),
                        client_type = Convert.ToString(dr["client_type"]),
                        client_name = Convert.ToString(dr["client_name"]),
                        phone_number = Convert.ToString(dr["phone_number"]),
                        email = Convert.ToString(dr["email"]),
                        id_number = Convert.ToString(dr["id_number"]),
                        kra_pin = Convert.ToString(dr["kra_pin"]),
                        physical_address = Convert.ToString(dr["physical_address"]),
                        postal_address = Convert.ToString(dr["postal_address"]),
                        industry = Convert.ToString(dr["industry"]),
                        remarks = Convert.ToString(dr["remarks"]),

                    });
                }
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetClientRecords | Exception ->" + ex.Message);
            }

            return recordlist;
        }

        public Int64 AddClient(ClientRecordModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_client_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@client_type", model.client_type);
                    cmd.Parameters.AddWithValue("@client_name", model.client_name);
                    cmd.Parameters.AddWithValue("@phone_number", model.phone_number);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@id_number", model.id_number);
                    cmd.Parameters.AddWithValue("@kra_pin", model.kra_pin);
                    cmd.Parameters.AddWithValue("@physical_address", model.physical_address);
                    cmd.Parameters.AddWithValue("@postal_address", model.postal_address);
                    cmd.Parameters.AddWithValue("@industry", model.industry);
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
                    cmd.Parameters.AddWithValue("@created_by", model.created_by);
                    cmd.ExecuteNonQuery();

                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                return i;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddClient | Exception ->" + ex.Message);
                return 0;
            }
        }

        public bool UpdateClient(ClientRecordModel model)
        {
            try
            {
                int i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("update_client_record", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", model.id);
                    cmd.Parameters.AddWithValue("@client_type", model.client_type);
                    cmd.Parameters.AddWithValue("@client_name", model.client_name);
                    cmd.Parameters.AddWithValue("@phone_number", model.phone_number);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@id_number", model.id_number);
                    cmd.Parameters.AddWithValue("@kra_pin", model.kra_pin);
                    cmd.Parameters.AddWithValue("@physical_address", model.physical_address);
                    cmd.Parameters.AddWithValue("@postal_address", model.postal_address);
                    cmd.Parameters.AddWithValue("@industry", model.industry);
                    cmd.Parameters.AddWithValue("@remarks", model.remarks);
                    i = (int)cmd.ExecuteNonQuery();
                }

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "UpdateClient | Exception ->" + ex.Message);
                return false;
            }
        }

        //EndClient

        //ClientFiles

        public bool AddClientFiles(ClientFilesModel model)
        {
            try
            {
                Int64 i = 0;
                using (MySqlConnection connect = new MySqlConnection(GetDataBaseConnection(DataBaseObject.HostDB)))
                {
                    using MySqlCommand cmd = new MySqlCommand("add_client_file", connect);
                    connect.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@client_id", model.client_id);
                    cmd.Parameters.AddWithValue("@file_name", model.file_name);

                    cmd.ExecuteNonQuery();
                    i = Convert.ToInt64(cmd.Parameters["@id"].Value.ToString());
                }
                if (i > 0)

                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "AddPost | Exception ->" + ex.Message);
                return false;
            }
        }
    }
}

