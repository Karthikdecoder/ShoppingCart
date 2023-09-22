using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartAPI.Models
{
    public class MenuRoleMapping
    {
        public int MenuRoleMappingId { get; set; }

        [Required]
        [ForeignKey("Menu")]
        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        [Required]
        [ForeignKey("RoleMaster")]
        public int RoleId { get; set; }
        public RoleMaster RoleMaster { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
