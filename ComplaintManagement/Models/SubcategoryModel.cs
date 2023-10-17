namespace ComplaintManagement.Models
{
    public class SubcategoryModel
    {
        public Int64 id { get; set; }
        public string? sub_name { get; set; }
        public Int64 category_id { get; set; }
        public DateTime created_on { get; set; }
        public Int64 created_by { get; set; }
        
    }
}
