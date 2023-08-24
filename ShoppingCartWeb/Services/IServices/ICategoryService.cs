using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface ICategoryService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int roleId, string token);
        Task<T> CreateAsync<T>(CategoryMasterDTO categoryMasterDTO, string token);
        Task<T> UpdateAsync<T>(CategoryMasterDTO categoryMasterDTO, string token);
        Task<T> DeleteAsync<T>(int roleId, string token);
    }
}
