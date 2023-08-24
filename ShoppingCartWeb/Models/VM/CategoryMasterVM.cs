using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class CategoryMasterVM
    {
        public CategoryMasterVM()
        {
            Registration = new RegistrationDTO();
        }
        public RegistrationDTO Registration { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
