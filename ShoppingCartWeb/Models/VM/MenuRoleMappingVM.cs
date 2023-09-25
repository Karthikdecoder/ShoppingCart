using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class MenuRoleMappingVM
    {
        public MenuRoleMappingVM()
        {
            
            MenuRoleMapping = new MenuRoleMappingDTO();
        }

        public MenuRoleMappingDTO MenuRoleMapping { get; set; }

        [ValidateNever]
        public IEnumerable<CustomSelectListItem> MenuList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }

    }

}
