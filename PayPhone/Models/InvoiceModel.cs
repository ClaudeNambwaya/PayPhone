namespace ComplaintManagement.Models
{
    public class InvoiceModel
    {
        public Int64 id { get; set; }
        public string? invoice_type { get; set; }
        public Int64 client_id { get; set; }
        public string? invoice_number { get; set; }
        public DateTime date_issued { get; set; }
        public DateTime due_date { get; set; }
        public string? amount { get; set; }
        public string? tax { get; set; }
        public string? duration { get; set; }
        public string? hourly_rate { get; set; }
        public string? description { get; set; }
        public Int64 created_by { get; set; }
        public DateTime created_on { get; set; }
        public bool approved { get; set; }
        public Int64 approved_by { get; set; }
        public DateTime approved_on { get; set; }
        public bool deleted { get; set; }
        public Int64 deleted_by { get; set; }
        public DateTime deleted_on { get; set; }
    }
}
