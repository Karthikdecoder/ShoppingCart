using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface IRegistrationService
    {
        Task<T> GetAllRegistrationAsync<T>(string token);
        Task<T> GetAllDeletedRegistrationAsync<T>(string token);
        Task<T> GetRegistrationAsync<T>(int registrationId, string token);
        Task<T> CreateRegistrationAsync<T>(RegistrationDTO registrationDTO, string token);
        Task<T> UpdateRegistrationAsync<T>(RegistrationDTO registrationDTO, string token);
        Task<T> EnableRegistrationAsync<T>(int registrationId, string token);
        Task<T> RemoveRegistrationAsync<T>(int registrationId, string token);
    }
}
