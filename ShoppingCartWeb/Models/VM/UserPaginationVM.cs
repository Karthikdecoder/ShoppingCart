using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class UserPaginationVM
    {
        public List<UserDTO> UserDTO { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string OrderBy { get; set; }
    }
}
