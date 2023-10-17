namespace ComplaintManagement.Models
{
    public class CategoryModel
    {
        public Int64 id { get; set; }
        public string? category_name { get; set; }
        public string? category_description { get; set; }
        public DateTime created_on { get; set; }
        public Int64 created_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
