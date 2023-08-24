using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IRegistrationService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int roleId, string token);
        Task<T> CreateAsync<T>(RegistrationDTO registrationDTO, string token);
        Task<T> UpdateAsync<T>(RegistrationDTO registrationDTO, string token);
        Task<T> DeleteAsync<T>(int roleId, string token);
    }
}
