namespace ComplaintManagement.Models
{
    public class AuditTrailModel
    {
        public int? id { get; set; }
        
        public string? user_name { get; set; }

        public string? action_type { get; set; }

        public string? action_description { get; set; }

        public string? page_accessed { get; set; }

        public string? client_ip_address { get; set; }

        public string? session_id { get; set; }
    }
}