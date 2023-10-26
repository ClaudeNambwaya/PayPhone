namespace ComplaintManagement.Models
{
    public class ComplaintFilesModel
    {
        public Int64 id { set; get; }
        public Int64 complaint_id { set; get; }
        public string? file_number { set; get; }
        public string? file_name { set; get; }
        
    }
}
