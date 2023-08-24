using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using System.Linq.Expressions;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IUserRepository 
    {
        Task<List<User>> GetAllUserAsync();
        Task<List<Registration>> GetAllRegistrationAsync();
        bool IsUniqueUser(string email);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<Registration> Register(RegistrationDTO registerationRequestDTO, string userId);
        Task<User> UserRegister(UserDTO registerationRequestDTO, string userId);
    }
}
