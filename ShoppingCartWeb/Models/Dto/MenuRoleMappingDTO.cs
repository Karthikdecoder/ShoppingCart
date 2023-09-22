using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppingCartWeb.Models.Dto
{
    public class MenuRoleMappingDTO
    {
       
        public int MenuRoleMappingId { get; set; }

        [Required]
        [ForeignKey("Menu")]
        public int MenuId { get; set; }
        public MenuDTO Menu { get; set; }

        [Required]
        [ForeignKey("RoleMaster")]
        public int RoleId { get; set; }
        public RoleMasterDTO RoleMaster { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public IEnumerable<string> SelectedMenuIds { get; set; }

    }
}
