
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class RegistrationModel
    {
        public Int64 id { get; set; }
        public Int64 role_id { get; set; }
        public string? mobile { get; set; }
        public string? user_name { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? password { get; set; }
        public string? avatar { get; set; }

        public string? menu_layout { get; set; }
        public bool locked { get; set; }
        public string? sec_key { get; set; }
        public bool google_authenticate { get; set; }
        public string? ReturnURL { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_at { get; set; }
        public int created_by { get; set; }
    }
}
