using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IRoleService
    {
        Task<T> GetAllRolesAsync<T>(string token);
        Task<T> GetAsync<T>(int roleId, string token);
        Task<T> CreateAsync<T>(RoleMasterDTO roleMasterDTO, string token);
        Task<T> UpdateAsync<T>(RoleMasterDTO roleMasterDTO, string token);
        Task<T> DeleteAsync<T>(int roleId, string token);
    }
}
