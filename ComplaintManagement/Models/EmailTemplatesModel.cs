using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class EmailTemplatesModel
    {
        [Key]
        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "Request Type"), Required(ErrorMessage = "Value is required.")]
        public string request_type { get; set; }

        [Display(Name = "Email Template"), Required(ErrorMessage = "Value is required.")]
        public string email_template { get; set; }
        
        [Display(Name = "Comments"), Required(ErrorMessage = "Value is required.")]
        public string comments { get; set; }
        
        [Display(Name = "Logo"), Required(ErrorMessage = "Value is required.")]
        public string logo { get; set; }
        public virtual Int32 created_by { get; set; }
    }
}