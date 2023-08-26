using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
	public interface ICountryService
	{
		Task<T> GetAllCountryAsync<T>(string token);
		Task<T> GetCountryAsync<T>(int countryId, string token);
		Task<T> CreateCountryAsync<T>(CountryMasterDTO countryMasterDTO, string token);
		Task<T> UpdateCountryAsync<T>(CountryMasterDTO countryMasterDTO, string token);
		Task<T> DeleteCountryAsync<T>(int countryId, string token);
	}
}
