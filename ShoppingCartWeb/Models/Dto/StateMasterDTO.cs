using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppingCartWeb.Models.Dto
{
    public class StateMasterDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }

        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[a-zA-Z '-]{2,20}$", ErrorMessage = "Invalid State")]
        public string StateName { get; set; }

        [Required]
        [ValidateNever]
        [ForeignKey("CountryMaster")]
        public int CountryId { get; set; }
        public CountryMasterDTO CountryMaster { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
