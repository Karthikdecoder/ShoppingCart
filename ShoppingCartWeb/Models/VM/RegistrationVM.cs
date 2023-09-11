using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class RegistrationVM
    {
        public List<RegistrationDTO> Registration { get; set; }
        public RegistrationDTO RegistrationDTO { get; set; }
        public string FirstNameSortOrder { get; set; } 
        public string LastNameSortOrder { get; set; } 
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string OrderBy { get; set; }
    }
}
