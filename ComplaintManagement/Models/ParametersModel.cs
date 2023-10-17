using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class ParametersModel
    {
        [Key]
        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "Item Key"), Required(ErrorMessage = "Value is required.")]
        public string? item_key { get; set; }

        [Display(Name = "Item Value"), Required(ErrorMessage = "Value is required.")]
        public string? item_value { get; set; }

        [Display(Name = "Comments"), Required(ErrorMessage = "Value is required.")]
        public string? comments { get; set; }
        public virtual Int32 created_by { get; set; }
    }
}