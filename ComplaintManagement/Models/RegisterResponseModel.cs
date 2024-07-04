using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models.Models
{
    public class RegisterResponseModel
    {
        public RegistrationModel? Message { get; set; }
    }

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
        public bool locked { get; set; }
        public string? sec_key { get; set; }
        public bool google_authenticate { get; set; }
        public string? ReturnURL { get; set; }
    }
}
