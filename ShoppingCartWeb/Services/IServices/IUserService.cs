using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IUserService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO objToCreate);
        Task<T> RegisterAsync<T>(UserDTO objToCreate, string Token);
        Task<T> GetAllUserAsync<T>(string token);
        Task<T> GetUserAsync<T>(int id, string token);
        Task<T> UpdateUserAsync<T>(UserDTO dto, string token);
        Task<T> RemoveUserAsync<T>(int id, string token);
    }
}
