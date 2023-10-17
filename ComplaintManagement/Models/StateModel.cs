namespace ComplaintManagement.Models
{
    public class StateModel
    {
        public Int64 id { get; set; }
        public string? state_name { get; set; }
        public string? description { get; set; }
        public DateTime created_on { get; set; }
        public Int64 created_by { get; set; }

    }
}
