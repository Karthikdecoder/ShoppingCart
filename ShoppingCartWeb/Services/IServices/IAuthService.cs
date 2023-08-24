using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO objToCreate);
        Task<T> RegisterAsync<T>(RegistrationDTO objToCreate, string Token);
        Task<T> GetAllRegistrationAsync<T>(string token);
        Task<T> GetAllCategoryAsync<T>(string token);
        Task<T> UserRegisterAsync<T>(UserDTO objToCreate, string Token);
    }
}
