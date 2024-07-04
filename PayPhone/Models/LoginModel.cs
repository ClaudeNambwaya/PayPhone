using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class LoginModel
    {
        
        public string? email { get; set; }

        public string? password { get; set; }

        public string? ReturnURL { get; set; }

        public bool isRemember { get; set; }

    }
}