using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class StateMasterCreateVM
    {
        public StateMasterCreateVM()
        {
            State = new StateMasterDTO();
        }
        public StateMasterDTO State { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CountryList { get; set; }
    }
}
