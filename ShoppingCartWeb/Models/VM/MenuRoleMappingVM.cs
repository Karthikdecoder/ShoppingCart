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

        // Add properties to represent the selected role and selected menu IDs
        public string SelectedRole { get; set; }
        public List<string> SelectedMenuIds { get; set; }

    }

}
