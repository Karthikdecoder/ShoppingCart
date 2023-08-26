using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface ICategoryService
    {
        Task<T> GetAllCategoryAsync<T>(string token);
        Task<T> GetCategoryAsync<T>(int roleId, string token);
        Task<T> CreateCategoryAsync<T>(CategoryMasterDTO categoryMasterDTO, string token);
        Task<T> UpdateCategoryAsync<T>(CategoryMasterDTO categoryMasterDTO, string token);
        Task<T> RemoveCategoryAsync<T>(int roleId, string token);
    }
}
