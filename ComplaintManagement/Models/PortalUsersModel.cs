using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class PortalUsersModel
    {
        //public PortalUsersModel()
        //{
        //    roles = new HashSet<RolesModel>();
        //}

        public Int32 id { get; set; }
        public Int32 role_id { get; set; }
        public string? mobile { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? user_name { get; set; }
        public string? menu_layout { get; set; }
        public string? role_name { get; set; }
        public string? password { get; set; }
        public virtual string? avatar { get; set; }
        public bool locked { get; set; }
        public bool google_authenticate { get; set; }
        public virtual string? sec_key { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_at { get; set; }
        public virtual Int32 created_by { get; set; }
        public bool approved { get; set; }
        public Int64 balance { get; set; }

    }
}