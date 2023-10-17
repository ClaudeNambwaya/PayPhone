
namespace ComplaintManagement.Models
{
    public class ComplaintModel
    {
        public Int64 id { get; set; }
        public string? user_id { get; set; }
        public Int64 category_id { get; set; }
        public Int64 subcategory_id { get; set; }
        public Int64 complaint_type { get; set; }
        public bool state_id { get; set; }
        public string? nature_of_complaint { get; set; }
        public string? complaint_description { get; set; }
        public Int64 status { get; set; }
        public DateTime created_on { get; set; }
        public Int64 county_id { get; set; }
        public Int64 sub_county_id { get; set; }
        public Int64 ward_id { get; set; }
        public string? address { get; set; }
        public bool isanonymous { get; set; }
        public string? remarks { get; set; }

    }
}
