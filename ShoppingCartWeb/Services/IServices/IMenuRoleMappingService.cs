using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IMenuRoleMappingService
    {
        Task<T> GetAllMenuRoleMappingAsync<T>(string token);
        Task<T> GetAllMenuByRoleIdMappingAsync<T>(string token);
        Task<T> GetMenuRoleMappingAsync<T>(int MenuRoleMappingId, string token);
        Task<T> GetSelectedMenuIdsForRoleAsync<T>(int roleId, string token);
        Task<T> CreateMenuRoleMappingAsync<T>(MenuRoleMappingDTO MenuRoleMappingDTO, string token);
        Task<T> UpdateMenuRoleMappingAsync<T>(MenuRoleMappingDTO MenuRoleMappingDTO, string token);
        Task<T> EnableMenuRoleMappingAsync<T>(int MenuRoleMappingId, string token);
        Task<T> RemoveMenuRoleMappingAsync<T>(int MenuRoleMappingId, string token);
    }
}
