using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class EmailTemplateParamsModel
    {
        [Key]
        [Display(Name = "Id")]
        public Int64 id { get; set; }

        [Display(Name = "Request Type"), Required(ErrorMessage = "Value is required.")]
        public Int64 request_type { get; set; }

        [Display(Name = "Parameter Order"), Required(ErrorMessage = "Value is required.")]
        public int param_order { get; set; }

        [Display(Name = "Parameter Name"), Required(ErrorMessage = "Value is required.")]
        public string param_name { get; set; }
        public virtual Int32 created_by { get; set; }
    }
}