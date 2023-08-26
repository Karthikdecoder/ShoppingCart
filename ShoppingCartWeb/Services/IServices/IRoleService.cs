using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IRoleService
    {
        Task<T> GetAllRoleAsync<T>(string token);
        Task<T> GetRoleAsync<T>(int roleId, string token);
        Task<T> CreateRoleAsync<T>(RoleMasterDTO roleMasterDTO, string token);
        Task<T> UpdateRoleAsync<T>(RoleMasterDTO roleMasterDTO, string token);
        Task<T> RemoveRoleAsync<T>(int roleId, string token);
    }
}
