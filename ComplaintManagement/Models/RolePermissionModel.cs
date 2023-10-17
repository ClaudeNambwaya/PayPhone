namespace ComplaintManagement.Models
{
    public class RolePermissionModel
    {
        public Int32 id { get; set; }
        
        public Int32 role_id { get; set; }
        
        public Int32 permission_id { get; set; }
    }
}