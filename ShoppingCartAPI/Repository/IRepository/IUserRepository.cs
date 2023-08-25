using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using System.Linq.Expressions;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IUserRepository  : IRepository<User>
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserAsync(int userId);
        Task<User> UpdateUserAsync(User User, string userId);
        Task<User> RegisterAsync(UserDTO User, string userId);
        Task RemoveAsync(User User);
        Task SaveAsync();
        bool IsUniqueUser(string Email);
    }
}
