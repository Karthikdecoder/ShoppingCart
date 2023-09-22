using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class MenuVM
    {
        public MenuVM()
        {
            Menu = new MenuDTO();
        }
        public MenuDTO Menu { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ParentList { get; set; }
    }
}
