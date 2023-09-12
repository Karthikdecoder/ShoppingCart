using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IMenuService
    {
        Task<T> GetAllMenuAsync<T>(string token);
        Task<T> GetMenuAsync<T>(int MenuId, string token);
        Task<T> CreateMenuAsync<T>(MenuDTO MenuDTO, string token);
        Task<T> UpdateMenuAsync<T>(MenuDTO MenuDTO, string token);
        Task<T> EnableMenuAsync<T>(int MenuId, string token);
        Task<T> RemoveMenuAsync<T>(int MenuId, string token);
    }
}
