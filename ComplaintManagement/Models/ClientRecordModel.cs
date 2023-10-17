namespace ComplaintManagement.Models
{
    public class ClientRecordModel
    {
        public Int64 id { get; set; }
        public string? client_type { get; set; }
        public string? client_name { get; set; }
        public string? phone_number { get; set; }
        public string? email { get; set; }
        public string? id_number { get; set; }
        public string? kra_pin { get; set; }
        public string? physical_address { get; set; }
        public string? postal_address { get; set; }
        public string? industry { get; set; }
        public string? remarks { get; set; }
        public Int64 created_by { get; set; }
        public DateTime created_on { get; set; }
        public bool approved { get; set; }
        public DateTime approved_on { get; set; }
        public bool is_deleted { get; set; }
        public DateTime deleted_on { get; set; }
        public Int64 deleted_by { get; set; }
    }

}
